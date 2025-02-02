
using HL7.Services;
using HL7.Services.Interfaces;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var configuration = builder.Configuration;
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<IInsertHl7DataService, JSONDataService>((provider) => new JSONDataService(configuration));
builder.Services.AddTransient<IInsertHl7DataService, JSONDataService>();
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection"); // 🔹 Register DbContext with SQL Server 
//builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

// builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer("Server=DESKTOP-IFQHBC0\\SQLEXPRESS;Database=HL7DB;TrustServerCertificate=True;"));
var app = builder.Build();

// Configure the HTTP request pipeline. 
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
