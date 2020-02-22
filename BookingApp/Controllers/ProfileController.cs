using Microsoft.AspNetCore.Mvc;

namespace BookingApp.Controllers
{
    public class ProfileController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}