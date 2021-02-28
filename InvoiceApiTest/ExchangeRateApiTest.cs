using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using NUnit.Framework;
using Moq;
using Moq.Protected;
using InvoiceApi.Services;

namespace InvoiceApiTest
{
    public class ExchangeRateApiTest
    {
        private const string EurCurrency = "EUR";
        private const string UsdCurrency = "USD";

        [TestCase("", "")]
        [TestCase("", null)]
        [TestCase(null, "")]
        [TestCase(null, null)]
        public async Task GetRateAsync_ArgumentsNullOrEmpty(string from, string to)
        {
            var logger = TestHelper.CreateDefaultLogger();
            var httpMessageHandler = new Mock<HttpMessageHandler>();
            var httpClient = new HttpClient(httpMessageHandler.Object);
            var exchageRateApi = new ExchangeRateApi(httpClient, logger.Object);

            var rate = await exchageRateApi.GetRateAsync(from, to);

            Assert.IsNull(rate);
        }

        [Test]
        public async Task GetRateAsync_ArgumentsEquals()
        {
            var logger = TestHelper.CreateDefaultLogger();
            var httpMessageHandler = new Mock<HttpMessageHandler>();
            var httpClient = new HttpClient(httpMessageHandler.Object);
            var exchageRateApi = new ExchangeRateApi(httpClient, logger.Object);

            var currency = EurCurrency;
            var rate = await exchageRateApi.GetRateAsync(currency, currency);

            Assert.AreEqual(1m, rate);
        }

        [Test]
        public async Task GetRateAsync_StatusCodeNotOk()
        {
            var logger = TestHelper.CreateDefaultLogger();
            var response = new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.NotFound,
                Content = null,
            };
            var httpMessageHandler = new Mock<HttpMessageHandler>();
            var httpClient = new HttpClient(httpMessageHandler.Object);
            httpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response);
            var exchageRateApi = new ExchangeRateApi(httpClient, logger.Object);

            var rate = await exchageRateApi.GetRateAsync(EurCurrency, UsdCurrency);

            Assert.IsNull(rate);
        }

        [Test]
        public async Task GetRateAsync_ResponseError()
        {
            var logger = TestHelper.CreateDefaultLogger();
            var response = new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(@"{""result"": ""error"",""error-type"": ""unknown-code""}"),
            };
            var httpMessageHandler = new Mock<HttpMessageHandler>();
            var httpClient = new HttpClient(httpMessageHandler.Object);
            httpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response);
            var exchageRateApi = new ExchangeRateApi(httpClient, logger.Object);

            var rate = await exchageRateApi.GetRateAsync(EurCurrency, UsdCurrency);

            Assert.IsNull(rate);
        }

        [Test]
        public async Task GetRateAsync_ResponseSuccessJsonWithoutRate()
        {
            var logger = TestHelper.CreateDefaultLogger();
            var response = new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(@"{""result"": ""success""}"),
            };
            var httpMessageHandler = new Mock<HttpMessageHandler>();
            var httpClient = new HttpClient(httpMessageHandler.Object);
            httpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response);
            var exchageRateApi = new ExchangeRateApi(httpClient, logger.Object);

            var rate = await exchageRateApi.GetRateAsync(EurCurrency, UsdCurrency);

            Assert.IsNull(rate);
        }

        [Test]
        public async Task GetRateAsync_ResponseSuccessJsonWrong()
        {
            var logger = TestHelper.CreateDefaultLogger();
            var response = new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                Content = null,
            };
            var httpMessageHandler = new Mock<HttpMessageHandler>();
            var httpClient = new HttpClient(httpMessageHandler.Object);
            httpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response);
            var exchageRateApi = new ExchangeRateApi(httpClient, logger.Object);

            var rate = await exchageRateApi.GetRateAsync(EurCurrency, UsdCurrency);

            Assert.IsNull(rate);
        }

        [Test]
        public async Task GetRateAsync_ResponseSuccess()
        {
            var logger = TestHelper.CreateDefaultLogger();
            var response = new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(@"{""result"": ""success"", ""conversion_rate"": 0.8412}"),
            };
            var httpMessageHandler = new Mock<HttpMessageHandler>();
            var httpClient = new HttpClient(httpMessageHandler.Object);
            httpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response);
            var exchageRateApi = new ExchangeRateApi(httpClient, logger.Object);

            var rate = await exchageRateApi.GetRateAsync(EurCurrency, UsdCurrency);

            Assert.AreEqual(0.8412m, rate);
        }
    }
}
