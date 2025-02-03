using Microsoft.EntityFrameworkCore;
using TaskManagementAPI.Data;
using TaskManagementAPI.Interface;
using TaskManagementAPI.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var Connection = builder.Configuration.GetConnectionString("Default");
builder.Services.AddDbContext<TaskManagementDbContext>(Options => Options.UseSqlServer(Connection));
builder.Services.AddScoped<IUsers, UsersService>();
builder.Services.AddScoped<IProject, ProjectService>();


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
         

         
     
     
