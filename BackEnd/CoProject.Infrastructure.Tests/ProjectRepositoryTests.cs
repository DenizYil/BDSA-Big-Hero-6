using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using CoProject.Infrastructure.DTOs;
using CoProject.Infrastructure.Entities;
using CoProject.Infrastructure.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CoProject.Infrastructure.Tests;

public class ProjectRepositoryTests : DefaultTest<ProjectRepository>
{
    [Fact]
    public async void Read_Given_Non_existing_id_returns_null()
    {
        Assert.Null(await _repo.Read(2));
    }
    
    [Fact]
    public async void Read_Given_Existing_Project_Returns_Project()
    {
        //Arrange
        var now = DateTime.Now;
        
        var project = new Project{
            Id = 1, 
            Name = "Karl", 
            Description = "yep hehe smiley", 
            SupervisorId = 1,
            Created = now, 
            State = State.Open
        };

        var expected = new ProjectDTO
        {
            Id = 1,
            Name = "Karl",
            Description = "yep hehe smiley",
            SupervisorId = 1,
            Created = now,
            State = State.Open
        };
        
        _context.Add(project);
        _context.SaveChanges();
        
        // Act
        var actual = await _repo.Read(1);

        //Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async void ReadAll_Given_Multiple_existing_Projects_Returning_All_Projects()
    {
        //Arrange
        var now = DateTime.Now;
        
        var expected = new List<ProjectDTO>
        {
            new()
            {
                Id = 1,
                Name = "Karl",
                Description = "yep hehe smiley",
                SupervisorId = 1,
                Created = now,
                State = State.Open
            },
            new()
            {
                Id = 2,
                Name = "Phillip",
                Description = "This is another cool description",
                SupervisorId = 2,
                Created = now,
                State = State.Open
            }
        };
        
        _context.Add(new Project{
            Id = 1, 
            Name = "Karl", 
            Description = "yep hehe smiley", 
            SupervisorId = 1,
            Created = now, 
            State = State.Open
        });
        _context.Add(new Project{
            Id = 2, 
            Name = "Phillip", 
            Description = "This is another cool description", 
            SupervisorId = 2,
            Created = now, 
            State = State.Open
        });
        _context.SaveChanges();
        
        // Act
        var actual = await _repo.ReadAll();

        //Assert
        Assert.Equal(expected, actual);
    }
}