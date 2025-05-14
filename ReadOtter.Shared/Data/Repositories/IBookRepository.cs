using ReadOtter.Shared.Data.Models;

namespace ReadOtter.Shared.Data.Repositories
{
    public interface IBookRepository
    {
        Book? GetBookById(Guid id);

        IEnumerable<Book> GetAllBooks();

        void RemoveBookById(Guid id);
    }
}
