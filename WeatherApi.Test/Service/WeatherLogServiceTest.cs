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
    public class WeatherLogServiceTest
    {
        private Fixture _fixture;
        private ILogger<WeatherLogService> _logger;
        private IWeatherLogRepository _repository;
        private IWeatherClient _client;
        private IDateTimeProvider _dateTimeProvider;
        private WeatherLogService sut;

        [TestInitialize]
        public void Setup()
        {
            _fixture = new Fixture();
            _logger = A.Fake<ILogger<WeatherLogService>>();
            _repository = A.Fake<IWeatherLogRepository>();
            _dateTimeProvider = A.Fake<IDateTimeProvider>();
            sut = new WeatherLogService(_logger, _repository);
        }

        [TestMethod]
        public async Task LogWeatherRequestAsync_SuccessLogged_CallsRepositoryWithCorrectValues()
        {
            // Arrange
            var id = _fixture.Create<Guid>();
            var description = _fixture.Create<string>();
            var success = true;
            var expected = _fixture.Build<WeatherLogEntry>()
                            .With(x => x.Id, id)
                            .With(x => x.Success, success)
                            .With(x => x.Description, description)
                            .Create();
            var cancellationToken = new CancellationToken();
            WeatherLogEntry actual = null;
            A.CallTo(() => _repository.AddLogEntryAsync(A<WeatherLogEntry>._, A<CancellationToken>._)).Invokes((WeatherLogEntry entry, CancellationToken token) => actual = entry);
            // Act
            await sut.LogWeatherRequestAsync(id, description, cancellationToken);

            // Assert
            A.CallTo(() => _repository.AddLogEntryAsync(A<WeatherLogEntry>._, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
            actual.Should().NotBeNull();
            actual.Should().BeEquivalentTo(expected, options => 
                options
                .Including(x => x.Id)
                .Including(x => x.Success)
                .Including(x => x.Description));
        }

        [TestMethod]
        public async Task LogWeatherRequestAsync_FailureLogged_CallsRepositoryWithCorrectValues()
        {
            // Arrange
            var description = _fixture.Create<string>();
            var success = false;
            var expected = _fixture.Build<WeatherLogEntry>()
                            .With(x => x.Success, success)
                            .With(x => x.Description, description)
                            .Create();
            var cancellationToken = new CancellationToken();
            WeatherLogEntry actual = null;
            A.CallTo(() => _repository.AddLogEntryAsync(A<WeatherLogEntry>._, A<CancellationToken>._)).Invokes((WeatherLogEntry entry, CancellationToken token) => actual = entry);
            // Act
            await sut.LogWeatherRequestFailureAsync(description, cancellationToken);

            // Assert
            A.CallTo(() => _repository.AddLogEntryAsync(A<WeatherLogEntry>._, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
            actual.Should().NotBeNull();
            actual.Should().BeEquivalentTo(expected, options =>
                options
                .Including(x => x.Success)
                .Including(x => x.Description));
        }

        [TestMethod]
        public async Task GetWeatherRequestLogsAsync_GetAllLogs_ReturnsCorrectValues()
        {
            // Arrange
            var expected = _fixture.CreateMany<WeatherRequestLog>();
            var logs = expected.Select(x => new WeatherLogEntry(x.Id, x.Success, x.Description)).ToAsyncEnumerable();
            var cancellationToken = new CancellationToken();
            A.CallTo(() => _repository.GetLogEntriesAsync(A<CancellationToken>._)).ReturnsNextFromSequence(logs);

            // Act
            var actual = await sut.GetWeatherRequestLogsAsync(cancellationToken);

            // Assert
            actual.Should().NotBeNull();
            actual.Should().BeEquivalentTo(expected, options =>
                options
                .Including(x => x.Id)
                .Including(x => x.Success)
                .Including(x => x.Description));
        }
    }
}
