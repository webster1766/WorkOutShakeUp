namespace WosiDomain
{
  public class BodyPartMovement
  {
    public int BodyPartId { get; set; }
    public BodyPart BodyPart { get; set; }
    public int MovementId { get; set; }
    public Movement Movement { get; set; }
  }
}
