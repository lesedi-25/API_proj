using Microsoft.AspNetCore.Mvc;

namespace API_proj.Controllers
{
    public class CategoriesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
