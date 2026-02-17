using System;
using System.Threading.Tasks;

namespace Abim.Platform.Program.Core.Identity
{
    public static class RetryHelper
    {
        /// <summary>
        /// RetryInterServiceMethod
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="task"></param>
        /// <param name="retryCount">it is number of additonal tries after unsuccessfull attempt</param>
        /// <param name="onUnauthorizedFailureAction"></param>
        /// <returns></returns>
        public static async Task<T> RetryTask<T>(Func<Task<T>> task, Action onUnauthorizedFailureAction, int retryCount = 2)
        {
            T retval = default(T);
            do
            {
                try
                {
                    return await task();
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("Unauthorized was returned"))
                        onUnauthorizedFailureAction();
                    else
                        throw;

                    if (retryCount <= 0)
                        throw;
                }

            } while (retryCount-- > 0);

            return retval;
        }
    }
}
