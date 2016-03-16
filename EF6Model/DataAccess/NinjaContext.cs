using EF6WebAPI.DataAccess;
using EF6Model.Models.Interfaces;
using EF6Model.RichModels;
using System.Data.Entity;

namespace EF6Model
{
public class NinjaContext : DbContext
{
  public DbSet<Ninja> Ninjas { get; set; }
  public DbSet<Clan> Clans { get; set; }

  public void FixState() {
    foreach (var entry in ChangeTracker.Entries<IObjectWithState>()) {
      IObjectWithState stateInfo = entry.Entity;
      entry.State = DataUtilities.ConvertState(stateInfo.State);
    }
  }
    protected override void OnModelCreating(DbModelBuilder modelBuilder) {
       modelBuilder.Types<IObjectWithState>().Configure(c => c.Ignore(p=>p.State));
    }
  }
}