using ReadOtter.Shared.Src.Data.Database;
using ReadOtter.Shared.Src.Data.Models;

namespace ReadOtter.Shared.Src.Services
{
    public class SeederService
    {
        ReadOtterLibraryDbContext context;

        public SeederService(ReadOtterLibraryDbContext context)
        {
            this.context = context;
        }

        public void SeedData()
        {
            context.Books.AddRange(GetBooksToSeed());

            context.SaveChanges();
        }

        public void ClearSeededData() { }

        public void ClearAllData()
        {
            context.Books.RemoveRange(context.Books);

            context.SaveChanges();
        }

        List<Book> GetBooksToSeed()
        {
            return new List<Book>
            {
                new Book
                {
                    Title = "ORV",
                    CurrentChapter = 1,
                    CurrentChapterPage = 1,
                    FilePath =
                        @"C:\Users\proga\source\repos\ReadOtter\ReadOtter.Shared\TestFiles\ORV.epub",
                },
                new Book
                {
                    Title = "Red Rising",
                    CurrentChapter = 1,
                    CurrentChapterPage = 1,
                    FilePath =
                        @"C:\Users\proga\source\repos\ReadOtter\ReadOtter.Shared\TestFiles\RedRising.epub",
                },
            };
        }
    }
}
