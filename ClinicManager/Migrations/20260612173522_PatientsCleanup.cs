using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinicManager.Migrations
{
    /// <inheritdoc />
    public partial class PatientsCleanup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Procedures_Visits_VisitId",
                table: "Procedures");

            migrationBuilder.DropIndex(
                name: "IX_Procedures_VisitId",
                table: "Procedures");

            migrationBuilder.DropColumn(
                name: "VisitId",
                table: "Procedures");

            migrationBuilder.AddColumn<int>(
                name: "ProcedureId",
                table: "Visits",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<DateOnly>(
                name: "DateOfBirth",
                table: "Patients",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

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

            migrationBuilder.AddColumn<int>(
                name: "VisitId",
                table: "Procedures",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateOfBirth",
                table: "Patients",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.CreateIndex(
                name: "IX_Procedures_VisitId",
                table: "Procedures",
                column: "VisitId");

            migrationBuilder.AddForeignKey(
                name: "FK_Procedures_Visits_VisitId",
                table: "Procedures",
                column: "VisitId",
                principalTable: "Visits",
                principalColumn: "Id");
        }
    }
}
