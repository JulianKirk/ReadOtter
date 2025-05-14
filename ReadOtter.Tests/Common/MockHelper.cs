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
        //I think this should be kept here for the sake of possible future removal features and integration tests - even though it is not immediately useful
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
            mockBookRepository.Setup(r => r.RemoveBookById(It.IsAny<Guid>()))
                .Callback<Guid>(id =>
                {
                    var bookToRemove = books.First(b => b.Id == id);
                    books.Remove(bookToRemove);
                });
            mockBookRepository.Setup(r => r.GetBookById(It.IsAny<Guid>()))
                .Returns<Guid>(id =>
                {
                    return books.FirstOrDefault(b => b.Id == id);
                });

            mockUnitOfWork.Setup(u => u.BookRepository).Returns(mockBookRepository.Object);
        }

        public static Mock<IVersOneAdaptor> MockVersOneWrapperService()
        {
            //This isn't useful most functionality that needs these tests need fields under Vers One data types
            throw new NotImplementedException();
        }
    }
}
