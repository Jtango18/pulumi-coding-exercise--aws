namespace S3Processor.Models;

public record StoredObject
{
    public string Key { get; init; }
    public long CreatedAt { get; init; }
}