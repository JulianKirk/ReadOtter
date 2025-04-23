using ReadOtter.Shared.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadOtter.Shared.Data.Services
{
    public interface IBookProvider
    {
        public Book GetEmptyOrIncompleteBook(int id);

        public Book GetFullBook(int id);

        public BookMetaData GetMetadata(int id);

        public BookContent GetContent(int id);

        public ContentChapter GetChapter(int id, string chapterName);
    }
}
