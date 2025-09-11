using Api.Context;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// MemoryCache
builder.Services.AddMemoryCache();

// DbContexts
builder.Services.AddDbContext<ApiDbContext>(options =>
	options.UseSqlServer(connectionString, sqlOptions =>
		sqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null)));

builder.Services.AddDbContext<CQRSDbContext>(options =>
	options.UseSqlServer(connectionString, sqlOptions =>
		sqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null)));

// MediatR
builder.Services.AddMediatR(typeof(Program));

// 🚀 API Versioning
builder.Services.AddApiVersioning(options =>
{
	options.DefaultApiVersion = new ApiVersion(1, 0);   // v1 por defecto
	options.AssumeDefaultVersionWhenUnspecified = true; // si no se manda versión, usa la default
	options.ReportApiVersions = true;                   // headers con versiones soportadas
	options.ApiVersionReader = ApiVersionReader.Combine(
		new QueryStringApiVersionReader("api-version"),
		new HeaderApiVersionReader("X-Version"),
		new UrlSegmentApiVersionReader()
	);
});

// API Explorer para Swagger multi-versión
builder.Services.AddVersionedApiExplorer(options =>
{
	options.GroupNameFormat = "'v'VVV";      // v1, v2
	options.SubstituteApiVersionInUrl = true;
});

// CORS
builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowAngular",
		policy => policy
			.WithOrigins("http://localhost:4200")
			.AllowAnyHeader()
			.AllowAnyMethod());
});

// Controllers
builder.Services.AddControllers()
	.AddJsonOptions(options =>
	{
		options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
		options.JsonSerializerOptions.WriteIndented = true;
	});

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 🔹 Swagger con versionado
if (app.Environment.IsDevelopment())
{
	var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

	app.UseSwagger();
	app.UseSwaggerUI(options =>
	{
		foreach (var description in provider.ApiVersionDescriptions)
		{
			options.SwaggerEndpoint(
				$"/swagger/{description.GroupName}/swagger.json",
				description.GroupName.ToUpperInvariant());
		}
	});
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.UseCors("AllowAngular");

// 🔹 Middleware para redirigir /api/... a /api/v1/...
app.Use(async (context, next) =>
{
	var path = context.Request.Path.Value;
	if (path != null && path.StartsWith("/api/", StringComparison.OrdinalIgnoreCase) &&
		!path.StartsWith("/api/v", StringComparison.OrdinalIgnoreCase))
	{
		var newPath = "/api/v1" + path.Substring(4);
		context.Response.Redirect(newPath + context.Request.QueryString, permanent: false);
		return;
	}
	await next();
});

// Map Controllers
app.MapControllers();

app.Run();
