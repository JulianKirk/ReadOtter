using ReadOtter.Shared.Src.Data.Models;

namespace ReadOtter.Shared.Src.Data;

public interface IBookProvider
{
    public IEnumerable<Book> GetAllBooks();

    public Book GetEmptyOrIncompleteBook(Guid bookId);

    public Book GetFullBook(Guid bookId);

    public BookMetaData GetMetadata(Guid bookId);

    public BookContent GetContent(Guid bookId);

    public ContentChapter GetChapter(Guid bookId, string chapterName);

    public ContentChapter GetChapter(Guid bookId, int chapterIndex);

    public int GetChapterCount(Guid bookId);

    public string GetCoverImage(Guid bookId);

        public Book AddBook(byte[] epubData, string fileName);

        public void SaveBookProgress(Guid bookId);
    }
}
