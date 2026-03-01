using ReadOtter.Shared.Src.Data;
using ReadOtter.Shared.Src.Data.Models;

namespace ReadOtter.Shared.Src.Services;

public class EpubContentService
{
    private readonly IBookProvider bookProvider;

    public EpubContentService(IBookProvider bookProvider)
    {
        this.bookProvider = bookProvider ?? throw new ArgumentNullException(nameof(bookProvider));
    }

    public int GetCurrentChapter(Guid id)
    {
        return bookProvider.GetEmptyOrIncompleteBook(id).CurrentChapter;
    }

    public void MarkAsOpened(Guid id)
    {
        var book = bookProvider.GetEmptyOrIncompleteBook(id);
        book.LastOpenedAt = DateTimeOffset.Now;
        bookProvider.SaveBookProgress(id);
    }

    public bool OffsetBookCurrentChapter(Guid id, int offset)
    {
        var book = bookProvider.GetEmptyOrIncompleteBook(id);
        return OffsetBookCurrentChapter(book, offset);
    }

    public bool OffsetBookCurrentChapter(Book book, int offset)
    {
        var totalChapterCount = bookProvider.GetChapterCount(book.Id);
        var newChapterNum = book.CurrentChapter + offset;

        if (newChapterNum < 0 || newChapterNum >= totalChapterCount)
        {
            return false;
        }

        book.CurrentChapter = newChapterNum;
        bookProvider.SaveBookProgress(book.Id);

        return true;
    }

    public string GetCurrentChapterTextContent(Guid id)
    {
        var book = bookProvider.GetEmptyOrIncompleteBook(id);
        return GetCurrentChapterTextContent(book);
    }

    public string GetCurrentChapterTextContent(Book book)
    {
        return bookProvider.GetChapter(book.Id, book.CurrentChapter).Content;
    }

    public bool NavigateToHref(Guid bookId, string href)
    {
        var chapterIndex = bookProvider.ResolveChapterIndex(bookId, href);
        if (chapterIndex == null)
        {
            return false;
        }

        var book = bookProvider.GetEmptyOrIncompleteBook(bookId);
        book.CurrentChapter = chapterIndex.Value;
        bookProvider.SaveBookProgress(bookId);

        return true;
    }
}
