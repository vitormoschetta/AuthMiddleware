using System.Net;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using TodoApi.Data;

namespace TodoApi.Middlewares;
public class AuthMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<AuthMiddleware> _logger;

    public AuthMiddleware(RequestDelegate next, ILogger<AuthMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context, AppDbContext db)
    {
        context.Response.ContentType = "application/json";

        try
        {
            if (context.Request.Path.Value.Contains("/v1/todos"))
            {
                var token = context.Request.Headers["Authorization"].ToString().Split(' ')[1];

                var user = await db.Users.FirstOrDefaultAsync(u => u.Token == token);
                if (user == null)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    var response = JsonSerializer.Serialize(new { error = "Unauthorized" });
                    await context.Response.WriteAsync(response);
                    return;
                }
            }

            await _next(context);
        }
        catch
        {
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            var response = JsonSerializer.Serialize(new { error = "Unauthorized" });
            await context.Response.WriteAsync(response);
        }
    }
}