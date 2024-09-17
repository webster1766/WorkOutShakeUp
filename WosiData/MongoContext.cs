using MongoDB.Driver;
using System.Configuration;
using WosiData.Properties;
using WosiDomain.MongoDocs;

namespace WosiData
{
  public class MongoContext
  {
    private IMongoDatabase mongoDb;

    public MongoContext()
    {
      string connectionStr = ConfigurationManager.ConnectionStrings["MongoDbConnStr"].ToString();
      if (string.IsNullOrWhiteSpace(connectionStr))
      {
        connectionStr = Settings.Default.WosuConnectionString;
      }

      string dbName = ConfigurationManager.ConnectionStrings["MongoDbName"].ToString();
      if (string.IsNullOrWhiteSpace(dbName))
      {
        dbName = Settings.Default.WosuDbName;
      }

      var client = new MongoClient(connectionStr);
      this.mongoDb = client.GetDatabase(dbName);
    }

    public IMongoCollection<MovementDoc> Movements
    {
      get
      {
        return this.mongoDb.GetCollection<MovementDoc>("movements");
      }
    }

    public IMongoCollection<BodyPartDoc> BodyParts
    {
      get
      {
        return this.mongoDb.GetCollection<BodyPartDoc>("bodyparts");
      }
    }

    public IMongoCollection<EquipmentDoc> Equipment
    {
      get
      {
        return this.mongoDb.GetCollection<EquipmentDoc>("equipment");
      }
    }
  }
}
