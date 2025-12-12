using ENSACO.RxPlatform.Hosting.Common;
using ENSACO.RxPlatform.Hosting.Interface;
using System.Runtime.InteropServices;

namespace ENSACO.RxPlatform.Hosting.Threading
{
    internal class HostThreadingSynchronizator
    {
        internal struct TaskInfo<T> where T : class
        {
            public Task<T?> Task;
            public IntPtr CallbackPtr => Marshal.GetFunctionPointerForDelegate<dotnetRuntimeResultDelegate>(dotnetRuntimeResult);
            public ulong TransId;
        }
        static unsafe void dotnetRuntimeResult(UInt64 transId
            , rx_result_struct result)
        {
            var exception = CommonInterface.GetExceptionFromResult(&result);
            Task.Run(() =>
            {
                TaskCompletionSource<Exception?>? tcs = null;
                lock (RuntimeExceptionTasks)
                {
                    if (RuntimeExceptionTasks.TryGetValue(transId, out tcs))
                    {
                        RuntimeExceptionTasks.Remove(transId);
                    }
                }
                if (tcs != null)
                {
                    tcs.SetResult(exception);
                }
            });
        }
        static UInt64 transId = 0; // TODO: generate transaction id
        static Dictionary<UInt64, TaskCompletionSource<Exception?>> RuntimeExceptionTasks = new Dictionary<UInt64, TaskCompletionSource<Exception?>>();

        internal static TaskInfo<Exception> AppendExceptioned()
        {
            TaskCompletionSource<Exception?> dotnetRuntimeTask = new TaskCompletionSource<Exception?>();
            
            var trans = Interlocked.Increment(ref transId);
            if(trans==0) // avoid 0 transaction
                trans = Interlocked.Increment(ref transId);
            lock (RuntimeExceptionTasks)
            {
                RuntimeExceptionTasks[trans] = dotnetRuntimeTask;
            }

            return new TaskInfo<Exception> {
                Task = dotnetRuntimeTask.Task,
                TransId = trans
            };
        }
    }
}
