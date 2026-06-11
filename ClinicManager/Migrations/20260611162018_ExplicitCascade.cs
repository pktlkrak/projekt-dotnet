using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinicManager.Migrations
{
    /// <inheritdoc />
    public partial class ExplicitCascade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PerscriptionItems_Perscriptions_PerscriptionId",
                table: "PerscriptionItems");

            migrationBuilder.AddForeignKey(
                name: "FK_PerscriptionItems_Perscriptions_PerscriptionId",
                table: "PerscriptionItems",
                column: "PerscriptionId",
                principalTable: "Perscriptions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PerscriptionItems_Perscriptions_PerscriptionId",
                table: "PerscriptionItems");

            migrationBuilder.AddForeignKey(
                name: "FK_PerscriptionItems_Perscriptions_PerscriptionId",
                table: "PerscriptionItems",
                column: "PerscriptionId",
                principalTable: "Perscriptions",
                principalColumn: "Id");
        }
    }
}
