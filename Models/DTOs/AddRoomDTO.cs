using HogwartsPotions.Models.Enums;

namespace HogwartsPotions.Models.DTOs
{
    public class AddRoomDTO
    {

        public long Capacity { get; set; }

        public HouseType RoomHouseType { get; set; }
    }
}
