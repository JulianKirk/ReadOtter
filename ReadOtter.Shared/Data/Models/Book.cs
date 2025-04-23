using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReadOtter.Shared.Data.Models
{
    public class Book
    {
        //Information needed to display information about the book without actually parsing the Epub data
        
        [Key]
        public int Id { get; set; }

        public string? Name { get; set; }

        public int CurrentChapter { get; set; }

        public int CurrentChapterPage { get; set; }

        public string FilePath { get; set; }

        [NotMapped]
        public BookMetaData MetaData { get; set; } //LOAD THIS DYNAMICALLY

        [NotMapped]
        public BookContent Content { get; set; } //LOAD THIS DYNAMICALLY
    }
}
