using LaChasseAuTresor_ISY.Models;
using LaChasseAuTresor_ISY.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddScoped<InputParserService>();
builder.Services.AddScoped<GameEngine>();


builder.Services.AddScoped<Map>(provider =>
{
    return new Map();
});

var app = builder.Build();

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseRouting();

app.UseHttpsRedirection();

app.UseAuthorization();

// Configure the default route
app.MapGet("/", () => Results.Redirect("/htmlpage.html"));

app.MapControllers();

app.Run();
