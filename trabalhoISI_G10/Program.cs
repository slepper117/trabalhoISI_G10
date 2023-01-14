using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;
using trabalhoISI_G10.Config;

var builder = WebApplication.CreateBuilder(args);
// Build Configuration Objects
Database.Host = builder.Configuration["db:host"];
Database.DB = builder.Configuration["db:database"];
Database.Username = builder.Configuration["db:username"];
Database.Password = builder.Configuration["db:password"];
Jwt.Issuer = builder.Configuration["jwt:issuer"];
Jwt.Audience = builder.Configuration["jwt:audience"];
Jwt.Secret = builder.Configuration["jwt:secret"];

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Add Coustom Documentation
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "TrabalhoISI_G10 API",
        Description = "Project developed for Information Integration Systems.",
        Contact = new OpenApiContact
        {
            Name = "Example Contact",
            Url = new Uri("https://example.com/contact")
        },
        License = new OpenApiLicense
        {
            Name = "Apache 2.0",
            Url = new Uri("https://example.com/license")
        }
    });

    // Add Security Definition
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });

    // Add Security Requirement
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    // Using System Reflection for documentation
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

// Add Authentication to Application
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(opt =>
    {
        //Contains a set of parameters that are used by a SecurityTokenHandler when validating a SecurityToken
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = Jwt.Issuer,
            ValidAudience = Jwt.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Jwt.Secret))
        };
    });

// Add Autorization to Application
builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Add Auth Middleware
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
