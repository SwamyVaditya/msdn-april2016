using EF6Model.Models;
using System.Data.Entity;

namespace EF6Model
{
    public class NinjaContextDefaultBehavior : DbContext
  {
    public DbSet<Ninja> Ninjas { get; set; }
    public DbSet<Clan> Clans { get; set; }

   
  }
}