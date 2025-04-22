# ReadOtter: A personalized epub file reading software

```mermaid
classDiagram
class Book{
    <<model>>
    +Id : int
    +Name : string
    +CurrentChapter : int
    +CurrentChapterPage : int
    +FilePath : string
    &lt&ltNotMapped&gt&gt +MetaData : BookMetaData
    &lt&ltNotMapped&gt&gt +Content : BookContent
}

class BookMetaData{
    +Creators : IEnumerable&ltstring&gt
    +Publishers : IEnumerable&ltstring&gt
}

class BookContent{
    +Chapters : IEnumerable&ltContentChapter&gt
}

class ContentChapter{
    +Title: string
    +Content: string
}

class BookProvider {
  <<interface>>
  +GetFullBook(bookId: string) Book
  +GetMetadata(bookId: string) BookMetaData
  +GetContent(bookId: string) BookContent
  +GetChapter(bookId: string) BookChapter
}

class CachedBookProvider {
    -BookCache: Dictionary&lt;Dictionary<int, Book>&gt;
    -MetaDataCache: Dictionary&lt;Dictionary<int, BookMetaData>&gt;
    -ContentCache: Dictionary&lt;Dictionary<int, BookContent>&gt;
    -ChapterCache: Dictionary&lt;Dictionary<int, List<BookChapter>>&gt;
 }

CachedBookProvider ..|> BookProvider
```
