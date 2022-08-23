using WA11;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

//app.UseMiddleware<MyMiddleware>();
app.UseMyMiddleware();
app.MapGet("/", () => "Hello World!");

app.Run();
