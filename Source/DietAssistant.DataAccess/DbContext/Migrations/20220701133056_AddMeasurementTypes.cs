using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DietAssistant.DataAccess.DbContext.Migrations
{
    public partial class AddMeasurementTypes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Weigth",
                table: "ProgressLogs",
                newName: "Measurement");

            migrationBuilder.AddColumn<int>(
                name: "MeasurementType",
                table: "ProgressLogs",
                type: "int",
                nullable: false,
                defaultValue: 1);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MeasurementType",
                table: "ProgressLogs");

            migrationBuilder.RenameColumn(
                name: "Measurement",
                table: "ProgressLogs",
                newName: "Weigth");
        }
    }
}
