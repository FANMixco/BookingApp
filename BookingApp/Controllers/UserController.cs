using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookingApp.Controllers
{
    public class UserController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserController(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Update(int id)
        {
            return View();
        }
    }
}
