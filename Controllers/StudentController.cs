using System.Collections.Generic;
using System.Threading.Tasks;
using HogwartsPotions.Models.Entities;
using HogwartsPotions.Services;
using Microsoft.AspNetCore.Mvc;

namespace HogwartsPotions.Controllers
{
    [ApiController, Route("/student")]
    public class StudentController : ControllerBase
    {
        private readonly StudentService _studentService;

        public StudentController(StudentService studentService)
        {
            _studentService = studentService;
        }

        [HttpGet]
        public async Task<List<Student>> GetAllStudents()
        {
            return await _studentService.GetAllStudents();
        }

        [HttpPost]
        public async Task AddStudent([FromBody] Student student)
        {
            await _studentService.AddStudent(student);
        }

        [HttpGet("/student/{id:long}")]
        public async Task<Student> GetStudentById(long id)
        {
            return await _studentService.GetStudent(id);
        }

        [HttpPut("/student/{id:long}")]
        public async Task UpdateStudentById(long id, [FromBody] Student updatedStudent)
        {
            await _studentService.UpdateStudent(id, updatedStudent);
        }

        [HttpDelete("/student/{id:long}")]
        public async Task DeleteStudentById(long id)
        {
            await _studentService.DeleteStudent(id);
        }
    }
}
