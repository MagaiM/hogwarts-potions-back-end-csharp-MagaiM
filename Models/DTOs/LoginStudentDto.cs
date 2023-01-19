#nullable enable
using System.ComponentModel.DataAnnotations;

namespace HogwartsPotions.Models.DTOs
{
    public class LoginStudentDto
    {
        [Required(ErrorMessage = "User Name is required")]
        public string? UserName { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }
    }
}
