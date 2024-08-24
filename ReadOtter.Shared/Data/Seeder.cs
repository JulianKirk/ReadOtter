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
        public Seeder(ReadOtterLibraryDbContext context) 
        {
            ClearAllData(context);
            SeedData(context);
        }

        public static void SeedData(ReadOtterLibraryDbContext context)
        {
            context.Books.AddRange(GetBooksToSeed());

            context.Books.Add(new Book { FilePath = "whatever", Name = "na,e" });

            context.SaveChanges();
        }

        public static void ClearSeededData(ReadOtterLibraryDbContext context)
        {
        }

        public static void ClearAllData(ReadOtterLibraryDbContext context)
        {
            context.Books.RemoveRange(context.Books);

            context.SaveChanges();
        }

        static List<Book> GetBooksToSeed()
        {
            return new List<Book>
            {
                new Book { Name = "ORV", CurrentChapter  = 1, CurrentChapterPage = 1, FilePath = @"C:\Users\proga\source\repos\ReadOtter\ReadOtter.Shared\TestFiles\ORV.epub"}
            };
        }
    }
}
