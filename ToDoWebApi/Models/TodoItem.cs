using Microsoft.EntityFrameworkCore;
using ToDoWebApi.Models;
namespace ToDoWebApi.Models
{
    public class TodoItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int Priority { get; set; } = 3;
        public string Location { get; set; } // Format: "latitude,longitude"
        public DateTime? DueDate { get; set; }
        public int? CategoryId { get; set; }
        public Category Category { get; set; }
    }
}
