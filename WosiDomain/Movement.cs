using System.Collections.Generic;

namespace WosiDomain
{
  public class Movement
  {
    public Movement()
    {
    }

    public int Id { get; set; }

    public string Name { get; set; }

    public string Notes { get; set; }

    public List<MovementEquipment> MovementEquipments { get; set; }

    public List<BodyPartMovement> BodyPartMovements { get; set; }
  }
}
