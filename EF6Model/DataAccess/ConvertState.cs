using EF6Model.Models.Enums;
using System.Data.Entity;

namespace EF6WebAPI.DataAccess
{
  public class DataUtilities
  {
    public static EntityState ConvertState(ObjectStates state) {
      switch (state) {
        case ObjectStates.Added:
          return EntityState.Added;
        case ObjectStates.Modified:
          return EntityState.Modified;
        case ObjectStates.Deleted:
          return EntityState.Deleted;
        default:
          return EntityState.Unchanged;
      }
    }
  }
}