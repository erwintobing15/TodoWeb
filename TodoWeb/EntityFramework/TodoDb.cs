using Microsoft.EntityFrameworkCore;
using TodoWeb.Models;

namespace TodoWeb.EntityFramework;

public class TodoDb : DbContext
{
    public DbSet<Todo> Todos => Set<Todo>();

    public TodoDb(DbContextOptions<TodoDb> options) 
        : base(options) { }
}