# ReadOtter: A personalized epub file reading software

## Backend Architecture

### Services / Dependency Injection
```mermaid
classDiagram

class IBookProvider {
    <<interface>>
    +GetFullBook(bookId: string)  Book
    +GetMetadata(bookId: string)  BookMetaData
    +GetContent(bookId: string)  BookContent
    +GetChapter(bookId: string)  BookChapter
}

class CachedBookProvider {
    +CachedBookProvider(unitOfWork: IUnitOfWork) CachedBookProvider
    -BookCache : Dictionary~int, Book~
    -MetaDataCache : Dictionary~int, BookMetaData~
    -ContentCache : Dictionary~int, BookContent~
    -ChapterCache : Dictionary~int, List~BookChapter~
}

class DirectBookProvider {
    +DirectBookProvider(unitOfWork: IUnitOfWork) DirectBookProvider
}

class BookCommonService {
    +BookCommonService(unitOfWork: IUnitOfWork) BookCommonService
    +GetAllBooks() IEnumerable~Book~
    +GetAllBookIds() IEnumerable~int~
}

class EpubContentService {
    +EpubContentService(unitOfWork: IUnitOfWork, versOneWrapperService: IVersOneAdaptor)  EpubContentService
    +OffsetBookCurrentChapter(id: int, offset: int) void
    +OffsetBookCurrentChapter(book: Book, offset: int) void
    +GetCurrentChapterTextContent(id: int) string
    +GetCurrentChapterTextContent(book: Book) string
}

class EpubMetadataService {
    +EpubMetadataService(unitOfWork: IUnitOfWork, versOneWrapperService: IVersOneAdaptor)  EpubMetadataService
    +GetBookMetaData(id: int) BookMetaData
    +GetBookMetaData(book: Book) BookMetaData
}

class InputService {
    +InputService()  InputService
    +HandleInput(input: string)  void
}

class IVersOneAdaptor {
    <<interface>>
    +EpubBookRef GetEpubBookRef(Book book, EpubReaderOptions? readerOptions = null)
    +EpubBook GetEpubBook(Book book, EpubReaderOptions? readerOptions = null)
    +BookMetaData GetMetaData(Book book)
    +string GetContentForChapter(Book book, int chapterIndex)
    +int GetTotalChapterCount(Book book)
}

class VersOneAdaptor {
    +GetEpubBookRef(book: Book, readerOptions: EpubReaderOptions )  EpubBookRef
    +GetEpubBook(book: Book, readerOptions: EpubReaderOptions )  EpubBook
    +GetMetaData(book: Book)  BookMetaData
    +GetContentForChapter(book: Book, chapterIndex: int)  string
    +GetTotalChapterCount(book: Book)  int
}

class IUnitOfWork {
    <<interface>>
    +Commit() void 
    +Rollback() void 
    +BookRepository IBookRepository 
}

class UnitOfWork {
    +UnitOfWork(DbContext context) UnitOfWork
}

VersOneAdaptor ..|> IVersOneAdaptor
CachedBookProvider ..|> IBookProvider
DirectBookProvider ..|> IBookProvider
UnitOfWork ..|> IUnitOfWork
CachedBookProvider --> IUnitOfWork : uses
DirectBookProvider --> IUnitOfWork : uses
BookCommonService --> CachedBookProvider : uses
EpubContentService --> IVersOneAdaptor : uses
EpubMetadataService --> IVersOneAdaptor : uses
```

### Database Access
```mermaid
classDiagram

class IUnitOfWork {
    <<interface>>
    +Commit() void 
    +Rollback() void 
    +BookRepository IBookRepository 
}

class UnitOfWork {
    +UnitOfWork(DbContext context) UnitOfWork
}

class IBookRepository {
    <<interface>>
    +GetBookById(int id) Book 
    +GetAllBooks() IEnumerable~Book~ 
    +RemoveBookById(int id) void 
}

class BookRepository {
    +BookRepository(DbContext context) BookRepository
}

class Repository~T~ {
    <<abstract>>
    +Repository(DbContext context) Repository~T~
    +GetById(int id) T
    +GetAll() IEnumerable~T~
    +Add(T entity) void
    +Remove(T entity) void
}

class ReadOtterLibraryDbContext {
    +DbPath string
    +Books : DbSet~Book~
    +ReadOtterLibraryDbContext() ReadOtterLibraryDbContext
    +OnConfiguring(DbContextOptionsBuilder optionsBuilder) void
    +OnModelCreating(ModelBuilder modelBuilder) void
}

class Seeder {
    +Seed(ModelBuilder modelBuilder) void 
}

class Book {
    <<model>>
    +Id : int
    +Name : string
    +CurrentChapter : int
    +CurrentChapterPage : int
    +FilePath : string
    ~NotMapped~ +MetaData : BookMetaData
    ~NotMapped~ +Content : BookContent
}

UnitOfWork ..|> IUnitOfWork
BookRepository ..|> IBookRepository
Repository~T~ <|-- BookRepository
IBookRepository --> Book
```

### Data
```mermaid
classDiagram

class Book {
    <<model>>
    +Id : int
    +Name : string
    +CurrentChapter : int
    +CurrentChapterPage : int
    +FilePath : string
    ~NotMapped~ +MetaData : BookMetaData
    ~NotMapped~ +Content : BookContent
}

class BookMetaData {
    +Creators : IEnumerable~string~
    +Publishers : IEnumerable~string~
}

class BookContent {
    +Chapters : IEnumerable~ContentChapter~
}

class ContentChapter {
    +Title : string
    +Content : string
}

Book --> BookMetaData : contains
Book --> BookContent : contains
BookContent --> ContentChapter : contains
```
