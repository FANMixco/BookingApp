using BookingApp.Controllers;
using System;
using Xunit;

namespace BookingApp.Test
{
    public class BookControllerTest
    {
        readonly BookController _controller;

        public BookControllerTest()
        {
            _controller = new BookController(null);
        }

        [Fact]
        public void InsertBookEmptyName()
        {
            Assert.False(_controller.Insert("", "J.K. Rowling", "1995", "5"));
        }

        [Fact]
        public void InsertBookEmptyAuthor()
        {
            Assert.False(_controller.Insert($"HP {DateTime.Now.Ticks}", "", "1995", "5"));
        }

        [Fact]
        public void InsertBookEmptyYear()
        {
            Assert.True(_controller.Insert($"HP {DateTime.Now.Ticks}", "J.K. Rowling", "", "5"));
        }

        [Fact]
        public void InsertBookEmptyCopies()
        {
            Assert.False(_controller.Insert($"HP {DateTime.Now.Ticks}", "J.K. Rowling", "1995", ""));
        }

        [Fact]
        public void InsertBookFullBook()
        {
            Assert.True(_controller.Insert($"HP {DateTime.Now.Ticks}", "J.K. Rowling", "1995", "5"));
        }
    }
}
