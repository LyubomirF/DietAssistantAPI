using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DietAssistant.DataAccess.DbContext.Migrations
{
    public partial class AddActivityLevel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ActivityLevel",
                table: "Goals",
                type: "int",
                nullable: false,
                defaultValue: 1);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActivityLevel",
                table: "Goals");
        }
    }
}
