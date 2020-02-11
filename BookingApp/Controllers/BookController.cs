using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookingApp.Controllers
{
    public class BookController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BookController(IHttpContextAccessor httpContextAccessor)
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
