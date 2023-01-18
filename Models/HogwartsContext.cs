using HogwartsPotions.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace HogwartsPotions.Models
{
    public class HogwartsContext : IdentityDbContext
    {
        public const int MaxIngredientsForPotions = 5;

        public HogwartsContext(DbContextOptions<HogwartsContext> options) : base(options)
        {
        }

        public DbSet<Room> Rooms { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<Potion> Potions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Room>().ToTable("Room");
            modelBuilder.Entity<Student>().ToTable("Student").HasIndex(s => s.UserName).IsUnique();
            modelBuilder.Entity<Ingredient>().ToTable("Ingredient");
            modelBuilder.Entity<Recipe>().ToTable("Recipe");
            modelBuilder.Entity<Potion>().ToTable("Potion");
        }
    }
}
