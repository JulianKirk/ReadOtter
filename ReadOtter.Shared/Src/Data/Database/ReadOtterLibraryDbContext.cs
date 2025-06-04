using Microsoft.EntityFrameworkCore;
using ReadOtter.Shared.Src.Data.Models;

namespace ReadOtter.Shared.Src.Data.Database
{
    public class ReadOtterLibraryDbContext : DbContext
    {
        string DbPath { get; set; }

        public ReadOtterLibraryDbContext()
        {
            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder);
            DbPath = Path.Join(path, "ReadOtterLibrary.db");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Data Source={DbPath}");
        }

        public virtual DbSet<Book> Books { get; set; }
    }
}
