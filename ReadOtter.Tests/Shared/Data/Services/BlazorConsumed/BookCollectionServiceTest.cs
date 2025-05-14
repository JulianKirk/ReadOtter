using Moq;
using ReadOtter.Shared.Data.Models;
using ReadOtter.Shared.Data.Services;
using ReadOtter.Tests.Common;

namespace ReadOtter.Tests.Shared.Data.Services
{
    public class BookCollectionServiceTest
    {
        [Test]
        public void TestGetAllBooks()
        {
            //Arrange
            var books = Enumerable.Range(1, 5)
                .Select(i => new Book { Id = Guid.NewGuid(), Title = $"Book {i}", FilePath = $"Filepath {i}" })
                .ToList();

            var mockBookProvider = new Mock<IBookProvider>();
            mockBookProvider.Setup(b => b.GetAllBooks()).Returns(books);

            var commonService = new BookCollectionService(mockBookProvider.Object);

            //Act
            var bookList = commonService.GetAllBooks();

            //Assert
            Assert.That(bookList, Is.EqualTo(books));
        }

        [Test]
        public void TestGetAllBookIds()
        {
            //Arrange
            var bookIds = Enumerable.Range(1, 5)
                .Select(i => Guid.NewGuid())
                .ToList();

            var books = bookIds.Select(id => new Book { Id = id, Title = $"Book {id}", FilePath = $"Filepath {id}" });

            var mockBookProvider = new Mock<IBookProvider>();
            mockBookProvider.Setup(b => b.GetAllBooks()).Returns(books);

            var commonService = new BookCollectionService(mockBookProvider.Object);

            //Act
            var bookList = commonService.GetAllBookIds();

            //Assert
            Assert.That(bookList, Is.EqualTo(bookIds));
        }
    }
}
