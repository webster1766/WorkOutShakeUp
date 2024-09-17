using Microsoft.EntityFrameworkCore;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using WosiDomain;

namespace WosiData
{
  public class ConnectedData
  {
    WosuContext _context;

    public ConnectedData()
    {
      _context = new WosuContext();
      _context.Database.Migrate(); //// will create db if doesn't exist.
    }

    public Movement CreateNewMovement()
    {
      Movement movement = new Movement { Name = "New Movement" };
      _context.Movements.Add(movement);
      return movement;
    }

    public BodyPart CreateNewBodyPart(string bodyPart)
    {
      if (string.IsNullOrWhiteSpace(bodyPart))
      {
        return null;
      }

      BodyPart bp = _context.BodyParts.Where(b => b.Name == bodyPart).FirstOrDefault();
      if (bp == null)
      {
        bp = new BodyPart { Name = bodyPart };
        _context.BodyParts.Add(bp);
      }

      return bp;
    }

    public Equipment CreateNewEquipment(string equip)
    {
      if (string.IsNullOrWhiteSpace(equip))
      {
        return null;
      }

      Equipment eq = _context.Equipments.Where(e => e.Name == equip).FirstOrDefault();
      if (eq == null)
      {
        eq = new Equipment { Name = equip };
        _context.Equipments.Add(eq);
      }

      return eq;
    }

    public BodyPartMovement CreateNewBodyPartMovement(int bodyPartId, int movementId)
    {
      BodyPart bp = _context.BodyParts.Find(bodyPartId);
      if (bp == null)
      {
        throw new Exception("Unable to find Body Part " + bodyPartId.ToString());
      }

      Movement movement = _context.Movements.Find(movementId);
      if (movement == null)
      {
        throw new Exception("Unable to find Movement " + movementId.ToString());
      }

      BodyPartMovement bpm = _context.BodyPartMovements.Find(bodyPartId, movementId);
      if (bpm == null)
      {
        bpm = new BodyPartMovement { BodyPartId = bodyPartId, MovementId = movementId };
        _context.BodyPartMovements.Add(bpm);
      }

      return bpm;
    }

    public MovementEquipment CreateNewMovementEquipment(int equipmentId, int movementId)
    {
      Equipment eq = _context.Equipments.Find(equipmentId);
      if (eq == null)
      {
        throw new Exception("Unable to find Equipment " + equipmentId.ToString());
      }

      Movement movement = _context.Movements.Find(movementId);
      if (movement == null)
      {
        throw new Exception("Unable to find Movement " + movementId.ToString());
      }

      MovementEquipment meq = _context.MovementEquipments.Find(equipmentId, movementId);
      if (meq == null)
      {
        meq = new MovementEquipment { EquipmentId = equipmentId, MovementId = movementId };
        _context.MovementEquipments.Add(meq);
      }

      return meq;
    }

    public ObservableCollection<Movement> GetMovements()
    {
      if (_context.Movements.Local.Count == 0)
      {
        _context.Movements.ToList();
      }

      return _context.Movements.Local.ToObservableCollection();
      ///return new ObservableCollection<Movement>(_context.Movements.Local.OrderBy(m => m.Name));
    }

    public ObservableCollection<BodyPart> GetBodyParts()
    {
      if (_context.BodyParts.Local.Count == 0)
      {
        _context.BodyParts.ToList();
      }

      return new ObservableCollection<BodyPart>(_context.BodyParts.Local.OrderBy(bp => bp.Name));
    }

    public ObservableCollection<BodyPartMovement> GetBodyPartMovements()
    {
      if (_context.BodyPartMovements.Local.Count == 0)
      {
        _context.BodyPartMovements.ToList();
      }

      return new ObservableCollection<BodyPartMovement>(_context.BodyPartMovements.Local.OrderBy(bpm => bpm.BodyPart.Name));
    }

    public ObservableCollection<Equipment> GetEquipment()
    {
      if (_context.Equipments.Local.Count == 0)
      {
        _context.Equipments.ToList();
      }

      return new ObservableCollection<Equipment>(_context.Equipments.Local.OrderBy(eq => eq.Name));
    }

    public ObservableCollection<MovementEquipment> GetMovementEquipments()
    {
      if (_context.MovementEquipments.Local.Count == 0)
      {
        _context.MovementEquipments.ToList();
      }

      return new ObservableCollection<MovementEquipment>(_context.MovementEquipments.Local.OrderBy(meq => meq.Equipment.Name));
    }

    public ObservableCollection<Movement> GetMovementsByBodyPart(int bodyPartId)
    {
      List<Movement> movementList = _context.BodyPartMovements.Where(x => x.BodyPartId == bodyPartId).Select(m => m.Movement).ToList();
      return new ObservableCollection<Movement>(movementList);
    }

    public ObservableCollection<Movement> RandomMovementByBodyPart(int bodyPartId)
    {
      List<Movement> movementList = _context.BodyPartMovements.Where(x => x.BodyPartId == bodyPartId).Select(m => m.Movement).ToList();
      movementList = movementList.RandomSubset(1).ToList();
      return new ObservableCollection<Movement>(movementList);
    }

    public Movement LoadMovement(int movementId)
    {
      Movement movement = _context.Movements.Find(movementId); // gets from tracker if there
      if (movement == null)
      {
        movement = this.CreateNewMovement();
      }

      _context.Entry(movement).Collection(m => m.BodyPartMovements).Load();
      _context.Entry(movement).Collection(m => m.MovementEquipments).Load();
      return movement;
    }

    public bool ChangesMade()
    {
      return _context.ChangeTracker.Entries()
          .Any(e => e.State == EntityState.Added
                  | e.State == EntityState.Modified
                  | e.State == EntityState.Deleted);
    }

    public void SaveChanges()
    {
      _context.SaveChanges();
    }

    /// <summary>
    /// NOTE to give sqlserver account permissions to backUpPath, 
    /// right click in explorer > properties > Shares (And Security)
    /// </summary>
    /// <param name="backUpPath">where to put the bak file.</param>
    /// <returns>full file path of back up.</returns>
    public string BackupDb(string backUpPath)
    {
      _context.Database.UseTransaction(null);
      string fullpath = Path.Combine(backUpPath, "Wosu.bak");
      string command = @"BACKUP DATABASE [Wosu] TO DISK = N'{0}' 
                         WITH NOFORMAT, NOINIT,  
                         NAME = N'Wosu-Full Database Backup', 
                         SKIP, NOREWIND, NOUNLOAD,  STATS = 10";
      string sqlcmd = string.Format(command, fullpath);
      _context.Database.ExecuteSqlCommand(sqlcmd);

      return fullpath;
    }
  }
}
