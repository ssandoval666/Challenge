using Api.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Text.Json.Serialization;



var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddMemoryCache();


builder.Services.AddDbContext<ApiDbContext>(options =>
	options.UseSqlServer(connectionString, sqlOptions =>
		sqlOptions.EnableRetryOnFailure(
			maxRetryCount: 5,                 // cantidad máxima de reintentos
			maxRetryDelay: TimeSpan.FromSeconds(10), // espera máxima entre reintentos
			errorNumbersToAdd: null           // podés especificar errores adicionales
		)));

builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowAngular",
		policy => policy
			.WithOrigins("http://localhost:4200") // Angular
			.AllowAnyHeader()
			.AllowAnyMethod());
});


// Add services to the container.

builder.Services.AddControllers()
	.AddJsonOptions(options =>
	{
		options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
		options.JsonSerializerOptions.WriteIndented = true;
	});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseCors("AllowAngular"); // 👈 agregar antes de MapControllers


app.MapControllers();

app.Run();
