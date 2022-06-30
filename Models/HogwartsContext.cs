using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HogwartsPotions.Models.Entities;
using HogwartsPotions.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace HogwartsPotions.Models
{
    public class HogwartsContext : DbContext
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
            modelBuilder.Entity<Room>().ToTable("Room");
            modelBuilder.Entity<Student>().ToTable("Student");
            modelBuilder.Entity<Ingredient>().ToTable("Ingredient");
            modelBuilder.Entity<Recipe>().ToTable("Recipe");
            modelBuilder.Entity<Potion>().ToTable("Potion");
        }

        public Task<Student> GetStudent(long studentId)
        {
            var student = Students.Include(student => student.Room).ToListAsync().Result
                .FirstOrDefault(student => student.ID == studentId);
            return Task.FromResult(student);
        }

        public Task<Room> GetRoom(long roomId)
        {
            var room = Rooms.Include(room => room.Residents).ToListAsync().Result.FirstOrDefault(room => room.ID == roomId);

            return Task.FromResult(room);
        }

        public Task<List<Room>> GetAllRooms()
        {
            var rooms = Rooms.Include(room => room.Residents).ToListAsync().Result;

            return Task.FromResult(rooms);
        }

        public async Task AddRoom(Room room)
        {
            Rooms.Add(room);
            await SaveChangesAsync();
        }

        public async Task UpdateRoom(long id, Room room)
        {
            var originalRoom = GetRoom(id);
            originalRoom.Result.Capacity = room.Capacity;
            await SaveChangesAsync();
        }

        public async Task DeleteRoom(long id)
        {
            try
            {
                var room = GetRoom(id);
                Rooms.Remove(room.Result);
                await SaveChangesAsync();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public Task<List<Room>> GetRoomsForRatOwners()
        {
            var allRooms = GetAllRooms().Result;
            var rooms = (from room in allRooms
                let ratable =
                    room.Residents.All(roomResident => roomResident.PetType is not (PetType.Cat or PetType.Owl))
                where ratable
                select room).ToList();

            return Task.FromResult(rooms);
        }

        public Task<List<Potion>> GetAllPotion()
        {
            var potions = Potions.Include(potion => potion.Ingredients).Include(potion => potion.Recipe)
                .Include(potion => potion.Student).ToListAsync().Result;

            return Task.FromResult(potions);
        }

        public async Task<Potion> AddPotion(CreatePotion createPotion)
        {
            var student = GetStudent(createPotion.StudentId).Result;
            var recipe = new Recipe();
            var ingredients = GetIngredients(createPotion.Ingredients);
            BrewingStatus brewingStatus;

            switch (createPotion.Ingredients.Count)
            {
                case <= 0:
                    return null;
                case < MaxIngredientsForPotions:
                    brewingStatus = BrewingStatus.Brew;
                    recipe = default;
                    break;
                case > MaxIngredientsForPotions:
                    return null;
                default:
                {
                    var allRecipes = Recipes.Include(r => r.Ingredients).Include(r => r.Student).ToListAsync().Result;
                    if (allRecipes.Any(r => r.Ingredients.SetEquals(ingredients)))
                    {
                        brewingStatus = BrewingStatus.Replica;
                        recipe = allRecipes.First(r => r.Ingredients.SetEquals(ingredients));
                    }
                    else
                    {
                        brewingStatus = BrewingStatus.Discovery;
                        var recipeName = GenerateRecipeName(student);
                        recipe = new Recipe
                        {
                            Ingredients = ingredients,
                            Name = recipeName,
                            Student = student
                        };
                        Recipes.Add(recipe);
                        await SaveChangesAsync();
                    }

                    break;
                }
            }

            var potion = new Potion
            {
                Ingredients = ingredients,
                Student = student,
                BrewingStatus = brewingStatus,
                Name = createPotion.Name,
                Recipe = recipe
            };

            Potions.Add(potion);
            await SaveChangesAsync();

            return potion;
        }

        private HashSet<Ingredient> GetIngredients(HashSet<Ingredient> createPotionIngredients)
        {
            var allIngredients = Ingredients.ToListAsync().Result;
            var ingredients = new HashSet<Ingredient>();
            foreach (var potionIngredient in createPotionIngredients)
            {
                ingredients.Add(allIngredients.Any(i => i.Name == potionIngredient.Name)
                    ? allIngredients.First(i => i.Name == potionIngredient.Name)
                    : potionIngredient);
            }
            return ingredients;
        }

        private string GenerateRecipeName(Student student)
        {
            var studentRecipes = Recipes.ToListAsync().Result.Where(recipe => recipe.Student.ID == student.ID).ToList();
            return $"{student.Name}'s discovery #{studentRecipes.Count + 1}";
        }

        public Task<List<Potion>> GetAllPotionByStudent(long studentId)
        {
            var potions = Potions.Include(potion => potion.Ingredients).Include(potion => potion.Recipe).Include(p => p.Student).ToListAsync()
                .Result.Where(p => p.Student.ID == studentId).ToList();

            return Task.FromResult(potions);
        }

        public async Task<Potion> BrewPotion(BrewPotion brewPotion)
        {
            var student = GetStudent(brewPotion.StudentId).Result;
            var potions = GetAllPotionByStudent(student.ID).Result;
            var brewingStatus = BrewingStatus.Brew;
            var ingredients = new HashSet<Ingredient>();
            Recipe recipe = null;
            var name = $"{student.Name}'s potion #{potions.Count + 1}";

            var potion = new Potion
            {
                Name = name,
                BrewingStatus = brewingStatus,
                Ingredients = ingredients,
                Student = student,
                Recipe = recipe
            };

            Potions.Add(potion);
            await SaveChangesAsync();

            return potion;
        }

        public async Task<Potion> AddIngredientToPotion(long potionId, Ingredient newIngredient)
        {
            var potion = Potions.Include(p => p.Ingredients).Include(p => p.Recipe).Include(p => p.Student)
                .ToListAsync().Result.FirstOrDefault(potion => potion.ID == potionId);
            if (potion == null || potion.Ingredients.Count >= MaxIngredientsForPotions) return null;

            var ingredients = potion.Ingredients;
            ingredients.Add(newIngredient);
            ingredients = GetIngredients(ingredients);
            potion.Ingredients = ingredients;

            if (ingredients.Count == MaxIngredientsForPotions)
            {

                var allRecipes = Recipes.Include(r => r.Ingredients).Include(r => r.Student).ToListAsync().Result;
                BrewingStatus brewingStatus;
                Recipe recipe;

                if (allRecipes.Any(r => r.Ingredients.SetEquals(ingredients)))
                {
                    brewingStatus = BrewingStatus.Replica;
                    recipe = allRecipes.First(r => r.Ingredients.SetEquals(ingredients));
                }
                else
                {
                    brewingStatus = BrewingStatus.Discovery;
                    var recipeName = GenerateRecipeName(potion.Student);
                    recipe = new Recipe
                    {
                        Ingredients = ingredients,
                        Name = recipeName,
                        Student = potion.Student
                    };
                    Recipes.Add(recipe);
                    await SaveChangesAsync();
                }

                potion.BrewingStatus = brewingStatus;
                potion.Recipe = recipe;
            }
            await SaveChangesAsync();

            return potion;
        }

        public async Task<List<Recipe>> HelpFinishBrew(long potionId)
        {
            var ingredients = Potions.Include(p => p.Ingredients).ToListAsync().Result
                .FirstOrDefault(p => p.ID == potionId)
                ?.Ingredients;
            if (ingredients == null) return null;

            var allRecipes = Recipes.Include(r => r.Ingredients).Include(r => r.Student).ToListAsync().Result;
            var result = allRecipes.FindAll(r => r.Ingredients.IsSupersetOf(ingredients));

            return result;
        }
    }
}
