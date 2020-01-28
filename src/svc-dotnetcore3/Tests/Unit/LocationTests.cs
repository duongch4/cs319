using FluentAssertions;
using Web.API.Application.Models;
using Xunit;

namespace Tests.Unit
{
    public class LocationTests
    {
        [Fact]
        public void PropertiesShouldBeEqual()
        {
            var properties = new Location
            {
                Id = 1,
                Code = "bur",
                Name = "Burnaby",
            };

            properties.Id.Should().Be(1);
            properties.Code.Should().Be("bur");
            properties.Name.Should().Be("Burnaby");
        }
    }
}
