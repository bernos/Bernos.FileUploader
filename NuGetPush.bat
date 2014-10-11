call nuget push Bernos.FileUploader.%1.nupkg
call nuget push Bernos.FileUploader.Nancy.%1.nupkg
call nuget push Bernos.FileUploader.WebApi.%1.nupkg
call nuget push Bernos.FileUploader.StorageProviders.LocalFileSystem.%1.nupkg
call nuget push Bernos.FileUploader.StorageProviders.S3.%1.nupkg

pause