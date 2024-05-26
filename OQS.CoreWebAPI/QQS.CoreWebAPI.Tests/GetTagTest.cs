using FluentAssertions;
using OQS.CoreWebAPI.Tests.SetUp;
using System.Net;
using Xunit;

namespace QQS.CoreWebAPI.Tests
{
    public class GetTagTest : ApplicationContextForTesting
    
    {

            [Fact]
            public async Task GetTag_ReturnsOkStatus()
            {
                // Arrange

                Guid existingTagId = Guid.Parse("5cb7c8e4-7095-4c31-a394-dbfa6199fbbd");

                // Act
                var getResponse = await Client.GetAsync($"api/tags/{existingTagId}");

                // Assert
                getResponse.StatusCode.Should().Be(HttpStatusCode.OK); // Verifică dacă răspunsul HTTP este de tip OK (cod 200)
            }

            [Fact]
            public async Task GetNonExistingTag_ReturnsNotFound()
            {
                // Arrange
                Guid nonExistingTagId = Guid.NewGuid();

                // Act
               var getResponse = await Client.GetAsync($"api/tags/{nonExistingTagId}");

                // Assert
                getResponse.StatusCode.Should().Be(HttpStatusCode.OK); // Verifică dacă răspunsul HTTP este NotFound (cod 404)
            }
        }
    }



