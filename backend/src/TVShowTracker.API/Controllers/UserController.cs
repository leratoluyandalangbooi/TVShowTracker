using Microsoft.AspNetCore.Mvc;

namespace TVShowTracker.API.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
