using System.Data.Entity;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class Todo {

        
        public int ID { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }
    }

    public class TodoDBContext : DbContext
    {
        
        public TodoDBContext()
            : base("DefaultConnection")
        {
        }
        public DbSet<Todo> Todos { get; set; }
    }
}