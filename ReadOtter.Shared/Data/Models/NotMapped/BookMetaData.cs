using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadOtter.Shared.Data.Models
{
    public class BookMetaData
    {
        public IEnumerable<string>? Descriptions { get; set; }

        public IEnumerable<string>? Creators { get; set; }

        public IEnumerable<string>? Publishers { get; set; }

        public IEnumerable<string>? Contributors { get; set; }
    }
}
