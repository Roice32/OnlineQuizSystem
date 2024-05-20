using OQS.CoreWebAPI.Contracts;
using OQS.CoreWebAPI.Tests.SetUp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;


namespace QQS.CoreWebAPI.Tests
{
    public class CreateTagTest : ApplicationContextForTesting
    {
        [Fact]
        public async Task CreateTag_ReturnsOkStatus()
        {
            // Arrange
            var newTag = new CreateTagRequest
            {
                Name = "TestCategory"
            };

            // Act
            var createResponse = await Client.PostAsJsonAsync("api/tags", newTag);

            // Assert
            createResponse
                .EnsureSuccessStatusCode(); // Check if the HTTP response indicates a successful operation (2xx code)
            createResponse.StatusCode.Should().Be(HttpStatusCode.OK); // Check if the HTTP response is of type OK (200 code)
        }

        [Fact]
        public async Task CreateTag_WithInvalidData_ReturnsBadRequest()
        {
            // Arrange
            var newTag = new CreateTagRequest
            {
                // Missing required fields to simulate invalid data
            };

            // Act
            var createResponse = await Client.PostAsJsonAsync("api/tags", newTag);

            // Assert
            createResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var responseContent = await createResponse.Content.ReadAsStringAsync();
            responseContent.Should().Contain("Name is required");
        }
    }
}

