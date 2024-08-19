using ReadOtter.Shared.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadOtter.Shared.Data
{
    public interface IUnitOfWork
    {
        void Commit();
        void Rollback();

        public BookRepository BookRepository { get; }
    }
}
