using System;
using System.Configuration;

namespace Bernos.FileUploader.StorageProviders.LocalFileSystem
{
    public class LocalFileSystemStorageProviderConfiguration
    {
        public static LocalFileSystemStorageProviderConfiguration FromAppSettings()
        {
            var configuration = new LocalFileSystemStorageProviderConfiguration();

            if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings["LocalFileSystemStorageProvider:UploadPath"]))
            {
                configuration.UploadPath = ConfigurationManager.AppSettings["LocalFileSystemStorageProvider:UploadPath"];
            }

            if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings["LocalFileSystemStorageProvider:BaseUrl"]))
            {
                configuration.BaseUrl = ConfigurationManager.AppSettings["LocalFileSystemStorageProvider:BaseUrl"];
            }

            return configuration;
        }
        public string UploadPath { get; set; }
        public string BaseUrl { get; set; }
        
        public LocalFileSystemStorageProviderConfiguration()
        {
            UploadPath = "Content/uploads";
            BaseUrl = "/Content/uploads";
        }
    }
}