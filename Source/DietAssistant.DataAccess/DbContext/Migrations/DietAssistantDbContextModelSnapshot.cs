﻿// <auto-generated />
using System;
using DietAssistant.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace DietAssistant.DataAccess.DbContext.Migrations
{
    [DbContext(typeof(DietAssistantDbContext))]
    partial class DietAssistantDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("DietAssistant.Domain.DietPlanning.DietPlan", b =>
                {
                    b.Property<int>("DietPlanId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("DietPlanId"), 1L, 1);

                    b.Property<string>("PlanName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("DietPlanId");

                    b.HasIndex("UserId");

                    b.ToTable("DietPlans");
                });

            modelBuilder.Entity("DietAssistant.Domain.DietPlanning.FoodPlan", b =>
                {
                    b.Property<int>("FoodPlanId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("FoodPlanId"), 1L, 1);

                    b.Property<string>("FoodId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("MealPlanId")
                        .HasColumnType("int");

                    b.Property<double>("ServingSize")
                        .HasColumnType("float");

                    b.Property<string>("Unit")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("FoodPlanId");

                    b.HasIndex("MealPlanId");

                    b.ToTable("FoodPlans");
                });

            modelBuilder.Entity("DietAssistant.Domain.DietPlanning.MealPlan", b =>
                {
                    b.Property<int>("MealPlanId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("MealPlanId"), 1L, 1);

                    b.Property<int>("DayOfWeek")
                        .HasColumnType("int");

                    b.Property<int?>("DietPlanId")
                        .HasColumnType("int");

                    b.Property<string>("MealPlanName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<TimeSpan>("Time")
                        .HasColumnType("time");

                    b.HasKey("MealPlanId");

                    b.HasIndex("DietPlanId");

                    b.ToTable("MealsPlan");
                });

            modelBuilder.Entity("DietAssistant.Domain.FoodServing", b =>
                {
                    b.Property<int>("FoodServingId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("FoodServingId"), 1L, 1);

                    b.Property<string>("FoodId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("MealId")
                        .HasColumnType("int");

                    b.Property<double>("NumberOfServings")
                        .HasColumnType("float");

                    b.Property<double>("ServingSize")
                        .HasColumnType("float");

                    b.Property<string>("ServingUnit")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("FoodServingId");

                    b.HasIndex("MealId");

                    b.ToTable("FoodServings");
                });

            modelBuilder.Entity("DietAssistant.Domain.Goal", b =>
                {
                    b.Property<int>("GoalId")
                        .HasColumnType("int");

                    b.Property<int>("ActivityLevel")
                        .HasColumnType("int");

                    b.Property<double>("CurrentWeight")
                        .HasColumnType("float");

                    b.Property<double>("GoalWeight")
                        .HasColumnType("float");

                    b.Property<int>("NutritionGoalId")
                        .HasColumnType("int");

                    b.Property<double>("StartDate")
                        .HasColumnType("float");

                    b.Property<double>("StartWeight")
                        .HasColumnType("float");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<int>("WeeklyGoal")
                        .HasColumnType("int");

                    b.HasKey("GoalId");

                    b.HasIndex("NutritionGoalId");

                    b.ToTable("Goals");
                });

            modelBuilder.Entity("DietAssistant.Domain.Meal", b =>
                {
                    b.Property<int>("MealId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("MealId"), 1L, 1);

                    b.Property<DateTime>("EatenOn")
                        .HasColumnType("datetime2");

                    b.Property<int>("Order")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("MealId");

                    b.HasIndex("UserId");

                    b.ToTable("Meals");
                });

            modelBuilder.Entity("DietAssistant.Domain.NutritionGoal", b =>
                {
                    b.Property<int>("NutritionGoalId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("NutritionGoalId"), 1L, 1);

                    b.Property<double>("Calories")
                        .HasColumnType("float");

                    b.Property<DateTime>("ChangedOnUTC")
                        .HasColumnType("datetime2");

                    b.Property<double>("PercentCarbs")
                        .HasColumnType("float");

                    b.Property<double>("PercentFat")
                        .HasColumnType("float");

                    b.Property<double>("PercentProtein")
                        .HasColumnType("float");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("NutritionGoalId");

                    b.HasIndex("UserId");

                    b.ToTable("NutritionGoals");
                });

            modelBuilder.Entity("DietAssistant.Domain.ProgressLog", b =>
                {
                    b.Property<int>("ProgressLogId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ProgressLogId"), 1L, 1);

                    b.Property<DateTime>("LoggedOn")
                        .HasColumnType("datetime2");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<double>("Weigth")
                        .HasColumnType("float");

                    b.HasKey("ProgressLogId");

                    b.HasIndex("UserId");

                    b.ToTable("ProgressLogs");
                });

            modelBuilder.Entity("DietAssistant.Domain.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("UserId"), 1L, 1);

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("DietAssistant.Domain.UserStats", b =>
                {
                    b.Property<int>("UserStatsId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("UserStatsId"), 1L, 1);

                    b.Property<DateTime>("DateOfBirth")
                        .HasColumnType("datetime2");

                    b.Property<int>("Gender")
                        .HasColumnType("int");

                    b.Property<double>("Height")
                        .HasColumnType("float");

                    b.Property<int>("HeightUnit")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<double>("Weight")
                        .HasColumnType("float");

                    b.Property<int>("WeightUnit")
                        .HasColumnType("int");

                    b.HasKey("UserStatsId");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("UsersStats");
                });

            modelBuilder.Entity("DietAssistant.Domain.DietPlanning.DietPlan", b =>
                {
                    b.HasOne("DietAssistant.Domain.User", "User")
                        .WithMany("DietPlans")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("DietAssistant.Domain.DietPlanning.FoodPlan", b =>
                {
                    b.HasOne("DietAssistant.Domain.DietPlanning.MealPlan", null)
                        .WithMany("FoodPlans")
                        .HasForeignKey("MealPlanId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("DietAssistant.Domain.DietPlanning.MealPlan", b =>
                {
                    b.HasOne("DietAssistant.Domain.DietPlanning.DietPlan", null)
                        .WithMany("MealPlans")
                        .HasForeignKey("DietPlanId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("DietAssistant.Domain.FoodServing", b =>
                {
                    b.HasOne("DietAssistant.Domain.Meal", "Meal")
                        .WithMany("FoodServings")
                        .HasForeignKey("MealId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Meal");
                });

            modelBuilder.Entity("DietAssistant.Domain.Goal", b =>
                {
                    b.HasOne("DietAssistant.Domain.User", "User")
                        .WithOne("Goal")
                        .HasForeignKey("DietAssistant.Domain.Goal", "GoalId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("DietAssistant.Domain.NutritionGoal", "NutritionGoal")
                        .WithMany()
                        .HasForeignKey("NutritionGoalId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("NutritionGoal");

                    b.Navigation("User");
                });

            modelBuilder.Entity("DietAssistant.Domain.Meal", b =>
                {
                    b.HasOne("DietAssistant.Domain.User", "User")
                        .WithMany("Meals")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("DietAssistant.Domain.NutritionGoal", b =>
                {
                    b.HasOne("DietAssistant.Domain.User", "User")
                        .WithMany("NutritionGoals")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("DietAssistant.Domain.ProgressLog", b =>
                {
                    b.HasOne("DietAssistant.Domain.User", "User")
                        .WithMany("ProgressLogs")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("DietAssistant.Domain.UserStats", b =>
                {
                    b.HasOne("DietAssistant.Domain.User", "User")
                        .WithOne("UserStats")
                        .HasForeignKey("DietAssistant.Domain.UserStats", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("DietAssistant.Domain.DietPlanning.DietPlan", b =>
                {
                    b.Navigation("MealPlans");
                });

            modelBuilder.Entity("DietAssistant.Domain.DietPlanning.MealPlan", b =>
                {
                    b.Navigation("FoodPlans");
                });

            modelBuilder.Entity("DietAssistant.Domain.Meal", b =>
                {
                    b.Navigation("FoodServings");
                });

            modelBuilder.Entity("DietAssistant.Domain.User", b =>
                {
                    b.Navigation("DietPlans");

                    b.Navigation("Goal")
                        .IsRequired();

                    b.Navigation("Meals");

                    b.Navigation("NutritionGoals");

                    b.Navigation("ProgressLogs");

                    b.Navigation("UserStats")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
