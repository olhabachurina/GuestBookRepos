using GuestBookRepos.Service;
using Microsoft.AspNetCore.Mvc;

namespace GuestBookRepos.Controllers
{
    public class AdminController : Controller
    {
        private readonly PasswordHasherScript _passwordHasherScript;

        public AdminController(PasswordHasherScript passwordHasherScript)
        {
            _passwordHasherScript = passwordHasherScript;
        }

        public async Task<IActionResult> RehashPasswords()
        {
            await _passwordHasherScript.RehashPasswords();
            return Content("Passwords rehashed successfully.");
        }
    }
}
