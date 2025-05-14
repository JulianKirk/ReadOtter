using ReadOtter.Shared.Data.Models;

namespace ReadOtter.Shared.Data.Services
{
    public class BookCollectionService
    {
        private readonly IBookProvider bookProvider;

        public BookCollectionService(IBookProvider bookProvider)
        {
            this.bookProvider =
                bookProvider ?? throw new ArgumentNullException(nameof(bookProvider));
        }

        public IEnumerable<Book> GetAllBooks()
        {
            return bookProvider.GetAllBooks();
        }

        public IEnumerable<Guid> GetAllBookIds()
        {
            return GetAllBooks().Select(b => b.Id);
        }
    }
}
