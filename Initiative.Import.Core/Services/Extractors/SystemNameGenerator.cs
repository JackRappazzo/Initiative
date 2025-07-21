namespace Initiative.Import.Core.Services.Extractors
{
    /// <summary>
    /// Service to generate system names from creature names
    /// </summary>
    public class SystemNameGenerator : ISystemNameGenerator
    {
        /// <summary>
        /// Generates a system-friendly name from the creature name
        /// </summary>
        /// <param name="name">Original creature name</param>
        /// <returns>System-friendly name</returns>
        public string GenerateSystemName(string name)
        {
            return name.ToLowerInvariant()
                      .Replace(" ", "-")
                      .Replace("'", "")
                      .Replace(",", "")
                      .Replace("(", "")
                      .Replace(")", "");
        }
    }
}