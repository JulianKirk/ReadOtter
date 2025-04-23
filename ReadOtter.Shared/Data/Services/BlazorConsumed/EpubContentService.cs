using ReadOtter.Shared.Data.Models;
using System.Collections.Concurrent;

namespace ReadOtter.Shared.Data.Services.BlazorConsumed
{
	public class EpubContentService: ServiceBase
	{
		private readonly IVersOneAdaptor versOneWrapperService;

        private readonly ConcurrentDictionary<(int BookId, int ChapterIndex), BookContent> chapterContentCache;

        public EpubContentService(IUnitOfWork unitOfWork, IVersOneAdaptor versOneWrapperService) : base(unitOfWork)
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
            var totalChapterCount = versOneWrapperService.GetTotalChapterCount(book);
            var newChapterNum = book.CurrentChapter + offset;

            if (newChapterNum < 0 || newChapterNum >= totalChapterCount)
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
            return versOneWrapperService.GetContentForChapter(book, book.CurrentChapter);
        }
    }
}
