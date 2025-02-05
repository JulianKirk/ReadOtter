using Moq;
using ReadOtter.Shared.Data;
using ReadOtter.Shared.Data.Models;
using ReadOtter.Shared.Data.Repositories;
using ReadOtter.Shared.Data.Services;

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

			var unitOfWork = MockUnitOfWork(books);
			var commonService =  new BookCommonService(unitOfWork);

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

			var unitOfWork = MockUnitOfWork(books);
			var commonService = new BookCommonService(unitOfWork);

			//Act
			var bookList = commonService.GetAllBookIds();

			//Assert
			Assert.That(bookList, Is.EqualTo(new List<int> { 1, 90, 32, 41, 5}));
		}

		IUnitOfWork MockUnitOfWork(List<Book> books)
		{
			var mockBookRepository = new Mock<IBookRepository>();
			mockBookRepository.Setup(r => r.GetAllBooks()).Returns(books);
			mockBookRepository.Setup(r => r.RemoveBookById(It.IsAny<int>()))
				.Callback<int>(id =>
				{
					var bookToRemove = books.FirstOrDefault(b => b.Id == id);
					books.Remove(bookToRemove);
				});

			var mockUnitOfWork = new Mock<IUnitOfWork>();

			mockUnitOfWork.Setup(u => u.BookRepository).Returns(mockBookRepository.Object);

			return mockUnitOfWork.Object;
		}
	}
}
