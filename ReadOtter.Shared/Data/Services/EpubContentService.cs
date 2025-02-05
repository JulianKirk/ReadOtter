namespace ReadOtter.Shared.Data.Services
{
	public class EpubContentService: ServiceBase
	{
		IVersOneWrapperService versOneWrapperService;

		public EpubContentService(IUnitOfWork unitOfWork, IVersOneWrapperService versOneWrapperService) : base(unitOfWork)
		{
			this.versOneWrapperService = versOneWrapperService;
		}

		public void OffsetBookCurrentChapter(int id, int offset)
		{
			var book = _unitOfWork.BookRepository.GetBookById(id);
			var epubBookRef = versOneWrapperService.GetEpubBookRef(book);

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
			var epubBook = versOneWrapperService.GetEpubBookRef(book);

			return epubBook.GetReadingOrder()[book.CurrentChapter].ReadContent();
		}
	}
}
