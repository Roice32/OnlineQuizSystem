using System;
using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using OQS.CoreWebAPI.Contracts;
using OQS.CoreWebAPI.Tests.SetUp;
using Xunit;

namespace QQS.CoreWebAPI.Tests
{
    public class UpdateTagTests : ApplicationContextForTesting
    {
        [Fact]
        public async Task UpdateTag_ReturnsOkStatus()
        {
            var updateTagRequest = new UpdateTagRequest
            {
                Name = "UpdatedTag"
            };

            var tagId = Guid.Parse("f792b82b-31ac-42d2-a208-9043eb57a359");

            var updateResponse = await Client.PatchAsJsonAsync($"api/tags/{tagId}", updateTagRequest);

            updateResponse.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
        }

        [Fact]
        public async Task UpdateTag_WithInvalidData_ReturnsBadRequest()
        {
            var updateTagRequest = new UpdateTagRequest
            {
                Name = string.Empty
            };

            var tagId = Guid.Parse("f792b82b-31ac-42d2-a208-9043eb57a359");

            var updateResponse = await Client.PatchAsJsonAsync($"api/tags/{tagId}", updateTagRequest);

            updateResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var responseContent = await updateResponse.Content.ReadAsStringAsync();
            responseContent.Should().Contain("Name is required");
        }
    }
}
