var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// GET /
//app.MapGet("/", () => "Hello World!");

app.MapGet("/", async context =>
{
    var path = context.Request.Path;

    context.Response.ContentType = "text/html";
    await context.Response.WriteAsync("<h1>Hello World!</h1>");
});

app.Run();
