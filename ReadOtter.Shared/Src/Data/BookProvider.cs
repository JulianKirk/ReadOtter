using LazyCache;
using Microsoft.Extensions.Options;
using ReadOtter.Shared.Src.Data.Database;
using ReadOtter.Shared.Src.Data.Epub;
using ReadOtter.Shared.Src.Data.Models;
using ReadOtter.Shared.Src.Settings;

namespace ReadOtter.Shared.Src.Data;

public class BookProvider : IBookProvider
{
    private readonly IUnitOfWork unitOfWork;
    private readonly IVersOneAdaptor versOneAdaptor;
    private readonly IAppCache cache;
    private readonly IOptionsMonitor<AppSettings> appSettings;
    private readonly Dictionary<Guid, HashSet<string>> chapterKeyTracker = new();

    public BookProvider(
        IUnitOfWork unitOfWork,
        IVersOneAdaptor versOneAdaptor,
        IAppCache cache,
        IOptionsMonitor<AppSettings> appSettings)
    {
        this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        this.versOneAdaptor =
            versOneAdaptor ?? throw new ArgumentNullException(nameof(versOneAdaptor));
        this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
        this.appSettings =
            appSettings ?? throw new ArgumentNullException(nameof(appSettings));
    }

    private TimeSpan Expiry => appSettings.CurrentValue.BookCacheExpiry;

    public IEnumerable<Book> GetAllBooks()
    {
        var books = unitOfWork.BookRepository.GetAllBooks();

        var bookList = books.ToList();
        foreach (var book in bookList)
        {
            cache.GetOrAdd($"book_{book.Id}", () => book, Expiry);
        }

        return bookList;
    }

    public Book GetEmptyOrIncompleteBook(Guid bookId)
    {
        return cache.GetOrAdd(
            $"book_{bookId}",
            () =>
                unitOfWork.BookRepository.GetBookById(bookId)
                ?? throw new InvalidOperationException(
                    $"Book with ID {bookId} not found."),
            Expiry);
    }

    public Book GetFullBook(Guid bookId)
    {
        var book = GetEmptyOrIncompleteBook(bookId);
        book.MetaData ??= GetMetadata(bookId);
        book.Content ??= GetContent(bookId);

        return book;
    }

    public BookMetaData GetMetadata(Guid bookId)
    {
        return cache.GetOrAdd(
            $"metadata_{bookId}",
            () =>
            {
                var book = GetEmptyOrIncompleteBook(bookId);
                return versOneAdaptor.GetMetaData(book);
            },
            Expiry);
    }

    public BookContent GetContent(Guid bookId)
    {
        return cache.GetOrAdd(
            $"content_{bookId}",
            () =>
            {
                var book = GetEmptyOrIncompleteBook(bookId);
                return versOneAdaptor.GetContent(book);
            },
            Expiry);
    }

    public ContentChapter GetChapter(Guid bookId, string chapterTitle)
    {
        var key = $"chapter_{bookId}_{chapterTitle}";
        TrackChapterKey(bookId, key);

        return cache.GetOrAdd(
            key,
            () =>
            {
                var book = GetEmptyOrIncompleteBook(bookId);
                return versOneAdaptor.GetChapterContent(book, chapterTitle);
            },
            Expiry);
    }

    public ContentChapter GetChapter(Guid bookId, int chapterIndex)
    {
        var key = $"chapter_{bookId}_{chapterIndex}";
        TrackChapterKey(bookId, key);

        return cache.GetOrAdd(
            key,
            () =>
            {
                var book = GetEmptyOrIncompleteBook(bookId);
                return versOneAdaptor.GetChapterContent(book, chapterIndex);
            },
            Expiry);
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

        cache.Add($"book_{book.Id}", book, Expiry);

        return book;
    }

    public void RemoveBook(Guid bookId)
    {
        unitOfWork.BookRepository.RemoveBookById(bookId);
        unitOfWork.Commit();

        cache.Remove($"book_{bookId}");
        cache.Remove($"metadata_{bookId}");
        cache.Remove($"content_{bookId}");

        if (chapterKeyTracker.Remove(bookId, out var chapterKeys))
        {
            foreach (var key in chapterKeys)
            {
                cache.Remove(key);
            }
        }
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

    private void TrackChapterKey(Guid bookId, string cacheKey)
    {
        if (!chapterKeyTracker.TryGetValue(bookId, out var keys))
        {
            keys = new HashSet<string>();
            chapterKeyTracker[bookId] = keys;
        }

        keys.Add(cacheKey);
    }
}
