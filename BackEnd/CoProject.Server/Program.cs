using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Identity.Web;
using CoProject.Infrastructure;
using CoProject.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CoProject.Server;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAdB2C"));

        builder.Services.Configure<JwtBearerOptions>(
    JwtBearerDefaults.AuthenticationScheme, options =>
    {
        options.TokenValidationParameters.NameClaimType = "name";
    });

        // Add services to the container.
        builder.Services.AddControllers();
        
        // Database handling
        builder.Services.AddDbContext<CoProjectContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("CoProject")));
        builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
        builder.Services.AddScoped<ICoProjectContext, CoProjectContext>();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
            {
                Title= "CoProject - API",
                Version = "v1",
                Description = "The documentation for the CoProject REST API."
            });
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }


        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }
}