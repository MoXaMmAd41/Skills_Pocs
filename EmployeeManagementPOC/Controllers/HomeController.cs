using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagementPOC.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
