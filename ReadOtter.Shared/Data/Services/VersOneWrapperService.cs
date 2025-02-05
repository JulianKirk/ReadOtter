using ReadOtter.Shared.Data.Models;
using VersOne.Epub;
using VersOne.Epub.Options;

namespace ReadOtter.Shared.Data.Services
{
	public class VersOneWrapperService : IVersOneWrapperService
	{
		readonly static EpubReaderOptions defaultReaderOptions = new EpubReaderOptions();

		public EpubBookRef GetEpubBookRef(Book book, EpubReaderOptions? readerOptions = null)
		{
			var options = readerOptions ?? defaultReaderOptions;

			return EpubReader.OpenBook(book.FilePath, options);
		}

		public EpubBook GetEpubBook(Book book, EpubReaderOptions? readerOptions = null)
		{
			var options = readerOptions ?? defaultReaderOptions;

			return EpubReader.ReadBook(book.FilePath, options);
		}
	}
}
