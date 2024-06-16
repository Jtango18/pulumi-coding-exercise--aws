using S3Processor.Models;

namespace S3Processor.Storage;

/// <summary>
/// High level abstraction for writing to some form of storage.
/// </summary>
public interface IStoredObjectWriter
{
    Task<string> WriteStoredObjectEnty(StoredObject toStore);
}