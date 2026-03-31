Console.WriteLine("=== 04 AdditionalFiles Feature Flag Generator ===");
foreach (var flag in Generated.FeatureFlags.All)
{
    Console.WriteLine($"- {flag}");
}
