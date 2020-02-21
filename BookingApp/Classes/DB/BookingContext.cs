﻿using System;
using Microsoft.EntityFrameworkCore;

namespace BookingApp.Classes.DB
{
    public class BookingContext : DbContext
    {
        public DbSet<Users> Users { get; set; }
        public DbSet<Books> Books { get; set; }
        public DbSet<ReservedBook> ReservedBook { get; set; }
        public DbSet<BooksCopies> BooksCopies { get; set; }

#if UnitTest
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite($"Data Source=booking.test.db");
#else
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite($"Data Source=booking.db");
#endif

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            /*modelBuilder.Entity<Users>(entity =>
            {
                entity.HasKey(p => new { p.UserId });
                entity.Property("UserId").ValueGeneratedOnAdd();
            });*/

            modelBuilder.Entity<Users>()
              .HasKey(p => new { p.UserId });
            modelBuilder.Entity<Books>()
              .HasKey(p => new { p.BookId });
            modelBuilder.Entity<ReservedBook>()
              .HasKey(p => new { p.ReservedBookId });
            modelBuilder.Entity<BooksCopies>()
              .HasKey(p => new { p.BooksCopiesId });
        }

        public async void DefaultData()
        {
            try
            {
                using var db = new BookingContext();

                if (await db.Users.CountAsync() == 0)
                {
                    _ = db.Add(new Users() { Username = "admin", Role = 0, Email = "admin@noreply.com", Password = "admin", Registered = DateTime.Now });
                    _ = db.SaveChanges();

                    _ = db.Add(new Books() { Name = "Harry Potter 1", Author = "J.K. Rowling", PublicationYear = 1997, Total = 5, Registered = DateTime.Now });
                    _ = db.SaveChanges();
                    _ = db.Add(new Books() { Name = "Harry Potter 2", Author = "J.K. Rowling", PublicationYear = 1998, Total = 3, Registered = DateTime.Now });
                    _ = db.SaveChanges();
                    _ = db.Add(new Books() { Name = "Harry Potter 3", Author = "J.K. Rowling", PublicationYear = 1999, Total = 1, Registered = DateTime.Now });
                    _ = db.SaveChanges();
                    _ = db.Add(new Books() { Name = "Harry Potter 4", Author = "J.K. Rowling", PublicationYear = 2000, Total = 2, Registered = DateTime.Now });
                    _ = db.SaveChanges();
                    _ = db.Add(new Books() { Name = "Harry Potter 5", Author = "J.K. Rowling", PublicationYear = 2003, Total = 4, Registered = DateTime.Now });
                    _ = db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
        }
    }
}
