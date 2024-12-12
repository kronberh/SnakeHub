using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SnakeHub.Filters;
using SnakeHub.FormModels;
using SnakeHub.Models;
using SnakeHub.Models.Game;
using System.Security.Claims;
using System.Text;

namespace SnakeHub.Controllers
{
    public class GamesController(IConfiguration configuration) : Controller
    {
        private readonly IConfiguration _configuration = configuration;

        [SinglePagePartialActionFilter]
        [HttpGet]
        public async Task<IActionResult> SessionsAsync()
        {
            using HttpClient client = new();
            HttpResponseMessage response = await client.GetAsync($"{_configuration["SnakeHubServer:Endpoint"]}{_configuration["SnakeHubServer:SessionsSlug"]}");
            response.EnsureSuccessStatusCode();
            string responseContent = await response.Content.ReadAsStringAsync();
            IEnumerable<SessionInfo> obj = JsonConvert.DeserializeObject<IEnumerable<SessionInfo>>(responseContent) ?? [];
            return PartialView(obj);
        }

        [SinglePagePartialActionFilter]
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> HostAsync(string modeId)
        {
            using HttpClient client = new();
            HttpResponseMessage response = await client.PostAsync($"{_configuration["SnakeHubServer:Endpoint"]}{_configuration["SnakeHubServer:SessionsSlug"]}?playerId={User.FindFirst(ClaimTypes.NameIdentifier)!.Value}&modeId={modeId}", null);
            response.EnsureSuccessStatusCode();
            string responseContent = await response.Content.ReadAsStringAsync();
            return PartialView("Game", new { gameId = responseContent, isHost=  true });
        }

        [SinglePagePartialActionFilter]
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> JoinAsync(string gameId)
        {
            using HttpClient client = new();
            HttpResponseMessage response = await client.PostAsync($"{_configuration["SnakeHubServer:Endpoint"]}{_configuration["SnakeHubServer:JoinGameSlug"]}?gameId={gameId}&playerId={User.FindFirst(ClaimTypes.NameIdentifier)!.Value}", null);
            response.EnsureSuccessStatusCode();
            return PartialView("Game", new { gameId, isHost = false });
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> StartGameAsync(string gameId)
        {
            using HttpClient client = new();
            HttpResponseMessage response = await client.PostAsync($"{_configuration["SnakeHubServer:Endpoint"]}{_configuration["SnakeHubServer:StartGameSlug"]}?gameId={gameId}", null);
            response.EnsureSuccessStatusCode();
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetGameStateAsync(string gameId)
        {
            using HttpClient client = new();
            HttpResponseMessage response = await client.GetAsync($"{_configuration["SnakeHubServer:Endpoint"]}{_configuration["SnakeHubServer:GetGameStateSlug"]}?gameId={gameId}");
            response.EnsureSuccessStatusCode();
            string responseContent = await response.Content.ReadAsStringAsync();
            GameObject[][] obj = JsonConvert.DeserializeObject<GameObject[][]>(responseContent) ?? [[]];
            return Ok(obj);
        }

        [HttpGet]
        public async Task<IActionResult> GetPlayerScoreAsync(string gameId)
        {
            using HttpClient client = new();
            HttpResponseMessage response = await client.GetAsync($"{_configuration["SnakeHubServer:Endpoint"]}{_configuration["SnakeHubServer:GetPlayerScoreSlug"]}?gameId={gameId}&playerId={User.FindFirst(ClaimTypes.NameIdentifier)!.Value}");
            response.EnsureSuccessStatusCode();
            string responseContent = await response.Content.ReadAsStringAsync();
            int obj = JsonConvert.DeserializeObject<int>(responseContent);
            return Ok(obj);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> PlayerActionAsync([FromQuery] string gameId, [FromBody] PlayerActionFormModel formModel)
        {
            using HttpClient client = new();
            StringContent requestContent = new(JsonConvert.SerializeObject(formModel), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync($"{_configuration["SnakeHubServer:Endpoint"]}{_configuration["SnakeHubServer:PlayerActionSlug"]}?gameId={gameId}&playerId={User.FindFirst(ClaimTypes.NameIdentifier)!.Value}", requestContent);
            response.EnsureSuccessStatusCode();
            string responseContent = await response.Content.ReadAsStringAsync();
            bool obj = JsonConvert.DeserializeObject<bool>(responseContent);
            return Ok(obj);
        }
    }
}
