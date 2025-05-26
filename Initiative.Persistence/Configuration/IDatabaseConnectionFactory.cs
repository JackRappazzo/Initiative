namespace Initiative.Persistence.Configuration
{
    public interface IDatabaseConnectionFactory
    {
        DatabaseConnectionConfiguration Create();
    }
}