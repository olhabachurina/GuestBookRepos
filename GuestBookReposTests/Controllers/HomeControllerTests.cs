using GuestBookRepos.Controllers;
using GuestBookRepos.Interfaces;
using GuestBookRepos.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuestBookRepos.Tests.Controllers
{
    public class HomeControllerTests
    {
        private readonly Mock<IRepository> _mockRepository;
        private readonly HomeController _controller;

        public HomeControllerTests()
        {
            _mockRepository = new Mock<IRepository>();

            // Создаем контроллер с замокированным репозиторием
            _controller = new HomeController(_mockRepository.Object);
        }

        [Fact]
        public async Task Index_ReturnsViewResult_WithMessages()
        {
            // Arrange
            var messages = new List<Message>
            {
                new Message { Content = "Test message", User = new User { Name = "User1" } }
            };

            // Мокаем вызов метода GetAllMessagesAsync, чтобы он вернул тестовые данные
            _mockRepository.Setup(repo => repo.GetAllMessagesAsync()).ReturnsAsync(messages);

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result); // Проверяем, что результат - это ViewResult
            var model = Assert.IsAssignableFrom<IEnumerable<Message>>(viewResult.Model); // Проверяем тип модели
            Assert.Single(model); // Проверяем, что в модели одно сообщение
        }

        //Успешное добавление сообщения:
        [Fact]
        public async Task AddMessage_ReturnsContentResult_WithMessagesHtml_WhenMessageIsAddedSuccessfully()
        {
            // Arrange
            var messageModel = new MessageViewModel { Content = "Test Content", Email = "test@example.com", WWW = "test.com" };
            var userId = 1;
            var user = new User { Id = userId, Name = "TestUser" };

            var messages = new List<Message>
    {
        new Message { Content = "Test Content", User = user }
    };

            // Мокаем сессию
            var mockSession = new Mock<ISession>();
            byte[] userIdBytes = BitConverter.GetBytes(userId);
            mockSession.Setup(s => s.TryGetValue("UserId", out userIdBytes)).Returns(true);

            // Мокаем HttpContext для сессии
            var httpContext = new DefaultHttpContext { Session = mockSession.Object };
            _controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

            // Мокаем вызов метода репозитория
            _mockRepository.Setup(repo => repo.GetUserByIdAsync(userId)).ReturnsAsync(user);
            _mockRepository.Setup(repo => repo.AddMessageAsync(It.IsAny<Message>())).Returns(Task.CompletedTask);
            _mockRepository.Setup(repo => repo.GetAllMessagesAsync()).ReturnsAsync(messages);

            // Act
            var result = await _controller.AddMessage(messageModel);

            // Assert
            var contentResult = Assert.IsType<ContentResult>(result);

            // Выводим содержимое результата для отладки
            Console.WriteLine(contentResult.Content);

            Assert.Contains("Test Content", contentResult.Content); // Проверяем, что HTML содержит текст сообщения
            Assert.Equal("text/html", contentResult.ContentType);
        }
        
    }
}
