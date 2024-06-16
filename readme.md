# S3 Upload Processor

This program uses a Lambda Function that listens to S3 creation events, i.e. `s3:ObjectCreated:*`.

When a new item is detected, it's filename and timestamp (ticks since epoch) are written to a DynamoDb table with the following format:

``` json
{
    "key": "uploadedFileName.txt',
    "createdAt": 638541275007442092
}
```

## Deployment
There is a deployment helper script named `deploy.sh`. Running this will 

1. Build and publish the Lambda code to a local .zip archive
2. Run the Pulumi program in `infra` that deploys all infra structure

Note: This assumes there is an AWS profile or Access Key and Access Secret set up in your environment.


## Notes + Assumptions

1. I've built this on an ARM64 Mac, and for ease of deplyoment/development this architecture is hardcoded into the Lambda `architecture` attribute - this could easily be worked around to allow multiplatform build/deploy
2. I assumed after deployment people would be comfortable enough to find the `upload` folder to test uploading files - in a production sscenario I may use a Pulumi output to make the bucket address more easily accessible.
3. I'm using AccessKey and SecretKey IAM credentials for access to AWS - I don't like this inherently insecure method and would strongly disocurage it in a real-world situation - I didn't want to side track by setting up OIDC with the AWS Subscription I used for development though.
4. Ask me about my Azure solution for the same problem - Oy Vey! 
