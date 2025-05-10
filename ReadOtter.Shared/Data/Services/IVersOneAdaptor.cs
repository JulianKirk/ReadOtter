using AutoMapper;
using ReadOtter.Shared.Data.Models;
using VersOne.Epub;
using VersOne.Epub.Options;

namespace ReadOtter.Shared.Data.Services
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