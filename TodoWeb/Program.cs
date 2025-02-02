using Microsoft.EntityFrameworkCore;
using TodoWeb.EntityFramework;
using TodoWeb.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<TodoDb>(opt => opt.UseInMemoryDatabase("TodoList"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
var app = builder.Build();

app.MapGet("/", () => "Index Page");

var todoitems = app.MapGroup("/todoitems");

todoitems.MapGet("/", GetAllTodos);
todoitems.MapGet("/complete", GetCompleteTodos);
todoitems.MapGet("/incomplete", GetIncompleteTodos);
todoitems.MapGet("/{id}", GetTodoById);
todoitems.MapPost("/", CreateTodoItem);
todoitems.MapPut("/", UpdateTodoItem);
todoitems.MapPut("/{id}", UpdateTodoItem);
todoitems.MapDelete("/{id}", DeleteTodoItem);


app.Run();

static async Task<IResult> GetAllTodos(TodoDb db)
{
    return TypedResults.Ok(await db.Todos.ToArrayAsync());
}

static async Task<IResult> GetCompleteTodos(TodoDb db)
{
    return TypedResults.Ok(await db.Todos.Where(x => x.IsComplete).ToArrayAsync());
}

static async Task<IResult> GetIncompleteTodos(TodoDb db)
{
    return TypedResults.Ok(await db.Todos.Where(x => !x.IsComplete).ToArrayAsync());
}

static async Task<IResult> GetTodoById(TodoDb db, int id)
{
    return await db.Todos.FindAsync(id)
        is Todo todo
        ? Results.Ok(todo)
        : Results.NotFound();
}

static async Task<IResult> CreateTodoItem(Todo todo, TodoDb db)
{
    db.Todos.Add(todo);
    await db.SaveChangesAsync();
    return Results.Created($"/todoitems/{todo.Id}", todo);
}

static async Task<IResult> UpdateTodoItem(Todo todo, TodoDb db)
{
    var existingTodo = await db.Todos.FindAsync(todo.Id);
    if (existingTodo is null) return Results.NotFound();
    existingTodo.Name = todo.Name;
    existingTodo.IsComplete = todo.IsComplete;
    db.Todos.Update(existingTodo);
    await db.SaveChangesAsync();
    return Results.NoContent();
}

static async Task<IResult> DeleteTodoItem(int id, TodoDb db)
{
    var existingTodo = await db.Todos.FindAsync(id);
    if (existingTodo is null) return Results.NotFound();
    db.Todos.Remove(existingTodo);
    await db.SaveChangesAsync();
    return Results.NoContent();
}