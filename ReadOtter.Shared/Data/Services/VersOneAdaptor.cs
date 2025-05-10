using AutoMapper;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using ReadOtter.Shared.Data.Models;
using VersOne.Epub;
using VersOne.Epub.Options;

namespace ReadOtter.Shared.Data.Services
{
	public class VersOneAdaptor : IVersOneAdaptor
	{
		readonly static EpubReaderOptions defaultReaderOptions = new EpubReaderOptions();

        private readonly IMapper mapper;

        public VersOneAdaptor(IMapper mapper)
		{
            this.mapper = mapper;
        }

        EpubBookRef GetEpubBookRef(Book book, EpubReaderOptions? readerOptions = null)
		{
			var options = readerOptions ?? defaultReaderOptions;

			return EpubReader.OpenBook(book.FilePath, options);
		}

		EpubBook GetEpubBook(Book book, EpubReaderOptions? readerOptions = null)
		{
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

            return new ContentChapter(contentFile.ReadContent(), title: title, key: contentFile.Key);
        }

        public ContentChapter GetChapterContent(Book book, int index)
        {
            var epubBook = GetEpubBookRef(book);
            var contentFile = epubBook.GetReadingOrder()[index];

            return new ContentChapter(contentFile.ReadContent(), index: index, key: contentFile.Key);
        }

        public BookContent GetContent(Book book)
        {
            var epubBook = GetEpubBook(book);
            var chapters = new List<ContentChapter>();

            int chapterIndex = 0;
            foreach (var chapter in epubBook.Content.Html.Local)
            {
               chapters.Add(new ContentChapter(chapter.Content, key: chapter.Key, index: chapterIndex));
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
    }
}
