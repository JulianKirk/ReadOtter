using ReadOtter.Shared.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadOtter.Shared.Data.Services
{
    public class DirectBookProvider : IBookProvider
    {
        public IEnumerable<Book> GetAllBooks()
        {
            throw new NotImplementedException();
        }

        public ContentChapter GetChapter(Guid id, string chapterName)
        {
            throw new NotImplementedException();
        }

        public ContentChapter GetChapter(Guid id, int chapterIndex)
        {
            throw new NotImplementedException();
        }

        public int GetChapterCount(Guid id)
        {
            throw new NotImplementedException();
        }

        public BookContent GetContent(Guid id)
        {
            throw new NotImplementedException();
        }

        public Book GetEmptyOrIncompleteBook(Guid id)
        {
            throw new NotImplementedException();
        }

        public Book GetFullBook(Guid id)
        {
            throw new NotImplementedException();
        }

        public BookMetaData GetMetadata(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
