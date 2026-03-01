using ReadOtter.Shared.Src.Data.Database.Repositories;

namespace ReadOtter.Shared.Src.Data.Database;

public interface IUnitOfWork
{
    public IBookRepository BookRepository { get; }

    public IAppSettingRepository AppSettingRepository { get; }

    void Commit();

    void Rollback();
}
