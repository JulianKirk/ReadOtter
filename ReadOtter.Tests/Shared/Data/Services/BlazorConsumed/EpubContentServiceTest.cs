using Moq;
using ReadOtter.Shared.Data.Services;
using ReadOtter.Shared.Src.Data.Models;

namespace ReadOtter.Tests.Shared.Data.Services
{
    public class EpubContentServiceTest
    {
        [Test]
        public void OffsetBookCurrentChapter_ValidOffset_UpdatesChapter()
        {
            // Arrange
            var bookId = Guid.NewGuid();
            var book = new Book
            {
                Id = bookId,
                Title = "Test",
                CurrentChapter = 1,
                FilePath = "file",
            };
            var mockBookProvider = new Mock<IBookProvider>();
            mockBookProvider.Setup(p => p.GetEmptyOrIncompleteBook(bookId)).Returns(book);
            mockBookProvider.Setup(p => p.GetChapterCount(bookId)).Returns(1000);

            var service = new EpubContentService(mockBookProvider.Object);

            // Act
            var result = service.OffsetBookCurrentChapter(bookId, 1);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(book.CurrentChapter, Is.EqualTo(2));
        }

        [Test]
        public void OffsetBookCurrentChapter_BelowLowerBound_ReturnsFalseAndDoesNotUpdateCurrentChapter()
        {
            // Arrange
            var bookId = Guid.NewGuid();
            var book = new Book
            {
                Id = bookId,
                Title = "Test",
                CurrentChapter = 0,
                FilePath = "file",
            };
            var mockBookProvider = new Mock<IBookProvider>();
            mockBookProvider.Setup(p => p.GetEmptyOrIncompleteBook(bookId)).Returns(book);
            mockBookProvider.Setup(p => p.GetChapterCount(bookId)).Returns(1000);

            var service = new EpubContentService(mockBookProvider.Object);

            // Act
            var result = service.OffsetBookCurrentChapter(bookId, -1);

            // Assert
            Assert.That(result, Is.False);
            Assert.That(book.CurrentChapter, Is.EqualTo(0));
        }

        [Test]
        public void OffsetBookCurrentChapter_AboveUpperBound_ReturnsFalseAndDoesNotUpdateCurrentChapter()
        {
            // Arrange
            var bookId = Guid.NewGuid();
            var book = new Book
            {
                Id = bookId,
                Title = "Test",
                CurrentChapter = 2,
                FilePath = "file",
            };
            var mockBookProvider = new Mock<IBookProvider>();
            mockBookProvider.Setup(p => p.GetEmptyOrIncompleteBook(bookId)).Returns(book);
            mockBookProvider.Setup(p => p.GetChapterCount(bookId)).Returns(2);

            var service = new EpubContentService(mockBookProvider.Object);

            // Act
            var result = service.OffsetBookCurrentChapter(bookId, 1);

            // Assert
            Assert.That(result, Is.False);
            Assert.That(book.CurrentChapter, Is.EqualTo(2));
        }

        [Test]
        public void GetCurrentChapterTextContent_ReturnsChapterContent()
        {
            // Arrange
            var bookId = Guid.NewGuid();
            var book = new Book
            {
                Id = bookId,
                Title = "Test",
                CurrentChapter = 1,
                FilePath = "file",
            };
            var chapter = new ContentChapter("Chapter Content", "Title", 1, "key");
            var mockBookProvider = new Mock<IBookProvider>();
            mockBookProvider.Setup(p => p.GetEmptyOrIncompleteBook(bookId)).Returns(book);
            mockBookProvider.Setup(p => p.GetChapter(bookId, 1)).Returns(chapter);

            var service = new EpubContentService(mockBookProvider.Object);

            // Act
            var content = service.GetCurrentChapterTextContent(bookId);

            // Assert
            Assert.That(content, Is.EqualTo("Chapter Content"));
        }

        [Test]
        public void Constructor_WhenNullBookProvider_ThrowsArgumentNullException()
        {
            // Act, Assert
            var ex = Assert.Throws<ArgumentNullException>(() => new EpubContentService(null));
            Assert.That(ex.ParamName, Is.EqualTo("bookProvider"));
        }
    }
}
