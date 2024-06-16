using Amazon.Lambda.Annotations;
using Amazon.Lambda.Core;
using Amazon.Lambda.S3Events;
using S3Processor.Models;
using S3Processor.Storage;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace S3Processor;

public class Function(IStoredObjectWriter storedObjectWriter)
{
    
    [LambdaFunction(Timeout = 120)]
    public async Task FunctionHandler(S3Event evnt, ILambdaContext context)
    {
        var eventRecords = evnt.Records ?? new List<S3Event.S3EventNotificationRecord>();
        foreach (var record in eventRecords)
        {
            var s3Event = record.S3;
            if (s3Event == null)
            {
                continue;
            }

            context.Logger.LogInformation($"BucketName: {s3Event.Bucket.Name}");
            context.Logger.LogInformation($"File Name: {s3Event.Object.Key}");

            var newStoredObject = new StoredObject
            {
                Key = s3Event.Object.Key,
                CreatedAt = DateTime.UtcNow.Ticks
            };

            try
            {
                var result = await storedObjectWriter.WriteStoredObjectEnty(newStoredObject);
                context.Logger.LogInformation(result);
            }
            catch(Exception e)
            {
                context.Logger.LogError($"Failed to write object with key {newStoredObject.Key}");
                context.Logger.LogError(e.Message);
                context.Logger.LogError(e.Source);
            }
        }
    }
}
