using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HogwartsPotions.Models.DTOs;
using HogwartsPotions.Models.Entities;
using HogwartsPotions.Models.View_Models;
using HogwartsPotions.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HogwartsPotions.Controllers
{
    [ApiController, Route("/student")]
    public class StudentController : ControllerBase
    {
        private readonly StudentService _studentService;
        private readonly RoomService _roomService;
        private readonly UserManager<Student> _userManager;
        private readonly SignInManager<Student> _signInManager;

        public StudentController(StudentService studentService, RoomService roomService, UserManager<Student> userManager, SignInManager<Student> signInManager)
        {
            _studentService = studentService;
            _roomService = roomService;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public async Task<List<Student>> GetAllStudents()
        {
            return await _studentService.GetAllStudents();
        }

        [HttpPost("/student/register")]
        public async Task<IActionResult> AddStudent([FromBody] AddStudentInProgressDto student)
        {
            try
            {
                if (_studentService.IsStudentNameAlreadyExists(student.UserName).Result)
                    return BadRequest("Student with this name already exists!");
                var roomlessStudent = await _studentService.AddRoomlessStudent(student);
                var availableRooms = _roomService.GetAvailableRooms(roomlessStudent);
                if (!availableRooms.Result.Any())
                {
                    var newRoomDto = new AddRoomDto()
                    {
                        Capacity = 10,
                        RoomHouseType = roomlessStudent.HouseType
                    };
                    var newRoom = await _roomService.AddRoom(newRoomDto);
                    availableRooms.Result.Add(newRoom);
                }

                var roomlessStudentView = new RoomlessStudentView()
                {
                    Id = roomlessStudent.Id,
                    UserName = roomlessStudent.UserName,
                    HouseType = roomlessStudent.HouseType,
                    PetType = roomlessStudent.PetType,
                    AvailableRooms = availableRooms.Result
                };
                return Ok(roomlessStudentView);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest("Something went wrong");
            }
        }

        [HttpPost("/student/add-room/{id}")]
        public async Task<IActionResult> AddStudentToRoom(string id, [FromBody] Room room)
        {
            try
            {
                var validRoom = _roomService.GetValidRoom(room.Id, id).Result;
                if (validRoom == null) return BadRequest("Invalid room was given!");
                var student =  _studentService.GetStudentById(id).Result;
                if (student == null) return BadRequest("Invalid student id! Or other failures occurred!");
                if (student.HouseType != validRoom.RoomHouseType) return BadRequest("You can only select a room with the same House type as yours!");
                await _studentService.AddStudentToRoom(id, validRoom);
                var studentView = new StudentView
                {
                    Id = student.Id,
                    UserName = student.UserName,
                    HouseType = student.HouseType,
                    PetType = student.PetType,
                    Room = student.Room
                };

                return Ok(studentView);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest("Something went wrong");
            }
        }

        [HttpGet("/student/{id}")]
        public async Task<StudentView> GetStudentById(string id)
        {
            var student = await _studentService.GetStudentById(id);

            var studentView = new StudentView()
            {
                Id = student.Id,
                UserName = student.UserName,
                HouseType = student.HouseType,
                PetType = student.PetType,
                Room = new Room()
                {
                    Id = student.Room.Id,
                    RoomHouseType = student.Room.RoomHouseType,
                    Capacity = student.Room.Capacity
                }
            };
            return studentView;
        }

        [HttpPut("/student/{id}")]
        public async Task UpdateStudentById(string id, [FromBody] Student updatedStudent)
        {
            await _studentService.UpdateStudent(id, updatedStudent);
        }

        [HttpDelete("/student/{id}")]
        public async Task DeleteStudentById(string id)
        {
            await _studentService.DeleteStudent(id);
        }

        [HttpPost("/login")]
        public async Task<IActionResult> LogIn([FromBody] Student student)
        {
            //TODO implement login
            await _signInManager.SignInAsync(student, isPersistent: true);
            return Ok();
        }
    }
}
