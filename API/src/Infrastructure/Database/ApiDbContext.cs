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
        public DbSet<Hall> Halls => Set<Hall>();
        public DbSet<Showtime> Showtimes => Set<Showtime>();
        public DbSet<Reservation> Reservations => Set<Reservation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().ToTable("users");
            modelBuilder.Entity<Movie>().ToTable("movies");
            modelBuilder.Entity<Photo>().ToTable("photos");
        }
    }
