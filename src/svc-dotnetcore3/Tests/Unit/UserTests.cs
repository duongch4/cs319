using FluentAssertions;
using Web.API.Application.Models;
using Xunit;

namespace Tests.Unit
{
    public class UserTests
    {
        [Fact]
        public void PropertiesShouldBeEqual()
        {
            var properties = new User
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Username = "doej",
                LocationId = 8
            };

            properties.Id.Should().Be(1);
            properties.FirstName.Should().Be("John");
            properties.LastName.Should().Be("Doe");
            properties.LocationId.Should().Be(8);
        }
    }
}
