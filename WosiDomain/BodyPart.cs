using System.Collections.Generic;

namespace WosiDomain
{
  public class BodyPart
  {
    public BodyPart()
    {
    }

    public int Id { get; set; }

    public string Name { get; set; }

    public List<BodyPartMovement> BodyPartMovements { get; set; }
  }
}
