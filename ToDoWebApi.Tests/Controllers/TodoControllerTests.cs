using Xunit;
using Moq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ToDoWebApi.Models;
using ToDoWebApi.Controllers;
using ToDoWebApi.Services;
using Microsoft.EntityFrameworkCore;
using ToDoWebApi.Data;

namespace ToDoWebApi.Tests.Controllers
{
    public class TodoControllerTests
    {
        private TodoContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<TodoContext>()
                .UseInMemoryDatabase(databaseName: "TodoTestDb")
                .Options;
            return new TodoContext(options);
        }
        [Fact]
        public async Task AddTodoItem_ShouldReturnCreatedResult_WhenTodoItemIsValid()
        {
            // Arrange
            var dbContext = GetInMemoryDbContext();
            var controller = new TodoController(dbContext, null); // Null for WeatherService in this case

            var newTodo = new TodoItem
            {
                Title = "Test To-Do Item",
                Priority = 2
            };

            // Act
            var result = await controller.AddTodoItem(newTodo);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var todoItem = Assert.IsType<TodoItem>(createdResult.Value);
            Assert.Equal("Test To-Do Item", todoItem.Title);
        }

        [Fact]
        public async Task GetTodoItem_ShouldReturnTodoItem_WhenIdIsValid()
        {
            // Arrange
            var dbContext = GetInMemoryDbContext();
            var todoItem = new TodoItem { Title = "Existing Item", Priority = 3 };
            dbContext.TodoItems.Add(todoItem);
            await dbContext.SaveChangesAsync();

            var controller = new TodoController(dbContext, null);

            // Act
            var result = await controller.GetTodoItem(todoItem.Id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedTodo = Assert.IsType<TodoItem>(okResult.Value);
            Assert.Equal("Existing Item", returnedTodo.Title);
        }

        [Fact]
        public async Task GetTodoItem_ShouldReturnNotFound_WhenIdIsInvalid()
        {
            // Arrange
            var dbContext = GetInMemoryDbContext();
            var controller = new TodoController(dbContext, null);

            // Act
            var result = await controller.GetTodoItem(999); // Assuming 999 does not exist

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
