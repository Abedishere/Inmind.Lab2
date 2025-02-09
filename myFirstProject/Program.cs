using myFirstProject.Filter;
using myFirstProject.Middleware;
using myFirstProject.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddScoped<LoggingActionFilter>();
builder.Services.AddScoped<IObjectMapperService, ObjectMapperService>();
builder.Services.AddScoped<IUserService, UserService>();

var app = builder.Build();

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.ContentType = "application/json";
        var exceptionHandler = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();
        await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(new
        {
            error = exceptionHandler?.Error.Message ?? "An error occurred"
        }));
    });
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();
app.UseAuthorization();
app.MapControllers();
app.UseMiddleware<RequestLoggingMiddleware>(); 
app.Run();