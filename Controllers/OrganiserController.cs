using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RaceDay.MVC.Models;
using RaceDay.MVC.Services;

namespace RaceDay.MVC.Controllers
{
    public class OrganiserController : Controller
    {
        private readonly ApiService _apiService;

        public OrganiserController(ApiService apiService)
        {
            _apiService = apiService;
        }

        private bool IsOrganiserLoggedIn()
        {
            var token = HttpContext.Session.GetString("JWToken");
            var role = HttpContext.Session.GetString("Role");

            return !string.IsNullOrEmpty(token) && role == "Organiser";
        }

        // DASHBOARD
        public IActionResult Dashboard()
        {
            if (!IsOrganiserLoggedIn())
                return RedirectToAction("Login", "Account");

            ViewBag.FirstName = HttpContext.Session.GetString("FirstName");
            ViewBag.Surname = HttpContext.Session.GetString("Surname");
            ViewBag.Email = HttpContext.Session.GetString("Email");

            return View();
        }

        // CREATE EVENT - GET
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            if (!IsOrganiserLoggedIn())
                return RedirectToAction("Login", "Account");

            await LoadRoutes();

            return View();
        }

        // CREATE EVENT - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EventCreateViewModel model)
        {
            if (!IsOrganiserLoggedIn())
                return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
            {
                await LoadRoutes();
                return View(model);
            }

            var token = HttpContext.Session.GetString("JWToken");

            try
            {
                var response = await _apiService.PostAsync(
                    "api/events", model, token);

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Event created successfully!";
                    return RedirectToAction("MyEvents");
                }

                var error = await response.Content.ReadAsStringAsync();

                ModelState.AddModelError("",
                    "Unable to create event. " + error);
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError("",
                    "Could not connect to the RaceDay API.");
            }

            await LoadRoutes();
            return View(model);
        }

        // MY EVENTS
        [HttpGet]
        public async Task<IActionResult> MyEvents()
        {
            if (!IsOrganiserLoggedIn())
                return RedirectToAction("Login", "Account");

            var token = HttpContext.Session.GetString("JWToken");

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

        // EDIT EVENT - GET
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (!IsOrganiserLoggedIn())
                return RedirectToAction("Login", "Account");

            var token = HttpContext.Session.GetString("JWToken");

            try
            {
                var ev = await _apiService.GetAsync<EventViewModel>(
                    $"api/events/{id}", token);

                if (ev == null)
                    return NotFound();

                var model = new EventCreateViewModel
                {
                    EventName = ev.EventName,
                    Description = ev.Description ?? string.Empty,
                    EventDate = ev.EventDate,
                    Location = ev.Location ?? string.Empty,
                    EventType = ev.EventType ?? string.Empty,
                    RouteMapUrl = ev.RouteMapUrl ?? string.Empty,
                    RouteId = ev.RouteId
                };

                await LoadRoutes();

                ViewBag.EventId = id;

                return View(model);
            }
            catch (HttpRequestException)
            {
                TempData["Error"] = "Unable to load the event.";

                return RedirectToAction("MyEvents");
            }
        }

        // EDIT EVENT - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id, EventCreateViewModel model)
        {
            if (!IsOrganiserLoggedIn())
                return RedirectToAction("Login", "Account");

            var token = HttpContext.Session.GetString("JWToken");

            if (!ModelState.IsValid)
            {
                await LoadRoutes();

                ViewBag.EventId = id;

                return View(model);
            }

            try
            {
                var response = await _apiService.PutAsync(
                    $"api/events/{id}", model, token);

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Event updated successfully!";

                    return RedirectToAction("MyEvents");
                }

                var error = await response.Content.ReadAsStringAsync();

                ModelState.AddModelError("",
                    "Unable to update event. " + error);
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError("",
                    "Could not connect to the RaceDay API.");
            }

            await LoadRoutes();

            ViewBag.EventId = id;

            return View(model);
        }

        // DELETE EVENT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (!IsOrganiserLoggedIn())
                return RedirectToAction("Login", "Account");

            var token = HttpContext.Session.GetString("JWToken");

            try
            {
                var response = await _apiService.DeleteAsync(
                    $"api/events/{id}", token);

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Event deleted successfully!";
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();

                    TempData["Error"] =
                        "Unable to delete event. " + error;
                }
            }
            catch (HttpRequestException)
            {
                TempData["Error"] =
                    "Could not connect to the RaceDay API.";
            }

            return RedirectToAction("MyEvents");
        }

        // CREATE ROUTE - GET
        [HttpGet]
        public IActionResult CreateRoute()
        {
            if (!IsOrganiserLoggedIn())
                return RedirectToAction("Login", "Account");

            return View();
        }

        // CREATE ROUTE - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRoute(RouteCreateViewModel model)
        {
            if (!IsOrganiserLoggedIn())
                return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
                return View(model);

            var token = HttpContext.Session.GetString("JWToken");

            try
            {
                var response = await _apiService.PostAsync(
                    "api/routes", model, token);

                if (response.IsSuccessStatusCode)
                {
                    TempData["RouteSuccess"] = "Route created successfully!";

                    return RedirectToAction("Dashboard");
                }

                var error = await response.Content.ReadAsStringAsync();

                ModelState.AddModelError("",
                    "Unable to create route: " + error);
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError("",
                    "Could not connect to the RaceDay API.");
            }

            return View(model);
        }

        // CREATE CATEGORY - GET
        [HttpGet]
        public async Task<IActionResult> CreateCategory()
        {
            if (!IsOrganiserLoggedIn())
                return RedirectToAction("Login", "Account");

            var token = HttpContext.Session.GetString("JWToken");

            await LoadEvents(token);

            return View();
        }

        // CREATE CATEGORY - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCategory(
            CategoryCreateViewModel model)
        {
            if (!IsOrganiserLoggedIn())
                return RedirectToAction("Login", "Account");

            var token = HttpContext.Session.GetString("JWToken");

            if (!ModelState.IsValid)
            {
                await LoadEvents(token);
                return View(model);
            }

            try
            {
                var response = await _apiService.PostAsync(
                    "api/categories", model, token);

                if (response.IsSuccessStatusCode)
                {
                    TempData["CategorySuccess"] =
                        "Category created successfully!";

                    return RedirectToAction("Dashboard");
                }

                var error = await response.Content.ReadAsStringAsync();

                ModelState.AddModelError("",
                    "Unable to create category. " + error);
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError("",
                    "Unable to connect to the API.");
            }

            await LoadEvents(token);

            return View(model);
        }

        // LOAD ROUTES
        private async Task LoadRoutes()
        {
            var token = HttpContext.Session.GetString("JWToken");

            var routes = await _apiService.GetAsync<List<RouteViewModel>>(
                "api/routes", token);

            ViewBag.Routes = new SelectList(
                routes ?? new List<RouteViewModel>(),
                "RouteId",
                "RouteName");
        }

        // LOAD EVENTS
        private async Task LoadEvents(string? token)
        {
            var events = await _apiService.GetAsync<List<EventViewModel>>(
                "api/events", token);

            ViewBag.Events = new SelectList(
                events ?? new List<EventViewModel>(),
                "EventId",
                "EventName");
        }
    }
}