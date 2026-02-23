using AutoMapper;
using Microsoft.Extensions.Logging;
using ReadOtter.Shared.Src.Data.Models;
using VersOne.Epub;
using VersOne.Epub.Options;

namespace ReadOtter.Shared.Src.Data.Epub;

public partial class VersOneAdaptor : IVersOneAdaptor
{
    private readonly IMapper mapper;
    private readonly ILogger<VersOneAdaptor> logger;
    private readonly Dictionary<string, Dictionary<string, (string MimeType, byte[] Bytes)>> imageLookupCache = new();
    static readonly EpubReaderOptions DefaultReaderOptions = new EpubReaderOptions();

    public VersOneAdaptor(IMapper mapper, ILogger<VersOneAdaptor> logger)
    {
        this.mapper = mapper;
        this.logger = logger;
    }

    public BookMetaData GetMetaData(Book book)
    {
        var epubBookRef = GetEpubBookRef(book);
        return mapper.Map<BookMetaData>(epubBookRef.Schema.Package.Metadata);
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
        if (imageLookupCache.TryGetValue(filePath, out var cached))
        {
            return cached;
        }

        var lookup = new Dictionary<string, (string, byte[])>(StringComparer.OrdinalIgnoreCase);
        foreach (var image in epubBookRef.Content.Images.Local)
        {
            var normalizedPath = NormalizePath(image.FilePath);
            lookup[normalizedPath] = (image.ContentMimeType, image.ReadContent());
        }

        imageLookupCache[filePath] = lookup;
        return lookup;
    }

    Dictionary<string, (string MimeType, byte[] Bytes)> BuildImageLookup(
        EpubBook epubBook,
        string filePath)
    {
        if (imageLookupCache.TryGetValue(filePath, out var cached))
        {
            return cached;
        }

        var lookup = new Dictionary<string, (string, byte[])>(StringComparer.OrdinalIgnoreCase);
        foreach (var image in epubBook.Content.Images.Local)
        {
            var normalizedPath = NormalizePath(image.FilePath);
            lookup[normalizedPath] = (image.ContentMimeType, image.Content);
        }

        imageLookupCache[filePath] = lookup;
        return lookup;
    }
}
