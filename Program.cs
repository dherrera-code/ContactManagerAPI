using ContactManagerAPI.Context;
using ContactManagerAPI.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddScoped<ContactServices>();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("Clients", policy =>
    {
        policy.WithOrigins("https://contactmanagerdbh.vercel.app", "http://localhost:3001", "http://localhost:3000")
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});
var connectionString = builder.Configuration.GetConnectionString("GetConnection");
builder.Services.AddDbContext<DataContext>(options => options.UseSqlServer(connectionString));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("Clients");

app.UseAuthorization();

app.MapControllers();

app.Run();
