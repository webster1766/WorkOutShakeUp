using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace WosiDomain.MongoDocs
{
  public class MovementDoc
  {
    public MovementDoc()
    {
    }

    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    /// <summary>
    /// name of the movement
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// supplemental notes for the movement
    /// </summary>
    public string Notes { get; set; }

    /// <summary>
    /// body part involved with the movement
    /// </summary>
    public List<BodyPartDoc> BodyParts = new List<BodyPartDoc>();

    /// <summary>
    /// equipment used for the movement
    /// </summary>
    public List<EquipmentDoc> Equipment = new List<EquipmentDoc>();
  }
}
