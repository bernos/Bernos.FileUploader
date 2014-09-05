using Bernos.FileUploader.StorageProviders.LocalFileSystem;
using Bernos.FileUploader.StorageProviders.S3;
using Nancy;
using Nancy.TinyIoc;

namespace Bernos.FileUploader.Nancy.Example
{
    public class Bootstrapper : DefaultNancyBootstrapper
    {
        // The bootstrapper enables you to reconfigure the composition of the framework,
        // by overriding the various methods and properties.
        // For more information https://github.com/NancyFx/Nancy/wiki/Bootstrapper

        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            base.ConfigureApplicationContainer(container);
            container
                .Register
                <LocalFileSystemStorageProviderConfiguration>((c,p) => new LocalFileSystemStorageProviderConfiguration
                {
                    UploadPath = "Content/my-uploads",
                    BaseUrl = "/Content/my-uploads"
                });

            container.Register<FileUploaderConfiguration>((c, p) => new FileUploaderConfiguration
            {
                StorageProvider = new S3StorageProvider(new S3StorageProviderConfiguration
                {
                    BucketName = "bernos-bucket",
                    Folder = "uploads",
                    Region = "us-east-1"
                })
            });
        }
    }

}
