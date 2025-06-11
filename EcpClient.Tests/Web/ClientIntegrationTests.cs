using Ecp.Web;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System.Net;

namespace EcpClient.Tests.Web
{
    public class ClientIntegrationTests
    {
        private readonly IHost _host;
        private readonly HttpClient _testHttpClient;
        private readonly string _baseAddress = "http://localhost";

        public ClientIntegrationTests()
        {
            _host = new HostBuilder()
                .ConfigureWebHost(webBuilder =>
                {
                    webBuilder.UseTestServer()
                        .Configure(app =>
                        {
                            app.Run(async context =>
                            {
                                if (context.Request.Path == "/api/test" && context.Request.Method == HttpMethod.Post.Method)
                                {
                                    context.Response.ContentType = "application/json";
                                    await context.Response.WriteAsync("{\"value\":\"ok\"}");
                                }
                                else
                                {
                                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                                }
                            });
                        });
                })
                .Start();

            _testHttpClient = _host.GetTestClient();
        }

        [Fact]
        public async Task PostJson_ShouldReturnDeserializedObject_WhenResponseIsValid()
        {
            // Arrange
            var client = new TestableClient(_baseAddress, _testHttpClient);
            var parameters = new Dictionary<string, string>
            {
                { "param1", "value1" }
            };

            // Act
            var result = await client.PostJson<TestResponse>("api/test", parameters, "api/test");

            // Assert
            result.Should().NotBeNull();
            result.Value.Should().Be("ok");
        }

        [Fact]
        public async Task Post_ShouldThrowNetworkException_WhenUrlIsInvalid()
        {
            // Arrange
            var client = new Client("http://invalid.local");
            var parameters = new Dictionary<string, string>
            {
                { "param1", "value1" }
            };

            // Act
            Func<Task> act = async () => await client.Post("api/test", parameters, "api/test");

            // Assert
            await act.Should().ThrowAsync<NetworkException>();
        }

        [Fact]
        public async Task PostJson_ShouldThrowDeserializeException_WhenResponseIsInvalidJson()
        {
            // Arrange
            var host = new HostBuilder()
                .ConfigureWebHost(webBuilder =>
                {
                    webBuilder.UseTestServer()
                        .Configure(app =>
                        {
                            app.Run(async context =>
                            {
                                context.Response.ContentType = "application/json";
                                await context.Response.WriteAsync("INVALID_JSON");
                            });
                        });
                }).Start();

            var httpClient = host.GetTestClient();
            var client = new TestableClient(_baseAddress, httpClient);
            var parameters = new Dictionary<string, string>();

            // Act
            Func<Task> act = async () => await client.PostJson<TestResponse>("", parameters, "");

            // Assert
            await act.Should().ThrowAsync<DeserializeException>();
        }

        [Fact]
        public async Task Post_ShouldThrowNetworkException_WhenNonHttpRequestExceptionThrown()
        {
            // Arrange
            var throwingHandler = new ThrowingHandler(new TaskCanceledException("Simulated timeout"));
            var httpClient = new HttpClient(throwingHandler);
            var client = new TestableClient("http://localhost", httpClient);

            var parameters = new Dictionary<string, string>
            {
                { "key", "value" }
            };

            // Act
            Func<Task> act = async () => await client.Post("some-path", parameters, "referer");

            // Assert
            await act.Should().ThrowAsync<NetworkException>()
                .WithMessage("*Simulated timeout*");
        }

        // Кастомный HttpMessageHandler, который выбрасывает исключение
        private class ThrowingHandler : HttpMessageHandler
        {
            private readonly Exception _exceptionToThrow;

            public ThrowingHandler(Exception exceptionToThrow)
            {
                _exceptionToThrow = exceptionToThrow;
            }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                throw _exceptionToThrow;
            }
        }

        private class TestableClient : Client
        {
            public TestableClient(string url, HttpClient httpClient) : base(url)
            {
                typeof(Client)
                    .GetField("client", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                    ?.SetValue(this, httpClient);
            }
        }

        private class TestResponse
        {
            [JsonProperty("value")]
            public string Value { get; set; }
        }
    }
}
