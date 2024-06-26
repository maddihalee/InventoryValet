﻿namespace InventoryValet.Data;
using InventoryValet.Models;
using Microsoft.EntityFrameworkCore;

public class InventoryVDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Item> Items { get; set; }
    public DbSet<Category> Category { get; set; }

    public InventoryVDbContext(DbContextOptions<InventoryVDbContext> context) : base(context) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Fluent API configuration for Item-Category relationship
        modelBuilder.Entity<Item>()
            .HasOne(i => i.Category)
            .WithMany(c => c.Items)
            .HasForeignKey(i => i.CategoryId);

        modelBuilder.Entity<User>().HasData(new User[]
        {
            new User { Id = 1, Name = "Maddi", Email = "maddi@email.com", FirebaseId = "abc123" }
        });
        modelBuilder.Entity<Item>().HasData(new Item[]
        {
            new Item { Id = 1, Name = "T-Shirt", Description = "A plain t-shirt", Image = "abc123", Price = 25, CategoryId = 1, Size="L"}
        });
        modelBuilder.Entity<Category>().HasData(new Category[]
        {
            new Category { Id = 1, Name = "Men" },
            new Category { Id = 2, Name = "Women"},
            new Category { Id = 3, Name = "Kids"}
        });
    }
}

