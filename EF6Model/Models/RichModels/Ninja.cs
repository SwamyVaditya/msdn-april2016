using System;
using System.Collections.Generic;
using EF6Model.Models.Enums;
using EF6Model.Models.Interfaces;
using System.Runtime.CompilerServices;


namespace EF6Model.RichModels
{

  public class Ninja : IObjectWithState
{
  public static Ninja CreateIndependent(string name, bool servedinOniwaban) {
    var ninja = new Ninja(name, servedinOniwaban);
    ninja.State = ObjectStates.Added;
    return ninja;
  }
  public static Ninja CreateBoundToClan(string name, bool servedinOniwaban, int clanId) {
    var ninja = new Ninja(name, servedinOniwaban);
    ninja.ClanId = clanId;
    ninja.State = ObjectStates.Added;
    return ninja;
  }
    private Ninja() {   }
  public Ninja(string name, bool servedinOniwaban) {
    EquipmentOwned = new List<NinjaEquipment>();
    Name = name;
    ServedInOniwaban = servedinOniwaban;
  }

  public int Id { get; private set; }
  public string Name { get; private set; }
  public bool ServedInOniwaban { get; private set; }
  public Clan Clan { get; private set; }
  public int ClanId { get; private set; }
  public List<NinjaEquipment> EquipmentOwned { get; private set; }
  public ObjectStates State { get; set; }

  public void ModifyOniwabanStatus(bool served) {
    ServedInOniwaban = served;
    SetModifedIfNotAdded();
  }
  private void SetModifedIfNotAdded() {
    if (State != ObjectStates.Added) {
      State = ObjectStates.Modified;
    }
  }
  public void SpecifyClan(Clan clan) {
    Clan = clan;
    ClanId = clan.Id;
    SetModifedIfNotAdded();
  }
  public void SpecifyClan(int id) {
    ClanId = id;
    SetModifedIfNotAdded();
  }
  public void AddEquipmentToNinja(string equipmentName) {
    EquipmentOwned.Add(NinjaEquipment.Create(Id, equipmentName));
  }
    public void TransferEquipmentFromAnotherNinja(NinjaEquipment equipment) {
      EquipmentOwned.Add(equipment.ChangeOwner(Id));
    }
    public void EquipmentNoLongerExists(NinjaEquipment equipment) {
      equipment.State = ObjectStates.Deleted;
    }


  }
}