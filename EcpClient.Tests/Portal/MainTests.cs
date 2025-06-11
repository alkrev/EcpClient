using Xunit;
using FluentAssertions;
using Moq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Ecp.Portal;
using Ecp.Web;

namespace EcpClient.Tests.Portal
{
    public class MainTests
    {
        [Fact]
        public async Task Login_ShouldReturnLoginReply_WhenCredentialsAreValid()
        {
            // Arrange
            var mockClient = new Mock<IClient>();
            var expectedLogin = "user";
            var expectedPassword = "pass";

            var expectedReply = new loginReply
            {
                Error_Msg = "",
                success = true
            };

            string expectedUrl = $"?c=main&m=index&method=Logon&login={expectedLogin}";
            string expectedReferer = "?c=portal&m=udp";
            var expectedParameters = new Dictionary<string, string>()
            {
                { "login", expectedLogin },
                { "psw", expectedPassword },
                { "swUserRegion", "" },
                { "swUserDBType", "" },
            };

            mockClient
                .Setup(c => c.PostJson<loginReply>(expectedUrl, It.Is<Dictionary<string, string>>(p =>
                    p["login"] == expectedLogin &&
                    p["psw"] == expectedPassword &&
                    p["swUserRegion"] == "" &&
                    p["swUserDBType"] == ""),
                    expectedReferer))
                .ReturnsAsync(expectedReply);

            var main = new Main(mockClient.Object);

            // Act
            var result = await main.Login(expectedLogin, expectedPassword);

            // Assert
            result.Should().NotBeNull();
            result.success.Should().BeTrue();
            result.Error_Msg.Should().BeEmpty();

            mockClient.Verify(c => c.PostJson<loginReply>(
                expectedUrl,
                It.IsAny<Dictionary<string, string>>(),
                expectedReferer),
                Times.Once);
        }
    }
}
