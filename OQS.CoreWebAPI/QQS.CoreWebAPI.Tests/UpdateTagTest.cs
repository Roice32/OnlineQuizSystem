using System;
using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using OQS.CoreWebAPI.Contracts;
using OQS.CoreWebAPI.Tests.SetUp;
using Xunit;

namespace OQS.CoreWebAPI.Tests
{
    public class UpdateTagTest : ApplicationContextForTesting
    {

        [Fact]
        public async Task UpdateTag_WithInvalidData_ReturnsBadRequest()
        {
            // Arrange
            var updateTagRequest = new UpdateTagRequest
            {
                Name = string.Empty
            };

            var tagId = Guid.Parse("f792b82b-31ac-42d2-a208-9043eb57a359");

            // Act
            var updateResponse = await Client.PatchAsJsonAsync($"api/tags/{tagId}", updateTagRequest);

            // Assert
            updateResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var responseContent = await updateResponse.Content.ReadAsStringAsync();
            responseContent.Should().Contain("Name is required");
        }
        [Fact]
        public async Task UpdateTag_ReturnsOkStatus()
        {/*
            // Arrange
            var updateTagRequest = new UpdateTagRequest
            {
                Name = "UpdatedTag1"
            };

            var tagId = Guid.Parse("f792b82b-31ac-42d2-a208-9043eb57a359");

            // Act
            var updateResponse = await Client.PatchAsJsonAsync($"api/tags/{tagId}", updateTagRequest);

            // Assert
            updateResponse.EnsureSuccessStatusCode();
            updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);*/
        }

    }
}
