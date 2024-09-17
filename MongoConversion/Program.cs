using System.Collections.Generic;
using System.Linq;
using WosiData;
using WosiDomain.MongoDocs;

namespace MongoConversion
{
  class Program
  {
    static readonly ConnectedData _sqlData = new ConnectedData();
    static readonly MongoData _mongoData = new MongoData();

    static void Main(string[] args)
    {
      var listBodyParts = new List<dynamic>();
      var bodyparts = _sqlData.GetBodyParts();
      for (int i = 0; i < bodyparts.Count(); ++i)
      {
        var bodyPartDoc = _mongoData.CreateNewBodyPart(bodyparts[i].Name);
        var temp = new
        {
          oldId = bodyparts[i].Id,
          newId = bodyPartDoc.Id,
          name = bodyPartDoc.Name
        };

        listBodyParts.Add(temp);
      }

      var listEquipment = new List<dynamic>();
      var equipment = _sqlData.GetEquipment();
      for (int i = 0; i < equipment.Count(); ++i)
      {
        var equipDoc = _mongoData.CreateNewEquipment(equipment[i].Name);
        var temp = new
        {
          oldId = equipment[i].Id,
          newId = equipDoc.Id,
          desc = equipDoc.Description
        };

        listEquipment.Add(temp);
      }

      var movebodyparts = _sqlData.GetBodyPartMovements();
      var moveequipment = _sqlData.GetMovementEquipments();

      var movements = _sqlData.GetMovements();

      for (int i = 0; i < movements.Count(); ++i)
      {
        MovementDoc newmove = _mongoData.CreateNewMovement();
        // update with name and notes
        newmove.Name = movements[i].Name;
        newmove.Notes = movements[i].Notes;
        newmove = _mongoData.UpdateMovement(newmove);

        // find body parts tied to current movement
        var bpIds = movebodyparts.Where(mb => mb.MovementId == movements[i].Id).Select(x => x.BodyPartId);
        var bpsToAdd = listBodyParts.Where(x => bpIds.ToList().Contains(x.oldId)).ToList();
        // add body part with new id to new movement
        bpsToAdd.ForEach(x => _mongoData.AddBodyPartToMovement(x.newId, newmove.Id));

        // find equipment tied to current movement
        var eqIds = moveequipment.Where(me => me.MovementId == movements[i].Id).Select(x => x.EquipmentId);
        var equipsToAdd = listEquipment.Where(x => eqIds.ToList().Contains(x.oldId)).ToList();
        // add equipment with new id to new movement
        equipsToAdd.ForEach(x => _mongoData.AddEquipmentToMovement(x.newId, newmove.Id));
      }
    }
  }
}
