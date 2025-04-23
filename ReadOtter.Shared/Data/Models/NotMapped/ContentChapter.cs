using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadOtter.Shared.Data.Models
{
    public class ContentChapter
    {
        public ContentChapter(string content, string? title = null, int? index = null, string? key = null)
        {
            Title = title;
            Content = content;
            Index = index;
            Key = key;

            if (title == null && index == null)
            {
                throw new ArgumentException("Either title or index must be provided.");
            }
        }

        public string? Key { get; set; } //As defined in the EPUB manifest

        public string? Title { get; set; }

        public int? Index { get; set; }

        public string Content { get; set; }
    }
}
