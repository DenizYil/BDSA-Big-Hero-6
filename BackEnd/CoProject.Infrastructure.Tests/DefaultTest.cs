using System;
using CoProject.Infrastructure.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace CoProject.Infrastructure.Tests;

public abstract class DefaultTest<TRepo> where TRepo : IRepository
{
    protected CoProjectContext _context;
    protected TRepo _repo;
        
    public DefaultTest()
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
        // Don't remove the object[] specification. It breaks it - despite rider saying it can be removed.
        _repo = (TRepo) Activator.CreateInstance(typeof(TRepo), new object[] {_context});
    }
}