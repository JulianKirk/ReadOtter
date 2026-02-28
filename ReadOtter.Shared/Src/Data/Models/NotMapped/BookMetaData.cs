namespace ReadOtter.Shared.Src.Data.Models;

public class BookMetaData
{
    public IEnumerable<string>? Descriptions { get; set; }

    public IEnumerable<string>? Creators { get; set; }

    public IEnumerable<string>? Publishers { get; set; }

    public IEnumerable<string>? Contributors { get; set; }
}
