using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Configuration;
using WosiDomain;

namespace WosiData
{
  /// <summary>
  /// verify pm console package source is nuget.org and default project is your DbContext project
  /// PM> get-help entityframeworkcore // to get list of commands
  /// PM> Add-Migration <<migration_name>>, ie initial // this is need after 1st and subsequent changes to the data model.
  /// PM> script-migration // to see sql scripts.
  /// PM> update-database -verbose // to create or update db
  /// </summary>
  public class WosuContext : DbContext
  {
    public LoggerFactory FileLoggerFactory;
    public DbSet<BodyPart> BodyParts { get; set; }

    public DbSet<Equipment> Equipments { get; set; }

    public DbSet<MovementEquipment> MovementEquipments { get; set; }

    public DbSet<Movement> Movements { get; set; }

    public DbSet<BodyPartMovement> BodyPartMovements { get; set; }

    public WosuContext()
    {
      FileLoggerFactory = new LoggerFactory();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
      string connectionString = ConfigurationManager.ConnectionStrings["SqlDbConnStr"].ToString();

      string logLocation = ConfigurationManager.AppSettings["LogFile"].ToString();
      logLocation = string.Format(logLocation, DateTime.Now.ToString("yyyyMMddhhmmss"));
      FileLoggerFactory.AddProvider(new LoggerFileProvider(logLocation));

      optionsBuilder
          .UseLoggerFactory(FileLoggerFactory)
          .EnableSensitiveDataLogging(true)
          .UseSqlServer(connectionString);
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      //// needed cuz many-to-many mapping
      //// doesn't follow ef core's conventions.
      modelBuilder.Entity<BodyPartMovement>()
          .HasKey(bpm => new { bpm.BodyPartId, bpm.MovementId });

      modelBuilder.Entity<MovementEquipment>()
        .HasKey(me => new { me.MovementId, me.EquipmentId });

      base.OnModelCreating(modelBuilder);
    }
  }
}
