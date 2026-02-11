using System.Text.RegularExpressions;
using AutoMapper;
using ReadOtter.Shared.Src.Data.Models;
using VersOne.Epub;
using VersOne.Epub.Options;

namespace ReadOtter.Shared.Src.Data.Epub;

public partial class VersOneAdaptor : IVersOneAdaptor
{
    static readonly EpubReaderOptions DefaultReaderOptions = new EpubReaderOptions();

    private readonly IMapper mapper;
    private readonly Dictionary<string, Dictionary<string, (string MimeType, byte[] Bytes)>> imageLookupCache = new();

    public VersOneAdaptor(IMapper mapper)
    {
        this.mapper = mapper;
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

        int chapterIndex = 0;
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

    static void ValidateFileExists(Book book)
    {
        if (!File.Exists(book.FilePath))
        {
            throw new FileNotFoundException(
                $"EPUB file not found at '{book.FilePath}' for book '{book.Title}' (Id: {book.Id}).",
                book.FilePath);
        }
    }

    static string ResolveImagesInHtml(
        string html,
        string chapterFilePath,
        Dictionary<string, (string MimeType, byte[] Bytes)> imageLookup)
    {
        var chapterDir = GetDirectoryPath(chapterFilePath);

        html = ImageSrcRegex().Replace(
            html,
            match => ReplaceImageMatch(match, chapterDir, imageLookup));

        html = SvgImageHrefRegex().Replace(
            html,
            match => ReplaceImageMatch(match, chapterDir, imageLookup));

        return html;
    }

    static string ReplaceImageMatch(
        Match match,
        string chapterDir,
        Dictionary<string, (string MimeType, byte[] Bytes)> imageLookup)
    {
        var src = match.Groups[2].Value;

        if (
            src.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
            || src.StartsWith("https://", StringComparison.OrdinalIgnoreCase)
            || src.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
        {
            return match.Value;
        }

        var decodedSrc = Uri.UnescapeDataString(src);
        var resolvedPath = ResolveRelativePath(chapterDir, decodedSrc);

        if (imageLookup.TryGetValue(resolvedPath, out var imageData))
        {
            var base64 = Convert.ToBase64String(imageData.Bytes);
            var dataUri = $"data:{imageData.MimeType};base64,{base64}";
            return $"{match.Groups[1].Value}{dataUri}{match.Groups[3].Value}";
        }

        return match.Value;
    }

    static string ResolveRelativePath(string chapterDir, string relativePath)
    {
        if (relativePath.StartsWith("/"))
        {
            return NormalizePath(relativePath);
        }

        var combined = string.IsNullOrEmpty(chapterDir)
            ? relativePath
            : $"{chapterDir}/{relativePath}";

        return NormalizePath(combined);
    }

    static string NormalizePath(string path)
    {
        path = path.Replace('\\', '/').TrimStart('/');

        var segments = path.Split('/');
        var stack = new List<string>();
        foreach (var segment in segments)
        {
            if (segment == "..")
            {
                if (stack.Count > 0)
                {
                    stack.RemoveAt(stack.Count - 1);
                }
            }
            else if (segment != "." && segment.Length > 0)
            {
                stack.Add(segment);
            }
        }

        return string.Join("/", stack);
    }

    static string GetDirectoryPath(string filePath)
    {
        var normalized = filePath.Replace('\\', '/');
        var lastSlash = normalized.LastIndexOf('/');
        return lastSlash >= 0 ? normalized[..lastSlash] : string.Empty;
    }

    [GeneratedRegex(@"(<img\b[^>]*?\bsrc\s*=\s*"")([^""]+)("")", RegexOptions.IgnoreCase | RegexOptions.Compiled, "en-AU")]
    private static partial Regex ImageSrcRegex();

    [GeneratedRegex(@"(<image\b[^>]*?\bxlink:href\s*=\s*"")([^""]+)("")", RegexOptions.IgnoreCase | RegexOptions.Compiled, "en-AU")]
    private static partial Regex SvgImageHrefRegex();
}
