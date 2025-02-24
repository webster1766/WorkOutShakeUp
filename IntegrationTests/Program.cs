using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WosiDomain.MongoDocs;

namespace IntegrationTests
{
  class Program
  {
    static IMongoDatabase mongoDb;

    static void Main(string[] args)
    {
      var client = new MongoClient("mongodb://localhost:27017");
      mongoDb = client.GetDatabase("wosu");
      var movements = mongoDb.GetCollection<MovementDoc>("movements");
      var bodyparts = mongoDb.GetCollection<BodyPartDoc>("bodyparts");
      var equipment = mongoDb.GetCollection<EquipmentDoc>("equipment");

      //await collection.InsertOneAsync(new BsonDocument("Name", "Jack"));

      //var list = await collection.Find(new BsonDocument("Name", "Jack"))
      //    .ToListAsync();

      var query = bodyparts.AsQueryable();
      var list = query.ToList();
      foreach (var document in list)
      {
        Console.WriteLine(document.Name);
      }

      Console.WriteLine("Press [Enter] to exit");
      Console.ReadLine();
    }

    public IMongoCollection<MovementDoc> Movements
    {
      get
      {
        return mongoDb.GetCollection<MovementDoc>("movements");
      }
    }

    public IMongoCollection<BodyPartDoc> BodyParts
    {
      get
      {
        return mongoDb.GetCollection<BodyPartDoc>("bodyparts");
      }
    }

    public IMongoCollection<EquipmentDoc> Equipment
    {
      get
      {
        return mongoDb.GetCollection<EquipmentDoc>("equipment");
      }
    }

  }
}
