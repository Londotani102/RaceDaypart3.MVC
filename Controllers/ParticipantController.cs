using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RaceDay.MVC.Models;
using RaceDay.MVC.Services;

namespace RaceDay.MVC.Controllers
{
    public class ParticipantController : Controller
    {
        private readonly ApiService _apiService;

        public ParticipantController(ApiService apiService)
        {
            _apiService = apiService;
        }

        // CHECK PARTICIPANT LOGIN
        private bool IsParticipantLoggedIn()
        {
            var token = HttpContext.Session.GetString("JWToken");
            var role = HttpContext.Session.GetString("Role");

            return !string.IsNullOrEmpty(token) && role == "Participant";
        }

        // DASHBOARD
        public IActionResult Dashboard()
        {
            if (!IsParticipantLoggedIn())
                return RedirectToAction("Login", "Account");

            ViewBag.FirstName = HttpContext.Session.GetString("FirstName");
            ViewBag.Surname = HttpContext.Session.GetString("Surname");
            ViewBag.Email = HttpContext.Session.GetString("Email");

            return View();
        }

        // PROFILE - GET
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            if (!IsParticipantLoggedIn())
                return RedirectToAction("Login", "Account");

            var token = HttpContext.Session.GetString("JWToken");

            try
            {
                var profile = await _apiService.GetAsync<ProfileViewModel>(
                    "api/profile", token);

                if (profile == null)
                {
                    ViewBag.Error = "Profile could not be found.";

                    return View(new ProfileViewModel());
                }

                return View(profile);
            }
            catch (HttpRequestException)
            {
                ViewBag.Error = "Unable to load your profile.";

                return View(new ProfileViewModel());
            }
        }

        // PROFILE - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(ProfileViewModel model)
        {
            if (!IsParticipantLoggedIn())
                return RedirectToAction("Login", "Account");

            var token = HttpContext.Session.GetString("JWToken");

            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var response = await _apiService.PutAsync(
                    "api/profile", model, token);

                if (response.IsSuccessStatusCode)
                {
                    HttpContext.Session.SetString(
                        "FirstName", model.FirstName);

                    HttpContext.Session.SetString(
                        "Surname", model.Surname);

                    TempData["Success"] =
                        "Your profile was updated successfully!";

                    return RedirectToAction("Profile");
                }

                ViewBag.Error = "Unable to update your profile.";
            }
            catch (HttpRequestException)
            {
                ViewBag.Error = "Unable to connect to the API.";
            }

            return View(model);
        }

        // MY ENROLMENTS
        [HttpGet]
        public async Task<IActionResult> MyEnrolments()
        {
            if (!IsParticipantLoggedIn())
                return RedirectToAction("Login", "Account");

            var token = HttpContext.Session.GetString("JWToken");

            try
            {
                var enrolments =
                    await _apiService.GetAsync<List<EnrolmentResultViewModel>>(
                        "api/enrolments", token);

                return View(
                    enrolments ?? new List<EnrolmentResultViewModel>());
            }
            catch (HttpRequestException)
            {
                ViewBag.Error = "Unable to load your enrolments.";

                return View(new List<EnrolmentResultViewModel>());
            }
        }

        // ENROL - GET
        [HttpGet]
        public async Task<IActionResult> Enrol(int eventId)
        {
            if (!IsParticipantLoggedIn())
                return RedirectToAction("Login", "Account");

            var token = HttpContext.Session.GetString("JWToken");

            await LoadCategories(eventId, token);

            var model = new EnrolmentViewModel
            {
                EventId = eventId
            };

            return View(model);
        }

        // ENROL - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Enrol(EnrolmentViewModel model)
        {
            if (!IsParticipantLoggedIn())
                return RedirectToAction("Login", "Account");

            var token = HttpContext.Session.GetString("JWToken");

            if (!ModelState.IsValid)
            {
                await LoadCategories(model.EventId, token);
                return View(model);
            }

            try
            {
                var response = await _apiService.PostAsync(
                    "api/enrolments", model, token);

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] =
                        "You have successfully enrolled in the event!";

                    return RedirectToAction("MyEnrolments");
                }

                ModelState.AddModelError("",
                    "Unable to enrol. You may already be enrolled or the selected category may be invalid.");
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError("",
                    "Unable to connect to the API.");
            }

            await LoadCategories(model.EventId, token);

            return View(model);
        }

        // LOAD CATEGORIES
        private async Task LoadCategories(int eventId, string? token)
        {
            var categories =
                await _apiService.GetAsync<List<CategoryViewModel>>(
                    "api/categories", token);

            var eventCategories = categories?
                .Where(c => c.EventId == eventId)
                .ToList() ?? new List<CategoryViewModel>();

            ViewBag.Categories = new SelectList(
                eventCategories,
                "CategoryId",
                "CategoryName");
        }
    }
}