using Bernos.FileUploader.StorageProviders.LocalFileSystem;
using Nancy;

namespace Bernos.FileUploader.Nancy.Example
{
    public class PathProvider : IPathProvider
    {
        private readonly IRootPathProvider _rootPathProvider;

        public PathProvider(IRootPathProvider rootPathProvider)
        {
            _rootPathProvider = rootPathProvider;
        }

        public string GetRootPath()
        {
            return _rootPathProvider.GetRootPath();
        }
    }
}