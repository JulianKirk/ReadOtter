using ReadOtter.Shared.Data.Models;
using VersOne.Epub.Schema;

namespace ReadOtter.Shared.Data.Services
{
	public class EpubMetadataService
	{
        private readonly IBookProvider bookProvider;

		public EpubMetadataService(IBookProvider bookProvider)
		{
            this.bookProvider = bookProvider ?? throw new ArgumentNullException(nameof(bookProvider));
		}

        public BookMetaData GetMetaData(Guid id)
        {
            return bookProvider.GetMetadata(id);
        }

        public BookMetaData GetMetaData(Book book)
		{
			return bookProvider.GetMetadata(book.Id);
        }

        public string GetCoverImage(Guid id)
		{
			var book = bookProvider.GetEmptyOrIncompleteBook(id);
			return GetCoverImage(book);
		}

		public string GetCoverImage(Book book)
		{
			//var book = bookProvider.GetEmptyOrIncompleteBook(book.Id);

			//var localAppDataFolder = Environment.SpecialFolder.LocalApplicationData;

			//var bookDirectoryPath = Environment.GetFolderPath(localAppDataFolder) + @$"\Books\Metadata\{book}";
			//var fileName = "cover.jpg";

			//var coverPath = Path.Combine(bookDirectoryPath, fileName);

			////Create the directory if it does not exist
			//if (!Directory.Exists(bookDirectoryPath))
			//{
			//	Directory.CreateDirectory(bookDirectoryPath);
			//}

			////Create the file if it does not exist
			//if (!File.Exists(coverPath))
			//{
			//	var imageBytes = epubBookRef.ReadCover();

			//	if (imageBytes is null)
			//	{

			//	}

			//	File.WriteAllBytes(coverPath, imageBytes);
			//}

			//var coverBytes = File.ReadAllBytes(coverPath);

			//return $"data:image/jpeg;base64,{Convert.ToBase64String(coverBytes)}";

			throw new NotImplementedException();
		}
	}
}
