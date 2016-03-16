
using EF6Model.Models.Enums;

namespace EF6Model.Models.Interfaces

{
  public interface IObjectWithState
  {
    ObjectStates State { get; set; }
  }
  
}

