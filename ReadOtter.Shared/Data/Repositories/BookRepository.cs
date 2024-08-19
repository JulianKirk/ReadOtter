using ReadOtter.Shared.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadOtter.Shared.Data.Repositories
{
    public class BookRepository : Repository<Book>
    {
        public BookRepository(ReadOtterLibraryDbContext context) : base(context)
        {
        }

        public Book GetBookById(int id)
        {
            return _context.Books.SingleOrDefault(b => b.Id == id);
        }

        public IEnumerable<Book> GetAllBooks() 
        {
            return _context.Books;
        }

        public void RemoveBookById(int id)
        {
            var book = GetBookById(id);

            if (book != null)
            {
                Remove(book);
            }
        }
    }
}
