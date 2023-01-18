using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HogwartsPotions.Helpers;
using HogwartsPotions.Models;
using HogwartsPotions.Models.DTOs;
using HogwartsPotions.Models.Entities;
using HogwartsPotions.Models.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HogwartsPotions.Services
{
    public class StudentService
    {
        private readonly HogwartsContext _context;
        private readonly UserManager<Student> _userManager;
        private readonly StudentHelper _studentHelper = new();

        public StudentService(
            HogwartsContext context,
            UserManager<Student> userManager
            )
        {
            _context = context;
            _userManager = userManager;
        }

        public Task<Student> GetStudentById(string studentId)
        {
            var student = _context.Students.Include(student => student.Room).ToListAsync().Result.FirstOrDefault(student => student.Id == studentId);

            return Task.FromResult(student);
        }

        public Task<Student> GetStudentByName(string studentName)
        {
            var student = _context.Students.Include(student => student.Room).ToListAsync().Result.FirstOrDefault(student => student.UserName == studentName);

            return Task.FromResult(student);
        }

        public Task<List<Student>> GetAllStudents()
        {
            var students = _context.Students.ToListAsync().Result;

            return Task.FromResult(students);
        }

        public async Task<Student> AddRoomlessStudent(AddStudentInProgressDto student)
        {
            if (student == null) return null;
            try
            {
                if (GetStudentByName(student.UserName).Result != null) return null;

                var newStudentHouseType = student.PreferredHouseType == null || (int)student.PreferredHouseType < 0 || (int)student.PreferredHouseType > 3 ?
                _studentHelper.GetRandomHouseType() :
                _studentHelper.GetRandomHouseType((HouseType)student.PreferredHouseType);

                var newStudent = new Student
                {
                    HouseType = newStudentHouseType,
                    UserName = student.UserName,
                    PetType = student.PetType
                };
                await _userManager.CreateAsync(newStudent, student.Password);
                await _context.SaveChangesAsync();
                return newStudent;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<Student> AddStudentToRoom(string id, Room validRoom)
        {
            try
            {
                var roomlessStudent = await GetStudentById(id);
                if (roomlessStudent == null) return null;
                roomlessStudent.Room = validRoom;
                validRoom.Residents.Add(roomlessStudent);
                await _context.SaveChangesAsync();
                return roomlessStudent;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public async Task UpdateStudent(string id, Student student)
        {
            //TODO if you can change name add a check if there is already a different user with the new name.
            var originalStudent = GetStudentById(id);
            originalStudent.Result.Room = student.Room;
            originalStudent.Result.UserName = student.UserName;
            originalStudent.Result.HouseType = student.HouseType;
            originalStudent.Result.PetType = student.PetType;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteStudent(string id)
        {
            try
            {
                var student = GetStudentById(id);
                _context.Students.Remove(student.Result);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public Task<bool> IsStudentNameAlreadyExists(string studentName)
        {
            var student = _context.Students.FirstOrDefault(s => s.UserName == studentName);

            return Task.FromResult(student != null);
        }
    }
}
