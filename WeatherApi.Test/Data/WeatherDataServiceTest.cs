#nullable disable

using AutoFixture;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using WeatherApi.Client;
using WeatherApi.Model;
using WeatherApi.Repository;
using WeatherApi.Service;

namespace WeatherApi.Test.Data
{
    [TestClass]
    public class WeatherDataServiceTest
    {
        private Fixture _fixture;
        private ILogger<WeatherDataService> _logger;
        private IWeatherDataRepository _repository;
        private IWeatherClient _client;
        private IDateTimeProvider _dateTimeProvider;
        private WeatherDataService sut;

        [TestInitialize]
        public void Setup()
        {
            _fixture = new Fixture();
            _logger = A.Fake<ILogger<WeatherDataService>>();
            _repository = A.Fake<IWeatherDataRepository>();
            _client = A.Fake<IWeatherClient>();
            _dateTimeProvider = A.Fake<IDateTimeProvider>();
            sut = new WeatherDataService(_logger, _repository, _client, _dateTimeProvider);
        }

        [TestMethod]
        public async Task GetAllWeatherData_ShouldReturnData()
        {
            // Arrange
            var dummyData = _fixture.CreateMany<WeatherBlob>(5);
            A.CallTo(() => _repository.GetWeatherDataAsync()).Returns(dummyData.ToAsyncEnumerable());

            // Act
            var result = await sut.LoadAllWeatherDataAsync();

            // Assert
            result.Should().NotBeNull();
            result.Count().Should().Be(5);
            A.CallTo(() => _repository.GetWeatherDataAsync()).MustHaveHappenedOnceExactly();
        }

        [TestMethod]
        public async Task SaveWeatherData_ShouldCallRepository()
        {
            // Arrange
            var weatherDatum = _fixture.Create<WeatherPayload>();

            // Act
            await sut.SaveWeatherReportAsync(weatherDatum);

            // Assert
            A.CallTo(() => _repository.SaveWeatherPayloadAsync(A<WeatherBlob>._)).MustHaveHappenedOnceExactly();
        }

        [TestMethod]
        public async Task SaveWeatherData_ShouldThrow_WhenRepositoryFails()
        {
            // Arrange
            var actualWeather = _fixture.Create<WeatherPayload>();
            A.CallTo(() => _repository.SaveWeatherPayloadAsync(A<WeatherBlob>._)).Throws<Exception>();

            // Act
            var result = () => sut.SaveWeatherReportAsync(actualWeather);

            // Assert
            await result.Should().ThrowAsync<Exception>();
        }
    }
}