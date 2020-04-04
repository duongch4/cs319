using Xunit;

namespace Tests.Integration.Utils
{
    /// <summary>
    /// Sets up each test class within the Integration
    /// collection to get AppFixture in their constructors
    /// </summary>
    [CollectionDefinition("Integration")]
    public class AppFixtureCollectionConnector : ICollectionFixture<AppFixture>
    {
    }
}
