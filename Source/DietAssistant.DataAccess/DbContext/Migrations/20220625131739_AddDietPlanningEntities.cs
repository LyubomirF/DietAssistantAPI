﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DietAssistant.DataAccess.DbContext.Migrations
{
    public partial class AddDietPlanningEntities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DietPlans",
                columns: table => new
                {
                    DietPlanId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlanName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DietPlans", x => x.DietPlanId);
                    table.ForeignKey(
                        name: "FK_DietPlans_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DayPlans",
                columns: table => new
                {
                    DayPlanId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DayOfWeek = table.Column<int>(type: "int", nullable: false),
                    DietPlanId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DayPlans", x => x.DayPlanId);
                    table.ForeignKey(
                        name: "FK_DayPlans_DietPlans_DietPlanId",
                        column: x => x.DietPlanId,
                        principalTable: "DietPlans",
                        principalColumn: "DietPlanId");
                });

            migrationBuilder.CreateTable(
                name: "MealsPlan",
                columns: table => new
                {
                    MealPlanId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MealPlanName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Time = table.Column<TimeSpan>(type: "time", nullable: false),
                    DayPlanId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MealsPlan", x => x.MealPlanId);
                    table.ForeignKey(
                        name: "FK_MealsPlan_DayPlans_DayPlanId",
                        column: x => x.DayPlanId,
                        principalTable: "DayPlans",
                        principalColumn: "DayPlanId");
                });

            migrationBuilder.CreateTable(
                name: "FoodPlans",
                columns: table => new
                {
                    FoodPlanId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FoodId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ServingSize = table.Column<double>(type: "float", nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MealPlanId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FoodPlans", x => x.FoodPlanId);
                    table.ForeignKey(
                        name: "FK_FoodPlans_MealsPlan_MealPlanId",
                        column: x => x.MealPlanId,
                        principalTable: "MealsPlan",
                        principalColumn: "MealPlanId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_DayPlans_DietPlanId",
                table: "DayPlans",
                column: "DietPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_DietPlans_UserId",
                table: "DietPlans",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_FoodPlans_MealPlanId",
                table: "FoodPlans",
                column: "MealPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_MealsPlan_DayPlanId",
                table: "MealsPlan",
                column: "DayPlanId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FoodPlans");

            migrationBuilder.DropTable(
                name: "MealsPlan");

            migrationBuilder.DropTable(
                name: "DayPlans");

            migrationBuilder.DropTable(
                name: "DietPlans");
        }
    }
}
