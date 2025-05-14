using ReadOtter.Shared.Data.Models;

namespace ReadOtter.Shared.Data.Services
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

        public BookMetaData GetMetaData(Book book)
        {
            return bookProvider.GetMetadata(book.Id);
        }

        public string GetCoverImage(Guid id)
        {
            var book = bookProvider.GetEmptyOrIncompleteBook(id);
            return GetCoverImage(book);
        }

        public string GetCoverImage(Book book)
        {
            return bookProvider.GetCoverImage(book.Id);
        }
    }
}
