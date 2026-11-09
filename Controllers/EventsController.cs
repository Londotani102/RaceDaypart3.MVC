
using Microsoft.AspNetCore.Mvc;
using RaceDay.MVC.Models;
using RaceDay.MVC.Services;

namespace RaceDay.MVC.Controllers
{
    public class EventsController : Controller
    {
        private readonly ApiService _apiService;

        public EventsController(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index()
        {
            var token = HttpContext.Session.GetString("JWToken");
            var role = HttpContext.Session.GetString("Role");

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            if (role != "Participant")
            {
                return RedirectToAction("Index", "Home");
            }

            try
            {
                var events = await _apiService.GetAsync<List<EventViewModel>>(
                    "api/events", token);

                return View(events ?? new List<EventViewModel>());
            }
            catch (HttpRequestException)
            {
                ViewBag.Error = "Unable to load events. Please try again.";
                return View(new List<EventViewModel>());
            }
        }
    }
}
