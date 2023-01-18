using HogwartsPotions.Models;
using System.Linq;
using System.Threading.Tasks;
using HogwartsPotions.Models.Entities;
using HogwartsPotions.Models.Enums;
using Microsoft.AspNetCore.Identity;

namespace HogwartsPotions.Data

{
    public static class DbInitializer
    {
        public static void Initialize(HogwartsContext context, UserManager<Student> userManager)
        {

            context.Database.EnsureCreated();

            // Look for any rooms.
            if (!context.Rooms.Any())
            {
                SeedRooms(context);
            }

            //Look for any students.
            if (!context.Students.Any())
            {
                var _ = SeedStudents(context, userManager);
            }

            // Look for any ingredients.
            if (!context.Ingredients.Any())
            {
                SeedIngredients(context);
            }

            // Look for any recipes.
            if (!context.Recipes.Any())
            {
                SeedRecipes(context);
            }

            // Look for any potions.
            if (!context.Potions.Any())
            {
                SeedPotions(context);
            }
        }

        private static void SeedPotions(HogwartsContext context)
        {
            var potions = new[]
                {
                    new Potion
                    {
                        Name = "Potion of Black Out",
                        Student = context.Students.First(student => student.UserName == "Draco Melfloy"),
                        Recipe = context.Recipes.First(recipe => recipe.Name == "Black Out Recipe"),
                        Ingredients = context.Ingredients.Where(ingredient => ingredient.ID > 0 && ingredient.ID <= 5).ToHashSet(),
                        BrewingStatus = BrewingStatus.Replica
                    },
                    new Potion
                    {
                        Name = "Something Stew",
                        Student = context.Students.First(student => student.UserName == "Harry Potter"),
                        Recipe = new Recipe
                        {
                            Name = "Something Stew Recipe",
                            Student = context.Students.First(student => student.UserName == "Harry Potter"),
                            Ingredients = context.Ingredients.Where(ingredient => ingredient.ID > 5 && ingredient.ID <= 10).ToHashSet(),
                        },
                        Ingredients = context.Ingredients.Where(ingredient => ingredient.ID > 5 && ingredient.ID <= 10).ToHashSet(),
                        BrewingStatus = BrewingStatus.Discovery
                    },
                    new Potion
                    {
                        Name = "Potion of Strength",
                        Student = context.Students.First(student => student.UserName == "Hermione Granger"),
                        Recipe = null,
                        Ingredients = context.Ingredients.Where(ingredient => ingredient.ID == 1 || ingredient.ID == 7).ToHashSet(),
                        BrewingStatus = BrewingStatus.Brew
                    },
                };

            foreach (var potion in potions)
            {
                context.Potions.Add(potion);
            }

            context.SaveChanges();
        }

        private static void SeedRecipes(HogwartsContext context)
        {
            var recipes = new[]
            {
                new Recipe
                {
                    Name = "Black Out Recipe",
                    Student = context.Students.First(student => student.UserName == "Hermione Granger"),
                    Ingredients = context.Ingredients.Where(ingredient => ingredient.ID > 0 && ingredient.ID <= 5).ToHashSet(),
                },
                new Recipe
                {
                    Name = "Russian Soup Recipe",
                    Student = context.Students.First(student => student.UserName == "Draco Melfloy"),
                    Ingredients = context.Ingredients.Where(ingredient => ingredient.ID == 1 || ingredient.ID == 5 || ingredient.ID == 6 || ingredient.ID == 8 || ingredient.ID == 10).ToHashSet(),
                },
            };

            foreach (var recipe in recipes)
            {
                context.Recipes.Add(recipe);
            }

            context.SaveChanges();
        }

        private static void SeedIngredients(HogwartsContext context)
        {
            var ingredients = new[]
            {
                new Ingredient
                {
                    Name = "Vodka"
                },
                new Ingredient
                {
                    Name = "Beer"
                },
                new Ingredient
                {
                    Name = "Water"
                },
                new Ingredient
                {
                    Name = "Rum"
                },
                new Ingredient
                {
                    Name = "Whiskey"
                },
                new Ingredient
                {
                    Name = "Potato"
                },
                new Ingredient
                {
                    Name = "Strawberry"
                },
                new Ingredient
                {
                    Name = "Tomato"
                },
                new Ingredient
                {
                    Name = "Apple"
                },
                new Ingredient
                {
                    Name = "Ginseng"
                },
            };

            foreach (var ingredient in ingredients)
            {
                context.Ingredients.Add(ingredient);
            }

            context.SaveChanges();
        }

        private static void SeedRooms(HogwartsContext context)
        {
            var rooms = new[]
            {
                new Room
                {
                    Capacity = 10,
                    RoomHouseType = HouseType.Gryffindor
                },
                new Room
                {
                    Capacity = 10,
                    RoomHouseType = HouseType.Hufflepuff
                },
                new Room
                {
                    Capacity = 10,
                    RoomHouseType = HouseType.Ravenclaw
                },
                new Room
                {
                    Capacity = 10,
                    RoomHouseType = HouseType.Slytherin
                }
            };

            foreach (var room in rooms)
            {
                context.Rooms.Add(room);
            }

            context.SaveChanges();
        }

        private static async Task<int> SeedStudents(HogwartsContext context, UserManager<Student> userManager)
        {
            var students = new[]
            {
                new Student
                {
                    UserName = "Hermione Granger",
                    HouseType = HouseType.Gryffindor,
                    PetType = PetType.Cat,
                    Room = context.Rooms.First(room => room.ID == 1)
                },
                new Student
                {
                    UserName = "Harry Potter",
                    HouseType = HouseType.Gryffindor,
                    PetType = PetType.Owl,
                    Room = context.Rooms.First(room => room.ID == 1)
                },
                new Student
                {
                    UserName = "Draco Melfloy",
                    HouseType = HouseType.Slytherin,
                    PetType = PetType.None,
                    Room = context.Rooms.First(room => room.ID == 4)
                }
            };

            foreach (var student in students)
            {
                await userManager.CreateAsync(student, "Abc123");
                context.Rooms.First(room => room.ID == student.Room.ID).Residents.Add(student);
            }
            return context.SaveChangesAsync().Result;
        }
    }
}
