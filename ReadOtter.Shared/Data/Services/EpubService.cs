using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadOtter.Shared.Data.Services
{
    public class EpubService : ServiceBase
    {
        public EpubService(UnitOfWork unitOfWork) :base(unitOfWork) 
        {
        }
    }
}
