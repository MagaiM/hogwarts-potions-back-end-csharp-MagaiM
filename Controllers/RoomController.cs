using System.Collections.Generic;
using System.Threading.Tasks;
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
        public async Task<Room> AddRoom([FromBody] Room room)
        {
            return await _roomService.AddRoom(room);
        }

        [HttpGet("/room/{id:long}")]
        public async Task<Room> GetRoomById(long id)
        {
            return await _roomService.GetRoom(id);
        }

        [HttpPut("/room/{id:long}")]
        public async Task UpdateRoomById(long id, [FromBody] Room updatedRoom)
        {
            await _roomService.UpdateRoom(id, updatedRoom);
        }

        [HttpDelete("/room/{id:long}")]
        public async Task DeleteRoomById(long id)
        {
            await _roomService.DeleteRoom(id);
        }

        [HttpGet("/room/rat-owners")]
        public async Task<List<Room>> GetRoomsForRatOwners()
        {
            return await _roomService.GetRoomsForRatOwners();
        }
    }
}
