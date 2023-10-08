using BookingApp.Controllers;
using System;
using System.Collections.Generic;
using Xunit;

namespace BookingApp.Test
{
    public class BookControllerTest
    {
        readonly BookController _controller;

        public BookControllerTest()
        {
            _controller = new BookController();
        }

        [Fact]
        public void InsertBookEmptyName()
        {
            Assert.False(_controller.Insert("", "J.K. Rowling", "1995", new List<string>() { new Random().ToString() }));
        }

        [Fact]
        public void InsertBookEmptyAuthor()
        {
            Assert.False(_controller.Insert($"HP {DateTime.Now.Ticks}", "", "1995", new List<string>() { new Random().ToString() }));
        }

        [Fact]
        public void InsertBookEmptyYear()
        {
            Assert.True(_controller.Insert($"HP {DateTime.Now.Ticks}", "J.K. Rowling", "", new List<string>() { new Random().ToString() }));
        }

        [Fact]
        public void InsertBookEmptyCopies()
        {
            Assert.False(_controller.Insert($"HP {DateTime.Now.Ticks}", "J.K. Rowling", "1995", new List<string>() { new Random().ToString() }));
        }

        [Fact]
        public void InsertBookFullBook()
        {
            Assert.True(_controller.Insert($"HP {DateTime.Now.Ticks}", "J.K. Rowling", "1995", new List<string>() { new Random().ToString() }));
        }
    }
}
