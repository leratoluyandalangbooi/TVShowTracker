using Microsoft.AspNetCore.Mvc;

namespace TVShowTracker.API.Controllers
{
    public class TvShowController : Controller
    {
        // POST: importTvShows..this will reaad the api and import the tv shows. this must be authenticated.
        public IActionResult Index()
        {
            return View();
        }
    }
}
