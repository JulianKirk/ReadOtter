namespace ReadOtter.Shared.Src.Data.Models
{
    public class ContentChapter
    {
        public ContentChapter(
            string content,
            string? title = null,
            int? index = null,
            string? key = null
        )
        {
            Title = title;
            Content = content;
            Index = index;
            Key = key;

            if (title == null && index == null)
            {
                throw new ArgumentException(
                    $"Either {nameof(title)} or {nameof(index)} must be provided."
                );
            }
        }

        public string? Key { get; } //As defined in the EPUB manifest

        public string? Title { get; }

        public int? Index { get; }

        public string Content { get; set; }
    }
}
