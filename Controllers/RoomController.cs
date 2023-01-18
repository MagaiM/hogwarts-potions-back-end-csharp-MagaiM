using System.Collections.Generic;
using System.Threading.Tasks;
using HogwartsPotions.Models.DTOs;
using HogwartsPotions.Models.Entities;
using HogwartsPotions.Services;
using Microsoft.AspNetCore.Mvc;

namespace HogwartsPotions.Controllers
{
    [ApiController, Route("/room")]
    public class RoomController : ControllerBase
    {
        private readonly RoomService _roomService;

        public RoomController(RoomService roomService)
        {
            _roomService = roomService;
        }

        [HttpGet]
        public async Task<List<Room>> GetAllRooms()
        {
            return await _roomService.GetAllRooms();
        }

        [HttpPost]
        public async Task<Room> AddRoom([FromBody] AddRoomDto room)
        {
            return await _roomService.AddRoom(room);
        }

        [HttpGet("/room/{id:long}")]
        public async Task<Room> GetRoomById(long id)
        {
            return await _roomService.GetRoom(id);
        }

        [HttpPut("/room/{id:long}")]
        public async Task<IActionResult> UpdateRoomById(long id, [FromBody] Room updatedRoom)
        {
            var result = await _roomService.UpdateRoom(id, updatedRoom);
            return result == "Success" ? Ok() : BadRequest(result);
        }

        [HttpDelete("/room/{id:long}")]
        public async Task<IActionResult> DeleteRoomById(long id)
        {
            var result = await _roomService.DeleteRoom(id);
            return result == "Success" ? Ok() : BadRequest(result);
        }

        [HttpGet("/room/rat-friendly")]
        public async Task<List<Room>> GetRoomsForRatOwners()
        {
            return await _roomService.GetRoomsForRatOwners();
        }

        [HttpPut("/room/{id:long}/move-student")]
        public async Task<IActionResult> AddStudentToRoom(long id, string studentId)
        {
            var result = await _roomService.AddStudentToRoom(id, studentId);
            return result == "Success" ? Ok() : BadRequest(result);
        }
    }
}
