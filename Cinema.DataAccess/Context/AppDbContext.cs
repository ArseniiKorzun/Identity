using Cinema.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Cinema.DataAccess.Context
{
    public class AppDbContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<Movie> Movies { get; set; }

        public DbSet<MovieComment> MovieComments { get; set; }

        public DbSet<MovieSession> MovieSessions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>(b =>
            {
                b.Property(u => u.Email).HasMaxLength(100);
                b.Property(u => u.UserName).HasMaxLength(100);
                b.HasIndex(u => u.NormalizedEmail).IsUnique();
            });
            modelBuilder.Entity<MovieComment>()
                .HasOne(c => c.Movie)
                .WithMany(m => m.Comments)
                .HasForeignKey(c => c.MovieId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<MovieSession>()
                .HasOne(s => s.Movie)
                .WithMany(m => m.Sessions)
                .HasForeignKey(s => s.MovieId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<MovieSession>()
                .Property(s => s.Price)
                .HasPrecision(10, 2);
            modelBuilder.Entity<MovieSession>()
                .HasIndex(s => s.StartsAt);
        }
    }
}
