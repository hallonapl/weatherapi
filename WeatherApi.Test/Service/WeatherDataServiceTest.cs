#nullable disable

using AutoFixture;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using WeatherApi.Client;
using WeatherApi.Exceptions;
using WeatherApi.Model;
using WeatherApi.Model.Persistence;
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
        public async Task LoadWeatherReportAsync_RepositoryResponds_ReturnsReport()
        {
            // Arrange
            var expected = _fixture.Create<WeatherPayload>();
            var blob = _fixture.Build<WeatherBlob>()
                .With(x => x.WeatherPayload, expected)
                .Create();
            var id = _fixture.Create<Guid>();
            var cancellationToken = new CancellationToken();
            A.CallTo(() => _repository.GetWeatherBlobByIdAsync(A<Guid>.That.IsEqualTo(id), A<CancellationToken>._)).Returns(blob);

            // Act
            var result = await sut.LoadWeatherReportAsync(id, cancellationToken);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task LoadWeatherReportAsync_RepositoryThrows_Throws()
        {
            // Arrange
            var id = _fixture.Create<Guid>();
            var cancellationToken = new CancellationToken();
            A.CallTo(() => _repository.GetWeatherBlobByIdAsync(A<Guid>.That.IsEqualTo(id), A<CancellationToken>._)).Throws<Exception>();

            // Act
            var action = () => sut.LoadWeatherReportAsync(id, cancellationToken);

            // Assert
            await action.Should().ThrowAsync<Exception>();
        }

        [TestMethod]
        public async Task SaveWeatherReportAsync_SaveWeatherReport_CallsRepository()
        {
            // Arrange
            var payload = _fixture.Create<WeatherPayload>();
            var dateTime = _fixture.Create<DateTime>();
            var expected = _fixture.Build<WeatherBlob>()
                .With(x => x.FetchedTimeStamp, dateTime)
                .With(x => x.WeatherPayload, payload)
                .Create();
            var cancellationToken = new CancellationToken();
            WeatherBlob actual = null;
            A.CallTo(() => _repository.SaveWeatherBlobAsync(A<WeatherBlob>._, A<CancellationToken>._))
                .Invokes((WeatherBlob blob, CancellationToken _) => actual = blob);
            A.CallTo(() => _dateTimeProvider.UtcNow).Returns(dateTime);

            // Act
            await sut.SaveWeatherReportAsync(payload, cancellationToken);

            // Assert
            actual.Should().NotBeNull();
            A.CallTo(() => _repository.SaveWeatherBlobAsync(A<WeatherBlob>._, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
            actual.Should().BeEquivalentTo(expected, options => options.Excluding(x => x.Id));
        }

        [TestMethod]
        public async Task SaveWeatherReportAsync_RepositoryThrows_Throws()
        {
            // Arrange
            var payload = _fixture.Create<WeatherPayload>();
            var cancellationToken = new CancellationToken();
            A.CallTo(() => _repository.SaveWeatherBlobAsync(A<WeatherBlob>._, A<CancellationToken>._)).Throws<BlobNotSavedException>();

            // Act
            var action = () => sut.SaveWeatherReportAsync(payload, cancellationToken);

            // Assert
            await action.Should().ThrowAsync<BlobNotSavedException>();
        }

        [TestMethod]
        public async Task GetWeatherReportAsync_ClientResponds_ReturnsReport()
        {
            // Arrange
            var expected = _fixture.Create<WeatherPayload>();
            var cancellationToken = new CancellationToken();
            A.CallTo(() => _client.GetWeatherReportAsync(A<CancellationToken>._)).Returns(expected);

            // Act
            var actual = await sut.GetWeatherReportAsync(cancellationToken);

            // Assert
            actual.Should().NotBeNull();
            actual.Should().BeEquivalentTo(expected);
        }
    }
}