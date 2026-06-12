using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinicManager.Migrations
{
    /// <inheritdoc />
    public partial class AddProcedureRefs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Visits_Procedures_ProcedureId",
                table: "Visits");

            migrationBuilder.DropIndex(
                name: "IX_Visits_ProcedureId",
                table: "Visits");

            migrationBuilder.DropColumn(
                name: "Cost",
                table: "Visits");

            migrationBuilder.DropColumn(
                name: "ProcedureId",
                table: "Visits");

            migrationBuilder.CreateTable(
                name: "ProcedureRefs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VisitId = table.Column<int>(type: "int", nullable: false),
                    ProcedureId = table.Column<int>(type: "int", nullable: false),
                    Cost = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcedureRefs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProcedureRefs_Procedures_ProcedureId",
                        column: x => x.ProcedureId,
                        principalTable: "Procedures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProcedureRefs_Visits_VisitId",
                        column: x => x.VisitId,
                        principalTable: "Visits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProcedureRefs_ProcedureId",
                table: "ProcedureRefs",
                column: "ProcedureId");

            migrationBuilder.CreateIndex(
                name: "IX_ProcedureRefs_VisitId",
                table: "ProcedureRefs",
                column: "VisitId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProcedureRefs");

            migrationBuilder.AddColumn<double>(
                name: "Cost",
                table: "Visits",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

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
    }
}
