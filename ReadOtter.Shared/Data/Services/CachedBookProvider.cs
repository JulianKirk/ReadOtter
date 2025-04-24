using ReadOtter.Shared.Data.Models;

namespace ReadOtter.Shared.Data.Services
{
    public class CachedBookProvider : IBookProvider
    {
        readonly IUnitOfWork unitOfWork;
        readonly IVersOneAdaptor versOneAdaptor;

        Dictionary<Guid, Book> bookCache = new();
        readonly Dictionary<Guid, BookMetaData> metaDataCache = new();
        readonly Dictionary<Guid, BookContent> contentCache = new();
        readonly Dictionary<Guid, List<ContentChapter>> chapterCache = new();

        public CachedBookProvider(IUnitOfWork unitOfWork, IVersOneAdaptor versOneAdaptor)
        {
            this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            this.versOneAdaptor = versOneAdaptor ?? throw new ArgumentNullException(nameof(versOneAdaptor));
        }

        public IEnumerable<Book> GetAllBooks()
        {
            var books = unitOfWork.BookRepository.GetAllBooks();

            bookCache = books.ToDictionary(b => b.Id, b => b);

            return books;
        }

        public Book GetEmptyOrIncompleteBook(Guid id)
        {
            if (bookCache.TryGetValue(id, out var book))
            {
                return book;
            }

            book = unitOfWork.BookRepository.GetBookById(id) ?? throw new InvalidOperationException($"Book with ID {id} not found."); ;
            bookCache.Add(id, book);

            return book;
        }

        public Book GetFullBook(Guid id)
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

        public BookMetaData GetMetadata(Guid id)
        {
            if (metaDataCache.TryGetValue(id, out var metaData))
            {
                return metaData;
            }

            var book = GetEmptyOrIncompleteBook(id);

            return versOneAdaptor.GetMetaData(book);
        }

        public BookContent GetContent(Guid id)
        {
            if (contentCache.TryGetValue(id, out var content))
            {
                return content;
            }

            var book = GetEmptyOrIncompleteBook(id);

            return versOneAdaptor.GetContent(book);
        }

        public ContentChapter GetChapter(Guid id, string chapterTitle)
        {
            var chapterExists = chapterCache.TryGetValue(id, out var chapters);

            if (chapters == null)
            {
                chapters = new List<ContentChapter>();
                chapterCache.Add(id, chapters);
            }

            var chapter = chapters.FirstOrDefault(c => c.Title != null && c.Title.Equals(chapterTitle, StringComparison.OrdinalIgnoreCase));

            if (chapter == null)
            {
                var book = GetEmptyOrIncompleteBook(id);
                chapter = versOneAdaptor.GetChapterContent(book, chapterTitle);
                chapters.Add(chapter);
            }

            return chapter;
        }

        public ContentChapter GetChapter(Guid id, int chapterIndex)
        {
            var chapterExists = chapterCache.TryGetValue(id, out var chapters);

            if (chapters == null)
            {
                chapters = new List<ContentChapter>();
                chapterCache.Add(id, chapters);
            }

            var chapter = chapters.FirstOrDefault(c => c.Index == chapterIndex);

            if (chapter == null)
            {
                var book = GetEmptyOrIncompleteBook(id);
                chapter = versOneAdaptor.GetChapterContent(book, chapterIndex);
                chapters.Add(chapter);
            }

            return chapter;
        }

        public int GetChapterCount(Guid id)
        {
            var book = GetEmptyOrIncompleteBook(id);
            return versOneAdaptor.GetTotalChapterCount(book);
        }
    }
}
