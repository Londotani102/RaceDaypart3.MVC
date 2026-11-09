
using Microsoft.AspNetCore.Mvc;
using RaceDay.MVC.Models;
using RaceDay.MVC.Services;
using System.Text.Json;

namespace RaceDay.MVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApiService _apiService;

        public AccountController(ApiService apiService)
        {
            _apiService = apiService;
        }

        // GET: Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var response = await _apiService.PostAsync(
                "api/auth/register", model);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Registration successful. You can now log in.";
                return RedirectToAction("Login");
            }

            var error = await response.Content.ReadAsStringAsync();

            ModelState.AddModelError(
                string.Empty,
                $"Registration failed (Status: {(int)response.StatusCode}). API response: {error}"
            );

            return View(model);
        }

        // GET: Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var response = await _apiService.PostAsync(
                "api/auth/login", model);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty,
                    "Invalid email or password.");

                return View(model);
            }

            var json = await response.Content.ReadAsStringAsync();

            using var document = JsonDocument.Parse(json);

            var root = document.RootElement;

            string token = root.GetProperty("token").GetString() ?? "";

            var user = root.GetProperty("user");

            HttpContext.Session.SetString("JWToken", token);
            HttpContext.Session.SetString(
                "FirstName", user.GetProperty("firstName").GetString() ?? "");
            HttpContext.Session.SetString(
                "Surname", user.GetProperty("surname").GetString() ?? "");
            HttpContext.Session.SetString(
                "Email", user.GetProperty("email").GetString() ?? "");
            HttpContext.Session.SetString(
                "Role", user.GetProperty("role").GetString() ?? "");

            // Redirect users according to their role
            string role = HttpContext.Session.GetString("Role") ?? "";

            if (role == "Participant")
            {
                return RedirectToAction("Dashboard", "Participant");
            }
            else if (role == "Organiser")
            {
                return RedirectToAction("Dashboard", "Organiser");
            }

            return RedirectToAction("Index", "Home");
        }

        // GET: Account/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return RedirectToAction("Index", "Home");
        }
    }
}
