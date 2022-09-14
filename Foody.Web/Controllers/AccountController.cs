using Microsoft.AspNetCore.Mvc;

namespace Foody.Web.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
