using Microsoft.AspNetCore.Identity;

namespace SnakeHub.Models
{
    public class User: IdentityUser
    {
        public int TotalGames { get; set; }
        public int TotalScore { get; set; }
    }
}
