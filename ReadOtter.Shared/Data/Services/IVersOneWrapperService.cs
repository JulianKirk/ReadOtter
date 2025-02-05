using ReadOtter.Shared.Data.Models;
using VersOne.Epub;
using VersOne.Epub.Options;

namespace ReadOtter.Shared.Data.Services
{
	public interface IVersOneWrapperService
	{
		EpubBook GetEpubBook(Book book, EpubReaderOptions? readerOptions = null);
		EpubBookRef GetEpubBookRef(Book book, EpubReaderOptions? readerOptions = null);
	}
}