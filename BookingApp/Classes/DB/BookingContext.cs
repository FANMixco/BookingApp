using System;
using Microsoft.EntityFrameworkCore;

namespace BookingApp.Classes.DB
{
    public class BookingContext : DbContext
    {
        public DbSet<Users> Users { get; set; }
        public DbSet<Books> Books { get; set; }
        public DbSet<ReservedBook> ReservedBook { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source=booking.db");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Users>()
              .HasKey(p => new { p.UserId });
            modelBuilder.Entity<Books>()
              .HasKey(p => new { p.BookId });
            modelBuilder.Entity<ReservedBook>()
              .HasKey(p => new { p.ReservedBookId });
        }

        public void DefaultData()
        {
            try
            {
                using var db = new BookingContext();
                _ = db.Add(new Users() { Username = "admin", Role = 0, Password = "admin" });
                _ = db.SaveChanges();

                _ = db.Add(new Books() { Name = "Harry Potter 1", Total = 5 });
                _ = db.SaveChanges();
                _ = db.Add(new Books() { Name = "Harry Potter 2", Total = 3 });
                _ = db.SaveChanges();
                _ = db.Add(new Books() { Name = "Harry Potter 3", Total = 1 });
                _ = db.SaveChanges();
                _ = db.Add(new Books() { Name = "Harry Potter 4", Total = 2 });
                _ = db.SaveChanges();
                _ = db.Add(new Books() { Name = "Harry Potter 5", Total = 4 });
                _ = db.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
        }
    }
}
