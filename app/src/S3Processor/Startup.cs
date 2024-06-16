using Amazon.DynamoDBv2;
using Amazon.Lambda.Annotations;
using Microsoft.Extensions.DependencyInjection;
using S3Processor.Storage;

namespace S3Processor;

[LambdaStartup]
public class Startup{

    public void ConfigureServices(IServiceCollection services){
        
        services.AddSingleton<IAmazonDynamoDB, AmazonDynamoDBClient>();
        
        services.AddSingleton(new Destination
        {
            TableName = Environment.GetEnvironmentVariable("TABLE_NAME")! //Ok to use ! because I want it to crash if not found
        });
        services.AddTransient<IStoredObjectWriter, DynamoDbWriter>();
    }
}
