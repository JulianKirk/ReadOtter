using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ReadOtter.Shared.Src.Data.Database
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ReadOtterLibraryDbContext>
    {
        public ReadOtterLibraryDbContext CreateDbContext(string[] args)
        {
            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder);
            var dbPath = Path.Join(path, "ReadOtterLibrary.db");

            var options = new DbContextOptionsBuilder<ReadOtterLibraryDbContext>()
                .UseSqlite($"Data Source={dbPath}")
                .Options;

            return new ReadOtterLibraryDbContext(options);
        }
    }
}
