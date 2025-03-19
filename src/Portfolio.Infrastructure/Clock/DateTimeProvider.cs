using Portfolio.Application.Abstractions.Clock;

namespace Portfolio.Infrastructure.Clock;

internal sealed class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
