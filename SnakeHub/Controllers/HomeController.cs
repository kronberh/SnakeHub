using Microsoft.AspNetCore.Mvc;
using SnakeHub.Filters;
using Microsoft.AspNetCore.Authorization;
using SnakeHub.Models;

namespace SnakeHub.Controllers
{
    [SinglePagePartialActionFilter]
    public class HomeController : Controller
	{
		[HttpGet]
        public async Task<IActionResult> IndexAsync()
		{
            return PartialView();
        }

        [HttpGet]
        public async Task<IActionResult> BrowseAsync()
		{
			return PartialView();
        }

        [HttpGet]
        public async Task<IActionResult> YourProjects()
        {
            return PartialView();
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return PartialView();
        }

        [HttpGet]
        public async Task<IActionResult> Play()
        {
            return PartialView();
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> AdminAsync()
        {
            return PartialView();
        }
	}
}
