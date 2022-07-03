using Microsoft.EntityFrameworkCore;
using TodoApi.Data;
using TodoApi.Middlewares;
using TodoApi.Models;
using TodoApi.ViewModels;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


try
{
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

    using (var db = builder.Services.BuildServiceProvider().GetService<AppDbContext>())
    {
        db.Database.EnsureCreated();
    }

    Console.WriteLine("PostgreSQL database created");
}
catch
{
    // if there is an error, using the in-memory database
    var dbService = builder.Services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));

    builder.Services.Remove(dbService);

    builder.Services.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase("TodoList"));

    Console.WriteLine("In-memory database created");
}

// Seeding database
using (var db = builder.Services.BuildServiceProvider().GetService<AppDbContext>())
{
    if (db.Users.Count() == 0)
    {
        db.Users.Add(
            new User(
                username: "admin",
                password: "admin",
                token: "3848cb5e-29f1-4ce8-9396-d7380b7b3fdd"));

        db.SaveChanges();
    }
}


var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseMiddleware<AuthMiddleware>();


// add MapPost for register user async
app.MapPost("/v1/users", async (LoginViewModel model, AppDbContext context) =>
{
    var user = await context.Users.FirstOrDefaultAsync(u => u.Username == model.Username);
    if (user != null)
        return Results.BadRequest("Username already exists");

    await context.Users.AddAsync(
        new User(
            username: model.Username,
            password: model.Password));

    await context.SaveChangesAsync();
    return Results.Ok();
});


app.MapPost("/v1/login", async (LoginViewModel model, AppDbContext db) =>
{
    var user = await db.Users.FirstOrDefaultAsync(u => u.Username == model.Username);
    if (user == null || user.Password != model.Password)
        return Results.BadRequest("Invalid username or password");

    return Results.Ok(new { token = user.Token });
});


app.MapGet("/v1/todos", async (AppDbContext db) =>
    await db.Todos.ToListAsync());


app.MapGet("/v1/todos/{id}", async (Guid id, AppDbContext db) =>
    await db.Todos.FindAsync(id)
        is Todo todo
            ? Results.Ok(todo)
            : Results.NotFound());


app.MapPost("/v1/todos", async (CreateTodoViewModel model, AppDbContext db) =>
{
    var todo = model.MapTo();
    if (!model.IsValid) return Results.BadRequest(model.Notifications);
    db.Todos.Add(todo);
    await db.SaveChangesAsync();
    return Results.Created($"/todoitems/{todo.Id}", todo);
});


app.MapPut("/todos/{id}", async (Guid id, UpdateTodoViewModel model, AppDbContext db) =>
{
    var todo = await db.Todos.FindAsync(id);
    if (todo is null) return Results.NotFound();
    model.MapTo(todo);
    if (!model.IsValid) return Results.BadRequest(model.Notifications);
    await db.SaveChangesAsync();
    return Results.Ok();
});


app.MapDelete("/todos/{id}", async (Guid id, AppDbContext db) =>
{
    var todo = await db.Todos.FindAsync(id);
    if (todo is null) return Results.NotFound();
    db.Todos.Remove(todo);
    await db.SaveChangesAsync();
    return Results.Ok();
});


app.Run();