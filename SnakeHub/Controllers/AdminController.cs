using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SnakeHub.Filters;
using SnakeHub.Models;

namespace SnakeHub.Controllers
{
    [SinglePagePartialActionFilter]
    [Authorize(Roles = "admin")]
    public class AdminController(IConfiguration configuration): Controller
    {
        private readonly IConfiguration _configuration = configuration;

        [HttpGet]
        public async Task<IActionResult> IndexAsync()
        {
            using HttpClient client = new();
            HttpResponseMessage response = await client.GetAsync($"{_configuration["SnakeHubServer:Endpoint"]}{_configuration["SnakeHubServer:UsersSlug"]}");
            response.EnsureSuccessStatusCode();
            string responseContent = await response.Content.ReadAsStringAsync();
            List<User> obj = JsonConvert.DeserializeObject<List<User>>(responseContent) ?? [];
            return PartialView(obj);
        }
    }
}
