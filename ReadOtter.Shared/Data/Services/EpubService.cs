using ReadOtter.Shared.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public EpubLocalTextContentFile GetCurrentChapterContent(int id)
        {
            var epubBook = GetEpubBook(id);

            var book = _unitOfWork.BookRepository.GetBookById(id);

            return epubBook.Navigation[book.CurrentChapter].HtmlContentFile;
        }
    }
}
