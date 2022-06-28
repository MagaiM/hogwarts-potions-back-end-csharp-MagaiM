using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
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

        public DbSet<Student> Students { get; set; }
        public DbSet<Room> Rooms { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Student>().ToTable("Student");
            modelBuilder.Entity<Room>().ToTable("Room");
        }

        public async Task AddRoom(Room room)
        {
            Rooms.Add(room);
            await SaveChangesAsync();
        }

        public Task<Room> GetRoom(long roomId)
        {
            return Task.FromResult(Rooms.ToListAsync().Result.First(room => room.ID == roomId));
        }

        public Task<List<Room>> GetAllRooms()
        {
            return Rooms.ToListAsync();
        }

        public async Task UpdateRoom(Room room)
        {
            var originalRoom = GetRoom(room.ID);
            Rooms.Remove(originalRoom.Result);
            Rooms.Add(room);
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
            }
        }

        public Task<List<Room>> GetRoomsForRatOwners()
        {
            return Task.FromResult((from room in Rooms let ratable = room.Residents.All(student => student.PetType != PetType.Cat && student.PetType != PetType.Owl) where ratable select room).ToList());
        }
    }
}
