using GuestBookRepos.Interfaces;
using GuestBookRepos.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace GuestBookRepos.Controllers
{
    public class HomeController : Controller
    {
        private readonly IRepository _repository;

        public HomeController(IRepository repository)
        {
            _repository = repository;
        }

        public async Task<IActionResult> Index()
        {
            var messages = await _repository.GetAllMessagesAsync();
            return View(messages);
        }

        [HttpPost]
        public async Task<IActionResult> AddMessage(MessageViewModel model)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (ModelState.IsValid)
            {
                var user = await _repository.GetUserByIdAsync(userId.Value);
                var newMessage = new Message
                {
                    Id_User = userId.Value,
                    Content = model.Content,
                    Email = model.Email,
                    WWW = model.WWW,
                    MessageDate = DateTime.Now,
                    User = user
                };

                await _repository.AddMessageAsync(newMessage);
                return RedirectToAction("Index");
            }

            var messages = await _repository.GetAllMessagesAsync();
            return View("Index", messages);
        }

        public async Task<IActionResult> MyReviews()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var messages = await _repository.GetMessagesByUserIdAsync(userId.Value);
            return View(messages);
        }
    }
}