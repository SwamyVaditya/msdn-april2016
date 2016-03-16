
namespace EF6Model.Models
{
  public class NinjaEquipment
  {
    public int Id { get; set; }
    public int NinjaId { get; private set; }
    public string Name { get; set; }

  }
}
