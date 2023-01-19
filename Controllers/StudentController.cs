using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using HogwartsPotions.Auth;
using HogwartsPotions.Helpers;
using HogwartsPotions.Models.DTOs;
using HogwartsPotions.Models.Entities;
using HogwartsPotions.Models.Enums;
using HogwartsPotions.Models.View_Models;
using HogwartsPotions.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace HogwartsPotions.Controllers
{
    [ApiController, Route("/student")]
    public class StudentController : ControllerBase
    {
        private readonly StudentService _studentService;
        private readonly RoomService _roomService;
        private readonly IConfiguration _configuration;
        private readonly UserManager<Student> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly StudentHelper _studentHelper = new();

        public StudentController(
            StudentService studentService, 
            RoomService roomService, 
            UserManager<Student> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration)
        {
            _studentService = studentService;
            _roomService = roomService;
            _userManager = userManager;
            _configuration = configuration;
            _roleManager = roleManager;
        }

        [HttpGet]
        public async Task<List<Student>> GetAllStudents()
        {
            return await _studentService.GetAllStudents();
        }

        [HttpPost("/student/register")]
        public async Task<IActionResult> RegisterStudent([FromBody] RegisterStudentDto model)
        {
            try
            {
                var userExists = await _userManager.FindByNameAsync(model.UserName);
                if (userExists != null)
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User already exists!" });
                
                var newStudentHouseType = model.PreferredHouseType == null || (int)model.PreferredHouseType < 0 || (int)model.PreferredHouseType > 3 ?
                    _studentHelper.GetRandomHouseType() :
                    _studentHelper.GetRandomHouseType((HouseType)model.PreferredHouseType);

                var newStudent = new Student
                {
                    HouseType = newStudentHouseType,
                    UserName = model.UserName,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    PetType = model.PetType
                };
                var result = await _userManager.CreateAsync(newStudent, model.Password);
                if (!result.Succeeded)
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User creation failed! Please check user details and try again." });

                return Ok(new Response { Status = "Success", Message = "User created successfully!" });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest("Something went wrong");
            }
        }

        [HttpPost("/admin/register")]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterStudentDto model)
        {
            var userExists = await _userManager.FindByNameAsync(model.UserName);
            if (userExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User already exists!" });

            Student user = new()
            {
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.UserName
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User creation failed! Please check user details and try again." });

            if (!await _roleManager.RoleExistsAsync(UserRoles.Admin))
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
            if (!await _roleManager.RoleExistsAsync(UserRoles.Student))
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.Student));

            if (await _roleManager.RoleExistsAsync(UserRoles.Admin))
            {
                await _userManager.AddToRoleAsync(user, UserRoles.Admin);
            }
            if (await _roleManager.RoleExistsAsync(UserRoles.Admin))
            {
                await _userManager.AddToRoleAsync(user, UserRoles.Student);
            }
            return Ok(new Response { Status = "Success", Message = "User created successfully!" });
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

        [HttpPost("/student/login")]
        public async Task<IActionResult> LogIn([FromBody] LoginStudentDto model)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                var userRoles = await _userManager.GetRolesAsync(user);

                var authClaims = new List<Claim>
                {
                    new(ClaimTypes.Name, user.UserName),
                    new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                var token = GetToken(authClaims);

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo
                });
            }
            return Unauthorized();
        }

        private JwtSecurityToken GetToken(IEnumerable<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"] ?? "JWTAuthenticationHIGHsecuredPasswordQSDaer163Etz"));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return token;
        }
    }
}

//Create ValidRoom If it does not exists
//var availableRooms = _roomService.GetAvailableRooms(roomlessStudent);
//if (!availableRooms.Result.Any())
//{
//    var newRoomDto = new AddRoomDto()
//    {
//        Capacity = 10,
//        RoomHouseType = roomlessStudent.HouseType
//    };
//    var newRoom = await _roomService.AddRoom(newRoomDto);
//    availableRooms.Result.Add(newRoom);
//}

//var roomlessStudentView = new RoomlessStudentView()
//{
//    Id = roomlessStudent.Id,
//    UserName = roomlessStudent.UserName,
//    HouseType = roomlessStudent.HouseType,
//    PetType = roomlessStudent.PetType,
//    AvailableRooms = availableRooms.Result
//};
//return Ok(roomlessStudentView);
