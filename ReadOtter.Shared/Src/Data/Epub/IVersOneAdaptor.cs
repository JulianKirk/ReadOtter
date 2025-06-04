using ReadOtter.Shared.Src.Data.Models;

namespace ReadOtter.Shared.Src.Data.Epub
{
    public interface IVersOneAdaptor
    {
        public BookMetaData GetMetaData(Book book);

        public ContentChapter GetChapterContent(Book book, string title);

        public ContentChapter GetChapterContent(Book book, int index);

        public BookContent GetContent(Book book);

        public int GetTotalChapterCount(Book book);

        public byte[]? GetCoverImage(Book book);
    }
}
