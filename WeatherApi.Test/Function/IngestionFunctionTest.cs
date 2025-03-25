#nullable disable

using AutoFixture;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using WeatherApi.Exceptions;
using WeatherApi.Model;
using WeatherApi.Model.Persistence;
using WeatherApi.Service;

namespace WeatherApi.Test.Function
{
    [TestClass]
    public class IngestionFunctionTest
    {
        private Fixture _fixture;
        private ILogger<IngestionFunction> _logger;
        private IWeatherDataService _dataService;
        private IWeatherLogService _logService;
        private IDateTimeProvider _dateTimeProvider;
        private IngestionFunction sut;

        [TestInitialize]
        public void Setup()
        {
            _fixture = new Fixture();
            _logger = A.Fake<ILogger<IngestionFunction>>();
            _dataService = A.Fake<IWeatherDataService>();
            _logService = A.Fake<IWeatherLogService>();
            _dateTimeProvider = A.Fake<IDateTimeProvider>();
            sut = new IngestionFunction(_logger, _dataService, _logService, _dateTimeProvider);
        }

        [TestMethod]
        public async Task Run_WhenRun_RequestsWeatherReport()
        {
            // Arrange
            var cancellationToken = new CancellationToken();
            var timerInfo = A.Fake<TimerInfo>();
            var context = A.Fake<FunctionContext>();

            // Act
            await sut.Run(timerInfo, context, cancellationToken);

            // Assert
            A.CallTo(() => _dataService.GetWeatherReportAsync(A<CancellationToken>._)).MustHaveHappenedOnceExactly();
        }

        [TestMethod]
        public async Task Run_WeatherClientFails_LogsError()
        {
            // Arrange
            var cancellationToken = new CancellationToken();
            var timerInfo = A.Fake<TimerInfo>();
            var context = A.Fake<FunctionContext>();
            A.CallTo(() => _dataService.GetWeatherReportAsync(A<CancellationToken>._)).Throws<WeatherClientFetchException>();

            // Act
            await sut.Run(timerInfo, context, cancellationToken);

            // Assert
            A.CallTo(() => _dataService.GetWeatherReportAsync(A<CancellationToken>._)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _logService.LogWeatherRequestFailureAsync(A<string>._, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
        }


        [TestMethod]
        public async Task Run_WeatherStoringFails_LogsError()
        {
            // Arrange
            var cancellationToken = new CancellationToken();
            var timerInfo = A.Fake<TimerInfo>();
            var context = A.Fake<FunctionContext>();
            A.CallTo(() => _dataService.SaveWeatherReportAsync(A<WeatherPayload>._, A<CancellationToken>._)).Throws<BlobNotSavedException>();

            // Act
            await sut.Run(timerInfo, context, cancellationToken);

            // Assert
            A.CallTo(() => _dataService.GetWeatherReportAsync(A<CancellationToken>._)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _dataService.SaveWeatherReportAsync(A<WeatherPayload>._, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _logService.LogWeatherRequestFailureAsync(A<string>._, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
        }

        [TestMethod]
        public async Task Run_HappyPath_LogsSuccess()
        {
            // Arrange
            var id = _fixture.Create<Guid>();
            var cancellationToken = new CancellationToken();
            var timerInfo = A.Fake<TimerInfo>();
            var context = A.Fake<FunctionContext>();
            A.CallTo(() => _dataService.SaveWeatherReportAsync(A<WeatherPayload>._, A<CancellationToken>._)).Returns(id);

            // Act
            await sut.Run(timerInfo, context, cancellationToken);

            // Assert
            A.CallTo(() => _dataService.GetWeatherReportAsync(A<CancellationToken>._)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _dataService.SaveWeatherReportAsync(A<WeatherPayload>._, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _logService.LogWeatherRequestAsync(A<Guid>.That.IsEqualTo(id), A<string>._, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
        }

        [TestMethod]
        public async Task Run_ClientReturnsPayload_StoresPayload()
        {
            // Arrange
            var expected = _fixture.Create<WeatherPayload>();
            var cancellationToken = new CancellationToken();
            var timerInfo = A.Fake<TimerInfo>();
            var context = A.Fake<FunctionContext>();
            WeatherPayload actual = null;
            A.CallTo(() => _dataService.GetWeatherReportAsync(A<CancellationToken>._)).Returns(expected);
            A.CallTo(() => _dataService.SaveWeatherReportAsync(A<WeatherPayload>._, A<CancellationToken>._))
                .Invokes((WeatherPayload payload, CancellationToken token) => actual = payload);

            // Act
            await sut.Run(timerInfo, context, cancellationToken);

            // Assert
            actual.Should().NotBeNull();
            actual.Should().BeEquivalentTo(expected);
        }
    }
}
