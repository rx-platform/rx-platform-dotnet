using ENSACO.RxPlatform.Hosting.Common;
using ENSACO.RxPlatform.Hosting.Interface;
using ENSACO.RxPlatform.Hosting.Internal;
using ENSACO.RxPlatform.Runtime;
using System.Runtime.InteropServices;
using System.Timers;

namespace ENSACO.RxPlatform.Hosting.Threading
{
    internal class ProperyValuesSynhronizator
    {
        static object changesLock = new object();
        static bool timerActive = false;
        static HashSet<Tuple<RxPlatformRuntimeBase, nuint>> changesSet =
            new HashSet<Tuple<RxPlatformRuntimeBase, nuint>>();
        static Dictionary<RxPlatformRuntimeBase, List<Tuple<int, object?>>> changes =
             new Dictionary<RxPlatformRuntimeBase, List<Tuple<int, object?>>>();

        static System.Timers.Timer timer = new System.Timers.Timer(10); // Set interval to 10ms


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
        private static void TimerElapsed(object? sender, ElapsedEventArgs e)
        {
            var toProcess = GetForProcessing();

            if (toProcess == null || toProcess.Count == 0)
                return;

            foreach (var item in toProcess)
            {
                item.Key.__ValuesCallback(item.Value.ToArray());
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
                        DoUpdate();
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
