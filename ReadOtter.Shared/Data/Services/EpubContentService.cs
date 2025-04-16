using ReadOtter.Shared.Data.Models;

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
            OffsetBookCurrentChapter(book, offset);
        }

        public void OffsetBookCurrentChapter(Book book, int offset)
        {
            var epubBookRef = versOneWrapperService.GetEpubBookRef(book);
            var newChapterNum = book.CurrentChapter + offset;

            if (newChapterNum < 0 || newChapterNum >= epubBookRef.GetReadingOrder().Count())
            {
                return;
            }

            book.CurrentChapter = newChapterNum;
        }

        public string GetCurrentChapterTextContent(int id)
		{
			var book = _unitOfWork.BookRepository.GetBookById(id);
			return GetCurrentChapterTextContent(book);
        }

        public string GetCurrentChapterTextContent(Book book)
        {
            var epubBook = versOneWrapperService.GetEpubBookRef(book);
            return epubBook.GetReadingOrder()[book.CurrentChapter].ReadContent();
        }
    }
}
