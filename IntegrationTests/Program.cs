using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using WosiDomain;
using WosiDomain.MongoDocs;
using static WosiDomain.Shared;

namespace IntegrationTests
{
  class Program
  {
    static IMongoDatabase mongoDb;

    static void Main(string[] args)
    {
      var client = new MongoClient("mongodb://localhost:27017");
      mongoDb = client.GetDatabase(DB_NAME);

      //await collection.InsertOneAsync(new BsonDocument("Name", "Jack"));

      //var list = await collection.Find(new BsonDocument("Name", "Jack"))
      //    .ToListAsync();

      Console.WriteLine("MOVEMENTS");
      var movements = mongoDb.GetCollection<MovementDoc>("movements");
      var mlist = GetList<MovementDoc>(movements);
      PrintList(mlist);

      Console.WriteLine("BODYPARTS");
      var bodyparts = mongoDb.GetCollection<BodyPartDoc>("bodyparts");
      var blist = GetList(bodyparts);
      PrintList(blist);

      Console.WriteLine("EQUIPMENT");
      var equipment = mongoDb.GetCollection<EquipmentDoc>("equipment");
      var elist = GetList(equipment);
      PrintList(elist);

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

    private static List<T> GetList<T>(IMongoCollection<T> collection)
    {
      var query = collection.AsQueryable();
      var list = query.ToList();

      return list;
    }

    private static void PrintList<T>(List<T> list)
    {
      list.ForEach(x =>
      {
        if (typeof(T) == typeof(MovementDoc))
        {
          var doc = x as MovementDoc;
          Console.WriteLine(doc.Name);
        }
        else if (typeof(T) == typeof(BodyPartDoc))
        {
          var doc = x as BodyPartDoc;
          Console.WriteLine(doc.Name);
        }
        else if (typeof(T) == typeof(EquipmentDoc))
        {
          var doc = x as EquipmentDoc;
          Console.WriteLine(doc.Description);
        }
      });

      Console.WriteLine();
    }
  }
}
