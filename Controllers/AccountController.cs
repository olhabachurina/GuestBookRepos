using Azure.Core;
using GuestBookRepos.Interfaces;
using GuestBookRepos.Models;
using GuestBookRepos.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GuestBookRepos.Controllers
{
    public class AccountController : Controller
    {
        private readonly IRepository _repository;
        private readonly IPasswordHasher<User> _passwordHasher;

        public AccountController(IRepository repository, IPasswordHasher<User> passwordHasher)
        {
            _repository = repository;
            _passwordHasher = passwordHasher;
        }

        public IActionResult Login() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _repository.GetUserByNameAsync(model.Name);
            if (user != null && _passwordHasher.VerifyHashedPassword(user, user.Pwd, model.Password) == PasswordVerificationResult.Success)
            {
                HttpContext.Session.SetInt32("UserId", user.Id);
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Неверный логин или пароль.";
            return View(model);
        }

        public IActionResult Register() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var existingUser = await _repository.GetUserByNameAsync(model.Name);
            if (existingUser != null)
            {
                ModelState.AddModelError(string.Empty, "Пользователь с таким логином уже существует.");
                return View(model);
            }

            var user = new User { Name = model.Name };
            user.Pwd = _passwordHasher.HashPassword(user, model.Password);
            await _repository.AddUserAsync(user);
            return RedirectToAction("Login");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("UserId");
            return Redirect(Request.Headers["Referer"].ToString());
        }
    }
}