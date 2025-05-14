using ReadOtter.Shared.Data.Repositories;

namespace ReadOtter.Shared.Data
{
    public interface IUnitOfWork
    {
        void Commit();
        void Rollback();

        public IBookRepository BookRepository { get; }
    }
}
