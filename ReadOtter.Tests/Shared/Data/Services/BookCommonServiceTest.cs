using ReadOtter.Shared.Data.Models;
using ReadOtter.Shared.Data.Services;
using ReadOtter.Tests.Common;

namespace ReadOtter.Tests.Shared.Data.Services
{
    public class BookCommonServiceTest
    {
        [Test]
        public void TestGetAllBooks()
        {
            //Arrange
            var books = new List<Book>
            {
                new Book {Id = 1},
                new Book {Id = 2},
                new Book {Id = 3},
                new Book {Id = 4},
                new Book {Id = 5}
            };

            var unitOfWork = MockHelper.MockUnitOfWork(books: books);
            var commonService = new BookCommonService(unitOfWork.Object);

            //Act
            var bookList = commonService.GetAllBooks();

            //Assert
            Assert.That(bookList, Is.EqualTo(books));
        }

        [Test]
        public void TestGetAllBookIds()
        {
            //Arrange
            var books = new List<Book>
            {
                new Book {Id = 1},
                new Book {Id = 90},
                new Book {Id = 32},
                new Book {Id = 41},
                new Book {Id = 5}
            };

            var unitOfWork = MockHelper.MockUnitOfWork(books: books);
            var commonService = new BookCommonService(unitOfWork.Object);

            //Act
            var bookList = commonService.GetAllBookIds();

            //Assert
            Assert.That(bookList, Is.EqualTo(new List<int> { 1, 90, 32, 41, 5 }));
        }
    }
}
