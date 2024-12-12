using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace SnakeHub.FormModels
{
    public class RegisterFormModel
    {
        [FromForm(Name = "login")]
        [RegularExpression(@"^\w*$", ErrorMessage = "Login can only contain letters, numbers and underscores.")]
        [Required]
        public string Login { get; set; } = null!;
        [FromForm(Name = "password")]
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;
        [FromForm(Name = "confirm-password")]
        [Required]
        [DataType(DataType.Password)]
        [Compare("Password")]
        public string ConfirmPassword { get; set; } = null!;
    }
}
