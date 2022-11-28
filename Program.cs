// See https://aka.ms/new-console-template for more information

using Polly;

namespace PollyRetryException;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var retryAndWaitPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(2, _ => TimeSpan.FromMilliseconds(250));

        var fallBackPolicy = Policy<string>
            .Handle<Exception>()
            .FallbackAsync(_ =>
            {
                Console.WriteLine("Fallback");
                return Task.FromResult("test");
            });

        var combinedPolicies = fallBackPolicy.WrapAsync(retryAndWaitPolicy);

        var attempt = 0;
        try
        {
            var output = await combinedPolicies.ExecuteAsync(async () =>
            {
                Console.WriteLine($"In Execute {++attempt}");
                return await DoSomething();
            });

            Console.WriteLine($"Result of output: {output}");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        try
        {
            Console.WriteLine("In try");
        }
        catch (Exception e)
        {
            Console.WriteLine($"Exception occurred: {e}");
        }
    }

    private static async Task<string> DoSomething()
    {
        throw new Exception("Thrown exception");
    }
}
