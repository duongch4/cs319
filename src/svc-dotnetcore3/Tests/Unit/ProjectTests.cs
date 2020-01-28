using FluentAssertions;
using System;
using Web.API.Application.Models;
using Xunit;

namespace Tests.Unit
{
    public class ProjectTests
    {
        private readonly DateTime dateTime = new DateTime(2020, 1, 1);

        [Fact]
        public void PropertiesShouldBeEqual()
        {
            var properties = new Project
            {
                Id = 1,
                Number = "2020-VAN-001",
                Title = "This is a test project",
                LocationId = 8,
                CreatedAt = dateTime,
                UpdatedAt = dateTime
            };

            properties.Id.Should().Be(1);
            properties.Number.Should().Be("2020-VAN-001");
            properties.Title.Should().Be("This is a test project");
            properties.LocationId.Should().Be(8);
            properties.CreatedAt.Should().Be(dateTime);
            properties.UpdatedAt.Should().Be(dateTime);
        }
    }
}
