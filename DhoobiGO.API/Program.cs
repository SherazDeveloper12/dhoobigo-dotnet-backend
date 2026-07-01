using DhoobiGO.Infrastructure.Persistence;
using DhoobiGO.Application.Interfaces;
using DhoobiGO.Application.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// --- FORCED NETWORK BROADCAST FOR MOBILE APP ---
builder.WebHost.UseUrls("http://0.0.0.0:5286");

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "DhoobiGO API", Version = "v1" });
    
    // --- ADD JWT AUTHORIZE BUTTON TO SWAGGER ---
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// Configure PostgreSQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IApplicationDbContext>(provider => 
    provider.GetRequiredService<ApplicationDbContext>());

// Configure JWT
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secret = jwtSettings["Secret"] ?? "a_very_long_and_secure_secret_that_is_at_least_32_characters_long";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "Bearer";
    options.DefaultChallengeScheme = "Bearer";
})
.AddJwtBearer("Bearer", options =>
{
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"] ?? "DhoobiGO",
        ValidAudience = jwtSettings["Audience"] ?? "DhoobiGO_Users",
        IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
            System.Text.Encoding.UTF8.GetBytes(secret)),
        NameClaimType = "name",
        RoleClaimType = System.Security.Claims.ClaimTypes.Role
    };
});

builder.Services.AddAuthorization();

// Configure Application Services
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<INotificationService, DhoobiGO.API.Services.SignalRNotificationService>();

builder.Services.AddSignalR();

// CORS for the React Admin App
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder
            .SetIsOriginAllowed(_ => true) // Allow any origin
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials()); // Required for SignalR
});

var app = builder.Build();

app.UseCors("AllowAll");

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(c => {
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "DhoobiGO API V1");
    c.RoutePrefix = string.Empty; // Serve Swagger at the root (/)
    c.DocumentTitle = "DhoobiGO API Documentation";
});

// app.UseHttpsRedirection();
app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        ctx.Context.Response.Headers.Append("Access-Control-Allow-Origin", "*");
        ctx.Context.Response.Headers.Append("Access-Control-Allow-Headers", "Origin, X-Requested-With, Content-Type, Accept");
    }
});
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<DhoobiGO.API.Hubs.ChatHub>("/chatHub");
app.MapHub<DhoobiGO.API.Hubs.NotificationHub>("/notificationHub");

// Database Initialization & Seeding
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        context.Database.Migrate();

        if (!context.ServiceTypes.Any())
        {
            context.ServiceTypes.AddRange(new List<DhoobiGO.Domain.Entities.ServiceType>
            {
                new() { Name = "Washing", Description = "Professional laundry wash", Icon = "water" },
                new() { Name = "Ironing", Description = "Steam iron finish", Icon = "shirt" },
                new() { Name = "Dry Clean", Description = "Premium chemical cleaning", Icon = "color-filter" },
                new() { Name = "Stain Removal", Description = "Targeted treatment for tough spots", Icon = "sparkles" }
            });
            context.SaveChanges();
            Console.WriteLine("--> Database Seeded: ServiceTypes added.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"--> Error during startup synchronization: {ex.Message}");
    }
}

app.Run();
