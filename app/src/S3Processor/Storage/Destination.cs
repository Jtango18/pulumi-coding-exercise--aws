namespace S3Processor.Storage;

public record Destination
{
    public string TableName { get; init; }
}