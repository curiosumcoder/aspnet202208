var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapGet("/Product/{id:int}", async context =>
{
    var id = context.Request.RouteValues["id"];

    string content = "Sin contenido";

    if (id != null)
    {
        string fileName = $"wwwroot\\{id}.html";
        if (File.Exists(fileName))
        {
            using (var sr = new StreamReader(fileName))
            {
                // Read the stream as a string, and write the string to the console.
                content = sr.ReadToEnd();
            }
        }
    }

    await context.Response.WriteAsync(content);
});

app.Run();
