namespace HelloGenerator.Templates;

internal static class HelloWorldSourceBuilder
{
    public static string Build()
    {
        return @"
namespace Generated
{
    public static class HelloMessages
    {
        public static string GetWelcomeMessage(string featureName)
        {
            return $""Hello from Source Generator: {featureName}"";
        }
    }
}";
    }
}
