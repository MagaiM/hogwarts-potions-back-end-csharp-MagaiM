#nullable enable
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using HogwartsPotions.Models.Enums;

namespace HogwartsPotions.Models.DTOs
{
    public class RegisterStudentDto
    {
        [Required(ErrorMessage = "User Name is required")]
        public string? UserName { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }

        [DefaultValue(PetType.None)]
        public PetType PetType { get; set; }

        public HouseType ?PreferredHouseType { get; set; }

    }
}
