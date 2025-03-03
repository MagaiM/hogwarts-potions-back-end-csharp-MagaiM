﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HogwartsPotions.Models;
using HogwartsPotions.Models.DTOs;
using HogwartsPotions.Models.Entities;
using HogwartsPotions.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace HogwartsPotions.Services
{
    public class RoomService
    {
        private readonly HogwartsContext _context;
        private readonly StudentService _studentService;
        private const int MinRoomCapacity = 1;
        private const int MaxRoomCapacity = 999;

        public RoomService(HogwartsContext context, StudentService studentService)
        {
            _context = context;
            _studentService = studentService;
        }

        public Task<Room> GetRoom(long roomId)
        {
            var room = _context.Rooms.Include(room => room.Residents).ToListAsync().Result.FirstOrDefault(room => room.Id == roomId);

            return Task.FromResult(room);
        }

        public Task<List<Room>> GetAllRooms()
        {
            try
            {
                var rooms = _context.Rooms.Include(room => room.Residents).ToListAsync().Result;
                return Task.FromResult(rooms);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.WriteLine(e.Message);
                throw;
            }
        }

        public async Task<Room> AddRoom(AddRoomDto room)
        {
            var newRoomCapacity = room.Capacity switch
            {
                < MinRoomCapacity => MinRoomCapacity,
                > MaxRoomCapacity => MaxRoomCapacity,
                _ => room.Capacity
            };
            if ((int) room.RoomHouseType < 0 || (int) room.RoomHouseType > 3) return null;
            
            var newRoom = new Room
            {
                Capacity = newRoomCapacity,
                RoomHouseType = room.RoomHouseType
            };
            _context.Rooms.Add(newRoom);
            await _context.SaveChangesAsync();
            return newRoom;
        }

        public async Task<string> UpdateRoom(long id, Room room)
        {
            var originalRoom = GetRoom(id);
            if (originalRoom == null) return "Room does not exists!";
            if (originalRoom.Result.Capacity != room.Capacity)
                originalRoom.Result.Capacity = room.Capacity switch
                {
                    > MaxRoomCapacity => MaxRoomCapacity,
                    < MinRoomCapacity => MinRoomCapacity,
                    _ => room.Capacity < originalRoom.Result.Residents.Count
                        ? originalRoom.Result.Residents.Count
                        : room.Capacity
                };
            if (originalRoom.Result.RoomHouseType != room.RoomHouseType)
            {
                if (originalRoom.Result.Residents.Any()) return "You can only change the House type of an empty room!";
                originalRoom.Result.RoomHouseType = room.RoomHouseType;
            }
            await _context.SaveChangesAsync();
            return "Success";
        }

        public async Task<string> DeleteRoom(long id)
        {
            try
            {
                var room = GetRoom(id);
                if (room.Result == null) return "Room does not exists!";
                if (room.Result.Residents.Any()) return "You can only delete empty rooms!";

                _context.Rooms.Remove(room.Result);
                await _context.SaveChangesAsync();
                return "Success";
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

        public Task<List<Room>> GetRoomsForRatOwners(List<Room> allRooms)
        {
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
        public async Task<string> AddStudentToRoom(long roomId, string studentId)
        {
            var room = GetRoom(roomId);
            if (room == null) return "Room does not exists!";
            var student = _studentService.GetStudentById(studentId);
            try
            {
                if (room.Result.RoomHouseType != student.Result.HouseType) return "Can't move to that room!";
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

        public Task<List<Room>> GetAvailableRooms(Student student)
        {
            try
            {
                var listOfRoomByHouseType = _context.Rooms.Where(r => r.RoomHouseType == student.HouseType && r.Capacity > r.Residents.Count).ToList();
                return student.PetType == PetType.Rat ? GetRoomsForRatOwners(listOfRoomByHouseType) : Task.FromResult(listOfRoomByHouseType);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task<Room> GetValidRoom(long roomId, string studentId)
        {
            var validRoom = await GetRoom(roomId);
            var availableRooms = GetAvailableRooms(_studentService.GetStudentById(studentId).Result).Result;
            if (validRoom != null && validRoom.Capacity > validRoom.Residents.Count && availableRooms.Contains(validRoom))
                return validRoom;
            return null;
        }
    }
}
