using Backend.Auth;
using Backend.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Backend.Services;
using Backend.Repositories;

var builder = WebApplication.CreateBuilder(args);
ConfigurationManager configuration = builder.Configuration;

// Add services to the container.

// For Entity Framework
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

// For Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();


builder.Services.AddCors(options =>
             {
                 options.AddPolicy(name: "ProdOrigins", builder =>
                      builder.AllowAnyOrigin()
                      .AllowAnyHeader()
                      .AllowAnyMethod());
                 options.AddPolicy(name: "DevOrigins", builder =>
                     builder.WithOrigins("https://leveragefc-frontend.onrender.com", "http://leveragefc-frontend.onrender.com")
                     .AllowAnyHeader()
                     .AllowAnyMethod()
                     .AllowCredentials());
             });


builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddTransient<ICompanyService, CompanyService>();
builder.Services.AddTransient<ILineOfBusinessService, LineOfBusinessService>();
builder.Services.AddTransient<IClientService, ClientService>();
builder.Services.AddTransient<IMilestoneService, MilestoneService>();
builder.Services.AddTransient<IOpportunityService, OpportunityService>();
builder.Services.AddTransient<IProjectService, ProjectService>();
builder.Services.AddTransient(typeof(IRepository<>), typeof(Repository<>)); ;

// Adding Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})

// Adding Jwt Bearer
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.Zero,

        ValidAudience = configuration["JWT:ValidAudience"],
        ValidIssuer = configuration["JWT:ValidIssuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]))
    };
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("DevOrigins");

//app.UseHttpsRedirection();

// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();