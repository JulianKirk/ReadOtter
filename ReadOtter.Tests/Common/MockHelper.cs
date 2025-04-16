using Moq;
using ReadOtter.Shared.Data;
using ReadOtter.Shared.Data.Models;
using ReadOtter.Shared.Data.Repositories;
using ReadOtter.Shared.Data.Services;
using VersOne.Epub;

namespace ReadOtter.Tests.Common
{
    public class MockHelper
    {
        //NOTE TO SELF - for the future can add more parameters to this other than books
        public static Mock<IUnitOfWork> MockUnitOfWork(List<Book>? books = null)
        {
            var mockUnitOfWork = new Mock<IUnitOfWork>();

            if (books != null)
            {
                MockBookRepository(mockUnitOfWork, books);
            }

            return mockUnitOfWork;
        }

        static void MockBookRepository(Mock<IUnitOfWork> mockUnitOfWork, List<Book> books)
        {
            var mockBookRepository = new Mock<IBookRepository>();
            mockBookRepository.Setup(r => r.GetAllBooks()).Returns(books);
            mockBookRepository.Setup(r => r.RemoveBookById(It.IsAny<int>()))
                .Callback<int>(id =>
                {
                    var bookToRemove = books.FirstOrDefault(b => b.Id == id);
                    books.Remove(bookToRemove);
                });
            mockBookRepository.Setup(r => r.GetBookById(It.IsAny<int>()))
                .Returns<int>(id =>
                {
                    return books.FirstOrDefault(b => b.Id == id);
                });

            mockUnitOfWork.Setup(u => u.BookRepository).Returns(mockBookRepository.Object);
        }

        public static Mock<IVersOneWrapperService> MockVersOneWrapperService()
        {
            //This isn't useful most functionality that needs these tests need fields under Vers One data types
            throw new NotImplementedException();
        }
    }
}
