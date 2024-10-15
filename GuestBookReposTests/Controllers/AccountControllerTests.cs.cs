using GuestBookRepos.Controllers;
using GuestBookRepos.Interfaces;
using GuestBookRepos.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuestBookReposTests.Controllers
{
    public class AccountControllerTests
    {
        private readonly Mock<IRepository> _mockRepository;
        private readonly Mock<IPasswordHasher<User>> _mockPasswordHasher;
        private readonly AccountController _controller;
        private readonly Mock<ISession> _mockSession;

        public AccountControllerTests()
        {
            _mockRepository = new Mock<IRepository>(); // Мокируем зависимость от IRepository
            _mockPasswordHasher = new Mock<IPasswordHasher<User>>(); // Мокируем IPasswordHasher
            _mockSession = new Mock<ISession>(); // Мокируем сессию

            // Создаем экземпляр контроллера с моками
            _controller = new AccountController(_mockRepository.Object, _mockPasswordHasher.Object);

            // Мокаем HttpContext для тестирования сессий
            var httpContext = new DefaultHttpContext();
            httpContext.Session = _mockSession.Object; // Устанавливаем замокированную сессию
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
        }

        [Fact]
        public void Login_Get_ReturnsViewResult()
        {
            // Act: Вызываем метод контроллера
            var result = _controller.Login();

            // Assert: Проверяем, что результат - это ViewResult
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Login_Post_RedirectsToHome_WhenLoginIsSuccessful()
        {
            // Arrange
            var loginModel = new LoginViewModel { Name = "TestUser", Password = "TestPassword" };
            var user = new User { Id = 1, Name = "TestUser", Pwd = "HashedPassword" };

            _mockRepository.Setup(repo => repo.GetUserByNameAsync(loginModel.Name)).ReturnsAsync(user);
            _mockPasswordHasher.Setup(pwdHasher => pwdHasher.VerifyHashedPassword(user, user.Pwd, loginModel.Password))
                               .Returns(PasswordVerificationResult.Success);

            // Преобразуем значение UserId в массив байтов
            var userIdBytes = BitConverter.GetBytes(user.Id);

            // Мокаем вызов Set для сессии
            _mockSession.Setup(s => s.Set("UserId", userIdBytes));

            // Act
            var result = await _controller.Login(loginModel);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Equal("Home", redirectResult.ControllerName);
        }

        [Fact]
        public async Task Login_Post_ReturnsViewWithError_WhenPasswordIsIncorrect()
        {
            // Arrange: создаем модель и данные пользователя
            var loginModel = new LoginViewModel { Name = "TestUser", Password = "WrongPassword" };
            var user = new User { Name = "TestUser", Pwd = "HashedPassword" };

            _mockRepository.Setup(repo => repo.GetUserByNameAsync(loginModel.Name)).ReturnsAsync(user);
            _mockPasswordHasher.Setup(pwdHasher => pwdHasher.VerifyHashedPassword(user, user.Pwd, loginModel.Password))
                               .Returns(PasswordVerificationResult.Failed);

            // Act: вызываем метод контроллера
            var result = await _controller.Login(loginModel);

            // Assert: проверяем, что результат - это ViewResult с ошибкой
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(loginModel, viewResult.Model);
            Assert.Equal("Неверный логин или пароль.", _controller.ViewBag.Error);
        }

        [Fact]
        public async Task Login_Post_ReturnsView_WhenModelStateIsInvalid()
        {
            // Arrange: добавляем ошибку в ModelState
            var loginModel = new LoginViewModel();
            _controller.ModelState.AddModelError("Name", "Required");

            // Act: вызываем метод контроллера
            var result = await _controller.Login(loginModel);

            // Assert: проверяем, что результат - это ViewResult
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(loginModel, viewResult.Model);
        }

        [Fact]
        public async Task Register_Post_RedirectsToLogin_WhenUserIsSuccessfullyRegistered()
        {
            // Arrange
            var registerModel = new RegisterViewModel { Name = "NewUser", Password = "Password123" };

            _mockRepository.Setup(repo => repo.GetUserByNameAsync(registerModel.Name)).ReturnsAsync((User)null);
            _mockPasswordHasher.Setup(pwdHasher => pwdHasher.HashPassword(It.IsAny<User>(), registerModel.Password))
                               .Returns("HashedPassword");

            // Act
            var result = await _controller.Register(registerModel);

            // Assert: проверяем, что результат - это RedirectToActionResult
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Login", redirectResult.ActionName);
        }

        [Fact]
        public async Task Register_Post_ReturnsViewWithError_WhenUserAlreadyExists()
        {
            // Arrange
            var registerModel = new RegisterViewModel { Name = "ExistingUser", Password = "Password123" };
            var existingUser = new User { Name = "ExistingUser" };

            _mockRepository.Setup(repo => repo.GetUserByNameAsync(registerModel.Name)).ReturnsAsync(existingUser);

            // Act
            var result = await _controller.Register(registerModel);

            // Assert: проверяем, что результат - это ViewResult с ошибкой
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(registerModel, viewResult.Model);
            Assert.True(_controller.ModelState.ContainsKey(string.Empty));
        }
    }
}