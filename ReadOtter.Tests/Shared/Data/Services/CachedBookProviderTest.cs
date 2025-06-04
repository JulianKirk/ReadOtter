using Moq;
using ReadOtter.Shared.Data;
using ReadOtter.Shared.Data.Models;
using ReadOtter.Shared.Data.Repositories;
using ReadOtter.Shared.Data.Services;

namespace ReadOtter.Tests.Shared.Data.Services
{
    public class CachedBookProviderTest
    {
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<IBookRepository> _mockBookRepository;
        private Mock<IVersOneAdaptor> _mockVersOneAdaptor;
        private CachedBookProvider testCachedBookProvider;

        [SetUp]
        public void SetUp()
        {
            _mockBookRepository = new Mock<IBookRepository>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockVersOneAdaptor = new Mock<IVersOneAdaptor>();

            _mockUnitOfWork.Setup(u => u.BookRepository).Returns(_mockBookRepository.Object);

            testCachedBookProvider = new CachedBookProvider(
                _mockUnitOfWork.Object,
                _mockVersOneAdaptor.Object
            );
        }

        [Test]
        public void GetAllBooks_ShouldReturnAllBooksFromRepository_WithNoCache()
        {
            // Arrange
            var testBooks = new List<Book>
            {
                new Book
                {
                    Id = Guid.NewGuid(),
                    Title = "Test Book 1",
                    FilePath = "/path/to/book1.epub",
                },
                new Book
                {
                    Id = Guid.NewGuid(),
                    Title = "Test Book 2",
                    FilePath = "/path/to/book2.epub",
                },
            };

            _mockBookRepository.Setup(repo => repo.GetAllBooks()).Returns(testBooks);

            // Act, Assert
            var firstResult = testCachedBookProvider.GetAllBooks().ToList();

            Assert.That(firstResult, Is.EqualTo(testBooks));
            _mockBookRepository.Verify(repo => repo.GetAllBooks(), Times.Once);

            var secondResult = testCachedBookProvider.GetAllBooks().ToList();

            Assert.That(secondResult, Is.EqualTo(testBooks));
            _mockBookRepository.Verify(repo => repo.GetAllBooks(), Times.Exactly(2));
        }

        [Test]
        public void GetEmptyOrIncompleteBook_ShouldReturnBookFromRepository_AndCacheIt()
        {
            // Arrange
            var testBook = new Book
            {
                Id = Guid.NewGuid(),
                Title = "Test Book 1",
                FilePath = "/path/to/book1.epub",
            };

            _mockBookRepository.Setup(repo => repo.GetBookById(testBook.Id)).Returns(testBook);

            // Act, Assert
            var firstResult = testCachedBookProvider.GetEmptyOrIncompleteBook(testBook.Id);

            Assert.That(firstResult, Is.EqualTo(testBook));
            _mockBookRepository.Verify(repo => repo.GetBookById(testBook.Id), Times.Once);

            var secondResult = testCachedBookProvider.GetEmptyOrIncompleteBook(testBook.Id);

            Assert.That(secondResult, Is.EqualTo(testBook));
            _mockBookRepository.Verify(repo => repo.GetBookById(testBook.Id), Times.Once);
        }

        [Test]
        public void GetEmptyOrIncompleteBook_ShouldReturnFromCache_WhenBookIsAlreadyCached()
        {
            // Arrange
            var testBook = new Book
            {
                Id = Guid.NewGuid(),
                Title = "Test Book 1",
                FilePath = "/path/to/book1.epub",
            };

            _mockBookRepository.Setup(repo => repo.GetBookById(testBook.Id)).Returns(testBook);

            // Act
            testCachedBookProvider.GetEmptyOrIncompleteBook(testBook.Id);
            var result = testCachedBookProvider.GetEmptyOrIncompleteBook(testBook.Id);

            // Assert
            Assert.That(result, Is.EqualTo(testBook));
            _mockBookRepository.Verify(repo => repo.GetBookById(testBook.Id), Times.Once);
        }

        [Test]
        public void GetEmptyOrIncompleteBook_ShouldThrowException_WhenBookNotFound()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();
            _mockBookRepository.Setup(repo => repo.GetBookById(nonExistentId)).Returns((Book)null);

            // Act & Assert
            Assert.That(
                () => testCachedBookProvider.GetEmptyOrIncompleteBook(nonExistentId),
                Throws.InvalidOperationException.With.Message.Contain(
                    $"Book with ID {nonExistentId} not found"
                )
            );
        }

        [Test]
        public void GetFullBook_ShouldLoadMetaDataAndContent_AndCacheIt()
        {
            // Arrange
            var testBook = new Book
            {
                Id = Guid.NewGuid(),
                Title = "Test Book 1",
                FilePath = "/path/to/book1.epub",
            };
            var testMetaData = new BookMetaData
            {
                Descriptions = new List<string>() { "testDesc" },
                Creators = new List<string>() { "testCreator" },
                Publishers = new List<string>() { "testPublisher" },
                Contributors = new List<string>() { "testContributor" },
            };
            var testContent = new BookContent(
                new List<ContentChapter>() { new ContentChapter("testcontent", title: "testtile") }
            );

            _mockBookRepository.Setup(repo => repo.GetBookById(testBook.Id)).Returns(testBook);
            _mockVersOneAdaptor
                .Setup(adaptor => adaptor.GetMetaData(It.IsAny<Book>()))
                .Returns(testMetaData);
            _mockVersOneAdaptor
                .Setup(adaptor => adaptor.GetContent(It.IsAny<Book>()))
                .Returns(testContent);

            // Act, Assert
            var firstResult = testCachedBookProvider.GetFullBook(testBook.Id);

            Assert.That(firstResult.Id, Is.EqualTo(testBook.Id));
            Assert.That(firstResult.MetaData, Is.SameAs(testMetaData));
            Assert.That(firstResult.Content, Is.SameAs(testContent));

            _mockVersOneAdaptor.Verify(adaptor => adaptor.GetContent(It.IsAny<Book>()), Times.Once);
            _mockVersOneAdaptor.Verify(
                adaptor => adaptor.GetMetaData(It.IsAny<Book>()),
                Times.Once
            );

            var secondResult = testCachedBookProvider.GetFullBook(testBook.Id);

            Assert.That(firstResult.Id, Is.EqualTo(testBook.Id));
            Assert.That(firstResult.MetaData, Is.SameAs(testMetaData));
            Assert.That(firstResult.Content, Is.SameAs(testContent));

            _mockVersOneAdaptor.Verify(adaptor => adaptor.GetContent(It.IsAny<Book>()), Times.Once);
            _mockVersOneAdaptor.Verify(
                adaptor => adaptor.GetMetaData(It.IsAny<Book>()),
                Times.Once
            );
        }

        [Test]
        public void GetMetadata_ShouldReturnCorrectMetadata_AndCacheIt()
        {
            // Arrange
            var testBook = new Book
            {
                Id = Guid.NewGuid(),
                Title = "Test Book 1",
                FilePath = "/path/to/book1.epub",
            };
            var testMetaData = new BookMetaData
            {
                Descriptions = new List<string>() { "testDesc" },
                Creators = new List<string>() { "testCreator" },
                Publishers = new List<string>() { "testPublisher" },
                Contributors = new List<string>() { "testContributor" },
            };

            _mockBookRepository.Setup(repo => repo.GetBookById(testBook.Id)).Returns(testBook);
            _mockVersOneAdaptor
                .Setup(adaptor => adaptor.GetMetaData(It.IsAny<Book>()))
                .Returns(testMetaData);

            // Act, Assert
            var firstResult = testCachedBookProvider.GetMetadata(testBook.Id);
            Assert.That(firstResult, Is.EqualTo(testMetaData));
            _mockVersOneAdaptor.Verify(
                adaptor => adaptor.GetMetaData(It.IsAny<Book>()),
                Times.Once
            );

            var secondResult = testCachedBookProvider.GetMetadata(testBook.Id);
            Assert.That(secondResult, Is.EqualTo(testMetaData));
            _mockVersOneAdaptor.Verify(
                adaptor => adaptor.GetMetaData(It.IsAny<Book>()),
                Times.Once
            );
        }

        [Test]
        public void GetContent_ShouldReturnCorrectContents_AndCacheIt()
        {
            // Arrange
            var testBook = new Book
            {
                Id = Guid.NewGuid(),
                Title = "Test Book 1",
                FilePath = "/path/to/book1.epub",
            };
            var testContent = new BookContent(
                new List<ContentChapter>()
                {
                    new ContentChapter("this is the content", "DummyTitle"),
                }
            );

            _mockBookRepository.Setup(repo => repo.GetBookById(testBook.Id)).Returns(testBook);
            _mockVersOneAdaptor
                .Setup(adaptor => adaptor.GetContent(It.IsAny<Book>()))
                .Returns(testContent);

            // Act, Assert
            var firstResult = testCachedBookProvider.GetContent(testBook.Id);

            Assert.That(firstResult, Is.SameAs(testContent));
            _mockVersOneAdaptor.Verify(adaptor => adaptor.GetContent(It.IsAny<Book>()), Times.Once);

            var secondResult = testCachedBookProvider.GetContent(testBook.Id);

            Assert.That(secondResult, Is.SameAs(testContent));
            _mockVersOneAdaptor.Verify(adaptor => adaptor.GetContent(It.IsAny<Book>()), Times.Once);
        }

        [Test]
        public void GetChapter_ByTitle_ShouldReturnCorrectChapterContent_AndCacheIt()
        {
            // Arrange
            var testBook = new Book
            {
                Id = Guid.NewGuid(),
                Title = "Test Book 1",
                FilePath = "/path/to/book1.epub",
            };
            const string chapterTitle = "Chapter 1";
            var testChapter = new ContentChapter("Test content", chapterTitle, 1);

            _mockBookRepository.Setup(repo => repo.GetBookById(testBook.Id)).Returns(testBook);
            _mockVersOneAdaptor
                .Setup(adaptor => adaptor.GetChapterContent(It.IsAny<Book>(), chapterTitle))
                .Returns(testChapter);

            // Act, Assert
            var firstResult = testCachedBookProvider.GetChapter(testBook.Id, chapterTitle);
            Assert.That(firstResult, Is.SameAs(testChapter));
            Assert.That(firstResult.Title, Is.EqualTo(chapterTitle));
            _mockVersOneAdaptor.Verify(
                adaptor => adaptor.GetChapterContent(It.IsAny<Book>(), chapterTitle),
                Times.Once
            );

            var secondResult = testCachedBookProvider.GetChapter(testBook.Id, chapterTitle);

            Assert.That(secondResult, Is.SameAs(testChapter));
            Assert.That(secondResult.Title, Is.EqualTo(chapterTitle));
            _mockVersOneAdaptor.Verify(
                adaptor => adaptor.GetChapterContent(It.IsAny<Book>(), chapterTitle),
                Times.Once
            );
        }

        [Test]
        public void GetChapter_ById_ShouldReturnCorrectChapterContent_AndCacheIt()
        {
            // Arrange
            var testBook = new Book
            {
                Id = Guid.NewGuid(),
                Title = "Test Book 1",
                FilePath = "/path/to/book1.epub",
            };
            const int chapterIndex = 1;
            var testChapter = new ContentChapter("Test content", "Chapter 1", chapterIndex);

            _mockBookRepository.Setup(repo => repo.GetBookById(testBook.Id)).Returns(testBook);
            _mockVersOneAdaptor
                .Setup(adaptor => adaptor.GetChapterContent(It.IsAny<Book>(), chapterIndex))
                .Returns(testChapter);

            // Act, Assert
            var firstResult = testCachedBookProvider.GetChapter(testBook.Id, chapterIndex);
            Assert.That(firstResult, Is.SameAs(testChapter));
            Assert.That(firstResult.Index, Is.EqualTo(chapterIndex));
            _mockVersOneAdaptor.Verify(
                adaptor => adaptor.GetChapterContent(It.IsAny<Book>(), chapterIndex),
                Times.Once
            );

            var secondResult = testCachedBookProvider.GetChapter(testBook.Id, chapterIndex);
            Assert.That(secondResult, Is.SameAs(testChapter));
            Assert.That(secondResult.Index, Is.EqualTo(chapterIndex));
            _mockVersOneAdaptor.Verify(
                adaptor => adaptor.GetChapterContent(It.IsAny<Book>(), chapterIndex),
                Times.Once
            );
        }

        [Test]
        public void GetChapterCount_ShouldReturnCount()
        {
            // Arrange
            var testBook = new Book
            {
                Id = Guid.NewGuid(),
                Title = "Test Book 1",
                FilePath = "/path/to/book1.epub",
            };
            const int expectedCount = 10;

            _mockBookRepository.Setup(repo => repo.GetBookById(testBook.Id)).Returns(testBook);
            _mockVersOneAdaptor
                .Setup(adaptor => adaptor.GetTotalChapterCount(It.IsAny<Book>()))
                .Returns(expectedCount);

            // Act
            var result = testCachedBookProvider.GetChapterCount(testBook.Id);

            // Assert
            Assert.That(result, Is.EqualTo(expectedCount));
        }

        [Test]
        public void GetCoverImage_ShouldReturnBase64EncodedImage_WhenImageExists()
        {
            // Arrange
            var testBook = new Book
            {
                Id = Guid.NewGuid(),
                Title = "Test Book 1",
                FilePath = "/path/to/book1.epub",
            };
            var imageBytes = new byte[] { 1, 2, 3, 4 };
            var expectedDataUrl = $"data:image/jpeg;base64,{Convert.ToBase64String(imageBytes)}";

            _mockBookRepository.Setup(repo => repo.GetBookById(testBook.Id)).Returns(testBook);
            _mockVersOneAdaptor
                .Setup(adaptor => adaptor.GetCoverImage(It.IsAny<Book>()))
                .Returns(imageBytes);

            // Act
            var result = testCachedBookProvider.GetCoverImage(testBook.Id);

            // Assert
            Assert.That(result, Is.EqualTo(expectedDataUrl));
        }

        [Test]
        public void GetCoverImage_ShouldReturnEmptyString_WhenImageDoesNotExist()
        {
            // Arrange
            var testBook = new Book
            {
                Id = Guid.NewGuid(),
                Title = "Test Book 1",
                FilePath = "/path/to/book1.epub",
            };

            _mockBookRepository.Setup(repo => repo.GetBookById(testBook.Id)).Returns(testBook);
            _mockVersOneAdaptor
                .Setup(adaptor => adaptor.GetCoverImage(It.IsAny<Book>()))
                .Returns((byte[])null);

            // Act
            var result = testCachedBookProvider.GetCoverImage(testBook.Id);

            // Assert
            Assert.That(result, Is.EqualTo(string.Empty));
        }
    }
}
