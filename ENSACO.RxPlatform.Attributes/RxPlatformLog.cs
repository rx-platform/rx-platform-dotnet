using ENSACO.RxPlatform.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ENSACO.RxPlatform
{
    public class RxPlatformLog
    {

        public static void WriteLogInfo(string source, string message, ushort severity = 200)
        {
            RxPlatformRuntimeBase.WriteLogInfo(source, message, severity);
        }
        public static void WriteLogError(string source, string message, ushort severity = 200)
        {
            RxPlatformRuntimeBase.WriteLogError(source, message, severity);
        }
        public static void WriteLogWarning(string source, string message, ushort severity = 200)
        {
            RxPlatformRuntimeBase.WriteLogWarning(source, message, severity);
        }
        public static void WriteLogDebug(string source, string message, ushort severity = 200)
        {
            RxPlatformRuntimeBase.WriteLogDebug(source, message, severity);
        }
        public static void WriteLogTrace(string source, string message, ushort severity = 200)
        {
            RxPlatformRuntimeBase.WriteLogTrace(source, message, severity);
        }
        public static void WriteLogCritical(string source, string message, ushort severity = 200)
        {
            RxPlatformRuntimeBase.WriteLogCritical(source, message, severity);
        }
    }
}
