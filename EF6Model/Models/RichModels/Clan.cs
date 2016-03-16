using System.Collections.Generic;
using EF6Model.Models.Enums;
using EF6Model.Models.Interfaces;

namespace EF6Model.RichModels
{
  public class Clan :IObjectWithState
  {
    public Clan() {
      Ninjas = new List<Ninja>();
    }
    public int Id { get; set; }
    public string ClanName { get; set; }
    public List<Ninja> Ninjas { get; set; }

    public ObjectStates State { get; set; }
  }
}