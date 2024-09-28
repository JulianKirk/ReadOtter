using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadOtter.Shared.Data.Services
{
    public abstract class ServiceBase
    {
        protected readonly IUnitOfWork _unitOfWork;

        protected ServiceBase(IUnitOfWork unitOfWork) 
        { 
            _unitOfWork = unitOfWork; 
        }
    }
}
