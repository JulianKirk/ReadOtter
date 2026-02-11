using ReadOtter.Shared.Src.Data.Models;

namespace ReadOtter.Shared.Src.Data.Database.Repositories;

public class BookRepository : Repository<Book>, IBookRepository
{
    public BookRepository(ReadOtterLibraryDbContext context)
        : base(context)
    {
    }

    public Book? GetBookById(Guid id)
    {
        return GetById(id);
    }

    public IEnumerable<Book> GetAllBooks()
    {
        return DbContext.Books;
    }

    public void RemoveBookById(Guid id)
    {
        var book = GetBookById(id);

        if (book != null)
        {
            Remove(book);
        }
    }

    public void AddBook(Book book)
    {
        Add(book);
    }
}
