namespace ReadOtter.Shared.Data.Services
{
	public class EpubContentService : EpubServiceBase
	{
		public EpubContentService(IUnitOfWork unitOfWork) : base(unitOfWork)
		{
		}

		public void OffsetBookCurrentChapter(int id, int offset)
		{
			var book = _unitOfWork.BookRepository.GetBookById(id);
			var epubBookRef = GetEpubBookRef(book);

			//Validation
			var newChapterNum = book.CurrentChapter + offset;

			if (newChapterNum < 0 || newChapterNum >= epubBookRef.GetReadingOrder().Count())
			{
				return;
			}

			book.CurrentChapter += offset;
		}

		public string GetCurrentChapterTextContent(int id)
		{
			var book = _unitOfWork.BookRepository.GetBookById(id);
			var epubBook = GetEpubBookRef(book);

			return epubBook.GetReadingOrder()[book.CurrentChapter].ReadContent();
		}
	}
}
