using Microsoft.EntityFrameworkCore;
using ToDoWebApi.Models;
namespace ToDoWebApi.Models
{
   
    public class Category
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int? ParentCategoryId { get; set; }
        public Category ParentCategory { get; set; }
        public ICollection<TodoItem> TodoItems { get; set; }
    }
}
