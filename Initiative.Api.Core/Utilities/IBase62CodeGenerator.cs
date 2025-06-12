namespace Initiative.Api.Core.Utilities
{
    public interface IBase62CodeGenerator
    {
        string GenerateCode(int length);
    }
}