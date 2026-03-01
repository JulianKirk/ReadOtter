using ReadOtter.Shared.Src.Data;
using ReadOtter.Shared.Src.Data.Models;

namespace ReadOtter.Shared.Src.Services;

public class EpubMetadataService
{
    private readonly IBookProvider bookProvider;

    public EpubMetadataService(IBookProvider bookProvider)
    {
        this.bookProvider =
            bookProvider ?? throw new ArgumentNullException(nameof(bookProvider));
    }

    public string GetBookTitle(Guid id)
    {
        return bookProvider.GetEmptyOrIncompleteBook(id).Title;
    }

    public int GetChapterCount(Guid id)
    {
        return bookProvider.GetChapterCount(id);
    }

    public BookMetaData GetMetaData(Guid id)
    {
        return bookProvider.GetMetadata(id);
    }

    public string GetCoverImage(Guid id)
    {
        var book = bookProvider.GetEmptyOrIncompleteBook(id);
        return GetCoverImage(book);
    }

    public string GetCreatorsDisplayString(Guid id)
    {
        var meta = GetMetaData(id);
        return meta.Creators?.Any() == true ? string.Join(", ", meta.Creators) : string.Empty;
    }

    public string GetPublishersDisplayString(Guid id)
    {
        var meta = GetMetaData(id);
        return meta.Publishers?.Any() == true ? string.Join(", ", meta.Publishers) : string.Empty;
    }

    public string GetContributorsDisplayString(Guid id)
    {
        var meta = GetMetaData(id);
        return meta.Contributors?.Any() == true ? string.Join(", ", meta.Contributors) : string.Empty;
    }

    string GetCoverImage(Book book)
    {
        return bookProvider.GetCoverImage(book.Id);
    }

    BookMetaData GetMetaData(Book book)
    {
        return bookProvider.GetMetadata(book.Id);
    }
}
