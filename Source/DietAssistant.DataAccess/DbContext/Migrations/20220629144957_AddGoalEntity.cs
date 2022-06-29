using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DietAssistant.DataAccess.DbContext.Migrations
{
    public partial class AddGoalEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BodyFatPercentage",
                table: "UsersStats");

            migrationBuilder.DropColumn(
                name: "MetricSystem",
                table: "UsersStats");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfBirth",
                table: "UsersStats",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "Gender",
                table: "UsersStats",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "HeightUnit",
                table: "UsersStats",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "WeightUnit",
                table: "UsersStats",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateTable(
                name: "NutritionGoals",
                columns: table => new
                {
                    NutritionGoalId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Calories = table.Column<double>(type: "float", nullable: false),
                    PercentProtein = table.Column<double>(type: "float", nullable: false),
                    PercentCarbs = table.Column<double>(type: "float", nullable: false),
                    PercentFat = table.Column<double>(type: "float", nullable: false),
                    ChangedOnUTC = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NutritionGoals", x => x.NutritionGoalId);
                    table.ForeignKey(
                        name: "FK_NutritionGoals_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Goals",
                columns: table => new
                {
                    GoalId = table.Column<int>(type: "int", nullable: false),
                    StartWeight = table.Column<double>(type: "float", nullable: false),
                    StartDate = table.Column<double>(type: "float", nullable: false),
                    CurrentWeight = table.Column<double>(type: "float", nullable: false),
                    GoalWeight = table.Column<double>(type: "float", nullable: false),
                    WeeklyGoal = table.Column<int>(type: "int", nullable: false),
                    ActivityLevel = table.Column<int>(type: "int", nullable: false),
                    NutritionGoalId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Goals", x => x.GoalId);
                    table.ForeignKey(
                        name: "FK_Goals_NutritionGoals_NutritionGoalId",
                        column: x => x.NutritionGoalId,
                        principalTable: "NutritionGoals",
                        principalColumn: "NutritionGoalId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Goals_Users_GoalId",
                        column: x => x.GoalId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Goals_NutritionGoalId",
                table: "Goals",
                column: "NutritionGoalId");

            migrationBuilder.CreateIndex(
                name: "IX_NutritionGoals_UserId",
                table: "NutritionGoals",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Goals");

            migrationBuilder.DropTable(
                name: "NutritionGoals");

            migrationBuilder.DropColumn(
                name: "DateOfBirth",
                table: "UsersStats");

            migrationBuilder.DropColumn(
                name: "Gender",
                table: "UsersStats");

            migrationBuilder.DropColumn(
                name: "HeightUnit",
                table: "UsersStats");

            migrationBuilder.DropColumn(
                name: "WeightUnit",
                table: "UsersStats");

            migrationBuilder.AddColumn<double>(
                name: "BodyFatPercentage",
                table: "UsersStats",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MetricSystem",
                table: "UsersStats",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
