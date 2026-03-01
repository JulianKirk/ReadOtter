using LazyCache;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ReadOtter.Shared.Src.Data.Models;
using ReadOtter.Shared.Src.Settings;
using VersOne.Epub;
using VersOne.Epub.Options;

namespace ReadOtter.Shared.Src.Data.Epub;

public partial class VersOneAdaptor : IVersOneAdaptor
{
    private readonly ILogger<VersOneAdaptor> logger;
    private readonly IAppCache cache;
    private readonly IOptionsMonitor<AppSettings> appSettings;

    private static readonly EpubReaderOptions DefaultReaderOptions = new EpubReaderOptions();

    public VersOneAdaptor(
        ILogger<VersOneAdaptor> logger,
        IAppCache cache,
        IOptionsMonitor<AppSettings> appSettings)
    {
        this.logger = logger;
        this.cache = cache;
        this.appSettings = appSettings;
    }

    public BookMetaData GetMetaData(Book book)
    {
        var epubBookRef = GetEpubBookRef(book);
        var meta = epubBookRef.Schema.Package.Metadata;
        return new BookMetaData
        {
            Descriptions = meta.Descriptions?.Select(d => d.Description),
            Creators = meta.Creators?.Select(c => c.Creator),
            Publishers = meta.Publishers?.Select(p => p.Publisher),
            Contributors = meta.Contributors?.Select(c => c.Contributor),
        };
    }

    public ContentChapter GetChapterContent(Book book, string title)
    {
        var epubBookRef = GetEpubBookRef(book);
        var contentFile = epubBookRef.GetReadingOrder().First(c => c.FilePath.Contains(title));

        var imageLookup = BuildImageLookup(epubBookRef, book.FilePath);

        var html = ResolveImagesInHtml(
            contentFile.ReadContent(),
            contentFile.FilePath,
            imageLookup);

        return new ContentChapter(html, title: title, key: contentFile.Key);
    }

    public ContentChapter GetChapterContent(Book book, int index)
    {
        var epubBookRef = GetEpubBookRef(book);
        var contentFile = epubBookRef.GetReadingOrder()[index];

        var imageLookup = BuildImageLookup(epubBookRef, book.FilePath);

        var html = ResolveImagesInHtml(
            contentFile.ReadContent(),
            contentFile.FilePath,
            imageLookup);

        return new ContentChapter(html, index: index, key: contentFile.Key);
    }

    public BookContent GetContent(Book book)
    {
        var epubBook = GetEpubBook(book);
        var imageLookup = BuildImageLookup(epubBook, book.FilePath);
        var chapters = new List<ContentChapter>();

        var chapterIndex = 0;
        foreach (var chapter in epubBook.Content.Html.Local)
        {
            var html = ResolveImagesInHtml(chapter.Content, chapter.FilePath, imageLookup);
            chapters.Add(new ContentChapter(html, key: chapter.Key, index: chapterIndex));
        }

        return new BookContent(chapters);
    }

    public int GetTotalChapterCount(Book book)
    {
        var epubBook = GetEpubBookRef(book);
        return epubBook.GetReadingOrder().Count;
    }

    public byte[]? GetCoverImage(Book book)
    {
        return GetEpubBookRef(book).ReadCover();
    }

    public int? ResolveChapterIndex(Book book, string href)
    {
        var fragmentIndex = href.IndexOf('#');
        var filePart = fragmentIndex >= 0 ? href[..fragmentIndex] : href;

        if (string.IsNullOrEmpty(filePart))
        {
            logger.LogWarning("File part of href is empty.");

            return null;
        }

        var normalized = NormalizePath(filePart);
        var readingOrder = GetEpubBookRef(book).GetReadingOrder();

        for (int i = 0; i < readingOrder.Count; i++)
        {
            var chapterPath = NormalizePath(readingOrder[i].FilePath);
            if (chapterPath.Equals(normalized, StringComparison.OrdinalIgnoreCase)
                || chapterPath.EndsWith(normalized, StringComparison.OrdinalIgnoreCase))
            {
                return i;
            }
        }

        return null;
    }

    public string GetTitle(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"EPUB file not found at '{filePath}'.", filePath);
        }

        using var epubBookRef = EpubReader.OpenBook(filePath);
        return epubBookRef.Title;
    }

    EpubBookRef GetEpubBookRef(Book book, EpubReaderOptions? readerOptions = null)
    {
        ValidateFileExists(book);
        var options = readerOptions ?? DefaultReaderOptions;

        return EpubReader.OpenBook(book.FilePath, options);
    }

    EpubBook GetEpubBook(Book book, EpubReaderOptions? readerOptions = null)
    {
        ValidateFileExists(book);
        var options = readerOptions ?? DefaultReaderOptions;

        return EpubReader.ReadBook(book.FilePath, options);
    }

    Dictionary<string, (string MimeType, byte[] Bytes)> BuildImageLookup(
        EpubBookRef epubBookRef,
        string filePath)
    {
        return cache.GetOrAdd(
            $"imagelookup_{filePath}",
            () =>
            {
                var lookup = new Dictionary<string, (string, byte[])>(
                    StringComparer.OrdinalIgnoreCase);
                foreach (var image in epubBookRef.Content.Images.Local)
                {
                    var normalizedPath = NormalizePath(image.FilePath);
                    lookup[normalizedPath] = (image.ContentMimeType, image.ReadContent());
                }

                return lookup;
            },
            appSettings.CurrentValue.CacheExpiry);
    }

    Dictionary<string, (string MimeType, byte[] Bytes)> BuildImageLookup(
        EpubBook epubBook,
        string filePath)
    {
        return cache.GetOrAdd(
            $"imagelookup_{filePath}",
            () =>
            {
                var lookup = new Dictionary<string, (string, byte[])>(
                    StringComparer.OrdinalIgnoreCase);
                foreach (var image in epubBook.Content.Images.Local)
                {
                    var normalizedPath = NormalizePath(image.FilePath);
                    lookup[normalizedPath] = (image.ContentMimeType, image.Content);
                }

                return lookup;
            },
            appSettings.CurrentValue.CacheExpiry);
    }
}
