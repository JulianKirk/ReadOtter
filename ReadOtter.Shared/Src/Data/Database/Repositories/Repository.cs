namespace ReadOtter.Shared.Src.Data.Database.Repositories
{
    public abstract class Repository<TEntity>
        where TEntity : class
    {
        protected readonly ReadOtterLibraryDbContext _context;

        public Repository(ReadOtterLibraryDbContext context)
        {
            _context = context;
        }

        protected ReadOtterLibraryDbContext DbContext
        {
            get { return _context as ReadOtterLibraryDbContext; }
        }

        protected TEntity? GetById(Guid id)
        {
            return _context.Set<TEntity>().Find(id);
        }

        protected IEnumerable<TEntity> GetAll()
        {
            return _context.Set<TEntity>().ToList();
        }

        protected void Add(TEntity entity)
        {
            _context.Set<TEntity>().Add(entity);
        }

        protected void Remove(TEntity entity)
        {
            _context.Set<TEntity>().Remove(entity);
        }

        protected void Update(TEntity entity)
        {
            _context.Set<TEntity>().Update(entity);
        }

        protected void SaveChanges()
        {
            _context.SaveChanges();
        }
    }
}
