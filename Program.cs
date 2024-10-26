using System.Text.Json.Serialization;
using lion_force_be.DBContext;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("MySQLConnection");
var serverVersion = new MySqlServerVersion(new Version(8, 0, 25));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});
builder.Services.AddDbContext<DbContextLF>(
    options => options.UseMySql(connectionString, serverVersion)
    .LogTo(Console.WriteLine, LogLevel.Information)
    .EnableSensitiveDataLogging()
    .EnableDetailedErrors()
);
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());


//agregar cors en un futuro





var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.MapGet("/", () => "The server is running here");
app.Run();