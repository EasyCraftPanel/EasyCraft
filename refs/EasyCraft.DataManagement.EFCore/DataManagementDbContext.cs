using System;
using System.Collections.Generic;
using System.Linq;
using EasyCraft.DataManagement.Abstraction;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EasyCraft.DataManagement;

public class DataManagementDbContext<TSelf> : DbContext where TSelf : DbContext
{
    public static List<Type> EntityTypes = new();

    public DataManagementDbContext(DbContextOptions<TSelf> options) : base(options)
    {
        
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Dynamic Load Entity Method to dynamic register entity
        foreach (var entityType in EntityTypes)
        {
            modelBuilder.Entity(entityType);
        }
    }
}