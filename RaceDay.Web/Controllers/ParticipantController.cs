using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using RaceDay.Web.Services;
using RaceDay.Web.ViewModels;

namespace RaceDay.Web.Controllers
{
    public class ParticipantController : Controller
    {
        private readonly ApiService _apiService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ParticipantController(
       ApiService apiService,
       IWebHostEnvironment webHostEnvironment)
        {
            _apiService = apiService;
            _webHostEnvironment = webHostEnvironment;
        }

        // Display the participant dashboard and upcoming events.
        // Display the participant dashboard and upcoming events.
        public async Task<IActionResult> Index(string? eventType)
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
                return RedirectToAction("Login", "Account");

            try
            {
                var events = await _apiService.GetAsync<List<EventViewModel>>(
                    "api/Events");

                var eventList = events ?? new List<EventViewModel>();

                // Filter events by event type.
                if (!string.IsNullOrWhiteSpace(eventType) && eventType != "All")
                {
                    eventList = eventList
                        .Where(e => !string.IsNullOrWhiteSpace(e.EventType) &&
                                    e.EventType.StartsWith(
                                        eventType,
                                        StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }

                // Load the saved banner image for each event.
                foreach (var eventItem in eventList)
                {
                    eventItem.EventBannerPath = GetEventBannerPath(eventItem.EventId);
                }

                ViewBag.SelectedEventType = eventType ?? "All";

                return View(eventList);
            }
            catch (HttpRequestException)
            {
                ViewBag.Error = "Could not load events. Please try again.";
                return View(new List<EventViewModel>());
            }
        }

        // Display details of a selected event.
        public async Task<IActionResult> Details(int id)
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
                return RedirectToAction("Login", "Account");

            try
            {
                var eventDetails = await _apiService.GetAsync<EventViewModel>(
                    $"api/Events/{id}");

                if (eventDetails == null)
                    return NotFound();
                // Load the saved banner image for this event.
                eventDetails.EventBannerPath = GetEventBannerPath(eventDetails.EventId);

                var categories = await _apiService.GetAsync<List<CategoryViewModel>>(
                    $"api/Categories/event/{id}");

                ViewBag.Categories = categories ?? new List<CategoryViewModel>();

                return View(eventDetails);
            }
            catch (HttpRequestException)
            {
                TempData["Error"] = "Unable to load event details. Please try again.";
                return RedirectToAction("Index");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Enrol(EnrolmentViewModel model)
        {
            int? participantId = HttpContext.Session.GetInt32("UserId");

            if (participantId == null)
                return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Please select a valid category.";
                return RedirectToAction("Details", new { id = model.EventId });
            }

            try
            {
                var result = await _apiService.PostAsync<object, System.Text.Json.JsonElement>(
                    "api/Enrolments",
                    new
                    {
                        ParticipantId = participantId.Value,
                        EventId = model.EventId,
                        CategoryId = model.CategoryId
                    });

                TempData["Success"] = "You have successfully enrolled in this event!";
                return RedirectToAction("MyEnrolments");
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = ex.StatusCode == System.Net.HttpStatusCode.Conflict
                    ? "You are already enrolled in this event."
                    : "Enrolment failed. Please check your selection and try again.";

                return RedirectToAction("Details", new { id = model.EventId });
            }
            catch (TaskCanceledException)
            {
                TempData["Error"] = "The request timed out. Please try again.";
                return RedirectToAction("Details", new { id = model.EventId });
            }
        }


        public async Task<IActionResult> MyEnrolments()
        {
            int? participantId = HttpContext.Session.GetInt32("UserId");

            if (participantId == null)
                return RedirectToAction("Login", "Account");

            try
            {
                var enrolments = await _apiService.GetAsync<List<EnrolmentApiViewModel>>(
                    $"api/Enrolments/participant/{participantId.Value}");

                var displayList = new List<EnrolmentDisplayViewModel>();

                if (enrolments != null)
                {
                    foreach (var enrolment in enrolments)
                    {
                        var eventDetails = await _apiService.GetAsync<EventViewModel>(
                            $"api/Events/{enrolment.EventId}");

                        var category = await _apiService.GetAsync<CategoryViewModel>(
                            $"api/Categories/{enrolment.CategoryId}");

                        if (eventDetails != null && category != null)
                        {
                            displayList.Add(new EnrolmentDisplayViewModel
                            {
                                EnrolmentId = enrolment.EnrolmentId,
                                ParticipantId = enrolment.ParticipantId,
                                EventId = enrolment.EventId,
                                CategoryId = enrolment.CategoryId,
                                EnrolmentDate = enrolment.EnrolmentDate,
                                Status = enrolment.Status,

                                EventName = eventDetails.EventName ?? "",
                                EventDescription = eventDetails.Description ?? "",
                                EventDate = eventDetails.EventDate,
                                Location = eventDetails.Location ?? "",
                                EventType = eventDetails.EventType ?? "",
                                DistanceKm = eventDetails.DistanceKm,
                                EntryFee = eventDetails.EntryFee,

                                CategoryName = category.CategoryName,
                                CategoryDescription = category.Description
                            });
                        }
                    }
                }
                return View(displayList);
            }
            catch (HttpRequestException)
            {
                ViewBag.Error = "Unable to load your enrolments.";
                return View(new List<EnrolmentDisplayViewModel>());
            }
        }
        // Display the participant's profile.
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("Login", "Account");

            try
            {
                var profile = await _apiService.GetAsync<ProfileViewModel>(
                    $"api/Users/{userId.Value}");

                if (profile == null)
                    return NotFound();

                string[] extensions = { ".jpg", ".jpeg", ".png", ".gif" };

                foreach (string extension in extensions)
                {
                    string fileName = $"{userId.Value}{extension}";

                    string filePath = Path.Combine(
                        _webHostEnvironment.WebRootPath,
                        "images",
                        "profiles",
                        fileName);

                    if (System.IO.File.Exists(filePath))
                    {
                        profile.ProfilePicturePath =
                            $"/images/profiles/{fileName}";

                        break;
                    }
                }

                return View(profile);
            }
            catch (HttpRequestException)
            {
                ViewBag.Error = "Unable to load your profile.";
                return View(new ProfileViewModel());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(ProfileViewModel model)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
                return View(model);

            string? savedImagePath = null;

            // Check whether a new picture was uploaded.
            if (model.ProfilePicture != null &&
                model.ProfilePicture.Length > 0)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };

                string extension = Path.GetExtension(
                    model.ProfilePicture.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError(
                        "ProfilePicture",
                        "Only JPG, JPEG, PNG and GIF images are allowed.");

                    return View(model);
                }

                // Maximum upload size: 5 MB.
                if (model.ProfilePicture.Length > 5 * 1024 * 1024)
                {
                    ModelState.AddModelError(
                        "ProfilePicture",
                        "The image cannot be larger than 5 MB.");

                    return View(model);
                }

                string folderPath = Path.Combine(
                    _webHostEnvironment.WebRootPath,
                    "images",
                    "profiles");

                Directory.CreateDirectory(folderPath);

                // Remove any previous picture for this participant.
                foreach (string oldExtension in allowedExtensions)
                {
                    string oldFile = Path.Combine(
                        folderPath,
                        $"{userId.Value}{oldExtension}");

                    if (System.IO.File.Exists(oldFile))
                    {
                        System.IO.File.Delete(oldFile);
                    }
                }

                string fileName = $"{userId.Value}{extension}";
                string filePath = Path.Combine(folderPath, fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ProfilePicture.CopyToAsync(fileStream);
                }

                savedImagePath = $"/images/profiles/{fileName}";
            }

            try
            {
                await _apiService.PutAsync(
                    $"api/Users/{userId.Value}",
                    new
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        PhoneNumber = model.PhoneNumber,
                        DateOfBirth = model.DateOfBirth
                    });

                HttpContext.Session.SetString("FirstName", model.FirstName);

                TempData["Success"] =
                    "Your profile has been updated successfully!";

                return RedirectToAction("Profile");
            }
            catch (HttpRequestException)
            {
                ViewBag.Error =
                    "Unable to update your profile. Please try again.";

                model.ProfilePicturePath = savedImagePath;
                return View(model);
            }
        }

        public async Task<IActionResult> ResultsHistory()
        {
            int? participantId = HttpContext.Session.GetInt32("UserId");

            if (participantId == null)
                return RedirectToAction("Login", "Account");

            var resultsList = new List<ResultViewModel>();

            try
            {
                var enrolments = await _apiService.GetAsync<List<EnrolmentApiViewModel>>(
                    $"api/Enrolments/participant/{participantId.Value}");

                if (enrolments != null)
                {
                    foreach (var enrolment in enrolments)
                    {
                        var results = await _apiService.GetAsync<List<ResultViewModel>>(
                            $"api/Results/enrolment/{enrolment.EnrolmentId}");

                        if (results != null)
                        {
                            resultsList.AddRange(results);
                        }
                    }
                }

                return View(resultsList);
            }
            catch (HttpRequestException)
            {
                ViewBag.Error = "Unable to load your race results.";
                return View(resultsList);
            }
        }





        // Find an existing event banner.
        private string? GetEventBannerPath(int eventId)
        {
            string[] extensions = { ".jpg", ".jpeg", ".png", ".gif" };

            foreach (string extension in extensions)
            {
                string fileName = $"{eventId}{extension}";

                string filePath = Path.Combine(
                    _webHostEnvironment.WebRootPath,
                    "images",
                    "events",
                    fileName);

                if (System.IO.File.Exists(filePath))
                {
                    return $"/images/events/{fileName}";
                }
            }

            return null;
        }
    }
}

