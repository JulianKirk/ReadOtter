using AutoMapper;
using ReadOtter.Shared.Data.Models;
using VersOne.Epub;
using VersOne.Epub.Options;

namespace ReadOtter.Shared.Data.Services
{
    public interface IVersOneAdaptor
    {
        EpubBook GetEpubBook(Book book, EpubReaderOptions? readerOptions = null);

        EpubBookRef GetEpubBookRef(Book book, EpubReaderOptions? readerOptions = null);

        string GetContentForChapter(Book book, int chapterIndex);

        int GetTotalChapterCount(Book book);
    }
}