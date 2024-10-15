using GuestBookRepos.Controllers;
using GuestBookRepos.Models;
using GuestBookRepos.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuestBookRepos.Tests.Controllers
{
    public class AdminControllerTests
    {
        private readonly AdminController _controller;

        public AdminControllerTests()
        {
            // Используем InMemory базу данных для создания ApplicationDbContext
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            var inMemoryDbContext = new ApplicationDbContext(options);

            // Мокаем IPasswordHasher<User>
            var mockPasswordHasher = new Mock<IPasswordHasher<User>>();

            // Создаем реальный экземпляр PasswordHasherScript с InMemory базой данных и замокированным IPasswordHasher
            var passwordHasherScript = new PasswordHasherScript(inMemoryDbContext, mockPasswordHasher.Object);

            // Создаем контроллер с реальной зависимостью PasswordHasherScript
            _controller = new AdminController(passwordHasherScript);
        }

        [Fact]
        public async Task RehashPasswords_ReturnsContentResult_WithSuccessMessage()
        {
            // Act: вызываем метод контроллера
            var result = await _controller.RehashPasswords();

            // Assert: проверяем, что результат - это ContentResult
            var contentResult = Assert.IsType<ContentResult>(result);
            Assert.Equal("Passwords rehashed successfully.", contentResult.Content);
        }
    }
}