using Microsoft.EntityFrameworkCore;
using ToDoWebApi.Models;
namespace ToDoWebApi.Models
{
    public class TodoApiResponse
    {
        public List<TodoItemDto> Todos { get; set; }
    }
}
