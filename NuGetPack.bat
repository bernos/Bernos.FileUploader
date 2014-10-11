call nuget pack -sym .\Bernos.FileUploader\Bernos.FileUploader.csproj -Prop Configuration=Release
call nuget pack -sym .\Bernos.FileUploader.Nancy\Bernos.FileUploader.Nancy.csproj -Prop Configuration=Release
call nuget pack -sym .\Bernos.FileUploader.WebApi\Bernos.FileUploader.WebApi.csproj -Prop Configuration=Release
call nuget pack -sym .\Bernos.FileUploader.StorageProviders.LocalFileSystem\Bernos.FileUploader.StorageProviders.LocalFileSystem.csproj -Prop Configuration=Release
call nuget pack -sym .\Bernos.FileUploader.StorageProviders.S3\Bernos.FileUploader.StorageProviders.S3.csproj -Prop Configuration=Release

pause