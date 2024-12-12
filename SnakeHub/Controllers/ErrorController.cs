using Microsoft.AspNetCore.Mvc;

namespace SnakeHub.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult Index()
        {
            return View("_Error");
        }
    }
}
