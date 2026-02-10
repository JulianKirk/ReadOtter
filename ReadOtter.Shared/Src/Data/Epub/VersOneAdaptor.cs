using AutoMapper;
using ReadOtter.Shared.Src.Data.Models;
using VersOne.Epub;
using VersOne.Epub.Options;

namespace ReadOtter.Shared.Src.Data.Epub
{
    public class VersOneAdaptor : IVersOneAdaptor
    {
        static readonly EpubReaderOptions defaultReaderOptions = new EpubReaderOptions();

        private readonly IMapper mapper;

        public VersOneAdaptor(IMapper mapper)
        {
            this.mapper = mapper;
        }

        static void ValidateFileExists(Book book)
        {
            if (!File.Exists(book.FilePath))
                throw new FileNotFoundException(
                    $"EPUB file not found at '{book.FilePath}' for book '{book.Title}' (Id: {book.Id}).",
                    book.FilePath
                );
        }

        EpubBookRef GetEpubBookRef(Book book, EpubReaderOptions? readerOptions = null)
        {
            ValidateFileExists(book);
            var options = readerOptions ?? defaultReaderOptions;

            return EpubReader.OpenBook(book.FilePath, options);
        }

        EpubBook GetEpubBook(Book book, EpubReaderOptions? readerOptions = null)
        {
            ValidateFileExists(book);
            var options = readerOptions ?? defaultReaderOptions;

            return EpubReader.ReadBook(book.FilePath, options);
        }

        public BookMetaData GetMetaData(Book book)
        {
            var epubBookRef = GetEpubBookRef(book);
            return mapper.Map<BookMetaData>(epubBookRef.Schema.Package.Metadata);
        }

        public ContentChapter GetChapterContent(Book book, string title)
        {
            var epubBook = GetEpubBookRef(book);
            var contentFile = epubBook.GetReadingOrder().First(c => c.FilePath.Contains(title));

            return new ContentChapter(
                contentFile.ReadContent(),
                title: title,
                key: contentFile.Key
            );
        }

        public ContentChapter GetChapterContent(Book book, int index)
        {
            var epubBook = GetEpubBookRef(book);
            var contentFile = epubBook.GetReadingOrder()[index];

            return new ContentChapter(
                contentFile.ReadContent(),
                index: index,
                key: contentFile.Key
            );
        }

        public BookContent GetContent(Book book)
        {
            var epubBook = GetEpubBook(book);
            var chapters = new List<ContentChapter>();

            int chapterIndex = 0;
            foreach (var chapter in epubBook.Content.Html.Local)
            {
                chapters.Add(
                    new ContentChapter(chapter.Content, key: chapter.Key, index: chapterIndex)
                );
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
                throw new FileNotFoundException(
                    $"EPUB file not found at '{filePath}'.",
                    filePath
                );

            using var epubBookRef = EpubReader.OpenBook(filePath);
            return epubBookRef.Title;
        }
    }
}
