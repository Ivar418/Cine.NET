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
        public DbSet<Ticket> Tickets => Set<Ticket>();
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().ToTable("users");
            modelBuilder.Entity<Movie>().ToTable("movies");
            modelBuilder.Entity<Photo>().ToTable("photos");
            modelBuilder.Entity<Ticket>().ToTable("tickets");
        }
    }
