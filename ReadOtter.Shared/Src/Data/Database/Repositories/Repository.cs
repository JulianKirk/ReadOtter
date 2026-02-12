namespace ReadOtter.Shared.Src.Data.Database.Repositories;

public abstract class Repository<TEntity>
    where TEntity : class
{
    readonly ReadOtterLibraryDbContext context;

    public Repository(ReadOtterLibraryDbContext context)
    {
        this.context = context;
    }

    protected ReadOtterLibraryDbContext DbContext
    {
        get { return context as ReadOtterLibraryDbContext; }
    }

    protected TEntity? GetById(Guid id)
    {
        return context.Set<TEntity>().Find(id);
    }

    protected IEnumerable<TEntity> GetAll()
    {
        return context.Set<TEntity>().ToList();
    }

    protected void Add(TEntity entity)
    {
        context.Set<TEntity>().Add(entity);
    }

    protected void Remove(TEntity entity)
    {
        context.Set<TEntity>().Remove(entity);
    }

    protected void Update(TEntity entity)
    {
        context.Set<TEntity>().Update(entity);
    }

    protected void SaveChanges()
    {
        context.SaveChanges();
    }
}
