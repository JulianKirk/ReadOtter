using ReadOtter.Shared.Data.Models;
using VersOne.Epub;
using VersOne.Epub.Options;

namespace ReadOtter.Shared.Data.Services
{
    public abstract class EpubServiceBase : ServiceBase
    {
        public EpubServiceBase(IUnitOfWork unitOfWork) :base(unitOfWork) 
        {
        }

        readonly static EpubReaderOptions defaultReaderOptions = new EpubReaderOptions();

        protected EpubBookRef GetEpubBookRef(int id, EpubReaderOptions? readerOptions = null)
        {
            var book = _unitOfWork.BookRepository.GetBookById(id);

            return GetEpubBookRef(book, readerOptions);
        }

        protected EpubBookRef GetEpubBookRef(Book book, EpubReaderOptions? readerOptions = null)
        {
			var options = readerOptions ?? defaultReaderOptions;

			return EpubReader.OpenBook(book.FilePath, options);
		}


		protected EpubBook GetEpubBook(int id, EpubReaderOptions? readerOptions = null)
        {
            var book = _unitOfWork.BookRepository.GetBookById(id);

			return GetEpubBook(book, readerOptions);
		}

		protected EpubBook GetEpubBook(Book book, EpubReaderOptions? readerOptions = null)
		{
			var options = readerOptions ?? defaultReaderOptions;

			return EpubReader.ReadBook(book.FilePath, options);
		}
	}
}
