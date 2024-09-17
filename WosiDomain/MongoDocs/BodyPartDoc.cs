using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WosiDomain.MongoDocs
{
  public class BodyPartDoc
  {
    public BodyPartDoc()
    {
    }

    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    public string Name { get; set; }
  }
}
