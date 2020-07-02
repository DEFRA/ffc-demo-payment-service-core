using System;
using System.Threading.Tasks;

namespace FFCDemoPaymentService.Tests
{
    public static class RetryHelper
    {
        public static void RetryOnException(int times, TimeSpan delay, Action operation)
        {
            var attempts = 0;
            do
            {
                try
                {
                    attempts++;
                    Console.WriteLine($"attempt no {attempts}");
                    operation();
                    break;
                }
                catch (Exception ex)
                {
                    if (attempts == times)
                        throw;

                    Console.WriteLine($"Exception caught on attempt {attempts} - will retry after delay {delay}", ex);

                    Task.Delay(delay).Wait();
                }
            } while (true);
        }
    }
}
