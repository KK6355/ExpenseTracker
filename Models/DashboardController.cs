using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Models
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
