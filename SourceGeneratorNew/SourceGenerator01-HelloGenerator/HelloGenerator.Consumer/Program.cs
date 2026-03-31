using HelloGenerator.Consumer.Models;

var request = new GreetingRequest();
Console.WriteLine("=== 01 Hello Generator ===");
Console.WriteLine(Generated.HelloMessages.GetWelcomeMessage(request.FeatureName));
