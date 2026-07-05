using ENSACO.RxPlatform.Hosting.Common;
using ENSACO.RxPlatform.Hosting.Interface;
using ENSACO.RxPlatform.Hosting.Internal;
using ENSACO.RxPlatform.Runtime;
using System.Runtime.InteropServices;
using System.Timers;

namespace ENSACO.RxPlatform.Hosting.Threading
{

    internal class ConditionTaskData
    {
        public Func<bool> Condition = () => false;
        public TaskCompletionSource<bool>? ConditionTask = null;
        public Int64 TimeoutTicks = 0;
    }
    internal class ProperyValuesSynhronizator
    {
        static object changesLock = new object();
        static bool timerActive = false;
        static HashSet<Tuple<RxPlatformRuntimeBase, nuint>> changesSet =
            new HashSet<Tuple<RxPlatformRuntimeBase, nuint>>();
        static Dictionary<RxPlatformRuntimeBase, List<Tuple<int, object?>>> changes =
             new Dictionary<RxPlatformRuntimeBase, List<Tuple<int, object?>>>();

        static System.Timers.Timer timer = new System.Timers.Timer(100); // Set interval to 10ms


        static bool run = true;
        static internal void Start()
        {
            timer.AutoReset = true;
            timer.Elapsed += TimerElapsed;
        }
        private static List<KeyValuePair<RxPlatformRuntimeBase, List<Tuple<int, object?>>>>? GetForProcessing()
        { 
            bool startTimer = false;
            List<KeyValuePair<RxPlatformRuntimeBase, List<Tuple<int, object?>>>> toProcess;
            lock (changesLock)
            {
                if(changes.Count == 0)
                    return null;
                toProcess = changes.ToList();
                changes.Clear();
                changesSet.Clear();

                if(timerActive == false)
                {
                    timerActive = true;
                    startTimer = true;
                }
            }
            if(startTimer)
                timer.Start();
            return toProcess;
        }

        private static List<ConditionTaskData>? GetConditions(long ticks)
        {
            List<ConditionTaskData> conditions = new List<ConditionTaskData>();
            lock (changesLock)
            {
                if (conditionTasks.Count == 0)
                    return null;


                foreach (var conditionTask in conditionTasks)
                {
                    if(conditionTask.ConditionTask != null)
                    {
                        conditions.Add(conditionTask);
                    }
                }
            }
            return conditions.Count > 0 ? conditions : null;
        }
        private static void RemoveConditions(List<ConditionTaskData> conditions)
        {
            lock (changesLock)
            {
                if (conditionTasks.Count == 0)
                    return;

                Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss.fff")} RemoveConditions: {conditionTasks.Count}");

                foreach (var condition in conditions)
                {
                    if(condition.ConditionTask == null)
                        continue;
                    Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss.fff")} ConditionTask Removing: {condition.ConditionTask.Task.Id}");

                    conditionTasks.Remove(condition);
                }
                Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss.fff")} RemoveConditions: {conditionTasks.Count}");

            }
        }
        private static void TimerElapsed(object? sender, ElapsedEventArgs e)
        {
            var toProcess = GetForProcessing();

            var ticks = Environment.TickCount64;
            var conditions = GetConditions(ticks);

            if (toProcess != null && toProcess.Count > 0)
            {
                foreach (var item in toProcess)
                {
                    item.Key.__ValuesCallback(item.Value.ToArray());
                }
            }
            lock (changesLock)
            {
                if (conditions != null && conditions.Count > 0)
                {
                    Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss.fff")} TimerElapsed: {Environment.TickCount64}, and has conditions");
                    List<ConditionTaskData> toRemove = new List<ConditionTaskData>();
                    foreach (var condition in conditionTasks)
                    {
                        if (condition.ConditionTask == null)
                            continue;

                        if (ticks >= condition.TimeoutTicks)
                        {
                            Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss.fff")} ConditionTask timeout: {condition.ConditionTask.Task.Id}");
                            condition.ConditionTask.SetResult(false);
                            toRemove.Add(condition);
                        }
                        else
                        {
                            try
                            {
                                bool result = condition.Condition();
                                if (result)
                                {
                                    Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss.fff")} ConditionTask SetResult: {condition.ConditionTask.Task.Id}");
                                    condition.ConditionTask.SetResult(result);
                                    toRemove.Add(condition);
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss.fff")} ConditionTask SetException: {condition.ConditionTask.Task.Id}");
                                condition.ConditionTask.SetException(ex);
                                toRemove.Add(condition);
                            }
                        }
                    }
                    if (toRemove.Count > 0)
                    {
                        foreach (var condition in toRemove)
                        {
                            conditionTasks.Remove(condition);
                        }
                    }
                }
                if (changes.Count > 0 && conditionTasks.Count > 0)
                {
                    timer.Start();
                }
            }
        }

        private static void DoUpdate()
        {
            var toProcess = GetForProcessing();

            if (toProcess == null || toProcess.Count == 0)
                return;


            Task.Run(() =>
            {
                foreach (var item in toProcess)
                {
                    item.Key.__ValuesCallback(item.Value.ToArray());
                }
            });
        }

        static internal void Stop()
        {
            run = false;
            timer.Stop();
            timer.Dispose();

        }
        static List<ConditionTaskData> conditionTasks = new List<ConditionTaskData>();


        internal static Task<bool> AppendCondition(RxPlatformRuntimeBase runtime, Func<bool> condition, UInt32 timeout)
        {
            if (!run)
            {
                return Task.FromResult(false);
            }
            TaskCompletionSource<bool> conditionTask = new TaskCompletionSource<bool>();

            lock (conditionTasks)
            {
                conditionTasks.Add(new ConditionTaskData
                {
                    Condition = condition,
                    ConditionTask = conditionTask,
                    TimeoutTicks = Environment.TickCount64 + timeout
                });
                //
                timer.Start();
            }
            return conditionTask.Task;
        }
        internal unsafe static void RuntimeValueChanged(RxPlatformRuntimeBase whose, nuint idx, object? value)
        {
            if (run)
            {
                lock (changesLock)
                {
                    var changeKey = new Tuple<RxPlatformRuntimeBase, nuint>(whose, idx);
                    if (changesSet.Contains(changeKey))
                    {
                        // already registered change for this property
                        // send previous changes to runtime
                        //DoUpdate();
                    }
                    else
                    {
                        changesSet.Add(changeKey);
                    }
                    if (!changes.TryGetValue(whose, out var list))
                    {
                        list = new List<Tuple<int, object?>>();
                        changes[whose] = list;
                    }
                    list.Add(new Tuple<int, object?>((int)idx, value));
                }
                //
                timer.Start();
            }
        }
    }
}
