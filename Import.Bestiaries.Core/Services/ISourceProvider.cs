namespace Import.Bestiaries.Core.Services
{
    public interface ISourceProvider
    {
        Stream OpenSource(string resourceFileName);
    }
}
