using HogwartsPotions.Models;
using System;
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

            // Look for any students and rooms.
            if (context.Students.Any() && context.Rooms.Any())
            {
                return;   // DB has been seeded
            }

            var rooms = new Room[]
            {
                new Room
                {
                    Capacity = 10,
                },
                new Room
                {
                    Capacity = 10,
                },
                new Room
                {
                    Capacity = 10,
                },
                new Room
                {
                    Capacity = 10,
                }
            };

            foreach (var room in rooms)
            {
                context.Rooms.Add(room);
            }
            context.SaveChanges();

            var students = new Student[]
            {
                new Student
                {
                    HouseType = HouseType.Gryffindor,
                    Name = "Hermione Granger",
                    PetType = PetType.Cat,
                    Room = rooms.First(room => room.ID == 1),
                },
                new Student
                {
                    HouseType = HouseType.Gryffindor,
                    Name = "Harry Potter",
                    PetType = PetType.Owl,
                    Room = rooms.First(room => room.ID == 1),
                },
                new Student
                {
                    HouseType = HouseType.Slytherin,
                    Name = "Draco Melfloy",
                    PetType = PetType.None,
                    Room = rooms.First(room => room.ID == 2),
                },
            };

            foreach (var student in students)
            {
                context.Students.Add(student);
                context.Rooms.First(room => room.ID == student.Room.ID).Residents.Add(student);
            }
            context.SaveChanges();

            // Look for any ingredients.
            if (context.Ingredients.Any())
            {
                return;   // DB has been seeded
            }

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
    }
}
