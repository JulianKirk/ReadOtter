using LazyCache;
using Microsoft.Extensions.Options;
using Moq;
using ReadOtter.Shared.Src.Data;
using ReadOtter.Shared.Src.Data.Database;
using ReadOtter.Shared.Src.Data.Database.Repositories;
using ReadOtter.Shared.Src.Data.Epub;
using ReadOtter.Shared.Src.Data.Models;
using ReadOtter.Shared.Src.Settings;

namespace ReadOtter.Tests.Shared.Data.Services;

public class BookProviderTest
{
    private Mock<IUnitOfWork> mockUnitOfWork;
    private Mock<IBookRepository> mockBookRepository;
    private Mock<IVersOneAdaptor> mockVersOneAdaptor;
    private Mock<IOptionsMonitor<AppSettings>> mockAppSettingsMonitor;
    private IAppCache appCache;
    private BookProvider testBookProvider;

    [SetUp]
    public void SetUp()
    {
        mockBookRepository = new Mock<IBookRepository>();
        mockUnitOfWork = new Mock<IUnitOfWork>();
        mockVersOneAdaptor = new Mock<IVersOneAdaptor>();

        mockUnitOfWork.Setup(u => u.BookRepository).Returns(mockBookRepository.Object);

        var appSettings = new AppSettings { BookCacheExpiry = TimeSpan.FromMinutes(30) };
        mockAppSettingsMonitor = new Mock<IOptionsMonitor<AppSettings>>();
        mockAppSettingsMonitor.Setup(m => m.CurrentValue).Returns(appSettings);

        appCache = new CachingService();

        testBookProvider = new BookProvider(
            mockUnitOfWork.Object,
            mockVersOneAdaptor.Object,
            appCache,
            mockAppSettingsMonitor.Object);
    }

    [Test]
    public void NullUnitOfWork_ShouldThrowArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(
            () =>
                new BookProvider(
                    null!,
                    mockVersOneAdaptor.Object,
                    appCache,
                    mockAppSettingsMonitor.Object));
    }

    [Test]
    public void NullVersOneAdaptor_ShouldThrowArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(
            () =>
                new BookProvider(
                    mockUnitOfWork.Object,
                    null!,
                    appCache,
                    mockAppSettingsMonitor.Object));
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

        mockBookRepository.Setup(repo => repo.GetAllBooks()).Returns(testBooks);

        // Act, Assert
        var firstResult = testBookProvider.GetAllBooks().ToList();

        Assert.That(firstResult, Is.EqualTo(testBooks));
        mockBookRepository.Verify(repo => repo.GetAllBooks(), Times.Once);

        var secondResult = testBookProvider.GetAllBooks().ToList();

        Assert.That(secondResult, Is.EqualTo(testBooks));
        mockBookRepository.Verify(repo => repo.GetAllBooks(), Times.Exactly(2));
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

        mockBookRepository.Setup(repo => repo.GetBookById(testBook.Id)).Returns(testBook);

        // Act, Assert
        var firstResult = testBookProvider.GetEmptyOrIncompleteBook(testBook.Id);

        Assert.That(firstResult, Is.EqualTo(testBook));
        mockBookRepository.Verify(repo => repo.GetBookById(testBook.Id), Times.Once);

        var secondResult = testBookProvider.GetEmptyOrIncompleteBook(testBook.Id);

        Assert.That(secondResult, Is.EqualTo(testBook));
        mockBookRepository.Verify(repo => repo.GetBookById(testBook.Id), Times.Once);
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

        mockBookRepository.Setup(repo => repo.GetBookById(testBook.Id)).Returns(testBook);

        // Act
        testBookProvider.GetEmptyOrIncompleteBook(testBook.Id);
        var result = testBookProvider.GetEmptyOrIncompleteBook(testBook.Id);

        // Assert
        Assert.That(result, Is.EqualTo(testBook));
        mockBookRepository.Verify(repo => repo.GetBookById(testBook.Id), Times.Once);
    }

    [Test]
    public void GetEmptyOrIncompleteBook_ShouldThrowException_WhenBookNotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        mockBookRepository.Setup(repo => repo.GetBookById(nonExistentId)).Returns((Book?)null);

        // Act & Assert
        Assert.That(
            () => testBookProvider.GetEmptyOrIncompleteBook(nonExistentId),
            Throws.InvalidOperationException.With.Message.Contain(
                $"Book with ID {nonExistentId} not found"));
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
            new List<ContentChapter>() { new ContentChapter("testcontent", title: "testtile") });

        mockBookRepository.Setup(repo => repo.GetBookById(testBook.Id)).Returns(testBook);
        mockVersOneAdaptor
            .Setup(adaptor => adaptor.GetMetaData(It.IsAny<Book>()))
            .Returns(testMetaData);
        mockVersOneAdaptor
            .Setup(adaptor => adaptor.GetContent(It.IsAny<Book>()))
            .Returns(testContent);

        // Act, Assert
        var firstResult = testBookProvider.GetFullBook(testBook.Id);

        Assert.That(firstResult.Id, Is.EqualTo(testBook.Id));
        Assert.That(firstResult.MetaData, Is.SameAs(testMetaData));
        Assert.That(firstResult.Content, Is.SameAs(testContent));

        mockVersOneAdaptor.Verify(adaptor => adaptor.GetContent(It.IsAny<Book>()), Times.Once);
        mockVersOneAdaptor.Verify(
            adaptor => adaptor.GetMetaData(It.IsAny<Book>()),
            Times.Once);

        var secondResult = testBookProvider.GetFullBook(testBook.Id);

        Assert.That(firstResult.Id, Is.EqualTo(testBook.Id));
        Assert.That(firstResult.MetaData, Is.SameAs(testMetaData));
        Assert.That(firstResult.Content, Is.SameAs(testContent));

        mockVersOneAdaptor.Verify(adaptor => adaptor.GetContent(It.IsAny<Book>()), Times.Once);
        mockVersOneAdaptor.Verify(
            adaptor => adaptor.GetMetaData(It.IsAny<Book>()),
            Times.Once);
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

        mockBookRepository.Setup(repo => repo.GetBookById(testBook.Id)).Returns(testBook);
        mockVersOneAdaptor
            .Setup(adaptor => adaptor.GetMetaData(It.IsAny<Book>()))
            .Returns(testMetaData);

        // Act, Assert
        var firstResult = testBookProvider.GetMetadata(testBook.Id);
        Assert.That(firstResult, Is.EqualTo(testMetaData));
        mockVersOneAdaptor.Verify(
            adaptor => adaptor.GetMetaData(It.IsAny<Book>()),
            Times.Once);

        var secondResult = testBookProvider.GetMetadata(testBook.Id);
        Assert.That(secondResult, Is.EqualTo(testMetaData));
        mockVersOneAdaptor.Verify(
            adaptor => adaptor.GetMetaData(It.IsAny<Book>()),
            Times.Once);
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
            });

        mockBookRepository.Setup(repo => repo.GetBookById(testBook.Id)).Returns(testBook);
        mockVersOneAdaptor
            .Setup(adaptor => adaptor.GetContent(It.IsAny<Book>()))
            .Returns(testContent);

        // Act, Assert
        var firstResult = testBookProvider.GetContent(testBook.Id);

        Assert.That(firstResult, Is.SameAs(testContent));
        mockVersOneAdaptor.Verify(adaptor => adaptor.GetContent(It.IsAny<Book>()), Times.Once);

        var secondResult = testBookProvider.GetContent(testBook.Id);

        Assert.That(secondResult, Is.SameAs(testContent));
        mockVersOneAdaptor.Verify(adaptor => adaptor.GetContent(It.IsAny<Book>()), Times.Once);
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

        mockBookRepository.Setup(repo => repo.GetBookById(testBook.Id)).Returns(testBook);
        mockVersOneAdaptor
            .Setup(adaptor => adaptor.GetChapterContent(It.IsAny<Book>(), chapterTitle))
            .Returns(testChapter);

        // Act, Assert
        var firstResult = testBookProvider.GetChapter(testBook.Id, chapterTitle);
        Assert.That(firstResult, Is.SameAs(testChapter));
        Assert.That(firstResult.Title, Is.EqualTo(chapterTitle));
        mockVersOneAdaptor.Verify(
            adaptor => adaptor.GetChapterContent(It.IsAny<Book>(), chapterTitle),
            Times.Once);

        var secondResult = testBookProvider.GetChapter(testBook.Id, chapterTitle);

        Assert.That(secondResult, Is.SameAs(testChapter));
        Assert.That(secondResult.Title, Is.EqualTo(chapterTitle));
        mockVersOneAdaptor.Verify(
            adaptor => adaptor.GetChapterContent(It.IsAny<Book>(), chapterTitle),
            Times.Once);
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

        mockBookRepository.Setup(repo => repo.GetBookById(testBook.Id)).Returns(testBook);
        mockVersOneAdaptor
            .Setup(adaptor => adaptor.GetChapterContent(It.IsAny<Book>(), chapterIndex))
            .Returns(testChapter);

        // Act, Assert
        var firstResult = testBookProvider.GetChapter(testBook.Id, chapterIndex);
        Assert.That(firstResult, Is.SameAs(testChapter));
        Assert.That(firstResult.Index, Is.EqualTo(chapterIndex));
        mockVersOneAdaptor.Verify(
            adaptor => adaptor.GetChapterContent(It.IsAny<Book>(), chapterIndex),
            Times.Once);

        var secondResult = testBookProvider.GetChapter(testBook.Id, chapterIndex);
        Assert.That(secondResult, Is.SameAs(testChapter));
        Assert.That(secondResult.Index, Is.EqualTo(chapterIndex));
        mockVersOneAdaptor.Verify(
            adaptor => adaptor.GetChapterContent(It.IsAny<Book>(), chapterIndex),
            Times.Once);
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

        mockBookRepository.Setup(repo => repo.GetBookById(testBook.Id)).Returns(testBook);
        mockVersOneAdaptor
            .Setup(adaptor => adaptor.GetTotalChapterCount(It.IsAny<Book>()))
            .Returns(expectedCount);

        // Act
        var result = testBookProvider.GetChapterCount(testBook.Id);

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

        mockBookRepository.Setup(repo => repo.GetBookById(testBook.Id)).Returns(testBook);
        mockVersOneAdaptor
            .Setup(adaptor => adaptor.GetCoverImage(It.IsAny<Book>()))
            .Returns(imageBytes);

        // Act
        var result = testBookProvider.GetCoverImage(testBook.Id);

        // Assert
        Assert.That(result, Is.EqualTo(expectedDataUrl));
    }

    [Test]
    public void RemoveBook_ShouldCallRepositoryRemoveAndCommit()
    {
        // Arrange
        var bookId = Guid.NewGuid();

        // Act
        testBookProvider.RemoveBook(bookId);

        // Assert
        mockBookRepository.Verify(repo => repo.RemoveBookById(bookId), Times.Once);
        mockUnitOfWork.Verify(uow => uow.Commit(), Times.Once);
    }

    [Test]
    public void RemoveBook_ShouldEvictAllCaches()
    {
        // Arrange
        var testBook = new Book
        {
            Id = Guid.NewGuid(),
            Title = "Test Book",
            FilePath = "/path/to/book.epub",
        };
        var testMetaData = new BookMetaData
        {
            Descriptions = new List<string>() { "desc" },
            Creators = new List<string>() { "creator" },
        };
        var testContent = new BookContent(
            new List<ContentChapter>() { new ContentChapter("content", title: "title") });
        var testChapter = new ContentChapter("chapter content", "Chapter 1", 0);

        mockBookRepository.Setup(repo => repo.GetBookById(testBook.Id)).Returns(testBook);
        mockVersOneAdaptor
            .Setup(adaptor => adaptor.GetMetaData(It.IsAny<Book>()))
            .Returns(testMetaData);
        mockVersOneAdaptor
            .Setup(adaptor => adaptor.GetContent(It.IsAny<Book>()))
            .Returns(testContent);
        mockVersOneAdaptor
            .Setup(adaptor => adaptor.GetChapterContent(It.IsAny<Book>(), 0))
            .Returns(testChapter);

        testBookProvider.GetMetadata(testBook.Id);
        testBookProvider.GetContent(testBook.Id);
        testBookProvider.GetChapter(testBook.Id, 0);

        // Act
        testBookProvider.RemoveBook(testBook.Id);

        // Assert -- subsequent calls should hit the repository again
        testBookProvider.GetMetadata(testBook.Id);
        testBookProvider.GetContent(testBook.Id);
        testBookProvider.GetChapter(testBook.Id, 0);

        mockVersOneAdaptor.Verify(
            adaptor => adaptor.GetMetaData(It.IsAny<Book>()),
            Times.Exactly(2));
        mockVersOneAdaptor.Verify(
            adaptor => adaptor.GetContent(It.IsAny<Book>()),
            Times.Exactly(2));
        mockVersOneAdaptor.Verify(
            adaptor => adaptor.GetChapterContent(It.IsAny<Book>(), 0),
            Times.Exactly(2));
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

        mockBookRepository.Setup(repo => repo.GetBookById(testBook.Id)).Returns(testBook);
        mockVersOneAdaptor
            .Setup(adaptor => adaptor.GetCoverImage(It.IsAny<Book>()))
            .Returns((byte[]?)null);

        // Act
        var result = testBookProvider.GetCoverImage(testBook.Id);

        // Assert
        Assert.That(result, Is.EqualTo(string.Empty));
    }
}
