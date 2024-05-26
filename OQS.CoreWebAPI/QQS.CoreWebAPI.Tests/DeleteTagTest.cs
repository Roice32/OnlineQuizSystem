using System.Net;
using FluentAssertions;
using OQS.CoreWebAPI.Tests.SetUp;
using Xunit;

namespace QQS.CoreWebAPI.Tests
{
    public class DeleteTagTests : ApplicationContextForTesting
    {
        [Fact]
        public async Task DeleteTag_WithValidId_ReturnsOkStatus()
        {
            // Arrange
            Guid tagId = Guid.Parse("f792b82b-31ac-42d2-a208-9043eb57a359");

            // Act
            var deleteResponse = await Client.DeleteAsync($"api/tags/{tagId}");

            // Assert
            deleteResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task DeleteTag_WithInvalidId_ReturnsBadRequest()
        {
            // Arrange
            var invalidId = Guid.Empty;

            // Act
            var deleteResponse = await Client.DeleteAsync($"api/tags/{invalidId}");

            // Assert
            deleteResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var responseContent = await deleteResponse.Content.ReadAsStringAsync();
            responseContent.Should().Contain("Id is required.");
        }

        [Fact]
        public async Task DeleteTag_WithNonExistentId_ReturnsNotFound()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act
            var deleteResponse = await Client.DeleteAsync($"api/tags/{nonExistentId}");

            // Assert
            deleteResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
            var responseContent = await deleteResponse.Content.ReadAsStringAsync();
            responseContent.Should().Contain("Tag not found.");
        }
    }
}
