using ReadOtter.Shared.Data.Models;
using VersOne.Epub.Schema;

namespace ReadOtter.Shared.Data.Services
{
	public class EpubMetadataService : ServiceBase
	{
		IVersOneWrapperService versOneWrapperService;

		public EpubMetadataService(IUnitOfWork unitOfWork, IVersOneWrapperService versOneWrapperService) : base(unitOfWork)
		{
			this.versOneWrapperService = versOneWrapperService;
		}

		public EpubMetadata GetEpubMetaData(Book book)
		{
			var epubBookRef = versOneWrapperService.GetEpubBookRef(book);
			return epubBookRef.Schema.Package.Metadata;
		}

		public EpubMetadata GetEpubMetaData(int id)
		{
			var book = _unitOfWork.BookRepository.GetBookById(id);
			var epubBookRef = versOneWrapperService.GetEpubBookRef(book);

			return epubBookRef.Schema.Package.Metadata;
		}

		public string GetCoverImage(int id)
		{
			var book = _unitOfWork.BookRepository.GetBookById(id);
			return GetCoverImage(book);
		}

		public string GetCoverImage(Book book)
		{
			var epubBookRef = versOneWrapperService.GetEpubBookRef(book);

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
