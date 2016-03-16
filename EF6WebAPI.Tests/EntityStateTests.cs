using Microsoft.VisualStudio.TestTools.UnitTesting;

using EF6Model;
using System.Data.Entity;
using System.Linq;
using System.Diagnostics;

using EF6Model.Models;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;

namespace EF6WebAPI.Tests
{
  [TestClass]
  public class ContextEntityStateTests
  {
    public ContextEntityStateTests() {
      Database.SetInitializer(new NullDatabaseInitializer<NinjaContext>());

    }
    [TestMethod]
    public void AddSingleUntrackedEntityToContextResultsinAddedState() {
      var ninja = new Ninja();
      using (var context = new NinjaContextDefaultBehavior()) {
        context.Ninjas.Add(ninja);
        Assert.AreEqual(EntityState.Added, context.ChangeTracker.Entries().FirstOrDefault().State);
      }
    }
    [TestMethod]
    public void AddUntrackedGraphOfNewEntitiesResultsinAddedStateOnAll() {
      var ninja = new Ninja();
      ninja.EquipmentOwned.Add(new NinjaEquipment());
      using (var context = new NinjaContextDefaultBehavior()) {
        context.Ninjas.Add(ninja);
        var entries = context.ChangeTracker.Entries();
        entries.ToList().ForEach(e => Debug.WriteLine($"{e.Entity.ToString()}:{e.State}"));
        CollectionAssert.AreEqual(new[] { EntityState.Added, EntityState.Added }, entries.Select(e => e.State).ToArray());
      }
    }
    [TestMethod]
    public void EFDoesNotComprehendsMixedStatesWhenAddingUntrackedGraph() {
      var ninja = new Ninja();
      ninja.Clan = new Clan { Id = 1 };
      using (var context = new NinjaContextDefaultBehavior()) {
        context.Ninjas.Add(ninja);
        var entries = context.ChangeTracker.Entries();
        entries.ToList().ForEach(e => Debug.WriteLine($"{e.Entity.ToString()}:{e.State}"));
        Assert.IsFalse(entries.Any(e => e.State != EntityState.Added));
      }
    }


    [TestMethod]
    public void EFComprehendsMixedStatesWhenAddingUntrackedGraph() {
      var ninja = EF6Model.RichModels.Ninja.CreateIndependent("julie", true);
      ninja.SpecifyClan(new EF6Model.RichModels.Clan { Id = 1, ClanName = "Clan from database" });
      using (var context = new NinjaContext()) {
        context.Ninjas.Add(ninja);
        var entries = context.ChangeTracker.Entries();
        OutputState(entries);
        context.FixState();
        OutputState(entries);
        Assert.IsTrue(entries.Any(e => e.State == EntityState.Unchanged));

      }
    }
    [TestMethod]
    public void MixedStatesWithExistingParentAndVaryingChildrenisUnderstood() {
      var ninja = EF6Model.RichModels.Ninja.CreateIndependent("julie", true);
      var pNinja =new PrivateObject(ninja);
      pNinja.SetProperty("Id", 1);
      var originalOwnerId = 99;
      var equip = EF6Model.RichModels.NinjaEquipment.Create(originalOwnerId, "arrow");
      ninja.TransferEquipmentFromAnotherNinja(equip);
      using (var context = new NinjaContext()) {
        context.Ninjas.Attach(ninja);
        var entries = context.ChangeTracker.Entries();
        OutputState(entries);
        context.FixState();
        OutputState(entries);
        Assert.IsTrue(entries.Any(e => e.State == EntityState.Modified));

      }
    }

    private static void OutputState(IEnumerable<DbEntityEntry> entries) {
      entries.ToList().ForEach(e => Debug.WriteLine($"Before:{e.Entity.ToString()}:{e.State}"));
    }
  }


}