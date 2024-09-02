using Microsoft.EntityFrameworkCore;
using ReadOtter.Shared.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadOtter.Shared.Data
{
    public class Seeder
    {
        ReadOtterLibraryDbContext context;

        public Seeder(ReadOtterLibraryDbContext context) 
        {
            this.context = context;

            //ClearAllData(context);
            //SeedData(context);
        }

        public void SeedData()
        {
            context.Books.AddRange(GetBooksToSeed());

            context.SaveChanges();
        }

        public void ClearSeededData()
        {
        }

        public void ClearAllData()
        {
            context.Books.RemoveRange(context.Books);

            context.SaveChanges();
        }

        List<Book> GetBooksToSeed()
        {
            return new List<Book>
            {
                new Book { Name = "ORV", CurrentChapter  = 1, CurrentChapterPage = 1, FilePath = @"C:\Users\proga\source\repos\ReadOtter\ReadOtter.Shared\TestFiles\ORV.epub" },
                new Book { Name = "Red Rising", CurrentChapter = 1, CurrentChapterPage = 1, FilePath = @"C:\Users\proga\source\repos\ReadOtter\ReadOtter.Shared\TestFiles\RedRising.epub" }
			};
        }
    }
}
