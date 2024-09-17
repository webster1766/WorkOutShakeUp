using Microsoft.EntityFrameworkCore.Migrations;

namespace WosiData.Migrations
{
    public partial class bpmDbSet : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BodyPartMovement_BodyParts_BodyPartId",
                table: "BodyPartMovement");

            migrationBuilder.DropForeignKey(
                name: "FK_BodyPartMovement_Movements_MovementId",
                table: "BodyPartMovement");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BodyPartMovement",
                table: "BodyPartMovement");

            migrationBuilder.RenameTable(
                name: "BodyPartMovement",
                newName: "BodyPartMovements");

            migrationBuilder.RenameIndex(
                name: "IX_BodyPartMovement_MovementId",
                table: "BodyPartMovements",
                newName: "IX_BodyPartMovements_MovementId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BodyPartMovements",
                table: "BodyPartMovements",
                columns: new[] { "BodyPartId", "MovementId" });

            migrationBuilder.AddForeignKey(
                name: "FK_BodyPartMovements_BodyParts_BodyPartId",
                table: "BodyPartMovements",
                column: "BodyPartId",
                principalTable: "BodyParts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BodyPartMovements_Movements_MovementId",
                table: "BodyPartMovements",
                column: "MovementId",
                principalTable: "Movements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BodyPartMovements_BodyParts_BodyPartId",
                table: "BodyPartMovements");

            migrationBuilder.DropForeignKey(
                name: "FK_BodyPartMovements_Movements_MovementId",
                table: "BodyPartMovements");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BodyPartMovements",
                table: "BodyPartMovements");

            migrationBuilder.RenameTable(
                name: "BodyPartMovements",
                newName: "BodyPartMovement");

            migrationBuilder.RenameIndex(
                name: "IX_BodyPartMovements_MovementId",
                table: "BodyPartMovement",
                newName: "IX_BodyPartMovement_MovementId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BodyPartMovement",
                table: "BodyPartMovement",
                columns: new[] { "BodyPartId", "MovementId" });

            migrationBuilder.AddForeignKey(
                name: "FK_BodyPartMovement_BodyParts_BodyPartId",
                table: "BodyPartMovement",
                column: "BodyPartId",
                principalTable: "BodyParts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BodyPartMovement_Movements_MovementId",
                table: "BodyPartMovement",
                column: "MovementId",
                principalTable: "Movements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
