﻿using CoProject.Infrastructure.Entities;

namespace CoProject.Infrastructure.DTOs;

#pragma warning disable CS8618

public record UserCreateDTO
{
    public int Id { get; set; }
    public string Email { get; set; }
    public string NormalizedEmail { get; set; }
    public IReadOnlyCollection<Project> Projects { get; set; }
    public bool Supervisor { get; set; }
    public bool EmailConfirmed { get; set; }
    public string PhoneNumber { get; set; }
    public bool LockoutEnabled { get; set; }
    public DateTimeOffset? LockoutEnd { get; set; }
    public string UserName { get; set; }
    public string ConcurrencyStamp { get; set; }
    public string PasswordHash { get; set; }
    public string SecurityStamp { get; set; }
    public int AccessFailedCount { get; set; }
    public string NormalizedUserName { get; set; }
    public bool PhoneNumberConfirmed { get; set; }
    public bool TwoFactorEnabled { get; set; }
}

public record UserUpdateDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
}

public record UserDetailsDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
}