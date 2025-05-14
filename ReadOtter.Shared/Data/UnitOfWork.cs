using ReadOtter.Shared.Data.Repositories;

namespace ReadOtter.Shared.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ReadOtterLibraryDbContext _context;

        private BookRepository? _bookRepository;

        public UnitOfWork(ReadOtterLibraryDbContext context)
        {
            _context = context;
        }

        public IBookRepository BookRepository => _bookRepository ??= new BookRepository(_context);

        public void Commit()
        {
            _context.SaveChanges();
        }

        public void Rollback()
        {
            //Insert rollback logic
        }
    }
}
