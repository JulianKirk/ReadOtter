using ReadOtter.Shared.Src.Data.Database.Repositories;

namespace ReadOtter.Shared.Src.Data.Database;

public class UnitOfWork : IUnitOfWork
{
    private readonly ReadOtterLibraryDbContext context;

    private BookRepository? bookRepository;

    private AppSettingRepository? appSettingRepository;

    public UnitOfWork(ReadOtterLibraryDbContext context)
    {
        this.context = context;
    }

    public IBookRepository BookRepository => bookRepository ??= new BookRepository(context);

    public IAppSettingRepository AppSettingRepository => appSettingRepository ??= new AppSettingRepository(context);

    public void Commit()
    {
        context.SaveChanges();
    }

    public void Rollback()
    {
        //Insert rollback logic
    }
}
