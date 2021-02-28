using System.Text.Json;
using Microsoft.Extensions.Logging;
using Moq;

namespace InvoiceApiTest
{
    internal class TestHelper
    {
        internal static bool AreEqualByJson(object expected, object actual)
        {
            var expectedJson = JsonSerializer.Serialize(expected);
            var actualJson = JsonSerializer.Serialize(actual);
            return expectedJson.Equals(actualJson);
        }

        internal static Mock<ILoggerFactory> CreateDefaultLogger()
        {
            var logger = new Mock<ILoggerFactory>();
            logger.Setup(x => x.CreateLogger(It.IsAny<string>()))
                .Returns(new Mock<ILogger>().Object);
            return logger;
        }
    }
}