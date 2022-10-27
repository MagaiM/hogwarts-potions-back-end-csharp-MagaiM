using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HogwartsPotions.Models;
using HogwartsPotions.Models.Entities;
using HogwartsPotions.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace HogwartsPotions.Services
{
    public class RoomService
    {
        private readonly HogwartsContext _context;

        public RoomService(HogwartsContext context)
        {
            _context = context;
        }

        public Task<Room> GetRoom(long roomId)
        {
            var room = _context.Rooms.Include(room => room.Residents).ToListAsync().Result.FirstOrDefault(room => room.ID == roomId);

            return Task.FromResult(room);
        }

        public Task<List<Room>> GetAllRooms()
        {
            var rooms = _context.Rooms.Include(room => room.Residents).ToListAsync().Result;

            return Task.FromResult(rooms);
        }

        public async Task<Room> AddRoom(Room room)
        {
            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();
            return room;
        }

        public async Task UpdateRoom(long id, Room room)
        {
            var originalRoom = GetRoom(id);
            originalRoom.Result.Capacity = room.Capacity;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteRoom(long id)
        {
            try
            {
                var room = GetRoom(id);
                _context.Rooms.Remove(room.Result);
                await _context.SaveChangesAsync();
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
    }
}
