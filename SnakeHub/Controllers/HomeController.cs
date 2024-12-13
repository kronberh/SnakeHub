using Microsoft.AspNetCore.Mvc;
using SnakeHub.Filters;

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
	}
}
