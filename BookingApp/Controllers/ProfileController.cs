using BookingApp.Filters.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingApp.Controllers
{
    [Authorize(Roles.CUSTOMER, Roles.ADMIN)]
    public class ProfileController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}