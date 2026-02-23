using ReadOtter.Shared.Src.Data.Database;
using ReadOtter.Shared.Src.Data.Epub;
using ReadOtter.Shared.Src.Data.Models;

namespace ReadOtter.Shared.Src.Data;

public class CachedBookProvider : IBookProvider
{
    readonly IUnitOfWork unitOfWork;
    readonly IVersOneAdaptor versOneAdaptor;

    readonly Dictionary<Guid, BookMetaData> metaDataCache = new();
    readonly Dictionary<Guid, BookContent> contentCache = new();
    readonly Dictionary<Guid, List<ContentChapter>> chapterCache = new();
    Dictionary<Guid, Book> bookCache = new();

    public CachedBookProvider(IUnitOfWork unitOfWork, IVersOneAdaptor versOneAdaptor)
    {
        this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        this.versOneAdaptor =
            versOneAdaptor ?? throw new ArgumentNullException(nameof(versOneAdaptor));
    }

    public IEnumerable<Book> GetAllBooks()
    {
        var books = unitOfWork.BookRepository.GetAllBooks();

        bookCache = books.ToDictionary(b => b.Id, b => b);

        return books;
    }

    public Book GetEmptyOrIncompleteBook(Guid bookId)
    {
        if (bookCache.TryGetValue(bookId, out var book))
        {
            return book;
        }

        book =
            unitOfWork.BookRepository.GetBookById(bookId)
            ?? throw new InvalidOperationException($"Book with ID {bookId} not found.");

        bookCache.Add(bookId, book);

        return book;
    }

    public Book GetFullBook(Guid bookId)
    {
        if (bookCache.TryGetValue(bookId, out var book))
        {
            book.MetaData ??= GetMetadata(bookId);
            book.Content ??= GetContent(bookId);
            return book;
        }

        book = GetEmptyOrIncompleteBook(bookId);
        book.MetaData = GetMetadata(bookId);
        book.Content = GetContent(bookId);

        bookCache[bookId] = book;

        return book;
    }

    public BookMetaData GetMetadata(Guid bookId)
    {
        if (metaDataCache.TryGetValue(bookId, out var metaData))
        {
            return metaData;
        }

        var book = GetEmptyOrIncompleteBook(bookId);
        metaData = versOneAdaptor.GetMetaData(book);

        metaDataCache.Add(bookId, metaData);

        return metaData;
    }

    public BookContent GetContent(Guid bookId)
    {
        if (contentCache.TryGetValue(bookId, out var content))
        {
            return content;
        }

        var book = GetEmptyOrIncompleteBook(bookId);
        content = versOneAdaptor.GetContent(book);

        contentCache.Add(book.Id, content);

        return content;
    }

    public ContentChapter GetChapter(Guid bookId, string chapterTitle)
    {
        var chapterExists = chapterCache.TryGetValue(bookId, out var chapters);

        if (chapters == null)
        {
            chapters = new List<ContentChapter>();
            chapterCache.Add(bookId, chapters);
        }

        var chapter = chapters.FirstOrDefault(c =>
            c.Title != null && c.Title.Equals(chapterTitle, StringComparison.OrdinalIgnoreCase));

        if (chapter == null)
        {
            var book = GetEmptyOrIncompleteBook(bookId);
            chapter = versOneAdaptor.GetChapterContent(book, chapterTitle);
            chapters.Add(chapter);
        }

        return chapter;
    }

    public ContentChapter GetChapter(Guid bookId, int chapterIndex)
    {
        var chapterExists = chapterCache.TryGetValue(bookId, out var chapters);

        if (chapters == null)
        {
            chapters = new List<ContentChapter>();
            chapterCache.Add(bookId, chapters);
        }

        var chapter = chapters.FirstOrDefault(c => c.Index == chapterIndex);

        if (chapter == null)
        {
            var book = GetEmptyOrIncompleteBook(bookId);
            chapter = versOneAdaptor.GetChapterContent(book, chapterIndex);
            chapters.Add(chapter);
        }

        return chapter;
    }

    public int GetChapterCount(Guid bookId)
    {
        var book = GetEmptyOrIncompleteBook(bookId);
        return versOneAdaptor.GetTotalChapterCount(book);
    }

    public string GetCoverImage(Guid bookId)
    {
        var book = GetEmptyOrIncompleteBook(bookId);
        var coverImageBytes = versOneAdaptor.GetCoverImage(book);

        if (coverImageBytes != null)
        {
            return $"data:image/jpeg;base64,{Convert.ToBase64String(coverImageBytes)}";
        }

        return string.Empty;
    }

    public Book AddBook(byte[] epubData, string fileName)
    {
        var booksDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "ReadOtter",
            "Books");

        Directory.CreateDirectory(booksDir);

        var filePath = Path.Combine(booksDir, fileName);

        if (File.Exists(filePath))
        {
            var name = Path.GetFileNameWithoutExtension(fileName);
            var ext = Path.GetExtension(fileName);
            filePath = Path.Combine(booksDir, $"{name}_{Guid.NewGuid():N}{ext}");
        }

        File.WriteAllBytes(filePath, epubData);

        var title = versOneAdaptor.GetTitle(filePath);

        var book = new Book
        {
            Id = Guid.NewGuid(),
            Title = title,
            FilePath = filePath,
            CurrentChapter = 0,
            CurrentChapterPage = 0,
        };

        unitOfWork.BookRepository.AddBook(book);
        unitOfWork.Commit();

        bookCache[book.Id] = book;

        return book;
    }

    public void RemoveBook(Guid bookId)
    {
        unitOfWork.BookRepository.RemoveBookById(bookId);
        unitOfWork.Commit();

        bookCache.Remove(bookId);
        metaDataCache.Remove(bookId);
        contentCache.Remove(bookId);
        chapterCache.Remove(bookId);
    }

    public void SaveBookProgress(Guid bookId)
    {
        unitOfWork.Commit();
    }

    public int? ResolveChapterIndex(Guid bookId, string href)
    {
        var book = GetEmptyOrIncompleteBook(bookId);
        return versOneAdaptor.ResolveChapterIndex(book, href);
    }
}
