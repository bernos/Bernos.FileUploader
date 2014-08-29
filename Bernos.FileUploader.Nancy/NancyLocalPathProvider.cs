using Nancy;

namespace Bernos.FileUploader.Nancy
{
    /// <summary>
    /// An implementation of IPathProvider that wraps the built in nancy IRootPathProvider
    /// </summary>
    public class NancyLocalPathProvider : IPathProvider
    {
        private readonly IRootPathProvider _rootPathProvider;

        public NancyLocalPathProvider(IRootPathProvider rootPathProvider)
        {
            _rootPathProvider = rootPathProvider;
        }

        public string GetRootPath()
        {
            return _rootPathProvider.GetRootPath();
        }
    }
}