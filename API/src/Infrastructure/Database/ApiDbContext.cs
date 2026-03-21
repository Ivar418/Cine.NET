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
        public DbSet<Genre> Genres => Set<Genre>();
        public DbSet<Ticket> Tickets => Set<Ticket>();
        public DbSet<Auditorium> Auditoriums => Set<Auditorium>();
        public DbSet<Showing> Showings => Set<Showing>();
        public DbSet<Reservation> Reservations => Set<Reservation>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderTicket> OrderTickets => Set<OrderTicket>();
        public DbSet<PaymentMethod> PaymentMethods => Set<PaymentMethod>();
        
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
            modelBuilder.Entity<Ticket>().ToTable("tickets");
            modelBuilder.Entity<Auditorium>().ToTable("auditoriums");
            modelBuilder.Entity<Showing>().ToTable("showings");
            modelBuilder.Entity<Reservation>().ToTable("reservations");
            modelBuilder.Entity<Order>().ToTable("orders");
            modelBuilder.Entity<Order>()
                .Property(o => o.TotalAmount)
                .HasPrecision(10, 2);
            modelBuilder.Entity<Order>()
                .HasOne(o => o.PaymentMethodNavigation)
                .WithMany(pm => pm.Orders)
                .HasForeignKey(o => o.PaymentMethod)
                .HasPrincipalKey(pm => pm.Code)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OrderTicket>().ToTable("order_tickets");
            modelBuilder.Entity<OrderTicket>()
                .HasKey(ot => new { ot.OrderId, ot.TicketId });
            modelBuilder.Entity<OrderTicket>()
                .HasOne(ot => ot.Order)
                .WithMany(o => o.OrderTickets)
                .HasForeignKey(ot => ot.OrderId);
            modelBuilder.Entity<OrderTicket>()
                .HasOne(ot => ot.Ticket)
                .WithMany()
                .HasForeignKey(ot => ot.TicketId);

                modelBuilder.Entity<PaymentMethod>().ToTable("payment_methods");
                modelBuilder.Entity<PaymentMethod>()
                    .HasKey(pm => pm.Code);
                modelBuilder.Entity<PaymentMethod>()
                    .Property(pm => pm.Code)
                    .HasMaxLength(20);
                modelBuilder.Entity<PaymentMethod>()
                    .Property(pm => pm.DisplayName)
                    .HasMaxLength(50);

                // Pricing related entities
                modelBuilder.Entity<TicketType>().ToTable("ticket_types");
                modelBuilder.Entity<TicketType>()
                    .Property(p => p.Discount)
                    .HasPrecision(10, 2);
                modelBuilder.Entity<PricingConfig>().ToTable("pricing_configs");
                modelBuilder.Entity<PricingConfig>()
                    .Property(p => p.Value)
                    .HasPrecision(10, 2);
                modelBuilder.Entity<PricingOption>().ToTable("pricing_options");
                modelBuilder.Entity<PricingOption>()
                    .Property(p => p.PriceModifier)
                    .HasPrecision(10, 2);
                modelBuilder.Entity<Genre>().ToTable("genres");

        }
    }
