using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReadOtter.Shared.Data.Models
{
    public class Book
    {
        [Key]
        public Guid Id { get; set; }

        public required string Title { get; set; } //If this is not present then maybe put a message discussing how the epub format provided is not accurate

        public int CurrentChapter { get; set; } = 0;

        public int CurrentChapterPage { get; set; } = 0; //Not yet used

        public required string FilePath { get; set; }

        [NotMapped]
        public BookMetaData? MetaData { get; set; }

        [NotMapped]
        public BookContent? Content { get; set; }
    }
}
