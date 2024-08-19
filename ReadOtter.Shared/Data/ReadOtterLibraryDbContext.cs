using Microsoft.EntityFrameworkCore;
using ReadOtter.Shared.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadOtter.Shared.Data
{
    public class ReadOtterLibraryDbContext : DbContext
    {
        string DbPath { get; set; }

        public ReadOtterLibraryDbContext()
        {
            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder);
            DbPath = Path.Join(path, "ReadOrderLibrary.db");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Data Source={DbPath}");
        }

        public virtual DbSet<Book> Books { get; set; }
    }
}
