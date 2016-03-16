using Microsoft.VisualStudio.TestTools.UnitTesting;

using EF6Model;
using System.Data.Entity;
using System.Linq;
using System.Diagnostics;

using EF6Model.Models;
using System.Text;
using System.Collections.Generic;

namespace EF6WebAPI.Tests
{
  [TestClass]
  public class VerifyDatabaseSavesTests
  {
    private StringBuilder _logBuilder = new StringBuilder();
    private string _log;


    public VerifyDatabaseSavesTests() {
      Database.SetInitializer(new DropCreateDatabaseAlways<NinjaContext>());

    }



    [TestMethod]
    public void EFComprehendsMixedStatesWhenAddingUntrackedGraph() {
      var ninja = EF6Model.RichModels.Ninja.CreateIndependent("julie", true);
      ninja.SpecifyClan(new EF6Model.RichModels.Clan { Id = 1, ClanName = "Clan from database" });
      using (var context = new NinjaContext()) {
        context.Ninjas.Add(ninja);
        var entries = context.ChangeTracker.Entries();
        entries.ToList().ForEach(e => Debug.WriteLine($"Before:{e.Entity.ToString()}:{e.State}"));
        context.FixState();
        entries.ToList().ForEach(e => Debug.WriteLine($"After:{e.Entity.ToString()}:{e.State}"));
        Assert.IsTrue(entries.Any(e => e.State == EntityState.Unchanged));

      }
    }
    [TestMethod]
    public void MixedStatesWithExistingParentAndVaryingChildrenisUnderstood() {

      var ninja = EF6Model.RichModels.Ninja.CreateIndependent("julie", true);
      var pNinja = new PrivateObject(ninja);
      pNinja.SetProperty("Id", 1);
      var originalOwnerId = 99;
      var equip = EF6Model.RichModels.NinjaEquipment.Create(originalOwnerId, "arrow");
      ninja.TransferEquipmentFromAnotherNinja(equip);
      using (var context = new NinjaContext()) {
        context.Ninjas.Attach(ninja);
        var entries = context.ChangeTracker.Entries();
        OutputState(entries); context.FixState();
        entries.ToList().ForEach(e => Debug.WriteLine($"After:{e.Entity.ToString()}:{e.State}"));
        Assert.IsTrue(entries.Any(e => e.State == EntityState.Modified));

      }
    }

    private static void OutputState(System.Collections.Generic.IEnumerable<System.Data.Entity.Infrastructure.DbEntityEntry> entries) {
      entries.ToList().ForEach(e => Debug.WriteLine($"Before:{e.Entity.ToString()}:{e.State}"));
    }

    [TestMethod]
    public void EquipmentInNewNinjaGraphGetsNinjaIdViaSaveChanges() {
      var ninja = EF6Model.RichModels.Ninja.CreateIndependent("julie", true);
      ninja.SpecifyClan(new EF6Model.RichModels.Clan { ClanName = "Vermont Ninjas", State = EF6Model.Models.Enums.ObjectStates.Added });
      ninja.AddEquipmentToNinja("arrow");
      using (var context = new NinjaContext()) {
        context.Ninjas.Add(ninja);
        var entries = context.ChangeTracker.Entries();
        OutputState(entries);
        SetupLogging(context);
        context.SaveChanges();
        WriteLog();
      }
      Assert.AreEqual(ninja.Id, ninja.EquipmentOwned.First().NinjaId);
    }

    [TestMethod]
    public void CanMoveEquipmentFromOneNinjaToAnother() {
      SeedForTest();
      List<EF6Model.RichModels.Ninja> ninjas;
      using (var context = new NinjaContext()) {
        ninjas = context.Ninjas.AsNoTracking().Include(n => n.EquipmentOwned).ToList();
      }
      var rivalEquip = ninjas.Find(n => n.Name == "rival").EquipmentOwned.FirstOrDefault();
      var julie = ninjas.Find(n => n.Name == "julie");
      var originalEquipmentCount = julie.EquipmentOwned.Count;
      julie.TransferEquipmentFromAnotherNinja(rivalEquip);
      using (var context = new NinjaContext()) {
        context.Ninjas.Add(julie);
        var entries = context.ChangeTracker.Entries();
        OutputState(entries);
        SetupLogging(context);
        context.SaveChanges();
        WriteLog();

      }
      using (var context = new NinjaContext()) {
        var juliefromDb = context.Ninjas.Include(n => n.EquipmentOwned)
                                         .Where(n => n.Name == "julie")
                                         .FirstOrDefault();
        Assert.AreEqual(originalEquipmentCount + 1, juliefromDb.EquipmentOwned.Count());

      }

    }


    private void SeedForTest() {
      var ninja = EF6Model.RichModels.Ninja.CreateIndependent("julie", true);
      ninja.SpecifyClan(new EF6Model.RichModels.Clan { ClanName = "Vermont Ninjas", State = EF6Model.Models.Enums.ObjectStates.Added });
      ninja.AddEquipmentToNinja("arrow");
      var rival = EF6Model.RichModels.Ninja.CreateIndependent("rival", true);
      rival.SpecifyClan(new EF6Model.RichModels.Clan { ClanName = "TraitorClan", State = EF6Model.Models.Enums.ObjectStates.Added });
      rival.AddEquipmentToNinja("cannon");
      using (var context = new NinjaContext()) {
        context.Ninjas.AddRange(new[] { ninja, rival });
        context.SaveChanges();
      }

    }

    private void WriteLog() {
      Debug.WriteLine(_log);

    }

    private void SetupLogging(NinjaContext context) {
      _logBuilder.Clear();
      context.Database.Log = BuildLogString;
    }

    private void BuildLogString(string message) {
      _logBuilder.Append(message);
      _log = _logBuilder.ToString();
    }
  }
}
