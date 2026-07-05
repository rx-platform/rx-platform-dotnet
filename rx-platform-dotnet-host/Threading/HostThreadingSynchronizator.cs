using ENSACO.RxPlatform.Hosting.Common;
using ENSACO.RxPlatform.Hosting.Interface;
using ENSACO.RxPlatform.Runtime;
using System.Runtime.InteropServices;

namespace ENSACO.RxPlatform.Hosting.Threading
{
    internal class ExecuteResponse
    {
        public Exception? Exception;
        public string Value = "";
    }
    internal class HostThreadingSynchronizator
    {
        internal struct TaskInfo<T> where T : class
        {
            internal TaskInfo(IntPtr callback, Task<T?> task, ulong transId)
            {
                CallbackPtr = callback;
                Task = task;
                TransId = transId;
            }
            public Task<T?> Task;
            public IntPtr CallbackPtr = IntPtr.Zero;
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

        static unsafe void dotnetExecuteRuntimeResult(UInt64 transId, char* value
            , rx_result_struct result)
        {
            string strVal = "";
            if (value != null)
            {
                var temp = Marshal.PtrToStringUTF8((IntPtr)value) ?? "";
                if (temp != null)
                    strVal = temp;
            }
            var exception = CommonInterface.GetExceptionFromResult(&result);
            Task.Run(() =>
            {
                TaskCompletionSource<ExecuteResponse?>? tcs = null;
                lock (RuntimeExecuteTasks)
                {
                    if (RuntimeExecuteTasks.TryGetValue(transId, out tcs))
                    {
                        RuntimeExecuteTasks.Remove(transId);
                    }
                }
                if (tcs != null)
                {
                    tcs.SetResult(new ExecuteResponse {  Value = strVal, Exception = exception });
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

            return new TaskInfo<Exception>(
                Marshal.GetFunctionPointerForDelegate<dotnetRuntimeResultDelegate>(dotnetRuntimeResult),
                dotnetRuntimeTask.Task, trans);
        }
        static Dictionary<UInt64, TaskCompletionSource<ExecuteResponse?>> RuntimeExecuteTasks = new Dictionary<UInt64, TaskCompletionSource<ExecuteResponse?>>();

        internal unsafe static TaskInfo<ExecuteResponse> AppendExecuteResponse()
        {
            TaskCompletionSource<ExecuteResponse?> dotnetRuntimeTask = new TaskCompletionSource<ExecuteResponse?>();
            var trans = Interlocked.Increment(ref transId);
            if (trans == 0) // avoid 0 transaction
                trans = Interlocked.Increment(ref transId);
            lock (RuntimeExecuteTasks)
            {
                RuntimeExecuteTasks[trans] = dotnetRuntimeTask;
            }

            return new TaskInfo<ExecuteResponse>(
                Marshal.GetFunctionPointerForDelegate<dotnetExecuteRuntimeResultDelegate>(dotnetExecuteRuntimeResult),
                dotnetRuntimeTask.Task, trans);
        }
    }
}
