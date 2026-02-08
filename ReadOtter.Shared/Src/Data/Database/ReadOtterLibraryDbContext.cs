using Microsoft.EntityFrameworkCore;
using ReadOtter.Shared.Src.Data.Models;

namespace ReadOtter.Shared.Src.Data.Database
{
    public class ReadOtterLibraryDbContext : DbContext
    {
        public ReadOtterLibraryDbContext(DbContextOptions<ReadOtterLibraryDbContext> options)
            : base(options) { }

        public virtual DbSet<Book> Books { get; set; }
    }
}
