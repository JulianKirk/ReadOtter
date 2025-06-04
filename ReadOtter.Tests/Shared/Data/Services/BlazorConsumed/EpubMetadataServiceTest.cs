using Moq;
using ReadOtter.Shared.Src.Data;
using ReadOtter.Shared.Src.Data.Models;
using ReadOtter.Shared.Src.Services;

namespace ReadOtter.Tests.Shared.Data.Services
{
    public class EpubMetadataServiceTest
    {
        private Mock<IBookProvider> mockBookProvider;
        private EpubMetadataService metadataService;
        private Guid testBookId;
        private Book testBook;
        private BookMetaData testMetadata;
        private string testCoverImage;

        [SetUp]
        public void Setup()
        {
            testBookId = Guid.NewGuid();
            testBook = new Book
            {
                Id = testBookId,
                Title = "Test Book",
                FilePath = "test/path.epub",
            };
            testMetadata = new BookMetaData
            {
                Descriptions = new[] { "Test description" },
                Creators = new[] { "Test Author" },
                Publishers = new[] { "Test Publisher" },
                Contributors = new[] { "Test Contributor" },
            };
            testCoverImage = "base64encodedimage";

            mockBookProvider = new Mock<IBookProvider>();
            mockBookProvider.Setup(p => p.GetEmptyOrIncompleteBook(testBookId)).Returns(testBook);
            mockBookProvider.Setup(p => p.GetMetadata(testBookId)).Returns(testMetadata);
            mockBookProvider.Setup(p => p.GetCoverImage(testBookId)).Returns(testCoverImage);

            metadataService = new EpubMetadataService(mockBookProvider.Object);
        }

        [Test]
        public void GetMetaData_ShouldCallProviderWithCorrectId()
        {
            // Act
            var result = metadataService.GetMetaData(testBookId);

            // Assert
            mockBookProvider.Verify(p => p.GetMetadata(testBookId), Times.Once);
            Assert.That(result, Is.EqualTo(testMetadata));
        }

        [Test]
        public void GetCoverImage_ShouldCallProvidersCorrectly()
        {
            // Act
            var result = metadataService.GetCoverImage(testBookId);

            // Assert
            mockBookProvider.Verify(p => p.GetEmptyOrIncompleteBook(testBookId), Times.Once);
            mockBookProvider.Verify(p => p.GetCoverImage(testBookId), Times.Once);
            Assert.That(result, Is.EqualTo(testCoverImage));
        }

        [Test]
        public void Constructor_WhenNullBookProvider_ThrowsArgumentNullException()
        {
            // Act, Assert
            var ex = Assert.Throws<ArgumentNullException>(() => new EpubMetadataService(null));
            Assert.That(ex.ParamName, Is.EqualTo("bookProvider"));
        }
    }
}
