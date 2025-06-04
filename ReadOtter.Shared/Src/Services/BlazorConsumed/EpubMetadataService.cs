using ReadOtter.Shared.Src.Data;
using ReadOtter.Shared.Src.Data.Models;

namespace ReadOtter.Shared.Src.Services
{
    public class EpubMetadataService
    {
        private readonly IBookProvider bookProvider;

        public EpubMetadataService(IBookProvider bookProvider)
        {
            this.bookProvider =
                bookProvider ?? throw new ArgumentNullException(nameof(bookProvider));
        }

        public BookMetaData GetMetaData(Guid id)
        {
            return bookProvider.GetMetadata(id);
        }

        BookMetaData GetMetaData(Book book)
        {
            return bookProvider.GetMetadata(book.Id);
        }

        public string GetCoverImage(Guid id)
        {
            var book = bookProvider.GetEmptyOrIncompleteBook(id);
            return GetCoverImage(book);
        }

        string GetCoverImage(Book book)
        {
            return bookProvider.GetCoverImage(book.Id);
        }
    }
}
