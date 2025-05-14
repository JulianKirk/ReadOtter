using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReadOtter.Shared.Data.Models
{
    public class Book
    {
        //Information needed to display information about the book without actually parsing the Epub data
        
        [Key]
        public Guid Id { get; set; }

        public required string Title { get; set; } //If this is not present then maybe put a message discussing how the epub format provided is not accurate

        public int CurrentChapter { get; set; } = 0;

        public int CurrentChapterPage { get; set; } = 0; //Unsure yet how exactly this will work out

        public required string FilePath { get; set; }

        [NotMapped]
        public BookMetaData? MetaData { get; set; } //LOAD THIS DYNAMICALLY

        [NotMapped]
        public BookContent? Content { get; set; } //LOAD THIS DYNAMICALLY
    }
}
