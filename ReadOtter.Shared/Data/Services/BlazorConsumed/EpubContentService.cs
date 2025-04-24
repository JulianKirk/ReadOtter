using ReadOtter.Shared.Data.Models;
using System.Collections.Concurrent;

namespace ReadOtter.Shared.Data.Services
{
	public class EpubContentService
	{
		private readonly IBookProvider bookProvider;

        public EpubContentService(IBookProvider bookProvider)
		{
			this.bookProvider = bookProvider ?? throw new ArgumentNullException(nameof(bookProvider));
        }

		public bool OffsetBookCurrentChapter(Guid id, int offset)
		{
			var book = bookProvider.GetEmptyOrIncompleteBook(id);
            return OffsetBookCurrentChapter(book, offset);
        }

        public bool OffsetBookCurrentChapter(Book book, int offset)
        {
            var totalChapterCount = bookProvider.GetChapterCount(book.Id);
            var newChapterNum = book.CurrentChapter + offset;

            if (newChapterNum < 0 || newChapterNum >= totalChapterCount)
            {
                return false;
            }

            book.CurrentChapter = newChapterNum;

            return true;
        }

        public string GetCurrentChapterTextContent(Guid id)
		{
            var book = bookProvider.GetEmptyOrIncompleteBook(id);
            return GetCurrentChapterTextContent(book);
        }

        public string GetCurrentChapterTextContent(Book book)
        {
            return bookProvider.GetChapter(book.Id, book.CurrentChapter).Content;
        }
    }
}
