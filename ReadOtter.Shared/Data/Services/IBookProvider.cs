using ReadOtter.Shared.Data.Models;

namespace ReadOtter.Shared.Data.Services
{
    public interface IBookProvider
    {
        public IEnumerable<Book> GetAllBooks();

        public Book GetEmptyOrIncompleteBook(Guid id);

        public Book GetFullBook(Guid id);

        public BookMetaData GetMetadata(Guid id);

        public BookContent GetContent(Guid id);

        public ContentChapter GetChapter(Guid id, string chapterName);

        public ContentChapter GetChapter(Guid id, int chapterIndex);

        public int GetChapterCount(Guid id);

        public string GetCoverImage(Guid id);
    }
}
