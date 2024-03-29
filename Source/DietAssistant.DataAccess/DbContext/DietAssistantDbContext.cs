﻿using DietAssistant.Domain;
using DietAssistant.Domain.DietPlanning;
using Microsoft.EntityFrameworkCore;

#pragma warning disable CS8618

namespace DietAssistant.DataAccess
{
    public class DietAssistantDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public DietAssistantDbContext(DbContextOptions options) 
            : base(options) { }

        #region DbSets
            
        public DbSet<User> Users { get; set; }

        public DbSet<UserStats> UsersStats { get; set; }

        public DbSet<ProgressLog> ProgressLogs { get; set; }

        public DbSet<Meal> Meals { get; set; }

        public DbSet<FoodServing> FoodServings { get; set; }

        public DbSet<DietPlan> DietPlans { get; set; } 

        public DbSet<MealPlan> MealsPlan { get; set; } 

        public DbSet<FoodPlan> FoodPlans { get; set; }

        public DbSet<Goal> Goals { get; set; }

        public DbSet<NutritionGoal> NutritionGoals { get; set; }

        #endregion

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<User>(options => 
            {
                options.HasKey(x => x.UserId);

                options
                    .HasOne(x => x.UserStats)
                    .WithOne(x => x.User);

                options
                    .HasMany(x => x.Meals)
                    .WithOne(x => x.User);

                options
                    .HasMany(x => x.ProgressLogs)
                    .WithOne(x => x.User);

                options
                    .HasMany(x => x.DietPlans)
                    .WithOne(x => x.User);

                options
                    .HasOne(x => x.Goal)
                    .WithOne(x => x.User)
                    .HasForeignKey<Goal>(x => x.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                options
                    .HasMany(x => x.NutritionGoals)
                    .WithOne(x => x.User);

            });

            builder.Entity<UserStats>(options => 
            {
                options
                    .HasKey(x => x.UserStatsId);

                options
                    .HasOne(x => x.User)
                    .WithOne(x => x.UserStats);
            });

            builder.Entity<Meal>(options =>
            {
                options.HasKey(x => x.MealId);

                options
                    .HasOne(x => x.User)
                    .WithMany(x => x.Meals);

                options
                    .HasMany(x => x.FoodServings)
                    .WithOne(x => x.Meal)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<DietPlan>(options =>
            {
                options.HasKey(x => x.DietPlanId);

                options
                    .HasOne(x => x.User)
                    .WithMany(x => x.DietPlans);

                options
                    .HasMany(x => x.MealPlans)
                    .WithOne()
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<MealPlan>(options => 
            {
                options.HasKey(x => x.MealPlanId);

                options
                    .HasMany(x => x.FoodPlans)
                    .WithOne()
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<FoodPlan>(options =>
            {
                options.HasKey(x => x.FoodPlanId);
            });

            builder.Entity<Goal>(options =>
            {
                options.HasKey(x => x.GoalId);

                options.HasOne(x => x.NutritionGoal);
            });

            builder.Entity<NutritionGoal>(options =>
            {
                options.HasKey(x => x.NutritionGoalId);

                options.HasOne(x => x.User).WithMany(x => x.NutritionGoals).HasForeignKey(x => x.UserId);
            });
        }
    }
}
