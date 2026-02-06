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

        static string GetTestFilePath(string fileName)
        {
            // TestFiles are copied to the output directory via CopyToOutputDirectory in the csproj.
            var candidate = Path.Combine(AppContext.BaseDirectory, "TestFiles", fileName);
            if (File.Exists(candidate))
                return candidate;

            throw new FileNotFoundException(
                $"Could not locate test file '{fileName}' at '{candidate}'. "
                    + "Ensure the TestFiles are set to CopyToOutputDirectory in ReadOtter.Shared.csproj."
            );
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
                    FilePath = GetTestFilePath("ORV.epub"),
                },
                new Book
                {
                    Title = "Red Rising",
                    CurrentChapter = 1,
                    CurrentChapterPage = 1,
                    FilePath = GetTestFilePath("RedRising.epub"),
                },
            };
        }
    }
}
