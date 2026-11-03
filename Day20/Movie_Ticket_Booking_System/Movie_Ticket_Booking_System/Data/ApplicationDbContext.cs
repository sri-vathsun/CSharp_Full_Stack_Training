using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.Models;

namespace MovieTicketBooking.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Movie> Movies => Set<Movie>();
    public DbSet<Theatre> Theatres => Set<Theatre>();
    public DbSet<Screen> Screens => Set<Screen>();

    public DbSet<Seat> Seats => Set<Seat>();


    public DbSet<Show> Shows => Set<Show>();

    public DbSet<ShowSeat> ShowSeats => Set<ShowSeat>();

    public DbSet<Booking> Bookings => Set<Booking>();

    public DbSet<BookingSeat> BookingSeats => Set<BookingSeat>();

    public DbSet<User> Users => Set<User>();


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Screen>()
            .HasOne(s => s.Theatre)
            .WithMany(t => t.Screens)
            .HasForeignKey(s => s.TheatreId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Screen>()
            .HasIndex(s => new { s.TheatreId, s.ScreenName })
            .IsUnique();

        modelBuilder.Entity<Seat>()
           .HasOne(s => s.Screen)
           .WithMany(sc => sc.Seats)
           .HasForeignKey(s => s.ScreenId)
           .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Seat>()
            .HasIndex(s => new
            {
                s.ScreenId,
                s.RowName,
                s.SeatNumber
            })
            .IsUnique();


        // Show relationships
        modelBuilder.Entity<Show>()
            .Property(s => s.TicketPrice)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Booking>()
            .Property(b => b.TotalAmount)
            .HasPrecision(18, 2);

        modelBuilder.Entity<BookingSeat>()
            .Property(bs => bs.Price)
            .HasPrecision(18, 2);

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<Booking>()
            .HasOne(b => b.User)
            .WithMany()
            .HasForeignKey(b => b.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Show>()
            .HasOne(s => s.Movie)
            .WithMany()
            .HasForeignKey(s => s.MovieId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Show>()
            .HasOne(s => s.Screen)
            .WithMany()
            .HasForeignKey(s => s.ScreenId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Show>()
            .ToTable(t =>
            {
                t.HasCheckConstraint(
                    "CK_Shows_EndTime",
                    "[EndTime] > [StartTime]");

                t.HasCheckConstraint(
                    "CK_Shows_TicketPrice",
                    "[TicketPrice] >= 0");
            });

        // ShowSeat relationships
        modelBuilder.Entity<ShowSeat>()
            .HasOne(ss => ss.Show)
            .WithMany(s => s.ShowSeats)
            .HasForeignKey(ss => ss.ShowId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ShowSeat>()
            .HasOne(ss => ss.Seat)
            .WithMany()
            .HasForeignKey(ss => ss.SeatId)
            .OnDelete(DeleteBehavior.Restrict);

        // One physical seat should appear only once per show
        modelBuilder.Entity<ShowSeat>()
            .HasIndex(ss => new { ss.ShowId, ss.SeatId })
            .IsUnique();

    }
}