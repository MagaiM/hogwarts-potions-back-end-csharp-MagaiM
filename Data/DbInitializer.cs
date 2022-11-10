using HogwartsPotions.Models;
using System.Linq;
using HogwartsPotions.Models.Entities;
using HogwartsPotions.Models.Enums;

namespace HogwartsPotions.Data

{
    public static class DbInitializer
    {
        public static void Initialize(HogwartsContext context)
        {
            context.Database.EnsureCreated();

            // Look for any rooms.
            if (!context.Rooms.Any())
            {
                var rooms = new Room[]
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

            //Look for any students.
            if (!context.Students.Any())
            {
                var students = new Student[]
                {
                    new Student
                    {
                        HouseType = HouseType.Gryffindor,
                        Name = "Hermione Granger",
                        PetType = PetType.Cat,
                        Room = context.Rooms.First(room => room.ID == 1),
                    },
                    new Student
                    {
                        HouseType = HouseType.Gryffindor,
                        Name = "Harry Potter",
                        PetType = PetType.Owl,
                        Room = context.Rooms.First(room => room.ID == 1),
                    },
                    new Student
                    {
                        HouseType = HouseType.Slytherin,
                        Name = "Draco Melfloy",
                        PetType = PetType.None,
                        Room = context.Rooms.First(room => room.ID == 4),
                    }
                };

                foreach (var student in students)
                {
                    context.Students.Add(student);
                    context.Rooms.First(room => room.ID == student.Room.ID).Residents.Add(student);
                }

                context.SaveChanges();
            }

            // Look for any ingredients.
            if (!context.Ingredients.Any())
            {
                var ingredients = new Ingredient[]
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
                        Name = "Whisky"
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

            // Look for any recipes.
            if (!context.Recipes.Any())
            {
                var recipes = new Recipe[]
                {
                    new Recipe
                    {
                        Name = "Black Out Recipe",
                        Student = context.Students.First(student => student.ID == 1),
                        Ingredients = context.Ingredients.Where(ingredient => ingredient.ID > 0 && ingredient.ID <= 5).ToHashSet(),
                    },
                    new Recipe
                    {
                        Name = "Russian Soup Recipe",
                        Student = context.Students.First(student => student.ID == 3),
                        Ingredients = context.Ingredients.Where(ingredient => ingredient.ID == 1 || ingredient.ID == 5 || ingredient.ID == 6 || ingredient.ID == 8 || ingredient.ID == 10).ToHashSet(),
                    },
                };

                foreach (var recipe in recipes)
                {
                    context.Recipes.Add(recipe);
                }

                context.SaveChanges();
            }

            // Look for any potions.
            if (!context.Potions.Any())
            {
                var potions = new Potion[]
                {
                    new Potion
                    {
                        Name = "Potion of Black Out",
                        Student = context.Students.First(student => student.ID == 3),
                        Recipe = context.Recipes.First(recipe => recipe.Name == "Black Out Recipe"),
                        Ingredients = context.Ingredients.Where(ingredient => ingredient.ID > 0 && ingredient.ID <= 5).ToHashSet(),
                        BrewingStatus = BrewingStatus.Replica
                    },
                    new Potion
                    {
                        Name = "Something Stew",
                        Student = context.Students.First(student => student.ID == 2),
                        Recipe = new Recipe
                        {
                            Name = "Something Stew Recipe",
                            Student = context.Students.First(student => student.ID == 2),
                            Ingredients = context.Ingredients.Where(ingredient => ingredient.ID > 5 && ingredient.ID <= 10).ToHashSet(),
                        },
                        Ingredients = context.Ingredients.Where(ingredient => ingredient.ID > 5 && ingredient.ID <= 10).ToHashSet(),
                        BrewingStatus = BrewingStatus.Discovery
                    },
                    new Potion
                    {
                        Name = "Potion of Strength",
                        Student = context.Students.First(student => student.ID == 1),
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
        }
    }
}
