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
        public Book GetFullBook(int id)
        {
            throw new NotImplementedException();
        }
        public BookMetaData GetMetadata(int id)
        {
            throw new NotImplementedException();
        }
        public BookContent GetContent(int id)
        {
            throw new NotImplementedException();
        }
        public ContentChapter GetChapter(int id, string chapterName)
        {
            throw new NotImplementedException();
        }
    }
}
