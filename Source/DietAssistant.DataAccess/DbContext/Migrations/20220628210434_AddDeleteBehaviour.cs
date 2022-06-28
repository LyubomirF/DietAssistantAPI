using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DietAssistant.DataAccess.DbContext.Migrations
{
    public partial class AddDeleteBehaviour : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FoodPlans_MealsPlan_MealPlanId",
                table: "FoodPlans");

            migrationBuilder.DropForeignKey(
                name: "FK_MealsPlan_DietPlans_DietPlanId",
                table: "MealsPlan");

            migrationBuilder.AddForeignKey(
                name: "FK_FoodPlans_MealsPlan_MealPlanId",
                table: "FoodPlans",
                column: "MealPlanId",
                principalTable: "MealsPlan",
                principalColumn: "MealPlanId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MealsPlan_DietPlans_DietPlanId",
                table: "MealsPlan",
                column: "DietPlanId",
                principalTable: "DietPlans",
                principalColumn: "DietPlanId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FoodPlans_MealsPlan_MealPlanId",
                table: "FoodPlans");

            migrationBuilder.DropForeignKey(
                name: "FK_MealsPlan_DietPlans_DietPlanId",
                table: "MealsPlan");

            migrationBuilder.AddForeignKey(
                name: "FK_FoodPlans_MealsPlan_MealPlanId",
                table: "FoodPlans",
                column: "MealPlanId",
                principalTable: "MealsPlan",
                principalColumn: "MealPlanId");

            migrationBuilder.AddForeignKey(
                name: "FK_MealsPlan_DietPlans_DietPlanId",
                table: "MealsPlan",
                column: "DietPlanId",
                principalTable: "DietPlans",
                principalColumn: "DietPlanId");
        }
    }
}
