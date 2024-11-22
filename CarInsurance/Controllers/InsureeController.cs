using Microsoft.AspNetCore.Mvc;

namespace CarInsurance.Controllers
{
    public class Insuree : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
