using Microsoft.EntityFrameworkCore;

namespace MyFirstAPI.Models
{
    public class TodoContext : DbContext
    {
        public DbSet<TodoItem> TodoItems { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<Session> Sessions { get; set; }

        public TodoContext(DbContextOptions<TodoContext> options) : base(options) { }
    }
}