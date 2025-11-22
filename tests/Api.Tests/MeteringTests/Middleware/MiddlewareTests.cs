using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using En.Metering.Middleware;

namespace MeteringTests.Middleware
{
    [TestFixture]
    public class MiddlewareTests
    {
        [Test]
        public async Task InvokeAsync_CatchesExceptionsAndWritesJsonResponse()
        {
            var loggerMock = new Mock<ILogger<ExceptionHandlingMiddleware>>();
            RequestDelegate next = (ctx) => throw new InvalidOperationException("boom");

            var middleware = new ExceptionHandlingMiddleware(next, loggerMock.Object);

            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();

            await middleware.InvokeAsync(context);

            context.Response.Body.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(context.Response.Body);
            var body = await reader.ReadToEndAsync();

            Assert.AreEqual(StatusCodes.Status500InternalServerError, context.Response.StatusCode);
            Assert.IsTrue(body.Contains("An unexpected error occurred."));
            Assert.IsTrue(body.Contains("boom"));

            // Validate JSON shape
            var doc = JsonDocument.Parse(body);
            Assert.IsTrue(doc.RootElement.TryGetProperty("message", out _));
            Assert.IsTrue(doc.RootElement.TryGetProperty("detail", out _));
        }
    }
}
