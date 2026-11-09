using Microsoft.AspNetCore.Mvc;
using RaceDay.MVC.Models;
using RaceDay.MVC.Services;

namespace RaceDay.MVC.Controllers
{
    public class ResultsController : Controller
    {
        private readonly ApiService _apiService;

        public ResultsController(ApiService apiService)
        {
            _apiService = apiService;
        }

        private bool IsOrganiser()
        {
            return !string.IsNullOrEmpty(
                HttpContext.Session.GetString("JWToken"))
                && HttpContext.Session.GetString("Role") == "Organiser";
        }

        private bool IsParticipant()
        {
            return !string.IsNullOrEmpty(
                HttpContext.Session.GetString("JWToken"))
                && HttpContext.Session.GetString("Role") == "Participant";
        }

        // Organiser: display result entry form
        [HttpGet]
        public IActionResult Create()
        {
            if (!IsOrganiser())
                return RedirectToAction("Login", "Account");

            return View(new ResultCreateViewModel());
        }

        // Organiser: submit a result
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ResultCreateViewModel model)
        {
            if (!IsOrganiser())
                return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
                return View(model);

            var token = HttpContext.Session.GetString("JWToken");

            try
            {
                var response = await _apiService.PostAsync(
                    "api/results", model, token);

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Result recorded successfully!";
                    return RedirectToAction("Index");
                }

                var error = await response.Content.ReadAsStringAsync();

                ModelState.AddModelError("",
                    "Unable to record result. " + error);
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError("",
                    "Could not connect to the RaceDay API.");
            }

            return View(model);
        }

        // Organiser: view all results
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (!IsOrganiser())
                return RedirectToAction("Login", "Account");

            var token = HttpContext.Session.GetString("JWToken");

            try
            {
                var results = await _apiService.GetAsync<List<ResultViewModel>>(
                    "api/results", token);

                return View(results ?? new List<ResultViewModel>());
            }
            catch (HttpRequestException)
            {
                ViewBag.Error = "Unable to load results.";
                return View(new List<ResultViewModel>());
            }
        }

        // Participant: view own results
        [HttpGet]
        public async Task<IActionResult> MyResults()
        {
            if (!IsParticipant())
                return RedirectToAction("Login", "Account");

            var token = HttpContext.Session.GetString("JWToken");

            try
            {
                var results = await _apiService.GetAsync<List<ResultViewModel>>(
                    "api/results/me", token);

                return View(results ?? new List<ResultViewModel>());
            }
            catch (HttpRequestException)
            {
                ViewBag.Error = "Unable to load your results.";
                return View(new List<ResultViewModel>());
            }
        }
    }
}