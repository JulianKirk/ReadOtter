using Microsoft.EntityFrameworkCore;
using ReadOtter.Shared.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadOtter.Shared.Data.Repositories
{
	public interface IBookRepository
	{
		Book GetBookById(int id);

		IEnumerable<Book> GetAllBooks();

		void RemoveBookById(int id);
	}
}
