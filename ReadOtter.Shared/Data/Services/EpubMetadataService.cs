using ReadOtter.Shared.Data.Models;
using VersOne.Epub.Schema;

namespace ReadOtter.Shared.Data.Services
{
	public class EpubMetadataService : EpubServiceBase
	{
		public EpubMetadataService(UnitOfWork unitOfWork) : base(unitOfWork)
		{
		}

		public EpubMetadata GetEpubMetaData(Book book)
		{
			var epubBookRef = GetEpubBookRef(book);
			return epubBookRef.Schema.Package.Metadata;
		}

		public EpubMetadata GetEpubMetaData(int id)
		{
			var epubBookRef = GetEpubBookRef(id);
			return epubBookRef.Schema.Package.Metadata;
		}

		public string GetCoverImage(int id)
		{
			var book = _unitOfWork.BookRepository.GetById(id);
			return GetCoverImage(book);
		}

		public string GetCoverImage(Book book)
		{
			var epubBookRef = GetEpubBookRef(book);

			var localAppDataFolder = Environment.SpecialFolder.LocalApplicationData;

			var bookDirectoryPath = Environment.GetFolderPath(localAppDataFolder) + @$"\Books\Metadata\{epubBookRef.Title}";
			var fileName = "cover.jpg";

			var coverPath = Path.Combine(bookDirectoryPath, fileName);

			//Create the directory if it does not exist
			if (!Directory.Exists(bookDirectoryPath))
			{
				Directory.CreateDirectory(bookDirectoryPath);
			}

			//Create the file if it does not exist
			if (!File.Exists(coverPath))
			{
				var imageBytes = epubBookRef.ReadCover();

				File.WriteAllBytes(coverPath, imageBytes);
			}

			var coverBytes = File.ReadAllBytes(coverPath);

			return $"data:image/jpeg;base64,{Convert.ToBase64String(coverBytes)}";
		}
	}
}
