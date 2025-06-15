using Entity.Configrations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;
using Api.MiddleWares;
using DataAccessLayer.Concrete.DatabaseFolder;
using DataAccessLayer.Concrete.DatabaseFolder.SeedData;
using Microsoft.AspNetCore.Diagnostics;
using System.Text.Json;
using Entity.DTOs;
using DataAccessLayer.Abstract;
using DataAccessLayer.Concrete;
using BusinessLogicLayer.Abstact;
using BusinessLogicLayer.Concrete;
using Entity.Models;
using BusinessLogicLayer.Concrete.EfCore;

var builder = WebApplication.CreateBuilder(args);

//SeriLog Configration
var logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .WriteTo.Console()
    .CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);

// Add Repository and UnitOfWork Dependencies
builder.Services.AddScoped<IDoctorRepository, DoctorRepository>();
builder.Services.AddScoped<IPatientRepository, PatientRepository>();
builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
builder.Services.AddScoped<IDoctorScheduleRepository, DoctorScheduleRepository>();
builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Add Business Logic Layer Dependencies
builder.Services.AddScoped<IDepartmentService, DepartmentManager>();
builder.Services.AddScoped<IDoctorService, DoctorManager>();
builder.Services.AddScoped<IPatientService, PatientManager>();
builder.Services.AddScoped<IDoctorScheduleService, DoctorScheduleManager>();
builder.Services.AddScoped<IAppointmentService, AppointmentManager>();
builder.Services.AddScoped<IAuthanticateService, EfCoreAuthService>();

// Add AutoMapper
builder.Services.AddAutoMapper(typeof(BusinessLogicLayer.Abstact.IDepartmentService).Assembly);

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder => builder
        .WithOrigins("http://127.0.0.1:5500", "http://192.168.1.103:5500", "http://localhost:5500", "http://localhost:3000",
            "http://192.168.1.103:3000", "http://192.168.188.148:3000")
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials());
});

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<CustomTokenOption>(builder.Configuration.GetSection("JWT"));

builder.Services.AddDbContext<ProjectMainContext>(opt =>
{
    if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
    {
        opt.UseSqlite(builder.Configuration["ConnectionStrings:ProjectMainContext"]);
    }
    else
    {
        opt.UseSqlite(builder.Configuration["ConnectionStrings:ProductSqliteConnection"]);
    }
});

builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<ProjectMainContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;

    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);

    options.Lockout.AllowedForNewUsers = true;
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedEmail = false;
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = true;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["JWT:SecurityKey"])),
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true
    };
});

builder.Services.AddMvc().ConfigureApiBehaviorOptions(options =>
{
    options.InvalidModelStateResponseFactory = (context) =>
    {
        var errors = context.ModelState.Values.SelectMany(x => x.Errors.Select(p => p.ErrorMessage)).ToList();
        var response = ServiceResponse<object>.Failure(errors);

        return new BadRequestObjectResult(response);
    };
});

var app = builder.Build();

// Create and seed the database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ProjectMainContext>();

        // Apply any pending migrations
        if (context.Database.GetPendingMigrations().Any())
        {
            context.Database.Migrate();
        }

        // Seed the database
        var seeder = new DatabaseSeeder(context);
        await seeder.SeedAsync();
    }
    catch (Exception ex)
    {
        var appLogger = services.GetRequiredService<ILogger<Program>>();
        appLogger.LogError(ex, "An error occurred while creating/seeding the database.");
    }
}

app.UseMiddleware<ExceptionMiddleWare>();
app.UseCors("CorsPolicy");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseEndpoints(opt =>
{
    opt.MapControllerRoute(
        name: "auth",
        pattern: "api/auth/{action}",
        defaults: new { controller = "Auth", action = "Login" }
    );
});

app.MapControllers();

app.Run();
