rm -rf ./infra/content
rm -rf ./app/bin
rm -rf ./app/obj

pushd app/src/S3Processor

dotnet clean

dotnet lambda package -farch arm64 -c Release -o ../../../infra/content/S3Processor.zip

popd

pushd infra

pulumi up -y

popd
