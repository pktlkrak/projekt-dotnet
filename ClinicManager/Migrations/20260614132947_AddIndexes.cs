using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinicManager.Migrations
{
    /// <inheritdoc />
    public partial class AddIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Visits_DoctorId",
                table: "Visits");

            migrationBuilder.AlterColumn<string>(
                name: "Pesel",
                table: "Patients",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Visits_DoctorId_ScheduledAt",
                table: "Visits",
                columns: new[] { "DoctorId", "ScheduledAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Patients_Pesel",
                table: "Patients",
                column: "Pesel");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Visits_DoctorId_ScheduledAt",
                table: "Visits");

            migrationBuilder.DropIndex(
                name: "IX_Patients_Pesel",
                table: "Patients");

            migrationBuilder.AlterColumn<string>(
                name: "Pesel",
                table: "Patients",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateIndex(
                name: "IX_Visits_DoctorId",
                table: "Visits",
                column: "DoctorId");
        }
    }
}
