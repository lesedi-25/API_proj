
using Microsoft.AspNetCore.Mvc;
using RaceDay.Web.Services;
using RaceDay.Web.ViewModels;

namespace RaceDay.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApiService _apiService;

        public AccountController(ApiService apiService)
        {
            _apiService = apiService;
        }

        // Display the login page
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // Process login through the Part 2 API
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var result = await _apiService.PostAsync<object, AuthResponse>(
                    "api/Auth/login",
                    new
                    {
                        model.Email,
                        model.Password
                    });

                if (result?.User == null ||
                    string.IsNullOrWhiteSpace(result.User.Role))
                {
                    ModelState.AddModelError("", "The API returned an invalid login response.");
                    return View(model);
                }

                // Store the authenticated user's details in session.
                HttpContext.Session.SetInt32("UserId", result.User.UserId);
                HttpContext.Session.SetString("FirstName", result.User.FirstName ?? "");
                HttpContext.Session.SetString("LastName", result.User.LastName ?? "");
                HttpContext.Session.SetString("Email", result.User.Email ?? "");
                HttpContext.Session.SetString("Role", result.User.Role);

                if (result.User.Role.Equals(
                    "Organiser", StringComparison.OrdinalIgnoreCase))
                {
                    return RedirectToAction("Index", "Organiser");
                }

                if (result.User.Role.Equals(
                    "Participant", StringComparison.OrdinalIgnoreCase))
                {
                    return RedirectToAction("Index", "Participant");
                }

                HttpContext.Session.Clear();
                ModelState.AddModelError("", "Your account has an unsupported role.");
                return View(model);
            }
            catch (HttpRequestException ex)
            {
                ModelState.AddModelError(
                    "",
                    "API ERROR: " + ex.Message);

                return View(model);
            }
            catch (TaskCanceledException)
            {
                ModelState.AddModelError(
                    "", "The API request timed out. Please try again.");
                return View(model);
            }
        }

        // Display the registration page
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // Register through the Part 2 API
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var result = await _apiService.PostAsync<object, AuthResponse>(
                    "api/Auth/register",
                    new
                    {
                        model.FirstName,
                        model.LastName,
                        model.Email,
                        model.Password,
                        model.PhoneNumber,
                        model.DateOfBirth,
                        model.Role
                    });

                if (result?.User == null)
                {
                    ModelState.AddModelError(
                        "", "Registration could not be completed.");
                    return View(model);
                }

                TempData["Success"] =
                    "Registration successful. You can now log in.";

                return RedirectToAction("Login");
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError(
                    "", "Registration failed. Check the details or whether the email is already registered.");
                return View(model);
            }
            catch (TaskCanceledException)
            {
                ModelState.AddModelError(
                    "", "The API request timed out. Please try again.");
                return View(model);
            }
        }

        // End the user's session
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return RedirectToAction("Login");
        }
    }
}

