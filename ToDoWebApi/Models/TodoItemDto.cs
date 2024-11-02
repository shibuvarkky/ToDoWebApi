using Microsoft.EntityFrameworkCore;
using ToDoWebApi.Models;
namespace ToDoWebApi.Models
{
    public class TodoItemDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public bool Completed { get; set; }

    }
}
