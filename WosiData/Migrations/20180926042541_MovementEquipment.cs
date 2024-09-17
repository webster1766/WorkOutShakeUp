using Microsoft.EntityFrameworkCore.Migrations;

namespace WosiData.Migrations
{
    public partial class MovementEquipment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Equipments_Movements_MovementId",
                table: "Equipments");

            migrationBuilder.DropIndex(
                name: "IX_Equipments_MovementId",
                table: "Equipments");

            migrationBuilder.DropColumn(
                name: "MovementId",
                table: "Equipments");

            migrationBuilder.CreateTable(
                name: "MovementEquipments",
                columns: table => new
                {
                    MovementId = table.Column<int>(nullable: false),
                    EquipmentId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovementEquipments", x => new { x.MovementId, x.EquipmentId });
                    table.ForeignKey(
                        name: "FK_MovementEquipments_Equipments_EquipmentId",
                        column: x => x.EquipmentId,
                        principalTable: "Equipments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MovementEquipments_Movements_MovementId",
                        column: x => x.MovementId,
                        principalTable: "Movements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MovementEquipments_EquipmentId",
                table: "MovementEquipments",
                column: "EquipmentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MovementEquipments");

            migrationBuilder.AddColumn<int>(
                name: "MovementId",
                table: "Equipments",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Equipments_MovementId",
                table: "Equipments",
                column: "MovementId");

            migrationBuilder.AddForeignKey(
                name: "FK_Equipments_Movements_MovementId",
                table: "Equipments",
                column: "MovementId",
                principalTable: "Movements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
