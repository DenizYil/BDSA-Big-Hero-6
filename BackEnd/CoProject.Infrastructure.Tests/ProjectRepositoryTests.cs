using System;
using CoProject.Infrastructure.DTOs;
using CoProject.Infrastructure.Entities;
using CoProject.Infrastructure.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CoProject.Infrastructure.Tests;

public class ProjectRepositoryTests
{
    private readonly CoProjectContext _context;
    private readonly ProjectRepository _repo;

    public ProjectRepositoryTests()
    {
        var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();

        var builder = new DbContextOptionsBuilder<CoProjectContext>();
        builder.UseSqlite(connection);
        builder.EnableSensitiveDataLogging();

        var context = new CoProjectContext(builder.Options);
        context.Database.EnsureCreated();
        context.SaveChanges();

        _context = context;
        _repo = new ProjectRepository(_context);
    }
    
    [Fact]
    public async void Read_Given_Non_existing_id_returns_null()
    {
        //Arrange
        //Act
        //Assert
        Assert.Null(await _repo.Read(2));
    }
    
    [Fact]
    public async void Read_Given_Existing_Project_Returns_Project()
    {
        var now = DateTime.Now;
        //Arrange
        Project hello = new Project{
            Id = 1, 
            Name = "Karl", 
            Description = "yep hehe smiley", 
            SupervisorId = 1,
            Created = now, 
            StateId = 1
        };

        ProjectDTO expectedDTO = new ProjectDTO(
            1,
            "Karl",
            "yep hehe smiley",
            now,
            1,
            null,
            null
        );
        
        //Act
        _context.Add(hello);
        _context.SaveChanges();
        
        //Assert
        Assert.Equal(expectedDTO, await _repo.Read(1));
    }
    
}