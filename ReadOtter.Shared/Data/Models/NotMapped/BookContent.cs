namespace ReadOtter.Shared.Data.Models
{
    public class BookContent
    {
        public BookContent(IEnumerable<ContentChapter> chapters)
        {
            Chapters = chapters ?? throw new ArgumentNullException(nameof(chapters));
        }

        public IEnumerable<ContentChapter> Chapters { get; }
    }
}
