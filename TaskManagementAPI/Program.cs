using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;
using TaskManagementAPI.Data;
using TaskManagementAPI.Interface;
using TaskManagementAPI.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

string connectionString;

if (environment == "Development")
{
    // Use local MySQL database
    connectionString = builder.Configuration.GetConnectionString("LocalMySql")??
        throw new ArgumentNullException("Local MySQL connection string is missing!");
}
else
{
    // Use MySQL database on Render
    connectionString = Environment.GetEnvironmentVariable("LiveConnection") ??
        throw new ArgumentNullException("MySQL connection string is missing!");
}
builder.Services.AddDbContext<TaskManagementDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

Console.WriteLine($"Using {environment} database: {connectionString}");

builder.Services.AddScoped<IUsers, UsersService>();
builder.Services.AddScoped<IProject, ProjectService>();
builder.Services.AddScoped<ITaskMag, TaskMagService>();




builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    //options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
                    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                });

string jwtKey, jwtIssuer, jwtAudience;

if (environment == "Development")
{
    jwtKey = builder.Configuration["Jwt:Key"] ?? throw new ArgumentNullException("JWT Key is missing in appsettings.json.");
    jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "DaraTrained";
    jwtAudience = builder.Configuration["Jwt:Audience"] ?? "JwtAudience";
}
else
{
    jwtKey = Environment.GetEnvironmentVariable("JWT_KEY") ?? throw new ArgumentNullException("JWT Key is not configured properly.");
    jwtIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? "DaraTrained";
    jwtAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? "JwtAudience";
}
var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

//var jwtKey = builder.Configuration["Jwt:Key"];
//if (string.IsNullOrEmpty(jwtKey))
//{
 //   throw new ArgumentNullException("JWT Key is not configured properly.");
//}
//var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = key,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

    // Define the security scheme
    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
       // Description = "Enter 'Bearer' [space] and then your token in the text input below.\n\nExample: \"Bearer abcdef12345\"",
    };

    c.AddSecurityDefinition("Bearer", securityScheme);

    var securityRequirement = new OpenApiSecurityRequirement
{
    {
        new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"  
            }
        },
        new string[] {} 
    }
};
});





var app = builder.Build();
app.UseAuthentication();

         // Configure the HTTP request pipeline.
         if (app.Environment.IsDevelopment())
         {
             app.UseSwagger();
             app.UseSwaggerUI();
         }
        app.UseSwagger();
        app.UseSwaggerUI();

 // var port = Environment.GetEnvironmentVariable("PORT") ?? "10000";
 // app.Urls.Add($"http://0.0.0.0:{port}");

   //Map some test route
   //app.MapGet("/", () => "Hello, Render!");
//   app.UseHttpsRedirection();

app.UseAuthorization();

         app.MapControllers();

app.Run();
         

         
     
     
