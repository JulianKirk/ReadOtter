using ReadOtter.Shared.Data.Models;

namespace ReadOtter.Shared.Data.Services.BlazorConsumed
{
	public class BookCommonService : ServiceBase
	{
		public BookCommonService(IUnitOfWork unitOfWork) : base(unitOfWork)
		{
		}

		public IEnumerable<Book> GetAllBooks()
		{
			return _unitOfWork.BookRepository.GetAllBooks();
		}

		public IEnumerable<int> GetAllBookIds()
		{
			return _unitOfWork.BookRepository.GetAllBooks().Select(b => b.Id);
		}
	}
}
