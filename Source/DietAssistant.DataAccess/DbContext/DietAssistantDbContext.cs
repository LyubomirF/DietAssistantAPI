using DietAssistant.Domain;
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
        
        public DbSet<Food> Foods { get; set; }

        public DbSet<Nutrient> Nutrients { get; set; }

        public DbSet<FoodServing> FoodServings { get; set; }

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
                    .WithOne(x => x.Meal);
            });

            builder.Entity<Food>(options => 
            {
                options.HasKey(x => x.FoodId);

                options.HasMany(x => x.Nutrients).WithOne(x => x.Food);
            });

            builder.Entity<Nutrient>(options =>
            {
                options.HasKey(x => x.NutrientId);

                options
                    .HasOne(x => x.Food)
                    .WithMany(x => x.Nutrients);
            });
        }
    }
}
