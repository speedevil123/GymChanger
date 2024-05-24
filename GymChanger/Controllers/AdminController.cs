using Microsoft.AspNetCore.Mvc;

namespace GymChanger.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
