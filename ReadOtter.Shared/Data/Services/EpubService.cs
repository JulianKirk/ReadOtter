using VersOne.Epub;
using VersOne.Epub.Options;

namespace ReadOtter.Shared.Data.Services
{
    public class EpubService : ServiceBase
    {
        public EpubService(UnitOfWork unitOfWork) :base(unitOfWork) 
        {
        }

        readonly static EpubReaderOptions defaultReaderOptions = new EpubReaderOptions();

        public EpubBookRef GetEpubBookRef(int id, EpubReaderOptions? readerOptions = null)
        {
            var book = _unitOfWork.BookRepository.GetBookById(id);

            var options = readerOptions ?? defaultReaderOptions;

            return EpubReader.OpenBook(book.FilePath, options);
        }

        public EpubBook GetEpubBook(int id, EpubReaderOptions? readerOptions = null)
        {
            var book = _unitOfWork.BookRepository.GetBookById(id);

            var options = readerOptions ?? defaultReaderOptions;

            return EpubReader.ReadBook(book.FilePath, options);
        }

        public void OffsetBookCurrentChapter(int id, int offset)
        {
			var book = _unitOfWork.BookRepository.GetBookById(id);
            var epubBookRef = GetEpubBookRef(id);

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
			var epubBook = GetEpubBookRef(id);

			var book = _unitOfWork.BookRepository.GetBookById(id);

			return epubBook.GetReadingOrder()[book.CurrentChapter].ReadContent();
		}
	}
}
