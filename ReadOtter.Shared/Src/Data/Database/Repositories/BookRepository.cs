using ReadOtter.Shared.Src.Data.Models;

namespace ReadOtter.Shared.Src.Data.Database.Repositories
{
    public class BookRepository : Repository<Book>, IBookRepository
    {
        public BookRepository(ReadOtterLibraryDbContext context)
            : base(context) { }

        public Book? GetBookById(Guid id)
        {
            return GetById(id);
        }

        public IEnumerable<Book> GetAllBooks()
        {
            return _context.Books;
        }

        public void RemoveBookById(Guid id)
        {
            var book = GetBookById(id);

            if (book != null)
            {
                Remove(book);
            }
        }
    }
}
