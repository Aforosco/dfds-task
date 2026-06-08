using DFDSVisitManagementAPI.Domain.src.Data;
using DFDSVisitManagementAPI.Business.src.Interfaces;
using DFDSVisitManagementAPI.Business.src.Services;
using DFDSVisitManagementAPI.src.Application.Controllers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;
using DFDSVisitManagementAPI.Business.Services;

var builder = WebApplication.CreateBuilder(args);

// Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity — users only, no roles
builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = false;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

// JWT Authentication
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
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
    };
});

// Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IVisitService, VisitService>();
builder.Services.AddScoped<IDriverService,DriverService>();
builder.Services.AddScoped<IActivityService, ActivityService>();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new System.Text.Json.Serialization.JsonStringEnumConverter());
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    })
    .AddApplicationPart(typeof(VisitController).Assembly);

    builder.Services.AddEndpointsApiExplorer();

    
var baseDir = AppContext.BaseDirectory;
Console.WriteLine($"Base dir: {baseDir}");
Console.WriteLine($"Files: {string.Join(", ", Directory.GetFiles(baseDir, "*.xml"))}");
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "DFDS Visit Management API",
        Version = "v1",
        Description = "API for managing truck visits at DFDS terminals",
        Contact = new OpenApiContact
        {
            Name = "DFDS Gate Management",
            Email = "gate@dfds.com"
        }
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT token. Example: eyJhbGci..."
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            Array.Empty<string>()
        }
    });

    // Framework XML (Program.cs lives here)
    var frameworkXml = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var frameworkXmlPath = Path.Combine(AppContext.BaseDirectory, frameworkXml);
    if (File.Exists(frameworkXmlPath))
        c.IncludeXmlComments(frameworkXmlPath);

    // Application XML (controllers live here) ← this was missing
    var appXml = "DFDSVisitManagementAPI.Application.xml";
    var appXmlPath = Path.Combine(AppContext.BaseDirectory, appXml);
    if (File.Exists(appXmlPath))
        c.IncludeXmlComments(appXmlPath, includeControllerXmlComments: true);
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader();
        
    });
});


var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.UseCors("AllowAll");
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "DFDS Visit Management API v1");
    c.RoutePrefix = string.Empty; 
});

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();