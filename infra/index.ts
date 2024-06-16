import * as pulumi from "@pulumi/pulumi";
import * as aws from "@pulumi/aws"
import * as awsn from "@pulumi/aws-native";

const getNativeDefaultTags = () => {
    return [{
        key: 'toDelete',
        value: 'true'
    },
    {
        key: 'createdBy',
        value: 'pulumi'
    }]
}

const getClassicDefaultTags = () => {
   return  {
        'toDelete' : 'true',
        'createdBy': 'pulumi'
    }
}

// Bucket
const uploadBucket = new awsn.s3.Bucket("upload-bucket", {
    tags: getNativeDefaultTags()
});


// Bucket Access
const s3AccessPolicyDocument = pulumi.all([uploadBucket.arn]).apply(([bucketArn]) => {
    return aws.iam.getPolicyDocument({
        statements: [
            {
                effect: "Allow",
                actions: [
                    "s3:GetObject"
                ],
                resources: [`${bucketArn}/*`]
            }
        ]
    })
});

const s3AccessPolicy = new aws.iam.Policy("s3-access-policy", {
    policy: s3AccessPolicyDocument.apply(p => p.json),
    name: "S3-Access-Policy",
    tags: getClassicDefaultTags()
});

// Execution Role
const s3LambdaExecutionRole = new aws.iam.Role("lambda-execution-role", {
    assumeRolePolicy: aws.iam.assumeRolePolicyForPrincipal(aws.iam.Principals.LambdaPrincipal),
    path: "/",
    name: "lambda-s3-access-role",
    managedPolicyArns: [s3AccessPolicy.arn],
    tags: getClassicDefaultTags()
});

const lambdaBasicExecutionPolicyAttachment = new aws.iam.RolePolicyAttachment("lambda-execution-policy-attachment", {
    role: s3LambdaExecutionRole.name,
    policyArn: "arn:aws:iam::aws:policy/service-role/AWSLambdaBasicExecutionRole"
});


// Lambda
const s3IndexingLambda = new aws.lambda.Function("s3-indexing-lambda", {
    runtime: aws.lambda.Runtime.Dotnet8,
    role: s3LambdaExecutionRole.arn,
    code: new pulumi.asset.FileArchive('./content/S3Processor.zip'),
    handler: "S3Processor::S3Processor.Function_FunctionHandler_Generated::FunctionHandler",
    
    architectures: [
        // TODO: We should handle whatever the host architecture is
        "arm64"
    ],
    tags: getClassicDefaultTags()

});



// Notification
const s3Notification = new aws.s3.BucketNotification("s3-bucket", {
    bucket: uploadBucket.id,
    lambdaFunctions: [
        {
            lambdaFunctionArn: s3IndexingLambda.arn,
            events: ["s3:ObjectCreated:*"],
            // Add Filters here if required later
        }
    ]
}, {dependsOn: [s3IndexingLambda, uploadBucket]})


