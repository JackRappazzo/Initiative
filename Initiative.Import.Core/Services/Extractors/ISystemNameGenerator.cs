namespace Initiative.Import.Core.Services.Extractors
{
    /// <summary>
    /// Interface for generating system names from creature names
    /// </summary>
    public interface ISystemNameGenerator
    {
        /// <summary>
        /// Generates a system-friendly name from the creature name
        /// </summary>
        /// <param name="name">Original creature name</param>
        /// <returns>System-friendly name</returns>
        string GenerateSystemName(string name);
    }
}