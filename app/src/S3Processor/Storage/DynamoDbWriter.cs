using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using S3Processor.Models;

namespace S3Processor.Storage;

public class DynamoDbWriter(IAmazonDynamoDB dynamoDb, Destination destination) : IStoredObjectWriter
{
    public async Task<string> WriteStoredObjectEnty(StoredObject toStore)
    {
        var result = await dynamoDb.PutItemAsync(destination.TableName, new Dictionary<string, AttributeValue>()
        {
            {
                "key", new AttributeValue
                {
                    S = toStore.Key
                }
            },
            {
                "createdAt", new AttributeValue
                {
                    N = toStore.CreatedAt.ToString()
                }
            }
        });

        return result.HttpStatusCode.ToString();
    }
}