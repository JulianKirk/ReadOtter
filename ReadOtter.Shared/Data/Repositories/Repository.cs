using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadOtter.Shared.Data.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected readonly ReadOtterLibraryDbContext _context;

        public Repository(ReadOtterLibraryDbContext context)
        {
            this._context = context;
        }

        public ReadOtterLibraryDbContext DbContext
        {
            get { return _context as ReadOtterLibraryDbContext; }
        }

        public TEntity GetById(int id)
        {
            return _context.Set<TEntity>().Find(id);
        }

        public IEnumerable<TEntity> GetAll()
        {
            return _context.Set<TEntity>().ToList();
        }

        public void Add(TEntity entity)
        {
            _context.Set<TEntity>().Add(entity);
        }

        public void Remove(TEntity entity)
        {
            _context.Set<TEntity>().Remove(entity);
        }

        public void Update(TEntity entity)
        {
            _context.Set<TEntity>().Update(entity);
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }
    }
}
