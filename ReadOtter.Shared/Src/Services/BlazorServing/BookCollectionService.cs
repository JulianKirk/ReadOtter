using ReadOtter.Shared.Src.Data;
using ReadOtter.Shared.Src.Data.Models;

namespace ReadOtter.Shared.Src.Services
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

        public Book AddBook(byte[] epubData, string fileName)
        {
            return bookProvider.AddBook(epubData, fileName);
        }

        public void RemoveBook(Guid bookId)
        {
            bookProvider.RemoveBook(bookId);
        }
    }
}
