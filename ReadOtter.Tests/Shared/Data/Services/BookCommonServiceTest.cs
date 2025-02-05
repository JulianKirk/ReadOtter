using Moq;
using ReadOtter.Shared.Data;
using ReadOtter.Shared.Data.Repositories;

namespace ReadOtter.Tests.Shared.Data.Services
{
	public class BookCommonServiceTest
	{
		//[SetUp]
		//public void SetUp()
		//{
		//	var mockBookRepository = new Mock<IBookRepository>();
		//	mockUnitOfWork = new Mock<IUnitOfWork>();

		//	mockUnitOfWork.Setup(u => u.BookRepository).Returns();
		//}

		[Test]
		public void TestGetAllBooks()
		{
			Assert.Pass();
		}

		[Test]
		public void TestGetAllBookIds()
		{
			Assert.Pass();
		}


		Mock<IUnitOfWork> mockUnitOfWork;
	}
}
