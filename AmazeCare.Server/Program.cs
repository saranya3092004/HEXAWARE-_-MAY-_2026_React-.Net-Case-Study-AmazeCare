using AmazeCare.Server.Data;
using AmazeCare.Server.Modules.AdminModule.Repository;
using AmazeCare.Server.Modules.AdminModule.Service;
using AmazeCare.Server.Modules.Auth.Repository.Implementation;
using AmazeCare.Server.Modules.Auth.Repository.Interface;
using AmazeCare.Server.Modules.Auth.Services.Implementation;
using AmazeCare.Server.Modules.Auth.Services.Interface;
using AmazeCare.Server.Modules.DoctorModule.Repository;
using AmazeCare.Server.Modules.DoctorModule.Service;
using AmazeCare.Server.Modules.Extensions;
using AmazeCare.Server.Modules.Middlewares; 
using AmazeCare.Server.Modules.PatientModule.Repository;
using AmazeCare.Server.Modules.PatientModule.Service;
using AmazeCare.Server.Repository.Implementations;
using AmazeCare.Server.Repository.Interfaces;
using AmazeCare.Server.Services.Implementations;
using AmazeCare.Server.Services.Interfaces;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using System.Reflection.Metadata;

var builder = WebApplication.CreateBuilder(args);



// 1. CORE & SWAGGER SERVICES
builder.Logging.AddAmazeCareLog4Net();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();


builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "AmazeCare",
        Version = "v1",
        Description = "Amaze Care Backend API"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter: Bearer {your JWT token}"
    });

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = []
    });

}); 

builder.Services.AddOpenApi();
builder.Services.AddMemoryCache();
builder.Logging.AddAmazeCareLog4Net();

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddDbContext<AmazeCareDBContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IJwtService, JwtService>();

builder.Services.AddScoped<IPatientRepository, PatientRepository>();
builder.Services.AddScoped<IPatientService, PatientService>();

builder.Services.AddScoped<IDoctorRepository, DoctorRepository>();
builder.Services.AddScoped<IDoctorService, DoctorService>();

builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
builder.Services.AddScoped<IAppointmentService, AppointmentService>();

builder.Services.AddScoped<IConsultationRepository, ConsultationRepository>();
builder.Services.AddScoped<IConsultationService, ConsultationService>();

builder.Services.AddScoped<IAdminRepository, AdminRepository>();
builder.Services.AddScoped<IAdminService, AdminService>();


builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddAmazeCareCors(builder.Configuration);


var app = builder.Build();



// MIDDLEWARE

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseGlobalExceptionHandling();
app.UseHttpsRedirection();
app.UseDefaultFiles();
app.MapStaticAssets();

// 2. CORS must execute before Authentication blocks cross-origin requests
app.UseCors(CorsSetupExtensions.PolicyName);

// 3. Identifies WHO the user is via the JWT Token
app.UseAuthentication();

// 4. Decides IF the user is allowed to access the controller endpoint
app.UseAuthorization();

// 5. Routing mappings
app.MapControllers();
app.MapFallbackToFile("/index.html");



app.MapGet("/debug/endpoints", (IEnumerable<EndpointDataSource> sources) =>
    sources.SelectMany(s => s.Endpoints)
           .OfType<RouteEndpoint>()
           .Select(e => $"{e.RoutePattern.RawText} -> {e.DisplayName}"));
app.Run();