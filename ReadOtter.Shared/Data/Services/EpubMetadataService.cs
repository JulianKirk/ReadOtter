using ReadOtter.Shared.Data.Models;
using VersOne.Epub;
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
	}
}
