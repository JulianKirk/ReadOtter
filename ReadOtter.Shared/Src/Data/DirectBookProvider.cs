using ReadOtter.Shared.Src.Data.Models;

namespace ReadOtter.Shared.Src.Data;

public class DirectBookProvider : IBookProvider
{
    public Book AddBook(byte[] epubData, string fileName)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Book> GetAllBooks()
    {
        throw new NotImplementedException();
    }

    public ContentChapter GetChapter(Guid id, string chapterName)
    {
        throw new NotImplementedException();
    }

    public ContentChapter GetChapter(Guid id, int chapterIndex)
    {
        throw new NotImplementedException();
    }

    public int GetChapterCount(Guid id)
    {
        throw new NotImplementedException();
    }

    public BookContent GetContent(Guid id)
    {
        throw new NotImplementedException();
    }

    public string GetCoverImage(Guid id)
    {
        throw new NotImplementedException();
    }

    public Book GetEmptyOrIncompleteBook(Guid id)
    {
        throw new NotImplementedException();
    }

    public Book GetFullBook(Guid id)
    {
        throw new NotImplementedException();
    }

    public BookMetaData GetMetadata(Guid id)
    {
        throw new NotImplementedException();
    }
}
