﻿// <auto-generated />
using System;
using DietAssistant.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace DietAssistant.DataAccess.DbContext.Migrations
{
    [DbContext(typeof(DietAssistantDbContext))]
    [Migration("20220529202752_AddInitialMigration")]
    partial class AddInitialMigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("DietAssistant.Domain.Food", b =>
                {
                    b.Property<int>("FoodId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("FoodId"), 1L, 1);

                    b.Property<double>("Amount")
                        .HasColumnType("float");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Unit")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("FoodId");

                    b.ToTable("Foods");
                });

            modelBuilder.Entity("DietAssistant.Domain.FoodServing", b =>
                {
                    b.Property<int>("FoodServingId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("FoodServingId"), 1L, 1);

                    b.Property<int>("FoodId")
                        .HasColumnType("int");

                    b.Property<int>("MealId")
                        .HasColumnType("int");

                    b.Property<int>("NumberOfServings")
                        .HasColumnType("int");

                    b.Property<double>("ServingSizeAmount")
                        .HasColumnType("float");

                    b.Property<string>("ServingSizeUnit")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("FoodServingId");

                    b.HasIndex("FoodId");

                    b.HasIndex("MealId");

                    b.ToTable("FoodServings");
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

            modelBuilder.Entity("DietAssistant.Domain.Nutrient", b =>
                {
                    b.Property<int>("NutrientId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("NutrientId"), 1L, 1);

                    b.Property<double>("Amount")
                        .HasColumnType("float");

                    b.Property<int>("FoodId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Unit")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("NutrientId");

                    b.HasIndex("FoodId");

                    b.ToTable("Nutrients");
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

                    b.Property<string>("Name")
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

                    b.Property<double?>("BodyFatPercentage")
                        .HasColumnType("float");

                    b.Property<double>("Height")
                        .HasColumnType("float");

                    b.Property<string>("MetricSystem")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<double>("Weight")
                        .HasColumnType("float");

                    b.HasKey("UserStatsId");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("UsersStats");
                });

            modelBuilder.Entity("DietAssistant.Domain.FoodServing", b =>
                {
                    b.HasOne("DietAssistant.Domain.Food", "Food")
                        .WithMany()
                        .HasForeignKey("FoodId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DietAssistant.Domain.Meal", "Meal")
                        .WithMany("FoodServings")
                        .HasForeignKey("MealId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Food");

                    b.Navigation("Meal");
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

            modelBuilder.Entity("DietAssistant.Domain.Nutrient", b =>
                {
                    b.HasOne("DietAssistant.Domain.Food", "Food")
                        .WithMany("Nutrients")
                        .HasForeignKey("FoodId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Food");
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

            modelBuilder.Entity("DietAssistant.Domain.Food", b =>
                {
                    b.Navigation("Nutrients");
                });

            modelBuilder.Entity("DietAssistant.Domain.Meal", b =>
                {
                    b.Navigation("FoodServings");
                });

            modelBuilder.Entity("DietAssistant.Domain.User", b =>
                {
                    b.Navigation("Meals");

                    b.Navigation("ProgressLogs");

                    b.Navigation("UserStats")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
