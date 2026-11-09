using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using RaceDay.Web.Services;
using RaceDay.Web.ViewModels;
using System.Collections.Generic;

namespace RaceDay.Web.Controllers
{
    public class OrganiserController : Controller
    {
        private readonly ApiService _apiService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public OrganiserController(
     ApiService apiService,
     IWebHostEnvironment webHostEnvironment)
        {
            _apiService = apiService;
            _webHostEnvironment = webHostEnvironment;
        }

        // Display the organiser dashboard.
        public async Task<IActionResult> Index()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            string? role = HttpContext.Session.GetString("Role");

            if (userId == null)
                return RedirectToAction("Login", "Account");

            if (role != "Organiser")
                return RedirectToAction("Index", "Home");

            try
            {
                var events = await _apiService.GetAsync<List<EventViewModel>>(
     "api/Events");

                // Show only events created by the logged-in organiser.
                var organiserEvents = (events ?? new List<EventViewModel>())
                    .Where(e => e.OrganiserId == userId.Value)
                    .ToList();
                // Load the banner path for each event.
                foreach (var eventItem in organiserEvents)
                {
                    eventItem.EventBannerPath = GetEventBannerPath(eventItem.EventId);
                }

                return View(organiserEvents);
            }
            catch (HttpRequestException)
            {
                ViewBag.Error = "Unable to load your events. Please try again.";
                return View(new List<EventViewModel>());
            }
        }

        // Display the Create Event form.
        [HttpGet]
        public IActionResult Create()
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
                return RedirectToAction("Login", "Account");

            if (HttpContext.Session.GetString("Role") != "Organiser")
                return RedirectToAction("Index", "Home");

            return View();
        }

        // Receive and save a new event.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateEventViewModel model)
        {
            int? organiserId = HttpContext.Session.GetInt32("UserId");

            if (organiserId == null)
                return RedirectToAction("Login", "Account");

            if (HttpContext.Session.GetString("Role") != "Organiser")
                return RedirectToAction("Index", "Home");
            // Validate the event banner.
            if (model.EventBanner != null)
            {
                string extension = Path.GetExtension(model.EventBanner.FileName)
                    .ToLowerInvariant();

                if (extension != ".jpg" &&
                    extension != ".jpeg" &&
                    extension != ".png" &&
                    extension != ".gif")
                {
                    ModelState.AddModelError(
                        nameof(model.EventBanner),
                        "Only JPG, JPEG, PNG and GIF images are allowed.");
                }

                if (model.EventBanner.Length > 5 * 1024 * 1024)
                {
                    ModelState.AddModelError(
                        nameof(model.EventBanner),
                        "The event banner must not exceed 5 MB.");
                }
            }
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var newEvent = new
                {
                    EventName = model.EventName,
                    EventDescription = model.EventDescription,
                    EventDate = model.EventDate,
                    Location = model.Location,
                    EventType = model.EventType,
                    DistanceKm = model.DistanceKm,
                    EntryFee = model.EntryFee,
                    RegistrationDeadline = model.RegistrationDeadline,
                    OrganiserId = organiserId.Value
                };

                // Create the event through the API.
                var createdEvent = await _apiService.PostAsync<object, JsonElement>(
                    "api/Events",
                    newEvent);

                // Get the newly created event ID.
                int createdEventId = 0;

                if (createdEvent.TryGetProperty("eventId", out var idValue) ||
                    createdEvent.TryGetProperty("EventId", out idValue))
                {
                    createdEventId = idValue.GetInt32();
                }
                // Save the event banner in wwwroot.
                if (model.EventBanner != null && createdEventId > 0)
                {
                    string extension = Path.GetExtension(model.EventBanner.FileName)
                        .ToLowerInvariant();

                    string folderPath = Path.Combine(
                        _webHostEnvironment.WebRootPath,
                        "images",
                        "events");

                    Directory.CreateDirectory(folderPath);

                    string fileName = $"{createdEventId}{extension}";
                    string filePath = Path.Combine(folderPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.EventBanner.CopyToAsync(stream);
                    }
                }

                TempData["Success"] = "Event created successfully!";

                return RedirectToAction("Index");
            }
            catch (HttpRequestException)
            {
                ViewBag.Error = "Unable to create the event. Please check your details.";
                return View(model);
            }
        }

        // Display the Edit Event form.
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            int? organiserId = HttpContext.Session.GetInt32("UserId");

            if (organiserId == null)
                return RedirectToAction("Login", "Account");

            if (HttpContext.Session.GetString("Role") != "Organiser")
                return RedirectToAction("Index", "Home");

            try
            {
                var eventItem = await _apiService.GetAsync<EventViewModel>(
                    $"api/Events/{id}");

                if (eventItem == null)
                    return NotFound();

                if (eventItem.OrganiserId != organiserId.Value)
                    return Forbid();

                var model = new EditEventViewModel
                {
                    EventId = eventItem.EventId,
                    EventName = eventItem.EventName ?? "",
                    EventDescription = eventItem.Description ?? "",
                    EventDate = eventItem.EventDate,
                    Location = eventItem.Location ?? "",
                    EventType = eventItem.EventType ?? "",
                    DistanceKm = eventItem.DistanceKm,
                    EntryFee = eventItem.EntryFee,

                    // Load the existing event banner.
                    EventBannerPath = GetEventBannerPath(eventItem.EventId)
                };

                return View(model);
            }
            catch (HttpRequestException)
            {
                TempData["Error"] = "Unable to load the event for editing.";
                return RedirectToAction("Index");
            }
        }

        // Save changes to an existing event.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditEventViewModel model)
        {
            int? organiserId = HttpContext.Session.GetInt32("UserId");

            if (organiserId == null)
                return RedirectToAction("Login", "Account");

            if (HttpContext.Session.GetString("Role") != "Organiser")
                return RedirectToAction("Index", "Home");

            // Validate a replacement event banner.
            if (model.EventBanner != null)
            {
                string extension = Path.GetExtension(model.EventBanner.FileName)
                    .ToLowerInvariant();

                if (extension != ".jpg" &&
                    extension != ".jpeg" &&
                    extension != ".png" &&
                    extension != ".gif")
                {
                    ModelState.AddModelError(
                        nameof(model.EventBanner),
                        "Only JPG, JPEG, PNG and GIF images are allowed.");
                }

                if (model.EventBanner.Length > 5 * 1024 * 1024)
                {
                    ModelState.AddModelError(
                        nameof(model.EventBanner),
                        "The event banner must not exceed 5 MB.");
                }
            }

            if (!ModelState.IsValid)
            {
                model.EventBannerPath = GetEventBannerPath(model.EventId);
                return View(model);
            }

            try
            {
                var existingEvent = await _apiService.GetAsync<EventViewModel>(
                    $"api/Events/{model.EventId}");

                if (existingEvent == null)
                    return NotFound();

                if (existingEvent.OrganiserId != organiserId.Value)
                    return Forbid();

                await _apiService.PutAsync(
                    $"api/Events/{model.EventId}",
                    new
                    {
                        EventId = model.EventId,
                        EventName = model.EventName,
                        EventDescription = model.EventDescription,
                        EventDate = model.EventDate,
                        Location = model.Location,
                        EventType = model.EventType,
                        DistanceKm = model.DistanceKm,
                        EntryFee = model.EntryFee,
                        RegistrationDeadline = model.RegistrationDeadline,
                        OrganiserId = organiserId.Value
                    });// Save a replacement event banner if one was uploaded.
                if (model.EventBanner != null)
                {
                    string folderPath = Path.Combine(
                        _webHostEnvironment.WebRootPath,
                        "images",
                        "events");

                    Directory.CreateDirectory(folderPath);

                    // Remove the previous banner.
                    string[] extensions = { ".jpg", ".jpeg", ".png", ".gif" };

                    foreach (string extension in extensions)
                    {
                        string oldFilePath = Path.Combine(
                            folderPath,
                            $"{model.EventId}{extension}");

                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }

                    // Save the new banner.
                    string newExtension = Path.GetExtension(model.EventBanner.FileName)
                        .ToLowerInvariant();

                    string fileName = $"{model.EventId}{newExtension}";
                    string filePath = Path.Combine(folderPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.EventBanner.CopyToAsync(stream);
                    }
                }

                TempData["Success"] = "Event updated successfully!";
                return RedirectToAction("Index");
            }
            catch (HttpRequestException)
            {
                ViewBag.Error = "Unable to update the event. Please try again.";
                return View(model);
            }
        }


        // GET: Organiser/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var role = HttpContext.Session.GetString("Role");

            if (userId == null || role != "Organiser")
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                // Retrieve the event from the API.
                var eventItem = await _apiService.GetAsync<EventViewModel>(
                    $"api/Events/{id}");

                if (eventItem == null)
                {
                    return NotFound();
                }

                // Make sure the organiser owns this event.
                if (eventItem.OrganiserId != userId.Value)
                {
                    return Forbid();
                }

                // Delete the event through the API.
                await _apiService.DeleteAsync($"api/Events/{id}");

                TempData["Success"] = "Event deleted successfully.";

                return RedirectToAction(nameof(Index));
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = $"Could not delete event: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }
        // GET: Organiser/Categories
        public async Task<IActionResult> Categories()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var role = HttpContext.Session.GetString("Role");

            if (userId == null || role != "Organiser")
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                var events = await _apiService.GetAsync<List<EventViewModel>>(
                    "api/Events");

                var myEvents = events?
                    .Where(e => e.OrganiserId == userId.Value)
                    .ToList() ?? new List<EventViewModel>();

                ViewBag.Events = myEvents;

                var categories = await _apiService.GetAsync<List<CategoryViewModel>>(
                    "api/Categories");

                var myEventIds = myEvents.Select(e => e.EventId).ToHashSet();

                var myCategories = categories?
                    .Where(c => myEventIds.Contains(c.EventId))
                    .ToList() ?? new List<CategoryViewModel>();

                return View(myCategories);
            }
            catch (HttpRequestException ex)
            {
                ViewBag.Error = ex.Message;
                return View(new List<CategoryViewModel>());
            }
        }

        // POST: Organiser/CreateCategory
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCategory(
            CreateCategoryViewModel model)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var role = HttpContext.Session.GetString("Role");

            if (userId == null || role != "Organiser")
            {
                return RedirectToAction("Login", "Account");
            }

            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Please enter valid category details.";
                return RedirectToAction(nameof(Categories));
            }

            try
            {
                var eventItem = await _apiService.GetAsync<EventViewModel>(
                    $"api/Events/{model.EventId}");

                if (eventItem == null || eventItem.OrganiserId != userId.Value)
                {
                    return Forbid();
                }

                var categoryData = new
                {
                    model.EventId,
                    model.CategoryName,
                    model.MinimumAge,
                    model.MaximumAge,
                    model.Description
                };

                await _apiService.PostAsync<object, CategoryViewModel>(
                    "api/Categories", categoryData);

                TempData["Success"] = "Category created successfully.";

                return RedirectToAction(nameof(Categories));
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Categories));
            }
        }
        // GET: Organiser/EditCategory/1
        [HttpGet]
        public async Task<IActionResult> EditCategory(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var role = HttpContext.Session.GetString("Role");

            if (userId == null || role != "Organiser")
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                var category = await _apiService.GetAsync<CategoryViewModel>(
                    $"api/Categories/{id}");

                if (category == null)
                {
                    return NotFound();
                }

                var eventItem = await _apiService.GetAsync<EventViewModel>(
                    $"api/Events/{category.EventId}");

                if (eventItem == null || eventItem.OrganiserId != userId.Value)
                {
                    return Forbid();
                }

                var model = new CreateCategoryViewModel
                {
                    EventId = category.EventId,
                    CategoryName = category.CategoryName,
                    MinimumAge = category.MinimumAge,
                    MaximumAge = category.MaximumAge,
                    Description = category.Description
                };

                ViewBag.CategoryId = category.CategoryId;

                return View(model);
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Categories));
            }
        }
        // POST: Organiser/EditCategory
        [HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> EditCategory(
    int categoryId, CreateCategoryViewModel model)
{
    var userId = HttpContext.Session.GetInt32("UserId");
    var role = HttpContext.Session.GetString("Role");

    if (userId == null || role != "Organiser")
    {
        return RedirectToAction("Login", "Account");
    }

    if (!ModelState.IsValid)
    {
        TempData["Error"] = "Please enter valid category details.";
        return RedirectToAction(nameof(Categories));
    }

    try
    {
        var existingCategory =
            await _apiService.GetAsync<CategoryViewModel>(
                $"api/Categories/{categoryId}");

        if (existingCategory == null)
        {
            return NotFound();
        }

        var eventItem = await _apiService.GetAsync<EventViewModel>(
            $"api/Events/{existingCategory.EventId}");

        if (eventItem == null || eventItem.OrganiserId != userId.Value)
        {
            return Forbid();
        }

        var updatedCategory = new
        {
            CategoryId = categoryId,
            model.EventId,
            model.CategoryName,
            model.MinimumAge,
            model.MaximumAge,
            model.Description
        };

        await _apiService.PutAsync(
            $"api/Categories/{categoryId}", updatedCategory);

        TempData["Success"] = "Category updated successfully.";

        return RedirectToAction(nameof(Categories));
    }
    catch (HttpRequestException ex)
    {
        TempData["Error"] = ex.Message;
        return RedirectToAction(nameof(Categories));
    }
}
        // POST: Organiser/DeleteCategory
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var role = HttpContext.Session.GetString("Role");

            if (userId == null || role != "Organiser")
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                var category = await _apiService.GetAsync<CategoryViewModel>(
                    $"api/Categories/{id}");

                if (category == null)
                {
                    return NotFound();
                }

                var eventItem = await _apiService.GetAsync<EventViewModel>(
                    $"api/Events/{category.EventId}");

                if (eventItem == null || eventItem.OrganiserId != userId.Value)
                {
                    return Forbid();
                }

                await _apiService.DeleteAsync($"api/Categories/{id}");

                TempData["Success"] = "Category deleted successfully.";

                return RedirectToAction(nameof(Categories));
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Categories));
            }
        }
        // GET: Organiser/Enrolments
        public async Task<IActionResult> Enrolments()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var role = HttpContext.Session.GetString("Role");

            if (userId == null || role != "Organiser")
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                // Get events belonging to this organiser.
                var events = await _apiService.GetAsync<List<EventViewModel>>(
                    "api/Events");

                var myEvents = events?
                    .Where(e => e.OrganiserId == userId.Value)
                    .ToList() ?? new List<EventViewModel>();

                var myEventIds = myEvents.Select(e => e.EventId).ToHashSet();

                // Get all enrolments.
                var allEnrolments =
                    await _apiService.GetAsync<List<EnrolmentApiViewModel>>(
                        "api/Enrolments");

                var myEnrolments = (allEnrolments ?? new List<EnrolmentApiViewModel>())
                    .Where(e => myEventIds.Contains(e.EventId))
                    .ToList();

                var result = new List<OrganiserEnrolmentViewModel>();

                foreach (var enrolment in myEnrolments)
                {
                    var eventItem = myEvents.FirstOrDefault(
                        e => e.EventId == enrolment.EventId);

                    var category = await _apiService.GetAsync<CategoryViewModel>(
                        $"api/Categories/{enrolment.CategoryId}");

                    var participant = await _apiService.GetAsync<ProfileViewModel>(
                        $"api/Users/{enrolment.ParticipantId}");

                    result.Add(new OrganiserEnrolmentViewModel
                    {
                        EnrolmentId = enrolment.EnrolmentId,
                        ParticipantId = enrolment.ParticipantId,
                        EventId = enrolment.EventId,
                        CategoryId = enrolment.CategoryId,
                        EnrolmentDate = enrolment.EnrolmentDate,
                        Status = enrolment.Status,
                        EventName = eventItem?.EventName ?? "Unknown Event",
                        CategoryName = category?.CategoryName ?? "Unknown Category",
                        ParticipantName = participant == null
                            ? "Unknown Participant"
                            : $"{participant.FirstName} {participant.LastName}",
                        ParticipantEmail = participant?.Email ?? ""
                    });
                }

                return View(result);
            }
            catch (HttpRequestException ex)
            {
                ViewBag.Error = ex.Message;
                return View(new List<OrganiserEnrolmentViewModel>());
            }
        }

        // POST: Organiser/UpdateEnrolmentStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateEnrolmentStatus(
            int enrolmentId, string status)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var role = HttpContext.Session.GetString("Role");

            if (userId == null || role != "Organiser")
            {
                return RedirectToAction("Login", "Account");
            }

            if (status != "Active" &&
                status != "Completed" &&
                status != "Cancelled")
            {
                TempData["Error"] = "Invalid enrolment status.";
                return RedirectToAction("Enrolments");
            }

            try
            {
                // Confirm that the enrolment belongs to one of this organiser's events.
                var enrolment = await _apiService.GetAsync<EnrolmentApiViewModel>(
                    $"api/Enrolments/{enrolmentId}");

                if (enrolment == null)
                {
                    return NotFound();
                }

                var events = await _apiService.GetAsync<List<EventViewModel>>(
                    "api/Events");

                var ownsEvent = events?.Any(e =>
                    e.EventId == enrolment.EventId &&
                    e.OrganiserId == userId.Value) ?? false;

                if (!ownsEvent)
                {
                    return Forbid();
                }

                await _apiService.PatchAsync(
                    $"api/Enrolments/{enrolmentId}/status",
                    status);

                TempData["Success"] = "Enrolment status updated successfully.";
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction("Enrolments");
        }
        // GET: Organiser/Results
        public async Task<IActionResult> Results()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var role = HttpContext.Session.GetString("Role");

            if (userId == null || role != "Organiser")
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                // Get events belonging to this organiser.
                var events = await _apiService.GetAsync<List<EventViewModel>>(
                    "api/Events");

                var myEvents = events?
                    .Where(e => e.OrganiserId == userId.Value)
                    .ToList() ?? new List<EventViewModel>();

                var myEventIds = myEvents.Select(e => e.EventId).ToHashSet();

                // Get enrolments for the organiser's events.
                var allEnrolments =
                    await _apiService.GetAsync<List<EnrolmentApiViewModel>>(
                        "api/Enrolments");

                var myEnrolments = (allEnrolments ??
                    new List<EnrolmentApiViewModel>())
                    .Where(e => myEventIds.Contains(e.EventId))
                    .ToList();

                var resultList = new List<OrganiserResultViewModel>();

                foreach (var enrolment in myEnrolments)
                {
                    var results = await _apiService.GetAsync<List<ResultViewModel>>(
                        $"api/Results/enrolment/{enrolment.EnrolmentId}");

                    var eventItem = myEvents.FirstOrDefault(
                        e => e.EventId == enrolment.EventId);

                    var participant = await _apiService.GetAsync<ProfileViewModel>(
                        $"api/Users/{enrolment.ParticipantId}");

                    foreach (var result in results ?? new List<ResultViewModel>())
                    {
                        resultList.Add(new OrganiserResultViewModel
                        {
                            ResultId = result.ResultId,
                            EnrolmentId = enrolment.EnrolmentId,
                            ParticipantId = enrolment.ParticipantId,
                            ParticipantName = participant == null
                                ? "Unknown Participant"
                                : $"{participant.FirstName} {participant.LastName}",
                            EventName = eventItem?.EventName ?? "Unknown Event",
                            FinishTime = result.FinishTime,
                            Position = result.Position,
                            Status = result.Status
                        });
                    }
                }

                return View(resultList);
            }
            catch (HttpRequestException ex)
            {
                ViewBag.Error = ex.Message;
                return View(new List<OrganiserResultViewModel>());
            }
        }
        // GET: Organiser/CreateResult
        [HttpGet]
        public async Task<IActionResult> CreateResult()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var role = HttpContext.Session.GetString("Role");

            if (userId == null || role != "Organiser")
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                var events = await _apiService.GetAsync<List<EventViewModel>>(
                    "api/Events");

                var myEvents = events?
                    .Where(e => e.OrganiserId == userId.Value)
                    .ToList() ?? new List<EventViewModel>();

                var myEventIds = myEvents.Select(e => e.EventId).ToHashSet();

                var allEnrolments =
                    await _apiService.GetAsync<List<EnrolmentApiViewModel>>(
                        "api/Enrolments");

                var myEnrolments = (allEnrolments ??
                    new List<EnrolmentApiViewModel>())
                    .Where(e => myEventIds.Contains(e.EventId))
                    .ToList();

                ViewBag.Enrolments = myEnrolments;

                return View(new CreateResultViewModel());
            }
            catch (HttpRequestException ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.Enrolments = new List<EnrolmentApiViewModel>();

                return View(new CreateResultViewModel());
            }
        }
        // POST: Organiser/CreateResult
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateResult(CreateResultViewModel model)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var role = HttpContext.Session.GetString("Role");

            if (userId == null || role != "Organiser")
            {
                return RedirectToAction("Login", "Account");
            }

            if (!ModelState.IsValid)
            {
                return await CreateResult();
            }

            try
            {
                // Find the selected enrolment.
                var enrolment = await _apiService.GetAsync<EnrolmentApiViewModel>(
                    $"api/Enrolments/{model.EnrolmentId}");

                if (enrolment == null)
                {
                    ModelState.AddModelError("", "Enrolment not found.");
                    return await CreateResult();
                }

                // Confirm that the enrolment belongs to this organiser's event.
                var events = await _apiService.GetAsync<List<EventViewModel>>(
                    "api/Events");

                var ownsEvent = events?.Any(e =>
                    e.EventId == enrolment.EventId &&
                    e.OrganiserId == userId.Value) ?? false;

                if (!ownsEvent)
                {
                    return Forbid();
                }

                // Send the result to the API.
                await _apiService.PostAsync<object, ResultViewModel>(
                    "api/Results",
                    new
                    {
                        EnrolmentId = model.EnrolmentId,
                        FinishTime = model.FinishTime,
                        Position = model.Position,
                        Status = model.Status
                    });

                TempData["Success"] = "Participant result recorded successfully.";

                return RedirectToAction("Results");
            }
            catch (HttpRequestException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return await CreateResult();
            }
        }

        // GET: Organiser/EditResult/1
        [HttpGet]
        public async Task<IActionResult> EditResult(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var role = HttpContext.Session.GetString("Role");

            if (userId == null || role != "Organiser")
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                var result = await _apiService.GetAsync<ResultViewModel>(
                    $"api/Results/{id}");

                if (result == null)
                {
                    return NotFound();
                }

                var enrolment =
                    await _apiService.GetAsync<EnrolmentApiViewModel>(
                        $"api/Enrolments/{result.EnrolmentId}");

                if (enrolment == null)
                {
                    return NotFound();
                }

                var events = await _apiService.GetAsync<List<EventViewModel>>(
                    "api/Events");

                var ownsEvent = events?.Any(e =>
                    e.EventId == enrolment.EventId &&
                    e.OrganiserId == userId.Value) ?? false;

                if (!ownsEvent)
                {
                    return Forbid();
                }

                var model = new EditResultViewModel
                {
                    ResultId = result.ResultId,
                    EnrolmentId = result.EnrolmentId,
                    FinishTime = result.FinishTime,
                    Position = result.Position,
                    Status = result.Status
                };

                return View(model);
            }
            catch (HttpRequestException ex)
            {
                ViewBag.Error = ex.Message;
                return RedirectToAction("Results");
            }
        }
        // POST: Organiser/EditResult
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditResult(EditResultViewModel model)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var role = HttpContext.Session.GetString("Role");

            if (userId == null || role != "Organiser")
            {
                return RedirectToAction("Login", "Account");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var enrolment =
                    await _apiService.GetAsync<EnrolmentApiViewModel>(
                        $"api/Enrolments/{model.EnrolmentId}");

                if (enrolment == null)
                {
                    return NotFound();
                }

                var events = await _apiService.GetAsync<List<EventViewModel>>(
                    "api/Events");

                var ownsEvent = events?.Any(e =>
                    e.EventId == enrolment.EventId &&
                    e.OrganiserId == userId.Value) ?? false;

                if (!ownsEvent)
                {
                    return Forbid();
                }

                await _apiService.PutAsync(
                    $"api/Results/{model.ResultId}",
                    new
                    {
                        ResultId = model.ResultId,
                        EnrolmentId = model.EnrolmentId,
                        FinishTime = model.FinishTime,
                        Position = model.Position,
                        Status = model.Status
                    });

                TempData["Success"] = "Result updated successfully.";

                return RedirectToAction("Results");
            }
            catch (HttpRequestException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }
        // POST: Organiser/DeleteResult
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteResult(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var role = HttpContext.Session.GetString("Role");

            if (userId == null || role != "Organiser")
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                var result = await _apiService.GetAsync<ResultViewModel>(
                    $"api/Results/{id}");

                if (result == null)
                {
                    return NotFound();
                }

                var enrolment =
                    await _apiService.GetAsync<EnrolmentApiViewModel>(
                        $"api/Enrolments/{result.EnrolmentId}");

                if (enrolment == null)
                {
                    return NotFound();
                }

                var events = await _apiService.GetAsync<List<EventViewModel>>(
                    "api/Events");

                var ownsEvent = events?.Any(e =>
                    e.EventId == enrolment.EventId &&
                    e.OrganiserId == userId.Value) ?? false;

                if (!ownsEvent)
                {
                    return Forbid();
                }

                await _apiService.DeleteAsync($"api/Results/{id}");

                TempData["Success"] = "Result deleted successfully.";
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction("Results");
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
