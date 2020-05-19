using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace Operation.RedisAggregate
{
    internal static class RetryHelper
    {
        private const string ErrorMessage = "Maximum number of tries exceeded to perform the action: {0}.";
        private const string WarningMessage = "Exception occurred performing an action. Retrying... {0}/{1}";

        public static T Retry<T>(Func<T> retryme, int timeOut, int retries)
        {
            var tries = 0;
            do
            {
                tries++;

                try
                {
                    return retryme();
                }
                catch (Exception<GetLockExceptionArgs> ex)
                {
                    if (tries >= retries)
                    {
                        throw ex;
                    }

                    Task.Delay(timeOut).Wait();
                }
                // might occur on lua script execution on a readonly slave because the master just died.
                // Should recover via fail over
                catch (RedisServerException ex)
                {
                    if (tries >= retries)
                    {
                        throw ex;
                    }

                    Task.Delay(timeOut).Wait();
                }
                catch (RedisConnectionException ex)
                {
                    if (tries >= retries)
                    {
                        throw ex;
                    }

                    Task.Delay(timeOut).Wait();
                }
                catch (TimeoutException ex)
                {
                    if (tries >= retries)
                    {
                        throw ex;
                    }

                    Task.Delay(timeOut).Wait();
                }
                catch (AggregateException aggregateException)
                {
                    if (tries >= retries)
                    {
                        throw;
                    }

                    aggregateException.Handle(e =>
                    {
                        if (e is RedisConnectionException || e is TimeoutException || e is RedisServerException)
                        {
                            Task.Delay(timeOut).Wait();
                            return true;
                        }

                        return false;
                    });
                }
            }
            while (tries < retries);

            return default;
        }

        public static void Retry(Action retryme, int timeOut, int retries)
        {
            Retry(
                () =>
                {
                    retryme();
                    return true;
                },
                timeOut,
                retries);
        }
    }
}
