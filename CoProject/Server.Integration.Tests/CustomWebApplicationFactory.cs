using System;
using CoProject.Infrastructure;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Server.Integration.Tests;

public class CustomWebApplicationFactory : WebApplicationFactory<ProgramServer>
{
    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var dbContext = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<CoProjectContext>));

            if (dbContext != null)
            {
                services.Remove(dbContext);
            }

            /* Overriding policies and adding Test Scheme defined in TestAuthHandler */
            services.AddMvc(options =>
            {
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .AddAuthenticationSchemes("Test")
                    .Build();

                options.Filters.Add(new AuthorizeFilter(policy));
            });

            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = "Test";
                    options.DefaultChallengeScheme = "Test";
                    options.DefaultScheme = "Test";
                })
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", options => { });

            var connection = new SqliteConnection("Filename=:memory:");

            services.AddDbContext<CoProjectContext>(options => { options.UseSqlite(connection); });

            var provider = services.BuildServiceProvider();
            using var scope = provider.CreateScope();
            using var appContext = scope.ServiceProvider.GetRequiredService<CoProjectContext>();
            appContext.Database.OpenConnection();
            appContext.Database.EnsureCreated();

            Seed(appContext);
        });

        builder.UseEnvironment("Integration");

        return base.CreateHost(builder);
    }

    private void Seed(CoProjectContext context)
    {
        var supervisorOne = new User
        {
            Id = "1",
            Name = "Supervisor One",
            Email = "test@gmail.com",
            Image = "/images/noimage.jpeg",
            Projects = new List<Project>(),
            Supervisor = true
        };

        var supervisorTwo = new User
        {
            Id = "2",
            Name = "Supervisor Two",
            Email = "test@gmail.com",
            Image = "/images/noimage.jpeg",
            Projects = new List<Project>(),
            Supervisor = true
        };

        var tags = new List<Tag>
        {
            new()
            {
                Id = 1,
                Name = "MYSQL",
                Projects = new List<Project>()
            }
        };

        var projectOne = new Project
        {
            Id = 1,
            Name = "Test Project One",
            Description = "Description for test project one",
            Created = DateTime.Now,
            Max = 2,
            Min = 0,
            State = State.Open,
            SupervisorId = supervisorOne.Id,
            Tags = tags,
            Users = new List<User> {supervisorOne}
        };

        var projectTwo = new Project
        {
            Id = 2,
            Name = "Test Project Two",
            Description = "Description for test project two",
            Created = DateTime.Now,
            Max = 5,
            Min = 1,
            State = State.Open,
            SupervisorId = supervisorOne.Id,
            Tags = tags,
            Users = new List<User>()
        };


        var projectThree = new Project
        {
            Id = 3,
            Name = "Test Project Three",
            Description = "Description for test project three",
            Created = DateTime.Now,
            Max = 10,
            Min = 5,
            State = State.Open,
            SupervisorId = supervisorOne.Id,
            Tags = tags,
            Users = new List<User>()
        };

        context.Projects.AddRange(projectOne, projectTwo, projectThree);
        context.Users.AddRange(supervisorOne, supervisorTwo);

        context.SaveChanges();
    }
}