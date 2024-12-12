using Microsoft.AspNetCore.Mvc;

namespace SnakeHub.FormModels
{
    public class LoginFormModel
    {
        [FromForm(Name = "login")]
        public string Login { get; set; } = null!;
        [FromForm(Name = "password")]
        public string Password { get; set; } = null!;
    }
}
