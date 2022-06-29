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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Room>().ToTable("Room");
            modelBuilder.Entity<Student>().ToTable("Student");
            modelBuilder.Entity<Ingredient>().ToTable("Ingredient");
            modelBuilder.Entity<Recipe>().ToTable("Recipe");
        }

        public async Task AddRoom(Room room)
        {
            Rooms.Add(room);
            await SaveChangesAsync();
        }

        public Task<Room> GetRoom(long roomId)
        {
            var room = Rooms.ToListAsync().Result.First(room => room.ID == roomId);
            var residents = Students.ToListAsync().Result.Where(student => student.Room.ID == room.ID);
            foreach (var resident in residents)
            {
                room.Residents.Add(resident);
            }
            return Task.FromResult(room);
        }

        public Task<List<Room>> GetAllRooms()
        {
            var rooms = Rooms.ToListAsync().Result;
            foreach (var room in rooms)
            {
                var residents = Students.ToListAsync().Result.Where(student => student.Room.ID == room.ID);
                foreach (var resident in residents)
                {
                    room.Residents.Add(resident);
                }
            }
            return Task.FromResult(rooms);
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
            catch (Exception)
            {
                // ignored
            }
        }

        public Task<List<Room>> GetRoomsForRatOwners()
        {
            var allRooms = GetAllRooms().Result;
            var rooms = (from room in allRooms let ratable = room.Residents.All(roomResident => roomResident.PetType is not (PetType.Cat or PetType.Owl)) where ratable select room).ToList();

            foreach (var room in rooms)
            {
                var residents = Students.ToListAsync().Result.Where(student => student.Room.ID == room.ID);
                foreach (var resident in residents)
                {
                    room.Residents.Add(resident);
                }
            }

            return Task.FromResult(rooms);
        }
    }
}
