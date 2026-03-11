namespace API.Infrastructure.Database;

using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using SharedLibrary.Domain.Entities;

public class ApiDbContext : DbContext
    {

        public ApiDbContext(DbContextOptions<ApiDbContext> options)
            : base(options) { }


        public DbSet<User> Users => Set<User>();
        public DbSet<Movie> Movies => Set<Movie>();
        public DbSet<Photo> Photos => Set<Photo>();
        // Pricing related entities
        public DbSet<TicketType> TicketTypes => Set<TicketType>();
        public DbSet<PricingConfig> PricingConfigs => Set<PricingConfig>();
        public DbSet<PricingOption> PricingOptions => Set<PricingOption>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().ToTable("users");
            modelBuilder.Entity<Movie>().ToTable("movies");
            modelBuilder.Entity<Photo>().ToTable("photos");
                // Pricing related entities
            modelBuilder.Entity<TicketType>()
                .ToTable("ticket_types")
                .Property(p => p.Discount)
                .HasPrecision(10, 2);
            modelBuilder.Entity<PricingConfig>()
                .ToTable("pricing_configs")
                .Property(p => p.Value)
                .HasPrecision(10, 2);
            modelBuilder.Entity<PricingOption>()
                .ToTable("pricing_options")
                .Property(p => p.PriceModifier)
                .HasPrecision(10, 2);
        }
    }
