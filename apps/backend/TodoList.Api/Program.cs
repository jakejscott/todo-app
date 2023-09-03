using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.OpenApi.Models;
using TodoList.Api.Endpoints.Todos;

namespace TodoList.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        // NOTE: Setup entity framework with sqlite
        var connectionString = builder.Configuration.GetConnectionString("Todos") ?? "Data Source=./todos.db";
        builder.Services.AddDbContext<TodoContext>(options =>
        {
            options.UseSqlite(connectionString);
            
            // NOTE: Turn off EF warnings that are logged by default, as we are using unique indexes on fields it's expected
            options.ConfigureWarnings(x =>
            {
                x.Ignore(CoreEventId.CoreBaseId);
                x.Ignore(RelationalEventId.CommandError);
            });
        });
        
        // NOTE: Setup cors
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAllHeaders", policy =>
            {
                policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
            });
        });

        // NOTE: Setup swagger/openapi
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "Todos API", Version = "v1", Description = "Todos API" });
            options.DescribeAllParametersInCamelCase();
            
            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
        });

        var app = builder.Build();
        
        // NOTE: Return problem+details if request is invalid
        app.UseStatusCodePages(async context =>
        {
            var response = Results.Problem(statusCode: context.HttpContext.Response.StatusCode);
            await response.ExecuteAsync(context.HttpContext);
        });

        app.UseCors("AllowAllHeaders");
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseTodosEndpoint();

        app.Run();
    }
}