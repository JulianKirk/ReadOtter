using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadOtter.Shared.Data.Models
{
    public class BookMetaData
    {
        IEnumerable<string> Creators { get; set; }

        IEnumerable<string> Publishers { get; set; }
    }
}
