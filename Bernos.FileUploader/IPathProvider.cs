namespace Bernos.FileUploader
{
    /// <summary>
    /// Resolves filesystem paths. 
    /// </summary>
    public interface IPathProvider
    {
        string GetRootPath();
    }
}