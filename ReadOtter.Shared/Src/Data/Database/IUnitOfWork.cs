using ReadOtter.Shared.Src.Data.Database.Repositories;

namespace ReadOtter.Shared.Src.Data.Database
{
    public interface IUnitOfWork
    {
        void Commit();
        void Rollback();

        public IBookRepository BookRepository { get; }
    }
}
