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
        private readonly StudentService _studentService;

        public RoomService(HogwartsContext context, StudentService studentService)
        {
            _context = context;
            _studentService = studentService;
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

        public async Task RemoveStudentFromRoom(Room room, Student student)
        {
            try
            {
                _context.Rooms.First(r => r.Equals(room)).Residents.Remove(student);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        public async Task<string> AddStudentToRoom(long roomId, long studentId)
        {
            var room = GetRoom(roomId);
            var student = _studentService.GetStudent(studentId);
            try
            {
                if (room.Result.Residents.Count != 0 && room.Result.HouseType != student.Result.HouseType) return "Can't move to that room!";
                if (student.Result.Room != null) await RemoveStudentFromRoom(student.Result.Room, student.Result);
                _context.Students.First(s => s.Equals(student.Result)).Room = room.Result;
                _context.Rooms.First(r => r.Equals(room.Result)).Residents.Add(student.Result);
                await _context.SaveChangesAsync();
                return "Success";
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return "Something went wrong!";
            }
        }
    }
}
