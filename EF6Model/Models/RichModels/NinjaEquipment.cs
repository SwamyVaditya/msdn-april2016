using EF6Model.Models.Enums;
using EF6Model.Models.Interfaces;

namespace EF6Model.RichModels
{
  public class NinjaEquipment: IObjectWithState
  {
    public int Id { get; set; }
    public int NinjaId { get; private set; }
    public string Name { get; set; }
  

    public ObjectStates State { get; set; }
    public static NinjaEquipment Create(int ninjaId, string name) {
      var equip = new NinjaEquipment(ninjaId, name);
      equip.State = ObjectStates.Added;
      return equip;
    }
    public NinjaEquipment(int ninjaId, string name) {
      NinjaId = ninjaId;
      Name = name;
    }
    public NinjaEquipment() {

    }
    public NinjaEquipment ChangeOwner(int newNinjaId) {
      NinjaId = newNinjaId;
      State = ObjectStates.Modified;
      return this;
    }
   
  }
}
