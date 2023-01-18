using HogwartsPotions.Models.Enums;

namespace HogwartsPotions.Models.DTOs
{
    public class AddStudentInProgressDto
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public PetType PetType { get; set; }
        public HouseType ?PreferredHouseType { get; set; }
    }
}
