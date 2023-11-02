using DinkToPdf.Contracts;
using DinkToPdf;
using Formatos.Pdf.Core.Interfaces;
using Formatos.Pdf.Exceptions;
using Formatos.Pdf.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
// Add services to the container.

services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));
services.AddTransient<IPdfService, PdfService>();

Path.Combine(Directory.GetCurrentDirectory(), "libwkhtmltox.dll");

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    var groupName = "v1";

    c.SwaggerDoc(groupName, new OpenApiInfo
    {
        Title = $"ADVANCE formularios web {groupName}",
        Version = groupName,
        Description = "Restful api, para la integración de formularios web con ADVANCE",
        Contact = new OpenApiContact
        {
            Name = "DMS",
            Email = "info@dms.ms",
            Url = new Uri("https://dms.ms/"),
        }
    });

    var securitySchema = new OpenApiSecurityScheme
    {
        Description = "Using the Authorization header with the Bearer scheme.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "bearer",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    };

    c.AddSecurityDefinition("Bearer", securitySchema);

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    { securitySchema, new[] { "Bearer" } }
                });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
});
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration.GetValue<string>("Authentication:Issuer"),
            ValidAudience = builder.Configuration.GetValue<string>("Authentication:Audience"),
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>("Authentication:SecretKey")))
        };
    });

builder.Services.AddCors(options =>
{
    options.AddPolicy("PrivacyPolicy", app =>
    {
        app.AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});


var app = builder.Build();

app.UseMiddleware<ExceptionHandlerMiddleware>();


// Configure the HTTP request pipeline.
app.UseSwagger();
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerUI();
}
else
{
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Api");
        // Add this line, it will work for you
        c.RoutePrefix = string.Empty;
    });
}

app.UseCors("PrivacyPolicy");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
