using ReadOtter.Shared.Data.Models;

namespace ReadOtter.Shared.Data.Services
{
	public class BookCommonService : ServiceBase
	{
		public BookCommonService(IUnitOfWork unitOfWork) : base(unitOfWork)
		{
		}

		public List<Book> GetAllBooks()
		{
			return _unitOfWork.BookRepository.GetAllBooks().ToList();
		}

		public List<int> GetAllBookIds()
		{
			return _unitOfWork.BookRepository.GetAllBooks().Select(b => b.Id).ToList();
		}
	}
}
