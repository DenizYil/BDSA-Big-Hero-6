using System;
using System.Collections.Generic;
using System.Linq;
using CoProject.Client.Pages;
using CoProject.Infrastructure;
using CoProject.Infrastructure.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace CoProject.Server.Integration.Tests;

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

            services.AddDbContext<CoProjectContext>(options =>
            {
                options.UseSqlite(connection);
            });

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
        var supervisorOne = new User()
        {
            Email = "test@gmail.com", Id = "1", Image = "/images/noimage.jpeg", Name = "Supervisor One",
            Projects = new List<Project>(), Supervisor = true
        };
        var supervisorTwo = new User()
        {
            Email = "test@gmail.com", Id = "2", Image = "/images/noimage.jpeg", Name = "Supervisor Two",
            Projects = new List<Project>(), Supervisor = true
        };
        var tagOne = new Tag(){Id = 1, Name = "MYSQL", Projects = new List<Project>()};
        var tags = new List<Tag>() {tagOne};

        var projectOne = new Project()
        {
            Created = DateTime.Now, Description = "Description for test project one", Id = 1, Max = 2, Min = 0,
            Name = "Test Project One", State = State.Open, SupervisorId = supervisorOne.Id, Tags = tags,
            Users = new List<User>()  {supervisorOne}
        };
        
        var projectTwo = new Project()
        {
            Created = DateTime.Now, Description = "Description for test project two", Id = 2, Max = 5, Min = 1,
            Name = "Test Project Two", State = State.Open, SupervisorId = supervisorOne.Id, Tags = tags,
            Users = new List<User>()
        };
        var projectThree = new Project()
        {
            Created = DateTime.Now, Description = "Description for test project three", Id = 3, Max = 10, Min = 5,
            Name = "Test Project Three", State = State.Open, SupervisorId = supervisorOne.Id, Tags = tags,
            Users = new List<User>()
        };
        context.Projects.AddRange(
            projectOne,
            projectTwo,
            projectThree
            );
        
        context.Users.AddRange(supervisorOne, supervisorTwo);

        context.SaveChanges();
    }
}