using ReadOtter.Shared.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadOtter.Shared.Data.Services
{
    public class CachedBookProvider : IBookProvider
    {
        readonly IUnitOfWork unitOfWork;
        readonly IVersOneAdaptor versOneAdaptor;

        readonly Dictionary<int, Book> bookCache = new();
        readonly Dictionary<int, BookMetaData> metaDataCache = new();
        readonly Dictionary<int, BookContent> contentCache = new();
        readonly Dictionary<int, List<ContentChapter>> chapterCache = new();

        public CachedBookProvider(IUnitOfWork unitOfWork, IVersOneAdaptor versOneAdaptor)
        {
            this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            this.versOneAdaptor = versOneAdaptor ?? throw new ArgumentNullException(nameof(versOneAdaptor));
        }

        public Book GetEmptyOrIncompleteBook(int id)
        {
            if (bookCache.TryGetValue(id, out var book))
            {
                return book;
            }

            book = unitOfWork.BookRepository.GetBookById(id) ?? throw new InvalidOperationException($"Book with ID {id} not found."); ;
            bookCache.Add(id, book);

            return book;
        }

        public Book GetFullBook(int id)
        {
            if (bookCache.TryGetValue(id, out var book))
            {
                book.MetaData ??= GetMetadata(id);
                book.Content ??= GetContent(id);
                return book;
            }

            book = GetEmptyOrIncompleteBook(id);
            book.MetaData = GetMetadata(id);
            book.Content = GetContent(id);

            bookCache[id] = book;

            return book;
        }

        public BookMetaData GetMetadata(int id)
        {
            if (metaDataCache.TryGetValue(id, out var metaData))
            {
                return metaData;
            }

            var book = GetEmptyOrIncompleteBook(id);

            var epubRef = versOneAdaptor.GetEpubBookRef(book);
        }

        public BookContent GetContent(int id)
        {
            if (contentCache.TryGetValue(id, out var content))
                return content;
        }

        public ContentChapter GetChapter(int id, string chapterName)
        {
            if (chapterCache.TryGetValue(id, out var chapters))
            {
                var chapter = chapters.Find(c => c.Title.Equals(chapterName, StringComparison.OrdinalIgnoreCase));
                if (chapter != null)
                    return chapter;
            }

            return null;
        }
    }
}
