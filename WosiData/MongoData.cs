using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using MoreLinq;
using WosiDomain.MongoDocs;

namespace WosiData
{
  public class MongoData
  {
    MongoContext context;

    public MongoData()
    {
      this.context = new MongoContext();
    }

    /// <summary>
    /// creates MovementDoc object with name of 'New Movement'
    /// </summary>
    /// <returns>new MovementDoc object.</returns>
    public MovementDoc CreateNewMovement()
    {
      var movement = new MovementDoc { Name = "New Movement" };
      this.context.Movements.InsertOne(movement);
      return movement;
    }

    public BodyPartDoc CreateNewBodyPart(string bodyPart)
    {
      if (string.IsNullOrWhiteSpace(bodyPart))
      {
        return null;
      }

      BodyPartDoc bpd = this.context.BodyParts
                                    .AsQueryable()
                                    .Where(b => b.Name.ToLower() == bodyPart.ToLower())
                                    .FirstOrDefault();

      if (bpd is null)
      {
        bpd = new BodyPartDoc { Name = bodyPart };
        this.context.BodyParts.InsertOne(bpd);
      }

      return bpd;
    }

    public EquipmentDoc CreateNewEquipment(string equip)
    {
      if (string.IsNullOrWhiteSpace(equip))
      {
        return null;
      }

      EquipmentDoc eq = this.context.Equipment
                                    .AsQueryable()
                                    .Where(e => e.Description.ToLower() == equip.ToLower())
                                    .FirstOrDefault();

      if (eq == null)
      {
        eq = new EquipmentDoc { Description = equip };
        this.context.Equipment.InsertOne(eq);
      }

      return eq;
    }

    public MovementDoc AddBodyPartToMovement(string bodyPartId, string movementId)
    {
      var bp = this.context.BodyParts.AsQueryable().Where(b => b.Id == bodyPartId).FirstOrDefault();
      if (bp == null)
      {
        throw new Exception($"Unable to find Body Part {bodyPartId}");
      }

      var move = this.context.Movements.AsQueryable().Where(m => m.Id == movementId).FirstOrDefault();
      if (move == null)
      {
        throw new Exception($"Unable to find Movement {movementId}");
      }

      if (move.BodyParts.Any(b => b.Id == bodyPartId) == false)
      {
        move.BodyParts.Add(bp);
        this.context.Movements.ReplaceOne(Builders<MovementDoc>.Filter.Where(m => m.Id == move.Id), move);
      }

      return move;
    }

    public MovementDoc AddEquipmentToMovement(string equipmentId, string movementId)
    {
      var equipment = this.context.Equipment.AsQueryable().Where(e => e.Id == equipmentId).FirstOrDefault();
      if (equipment == null)
      {
        throw new Exception($"Unable to find Equipment {equipmentId}");
      }

      var move = this.context.Movements.AsQueryable().Where(m => m.Id == movementId).FirstOrDefault();
      if (move == null)
      {
        throw new Exception($"Unable to find Movement {movementId}");
      }

      if (move.Equipment.Any(m => m.Id == equipmentId) == false)
      {
        move.Equipment.Add(equipment);
        this.context.Movements.ReplaceOne(Builders<MovementDoc>.Filter.Where(m => m.Id == move.Id), move);
      }

      return move;
    }

    public MovementDoc UpdateMovement(MovementDoc movement)
    {
      this.context.Movements.ReplaceOne(Builders<MovementDoc>.Filter.Where(m => m.Id == movement.Id), movement);

      return movement;
    }

    public ObservableCollection<MovementDoc> GetMovements()
    {
      return new ObservableCollection<MovementDoc>(this.context.Movements.AsQueryable().ToList());
    }

    public ObservableCollection<BodyPartDoc> GetBodyParts()
    {
      return new ObservableCollection<BodyPartDoc>(this.context.BodyParts.AsQueryable().ToList());
    }

    public ObservableCollection<EquipmentDoc> GetEquipment()
    {
      return new ObservableCollection<EquipmentDoc>(this.context.Equipment.AsQueryable().ToList());
    }

    public MovementDoc GetMovement(string id)
    {
      return this.GetMovements().Where(m => m.Id == id).FirstOrDefault();
    }

    public ObservableCollection<MovementDoc> GetMovementsByBodyPart(string bodyPartId)
    {
      var moves = new List<MovementDoc>();
      foreach (MovementDoc doc in this.context.Movements.AsQueryable())
      {
        if (doc.BodyParts.Any(b => b.Id == bodyPartId))
        {
          moves.Add(doc);
        }
      }
      //var moves = this.context.Movements.AsQueryable().SelectMany(m => m.BodyParts.Where(b => b.Id == bodyPartId), (m, b) => m);
      // var movements = this.GetMovements().SelectMany(m => m.BodyParts.Where(b => b.Id == bodyPartId), (m, b) => m);
      return new ObservableCollection<MovementDoc>(moves);
    }

    public ObservableCollection<MovementDoc> RandomMovementByBodyPart(string bodyPartId)
    {
      var moves = this.GetMovementsByBodyPart(bodyPartId);
      ////var moves = this.context.Movements
      ////                        .AsQueryable()
      ////                        .SelectMany(m => m.BodyParts.Where(b => b.Id == bodyPartId), (m, b) => m)
      ////                        .ToList()
      ////                        .RandomSubset(1);
      return new ObservableCollection<MovementDoc>(moves.RandomSubset(1));
    }

    public string BackupDb(string path)
    {
      return string.Empty;
    }
  }
}
