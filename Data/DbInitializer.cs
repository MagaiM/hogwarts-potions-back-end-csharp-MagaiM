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
        }
    }
}
