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
    public class StudentService
    {
        private readonly HogwartsContext _context;

        public StudentService(HogwartsContext context)
        {
            _context = context;
        }

        public Task<Student> GetStudent(long studentId)
        {
            var student = _context.Students.Include(student => student.Room).ToListAsync().Result.FirstOrDefault(student => student.ID == studentId);

            return Task.FromResult(student);
        }

        public Task<List<Student>> GetAllStudents()
        {
            var students = _context.Students.ToListAsync().Result;

            return Task.FromResult(students);
        }

        public async Task AddStudent(Student student)
        {
            _context.Students.Add(student);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateStudent(long id, Student student)
        {
            var originalStudent = GetStudent(id);
            originalStudent.Result.Room = student.Room;
            originalStudent.Result.Name = student.Name;
            originalStudent.Result.HouseType = student.HouseType;
            originalStudent.Result.PetType = student.PetType;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteStudent(long id)
        {
            try
            {
                var student = GetStudent(id);
                _context.Students.Remove(student.Result);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
