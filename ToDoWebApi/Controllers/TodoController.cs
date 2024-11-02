using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using System.Text.Json;
using ToDoWebApi.Data;
using ToDoWebApi.Models;
using ToDoWebApi.Services;

namespace ToDoWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoController : Controller
    {
        private readonly TodoContext _context;
        private readonly WeatherService _weatherService;

        public TodoController(TodoContext context, WeatherService weatherService)
        {
            _context = context;
            _weatherService = weatherService;
        }

        // Fetch and store the To-Do list from dummyjson.com API
        [HttpPost("fetch-and-store-todos")]
        public async Task<IActionResult> FetchAndStoreTodos()
        {
            var httpClient = new HttpClient();
            var response = await httpClient.GetStringAsync("https://dummyjson.com/todos");

            // Deserialize JSON response to .NET object
            var todosResponse = JsonSerializer.Deserialize<TodoApiResponse>(response);
            if (todosResponse?.Todos == null)
                return BadRequest("Failed to fetch to-do items");

            // Map each To-Do item to internal model and save to database
            var todos = todosResponse.Todos.Select(todo => new TodoItem
            {
                Title = todo.Title,
                Priority = 3
            }).ToList();

            _context.TodoItems.AddRange(todos);
            await _context.SaveChangesAsync();

            return Ok($"{todos.Count} To-Do items fetched and stored successfully");
        }

        // CRUD operations and weather functionality
        [HttpPost]
        public async Task<IActionResult> AddTodoItem(TodoItem todoItem)
        {
            _context.TodoItems.Add(todoItem);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetTodoItem), new { id = todoItem.Id }, todoItem);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTodoItem(int id)
        {
            var todoItem = await _context.TodoItems.Include(t => t.Category).FirstOrDefaultAsync(t => t.Id == id);
            if (todoItem == null) return NotFound();

            if (!string.IsNullOrEmpty(todoItem.Location))
            {
                var (temperature, condition) = await _weatherService.GetCurrentWeather(todoItem.Location);
                return Ok(new { todoItem, weather = new { temperature, condition } });
            }

            return Ok(todoItem);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTodoItem(int id, TodoItem updatedTodo)
        {
            var todoItem = await _context.TodoItems.FindAsync(id);
            if (todoItem == null) return NotFound();

            todoItem.Title = updatedTodo.Title;
            todoItem.Priority = updatedTodo.Priority;
            todoItem.DueDate = updatedTodo.DueDate;
            todoItem.Location = updatedTodo.Location;
            todoItem.CategoryId = updatedTodo.CategoryId;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodoItem(int id)
        {
            var todoItem = await _context.TodoItems.FindAsync(id);
            if (todoItem == null) return NotFound();

            _context.TodoItems.Remove(todoItem);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchTodos(string title = null, int? priority = null, DateTime? dueDate = null)
        {
            var query = _context.TodoItems.AsQueryable();

            if (!string.IsNullOrEmpty(title))
                query = query.Where(t => t.Title.Contains(title));

            if (priority.HasValue)
                query = query.Where(t => t.Priority == priority.Value);

            if (dueDate.HasValue)
                query = query.Where(t => t.DueDate == dueDate.Value);

            return Ok(await query.ToListAsync());
        }
    }
}
