namespace Bernos.FileUploader
{
    public class DefaultPathProvider : IPathProvider
    {
        private readonly string _rootPath;

        public DefaultPathProvider(string rootPath)
        {
            _rootPath = rootPath;
        }

        public string GetRootPath()
        {
            return _rootPath;
        }
    }
}