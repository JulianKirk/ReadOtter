using ReadOtter.Shared.Src.Data.Models;

namespace ReadOtter.Shared.Src.Data.Database.Repositories
{
    public interface IBookRepository
    {
        Book? GetBookById(Guid id);

        IEnumerable<Book> GetAllBooks();

        void RemoveBookById(Guid id);

        void AddBook(Book book);
    }
}
