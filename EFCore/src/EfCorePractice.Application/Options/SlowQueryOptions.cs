namespace EfCorePractice.Application.Options;

public class SlowQueryOptions
{
    public const string SectionName = "SlowQuery";

    public int ThresholdMs { get; set; } = 100;

    public int RecentLogCapacity { get; set; } = 50;

    public bool LogParameters { get; set; } = true;
}
