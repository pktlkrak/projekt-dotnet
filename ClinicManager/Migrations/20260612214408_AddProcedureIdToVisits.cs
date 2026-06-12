using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinicManager.Migrations
{
    /// <inheritdoc />
    public partial class AddProcedureIdToVisits : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProcedureId",
                table: "Visits",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Visits_ProcedureId",
                table: "Visits",
                column: "ProcedureId");

            migrationBuilder.AddForeignKey(
                name: "FK_Visits_Procedures_ProcedureId",
                table: "Visits",
                column: "ProcedureId",
                principalTable: "Procedures",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Visits_Procedures_ProcedureId",
                table: "Visits");

            migrationBuilder.DropIndex(
                name: "IX_Visits_ProcedureId",
                table: "Visits");

            migrationBuilder.DropColumn(
                name: "ProcedureId",
                table: "Visits");
        }
    }
}
