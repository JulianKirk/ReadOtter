using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

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
    }
}
