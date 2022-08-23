var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();


#region Custom default
//DefaultFilesOptions options = new DefaultFilesOptions();
//System.Diagnostics.Debug.WriteLine(options.DefaultFileNames.Aggregate("", (result, next) => { return $"{result}, {next}"; }));
//options.DefaultFileNames.Clear();
//options.DefaultFileNames.Add("main.html");
//app.UseDefaultFiles(options);
#endregion

app.UseDefaultFiles();
app.UseStaticFiles();

app.Run();
