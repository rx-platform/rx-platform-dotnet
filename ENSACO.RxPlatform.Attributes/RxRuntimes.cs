using ENSACO.RxPlatform.Attributes;
using ENSACO.RxPlatform.Model;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ENSACO.RxPlatform.Runtime
{

    public delegate Task<bool> CreateRuntimesDelegate(object prototype, string name, string path, RxNodeId id, Assembly assembly);
    public delegate Task DeleteRuntimesDelegate(byte type, RxNodeId id);

    public delegate Task<RxPlatformObjectRuntime?> RegisterRuntimesDelegate(byte type, object prototype, string name, string path, RxNodeId id);
    public delegate Task UnregisterRuntimesDelegate(byte typeId, Type type, RxPlatformObjectRuntime instance, string name, string path, RxNodeId id);

    public delegate Task<RxPlatformObjectRuntime?> MonitorRuntimesDelegate(Type monitorTpe, byte type, object prototype, string name, string path, RxNodeId id);
    public delegate Task UnmonitorRuntimesDelegate(byte typeId, Type type, RxPlatformObjectRuntime instance, string name, string path, RxNodeId id);


    public delegate Task<bool> WaitConditionDelegate(RxPlatformRuntimeBase instance, Func<bool> condition, UInt32 timeout);

    public delegate Task<bool> WriteRuntimeBoolDelegate(byte type, IntPtr instance, int index, bool value);
    public delegate Task<bool> WriteRuntimeInt8Delegate(byte type, IntPtr instance, int index, sbyte value);
    public delegate Task<bool> WriteRuntimeInt16Delegate(byte type, IntPtr instance, int index, short value);
    public delegate Task<bool> WriteRuntimeInt32Delegate(byte type, IntPtr instance, int index, int value);
    public delegate Task<bool> WriteRuntimeInt64Delegate(byte type, IntPtr instance, int index, long value);
    public delegate Task<bool> WriteRuntimeUInt8Delegate(byte type, IntPtr instance, int index, byte value);
    public delegate Task<bool> WriteRuntimeUInt16Delegate(byte type, IntPtr instance, int index, ushort value);
    public delegate Task<bool> WriteRuntimeUInt32Delegate(byte type, IntPtr instance, int index, uint value);
    public delegate Task<bool> WriteRuntimeUInt64Delegate(byte type, IntPtr instance, int index, ulong value);
    public delegate Task<bool> WriteRuntimeFloatDelegate(byte type, IntPtr instance, int index, float value);
    public delegate Task<bool> WriteRuntimeDoubleDelegate(byte type, IntPtr instance, int index, double value);
    public delegate Task<bool> WriteRuntimeStringDelegate(byte type, IntPtr instance, int index, string value);
    public delegate Task<bool> WriteRuntimeDateTimeDelegate(byte type, IntPtr instance, int index, DateTime value);
    public delegate Task<bool> WriteRuntimeBytesDelegate(byte type, IntPtr instance, int index, byte[] value);
    public delegate Task<bool> WriteRuntimeUuidDelegate(byte type, IntPtr instance, int index, Guid value);
    public delegate Task<bool> WriteRuntimeObjectDelegate(byte type, IntPtr instance, int index, object value);
    public delegate Task<string> ExecuteRuntimeObjectDelegate(byte type, IntPtr instance, int index, object? value);



    public delegate void SourceExecuteDoneDelegate(IntPtr instance, UInt32 transId, string value, Exception? exception);

    public delegate void SourceWriteDoneDelegate(IntPtr instance, UInt32 transId, Exception? exception);

    public delegate void MapperWriteDelegate(IntPtr instance, UInt32 transId,object? value);

    public delegate void MapperExecuteDelegate(IntPtr instance, UInt32 transId, object? value);

    public delegate void MapCurrentDelegate(IntPtr instance);

    // source changed delegates
    public delegate void SourceChangedBoolDelegate(IntPtr instance, bool value);
    public delegate void SourceChangedInt8Delegate(IntPtr instance, sbyte value);
    public delegate void SourceChangedInt16Delegate(IntPtr instance, short value);
    public delegate void SourceChangedInt32Delegate(IntPtr instance, int value);
    public delegate void SourceChangedInt64Delegate(IntPtr instance, long value);
    public delegate void SourceChangedUInt8Delegate(IntPtr instance, byte value);
    public delegate void SourceChangedUInt16Delegate(IntPtr instance, ushort value);
    public delegate void SourceChangedUInt32Delegate(IntPtr instance, uint value);
    public delegate void SourceChangedUInt64Delegate(IntPtr instance, ulong value);
    public delegate void SourceChangedFloatDelegate(IntPtr instance, float value);
    public delegate void SourceChangedDoubleDelegate(IntPtr instance, double value);
    public delegate void SourceChangedStringDelegate(IntPtr instance, string value);
    public delegate void SourceChangedDateTimeDelegate(IntPtr instance, DateTime value);
    public delegate void SourceChangedBytesDelegate(IntPtr instance, byte[] value);
    public delegate void SourceChangedUuidDelegate(IntPtr instance, Guid value);
    public delegate void SourceChangedObjectDelegate(IntPtr instance, object value);
    public delegate void SourceChangedBadDelegate(IntPtr instance);

    public delegate void FireEventDelegate(IntPtr instance, string value, string queue, bool state, bool remove);

    public delegate RxPlatformObjectRuntime? GetInstanceDelegate(IntPtr instancePtr);

    public delegate void WriteLogDelegate(string source, ushort severity, string message);

    public struct RxRuntimeDelegates
    {
        public MonitorRuntimesDelegate? MonitorRuntimes;
        public UnmonitorRuntimesDelegate? UnmonitorRuntimes;
        public RegisterRuntimesDelegate? RegisterRuntimes;
        public UnregisterRuntimesDelegate? UnregisterRuntimes;
        public CreateRuntimesDelegate? CreateRuntimes;
        public DeleteRuntimesDelegate? DeleteRuntimes;

        public WaitConditionDelegate? WaitCondition;

        public WriteRuntimeBoolDelegate? WriteBoolRuntime;
        public WriteRuntimeInt8Delegate? WriteInt8Runtime;
        public WriteRuntimeInt16Delegate? WriteInt16Runtime;
        public WriteRuntimeInt32Delegate? WriteInt32Runtime;
        public WriteRuntimeInt64Delegate? WriteInt64Runtime;
        public WriteRuntimeUInt8Delegate? WriteUInt8Runtime;
        public WriteRuntimeUInt16Delegate? WriteUInt16Runtime;
        public WriteRuntimeUInt32Delegate? WriteUInt32Runtime;
        public WriteRuntimeUInt64Delegate? WriteUInt64Runtime;
        public WriteRuntimeFloatDelegate? WriteFloatRuntime;
        public WriteRuntimeDoubleDelegate? WriteDoubleRuntime;
        public WriteRuntimeStringDelegate? WriteStringRuntime;
        public WriteRuntimeDateTimeDelegate? WriteDateTimeRuntime;
        public WriteRuntimeBytesDelegate? WriteBytesRuntime;
        public WriteRuntimeUuidDelegate? WriteUuidRuntime;
        public WriteRuntimeObjectDelegate? WriteObjectRuntime;


        public SourceChangedBoolDelegate? SourceChangedBool;
        public SourceChangedInt8Delegate? SourceChangedInt8;
        public SourceChangedInt16Delegate? SourceChangedInt16;
        public SourceChangedInt32Delegate? SourceChangedInt32;
        public SourceChangedInt64Delegate? SourceChangedInt64;
        public SourceChangedUInt8Delegate? SourceChangedUInt8;
        public SourceChangedUInt16Delegate? SourceChangedUInt16;
        public SourceChangedUInt32Delegate? SourceChangedUInt32;
        public SourceChangedUInt64Delegate? SourceChangedUInt64;
        public SourceChangedFloatDelegate? SourceChangedFloat;
        public SourceChangedDoubleDelegate? SourceChangedDouble;
        public SourceChangedStringDelegate? SourceChangedString;
        public SourceChangedDateTimeDelegate? SourceChangedDateTime;
        public SourceChangedBytesDelegate? SourceChangedBytes;
        public SourceChangedUuidDelegate? SourceChangedUuid;
        public SourceChangedObjectDelegate? SourceChangedObject;
        public SourceChangedBadDelegate? SourceChangedBad;

        public FireEventDelegate? FireEvent;

        public GetInstanceDelegate? GetInstance;

        public ExecuteRuntimeObjectDelegate? ExecuteObjectRuntime;

        public WriteLogDelegate? WriteLogInfo;
        public WriteLogDelegate? WriteLogError;
        public WriteLogDelegate? WriteLogWarning;
        public WriteLogDelegate? WriteLogDebug;
        public WriteLogDelegate? WriteLogTrace;
        public WriteLogDelegate? WriteLogCritical;
    }

    public class RxPlatformRuntimeBase
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public TaskCompletionSource? __initSource = null;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        static protected JsonSerializerOptions __jsonContext = new JsonSerializerOptions { };

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        static internal RxRuntimeDelegates __runtimeFunctions;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal IntPtr __nativeObjectPtr = IntPtr.Zero;
        public static void __InitRuntime(RxRuntimeDelegates delegates, JsonSerializerOptions jsonContext)
        {
            __runtimeFunctions = delegates;
            __jsonContext = jsonContext;
        }
        public string __GetCodeInfo()
        {
            return "";
        }
        public void __BindObject(IntPtr nativeObjectPtr, RxNodeId id, string path)
        {
            lock (this)
            {
                this.__nativeObjectPtr = nativeObjectPtr;
                this.id = id;
                this.path = path;
            }
        }
        object? ConvertObject(object? value, Type targetType)
        {
            if (value == null)
                return null;
            try
            {
                var underlyingType = Nullable.GetUnderlyingType(targetType);
                return Convert.ChangeType(value, underlyingType ?? targetType);
            }
            catch
            {
                return null;
            }
        }
        public void __rxInitialValuesCallback(Tuple<string, object?>[] values, Dictionary<string, object> children)
        {
            string fieldName;
            foreach (var value in values)
            {
                if (value.Item2 != null)
                {
                    fieldName = value.Item1 + "____rxImplementation";
                    var fieldInfo = this.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
                    
                    var propInfo = this.GetType().GetProperty(value.Item1);
                    if (propInfo != null && propInfo.CanWrite)
                    {
                        var underlyingType = Nullable.GetUnderlyingType(propInfo.PropertyType);
                        Type propType = underlyingType != null ? underlyingType : propInfo.PropertyType;
                        if (propType.GetCustomAttribute<RxPlatformDataType>() != null)
                        {
                            string jsonStr = value.Item2 as string ?? "{ }";
                            var convertedValue = JsonSerializer.Deserialize(jsonStr, propType, __jsonContext);
                            if (fieldInfo != null)
                            {
                                fieldInfo.SetValue(this, convertedValue);
                            }
                            else
                            {
                                propInfo.SetValue(this, convertedValue);
                            }
                        }
                        else
                        {
                            if (!propType.IsAssignableFrom(value.Item2.GetType()))
                            {
                                // type mismatch, try to convert
                                var convertedValue = ConvertObject(value.Item2, propType);
                                if (convertedValue != null)
                                {
                                    if (fieldInfo != null)
                                    {
                                        fieldInfo.SetValue(this, convertedValue);
                                    }
                                    else
                                    {
                                        propInfo.SetValue(this, convertedValue);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            foreach (var value in children)
            {
                fieldName = value.Key + "____rxImplementation";
                var fieldInfo = this.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
                if (fieldInfo != null)
                {
                    fieldInfo.SetValue(this, value.Value);
                }
            }
            if (__initSource != null)
            {
                __initSource.SetResult();
            }
        }
        protected virtual void __rxValueCallback(int index, object? value)
        {
        }
        public void __ValuesCallback(Tuple<int, object?>[] values)
        {
            lock (this)
            {
                foreach (var value in values)
                {
                    __rxValueCallback(value.Item1, value.Item2);
                }
            }
        }
        public void __rxStructSerialize(Utf8JsonWriter writer)
        {
            JsonSerializer.Serialize(writer, this, this.GetType(), __jsonContext);
        }
        //public void __rxStructDeserialize(ref Utf8JsonReader reader)
        //{
        //    var obj = JsonSerializer.Deserialize(ref reader, this.GetType(), __jsonContext);
        //    if (obj != null)
        //    {
        //        foreach (var prop in this.GetType().GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance))
        //        {
        //            var strSrc = prop.GetValue(obj) as RxPlatformStructRuntime;
        //            if (strSrc != null)
        //            {
        //                var strRt = prop.GetValue(this) as RxPlatformStructRuntime;
        //                if (strRt != null)
        //                {
        //                    MemoryStream ms = new MemoryStream();
        //                    Utf8JsonWriter writer = new Utf8JsonWriter(ms);
        //                    strSrc.__rxStructSerialize(writer);
        //                    writer.Flush();
        //                    ms.Position = 0;
        //                    Utf8JsonReader tempReader = new Utf8JsonReader(ms.ToArray());
        //                    strRt.__rxStructDeserialize(ref tempReader);
        //                }
        //            }
        //            else
        //            {

        //                var eventSrc = prop.GetValue(obj) as RxPlatformEventRuntime;
        //                if (eventSrc != null)
        //                {
        //                    var eventRt = prop.GetValue(this) as RxPlatformEventRuntime;
        //                    if (eventRt != null)
        //                    {
        //                        MemoryStream ms = new MemoryStream();
        //                        Utf8JsonWriter writer = new Utf8JsonWriter(ms);
        //                        eventSrc.__rxStructSerialize(writer);
        //                        writer.Flush();
        //                        ms.Position = 0;
        //                        Utf8JsonReader tempReader = new Utf8JsonReader(ms.ToArray());
        //                        eventRt.__rxStructDeserialize(ref tempReader);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}
        internal RxNodeId id = new RxNodeId();
        internal string? name = null;
        internal string path = "";
       
        public RxPlatformRuntimeBase()
        {
        }
        static public Task CreateObject(object prototype, string name, string path = "", RxNodeId id = new RxNodeId(), Assembly? assembly = null)
        {
            if (__runtimeFunctions.CreateRuntimes != null)
            {
                if (string.IsNullOrEmpty(name))
                {
                    throw new Exception("RxPlatformObjectRuntime: Name cannot be null or empty!");
                }
                if(assembly == null)
                    assembly = Assembly.GetCallingAssembly();
                return __runtimeFunctions.CreateRuntimes(prototype, name, path, id, assembly);
            }
            else
            {
                throw new Exception("RxPlatformObjectRuntime: Runtime delegates not initialized!");
            }
        }

        protected Task<bool> WaitCondition(Func<bool> condition, UInt32 timeout)
        {
            return WaitCondition(this, condition, timeout);
        }
        protected Task<bool> __WriteProperty(int index, bool value)
        {
            return WriteBoolProperty(this, index, value);
        }
        protected Task<bool> __WriteProperty(int index, sbyte value)
        {
            return WriteInt8Property(this, index, value);
        }
        protected Task<bool> __WriteProperty(int index, short value)
        {
            return WriteInt16Property(this, index, value);
        }
        protected Task<bool> __WriteProperty(int index, int value)
        {
            return WriteInt32Property(this, index, value);
        }
        protected Task<bool> __WriteProperty(int index, long value)
        {
            return WriteInt64Property(this, index, value);
        }
        protected Task<bool> __WriteProperty(int index, byte value)
        {
            return WriteUInt8Property(this, index, value);
        }
        protected Task<bool> __WriteProperty(int index, ushort value)
        {
            return WriteUInt16Property(this, index, value);
        }        protected Task<bool> __WriteProperty(int index, uint value)
        {
            return WriteUInt32Property(this, index, value);
        }        protected Task<bool> __WriteProperty(int index, ulong value)
        {
            return WriteUInt64Property(this, index, value);
        }
        protected Task<bool> __WriteProperty(int index, float value)
        {
            return WriteFloatProperty(this, index, value);
        }
        protected Task<bool> __WriteProperty(int index, double value)
        {
            return WriteDoubleProperty(this, index, value);
        }
        protected Task<bool> __WriteProperty(int index, string value)
        {
            return WriteStringProperty(this, index, value);
        }
        protected Task<bool> __WriteProperty(int index, DateTime value)
        {
            return WriteDateTimeProperty(this, index, value);
        }
        protected Task<bool> __WriteProperty(int index, byte[] value)
        {
            return WriteBytesProperty(this, index, value);
        }
        protected Task<bool> __WriteProperty(int index, Guid value)
        {
            return WriteUuidProperty(this, index, value);
        }
        protected Task<bool> __WriteProperty(int index, object value)
        {
            return WriteObjectProperty(this, index, value);
        }
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        virtual internal byte RxType { get { return 0; } }
        static async Task<bool> WaitCondition(RxPlatformRuntimeBase whose, Func<bool> condition, UInt32 timeout)
        {
            if (__runtimeFunctions.WaitCondition != null && whose.__nativeObjectPtr != IntPtr.Zero)
            {
                return await __runtimeFunctions.WaitCondition(whose, condition, timeout);
            }
            // Placeholder for property write logic
            await Task.CompletedTask;
            return false;
        }
        static async Task<bool> WriteBoolProperty(RxPlatformRuntimeBase whose, int index, bool value)
        {
            if (__runtimeFunctions.WriteBoolRuntime != null && whose.__nativeObjectPtr != IntPtr.Zero)
            {
                return await __runtimeFunctions.WriteBoolRuntime(whose.RxType, whose.__nativeObjectPtr, index, value);
            }
            // Placeholder for property write logic
            await Task.CompletedTask;
            return false;
        }
        static async Task<bool> WriteInt8Property(RxPlatformRuntimeBase whose, int index, sbyte value)
        {
            if (__runtimeFunctions.WriteInt8Runtime != null && whose.__nativeObjectPtr != IntPtr.Zero)
            {
                return await __runtimeFunctions.WriteInt8Runtime(whose.RxType, whose.__nativeObjectPtr, index, value);
            }
            // Placeholder for property write logic
            await Task.CompletedTask;
            return false;
        }
        static async Task<bool> WriteInt16Property(RxPlatformRuntimeBase whose, int index, short value)
        {
            if (__runtimeFunctions.WriteInt16Runtime != null && whose.__nativeObjectPtr != IntPtr.Zero)
            {
                return await __runtimeFunctions.WriteInt16Runtime(whose.RxType, whose.__nativeObjectPtr, index, value);
            }
            // Placeholder for property write logic
            await Task.CompletedTask;
            return false;
        }       
        static async Task<bool> WriteInt32Property(RxPlatformRuntimeBase whose, int index, int value)
        {
            if (__runtimeFunctions.WriteInt32Runtime != null && whose.__nativeObjectPtr != IntPtr.Zero)
            {
                return await __runtimeFunctions.WriteInt32Runtime(whose.RxType, whose.__nativeObjectPtr, index, value);
            }
            // Placeholder for property write logic
            await Task.CompletedTask;
            return false;
        }
        static async Task<bool> WriteInt64Property(RxPlatformRuntimeBase whose, int index, long value)
        {
            if (__runtimeFunctions.WriteInt64Runtime != null && whose.__nativeObjectPtr != IntPtr.Zero)
            {
                return await __runtimeFunctions.WriteInt64Runtime(whose.RxType, whose.__nativeObjectPtr, index, value);
            }
            // Placeholder for property write logic
            await Task.CompletedTask;
            return false;
        }
        static async Task<bool> WriteUInt8Property(RxPlatformRuntimeBase whose, int index, byte value)
        {
            if (__runtimeFunctions.WriteUInt8Runtime != null && whose.__nativeObjectPtr != IntPtr.Zero)
            {
                return await __runtimeFunctions.WriteUInt8Runtime(whose.RxType, whose.__nativeObjectPtr, index, value);
            }
            // Placeholder for property write logic
            await Task.CompletedTask;
            return false;
        }
        static async Task<bool> WriteUInt16Property(RxPlatformRuntimeBase whose, int index, ushort value)
        {
            if (__runtimeFunctions.WriteUInt16Runtime != null && whose.__nativeObjectPtr != IntPtr.Zero)
            {
                return await __runtimeFunctions.WriteUInt16Runtime(whose.RxType, whose.__nativeObjectPtr, index, value);
            }
            // Placeholder for property write logic
            await Task.CompletedTask;
            return false;
        }
        static async Task<bool> WriteUInt32Property(RxPlatformRuntimeBase whose, int index, uint value)
        {
            if (__runtimeFunctions.WriteUInt32Runtime != null && whose.__nativeObjectPtr != IntPtr.Zero)
            {
                return await __runtimeFunctions.WriteUInt32Runtime(whose.RxType, whose.__nativeObjectPtr, index, value);
            }
            // Placeholder for property write logic
            await Task.CompletedTask;
            return false;
        }
        static async Task<bool> WriteUInt64Property(RxPlatformRuntimeBase whose, int index, ulong value)
        {
            if (__runtimeFunctions.WriteUInt64Runtime != null && whose.__nativeObjectPtr != IntPtr.Zero)
            {
                return await __runtimeFunctions.WriteUInt64Runtime(whose.RxType, whose.__nativeObjectPtr, index, value);
            }
            // Placeholder for property write logic
            await Task.CompletedTask;
            return false;
        }
        static async Task<bool> WriteFloatProperty(RxPlatformRuntimeBase whose, int index, float value)
        {
            if (__runtimeFunctions.WriteFloatRuntime != null && whose.__nativeObjectPtr != IntPtr.Zero)
            {
                return await __runtimeFunctions.WriteFloatRuntime(whose.RxType, whose.__nativeObjectPtr, index, value);
            }
            // Placeholder for property write logic
            await Task.CompletedTask;
            return false;
        }
        static async Task<bool> WriteDoubleProperty(RxPlatformRuntimeBase whose, int index, double value)
        {
            if (__runtimeFunctions.WriteDoubleRuntime != null && whose.__nativeObjectPtr != IntPtr.Zero)
            {
                return await __runtimeFunctions.WriteDoubleRuntime(whose.RxType, whose.__nativeObjectPtr, index, value);
            }
            // Placeholder for property write logic
            await Task.CompletedTask;
            return false;
        }
        static async Task<bool> WriteStringProperty(RxPlatformRuntimeBase whose, int index, string value)
        {
            if (__runtimeFunctions.WriteStringRuntime != null && whose.__nativeObjectPtr != IntPtr.Zero)
            {
                return await __runtimeFunctions.WriteStringRuntime(whose.RxType, whose.__nativeObjectPtr, index, value);
            }
            // Placeholder for property write logic
            await Task.CompletedTask;
            return false;
        }
        static async Task<bool> WriteDateTimeProperty(RxPlatformRuntimeBase whose, int index, DateTime value)
        {
            if (__runtimeFunctions.WriteDateTimeRuntime != null && whose.__nativeObjectPtr != IntPtr.Zero)
            {
                return await __runtimeFunctions.WriteDateTimeRuntime(whose.RxType, whose.__nativeObjectPtr, index, value);
            }
            // Placeholder for property write logic
            await Task.CompletedTask;
            return false;
        }
        static async Task<bool> WriteBytesProperty(RxPlatformRuntimeBase whose, int index, byte[] value)
        {
            if (__runtimeFunctions.WriteBytesRuntime != null && whose.__nativeObjectPtr != IntPtr.Zero)
            {
                return await __runtimeFunctions.WriteBytesRuntime(whose.RxType, whose.__nativeObjectPtr, index, value);
            }
            // Placeholder for property write logic
            await Task.CompletedTask;
            return false;
        }
        static async Task<bool> WriteUuidProperty(RxPlatformRuntimeBase whose, int index, Guid value)
        {
            if (__runtimeFunctions.WriteUuidRuntime != null && whose.__nativeObjectPtr != IntPtr.Zero)
            {
                return await __runtimeFunctions.WriteUuidRuntime(whose.RxType, whose.__nativeObjectPtr, index, value);
            }
            // Placeholder for property write logic
            await Task.CompletedTask;
            return false;
        }
        static async Task<bool> WriteObjectProperty(RxPlatformRuntimeBase whose, int index, object value)
        {
            if (__runtimeFunctions.WriteObjectRuntime != null && whose.__nativeObjectPtr != IntPtr.Zero)
            {
                return await __runtimeFunctions.WriteObjectRuntime(whose.RxType, whose.__nativeObjectPtr, index, value);
            }
            // Placeholder for property write logic
            await Task.CompletedTask;
            return false;
        }

        internal static void WriteLogInfo(string source, string message, ushort severity)
        {
            if (__runtimeFunctions.WriteLogInfo != null)
            {
                __runtimeFunctions.WriteLogInfo(source, severity, message);
            }
        }
        internal static void WriteLogError(string source, string message, ushort severity)
        {
            if (__runtimeFunctions.WriteLogError != null)
            {
                __runtimeFunctions.WriteLogError(source, severity, message);
            }
        }
        internal static void WriteLogWarning(string source, string message, ushort severity)
        {
            if (__runtimeFunctions.WriteLogWarning != null)
            {
                __runtimeFunctions.WriteLogWarning(source, severity, message);
            }
        }
        internal static void WriteLogDebug(string source, string message, ushort severity)
        {
            if (__runtimeFunctions.WriteLogDebug != null)
            {
                __runtimeFunctions.WriteLogDebug(source, severity, message);
            }
        }
        internal static void WriteLogTrace(string source, string message, ushort severity)
        {
            if (__runtimeFunctions.WriteLogTrace != null)
            {
                __runtimeFunctions.WriteLogTrace(source, severity, message);
            }
        }
        internal static void WriteLogCritical(string source, string message, ushort severity)
        {
            if (__runtimeFunctions.WriteLogCritical != null)
            {
                __runtimeFunctions.WriteLogCritical(source, severity, message);
            }
        }
    }
    public class RxPlatformObjectRuntime : RxPlatformRuntimeBase, IDisposable, IAsyncDisposable
    {
        ~RxPlatformObjectRuntime()
        {
            Dispose();
        }
        public void Dispose()
        {
            if (name != null && __runtimeFunctions.UnregisterRuntimes != null)
            {
                RxNodeId localId = new RxNodeId();
                lock (this)
                {
                    if (!this.id.IsNull())
                        localId = this.id;
                }
                if (!localId.IsNull() && __runtimeFunctions.UnregisterRuntimes != null)
                {
                    try
                    {
                        // fire and forget
                        _ = __runtimeFunctions.UnregisterRuntimes(RxType, GetType(), this, name, path, localId);
                    }
                    catch (NullReferenceException)
                    {
                        // This should only happen if the runtime functions were not properly initialized
#if DEBUG
                        System.Diagnostics.Debug.Assert(false);
#endif
                    }
                    catch (TaskCanceledException)
                    {
                        // this one i don't care
                    }
                    catch (Exception)
                    {
                        // Log the exception or handle it as needed
                    }
                }
            }
            GC.SuppressFinalize(this);
        }
        public async ValueTask DisposeAsync()
        {
            if (name != null && __runtimeFunctions.UnregisterRuntimes != null)
            {
                RxNodeId localId = new RxNodeId();
                lock (this)
                {
                    if (!this.id.IsNull())
                        localId = this.id;
                }
                if (!localId.IsNull())
                {
                    if(__initSource != null)
                    {
                        __initSource.TrySetResult();
                    }
                    try
                    {
                        await __runtimeFunctions.UnregisterRuntimes(RxType, GetType(), this, name, path, localId);
                    }
                    catch (NullReferenceException)
                    {
                        // This should only happen if the runtime functions were not properly initialized
#if DEBUG
                        System.Diagnostics.Debug.Assert(false);
#endif
                    }
                    catch (Exception)
                    {
                        // Log the exception or handle it as needed
                        System.Diagnostics.Debug.Assert(false);
                    }
                }
            }
            GC.SuppressFinalize(this);
        }
        [JsonIgnore()]
        public RxNodeId NodeId
        {
            get
            {
                lock (this)
                {
                    return this.id;
                }
            }
        }
        [JsonIgnore()]
        public string Path
        {
            get
            {
                lock (this)
                {
                    return this.path;
                }
            }
        }
        override internal byte RxType { get { return 5;/*rx_object*/ } }
        static public async Task<T?> CreateInstance<T>(T prototype, string name, string path = "", RxNodeId id = new RxNodeId()) where T : RxPlatformObjectRuntime, new()
        {
            T? instance = null;
            if (__runtimeFunctions.RegisterRuntimes != null)
            {
                if (string.IsNullOrEmpty(name))
                {
                    throw new Exception("RxPlatformObjectRuntime: Name cannot be null or empty!");
                }
                instance = await __runtimeFunctions.RegisterRuntimes(5/*rx_object*/, prototype, name, path, id) as T;
                if (instance != null)
                {
                    instance.name = name;
                    if(instance.__initSource != null)
                    {
                        await instance.__initSource.Task;
                    }
                }
            }
            return instance;
        }

        static public async Task<T?> MonitorInstance<T>(object prototype, string name, string path = "", RxNodeId id = new RxNodeId()) where T : RxPlatformObjectRuntime, new()
        {
            T? instance = null;
            if (__runtimeFunctions.MonitorRuntimes != null)
            {
                if (string.IsNullOrEmpty(name) && id.IsNull())
                {
                    throw new Exception("RxPlatformObjectRuntime: either name or id must be provided!");
                }
                instance = await __runtimeFunctions.MonitorRuntimes(typeof(T),  5/*rx_object*/, prototype, name, path, id) as T;
                if (instance != null)
                {
                    instance.name = name;
                    if (instance.__initSource != null)
                    {
                        await instance.__initSource.Task;
                    }
                }
            }
            return instance;
        }


        protected Task<string> __ExecuteProperty(int index, object? value)
        {
            return ExecuteObjectProperty(this, index, value);
        }
        public virtual Task<string> __rxExecuteMethod(string method, string args)
        {
            throw new NotImplementedException("Execute method not overridden");
        }
        public RxPlatformObjectRuntime? __GetInstance(IntPtr instancePtr)
        {
            if (__runtimeFunctions.GetInstance != null)
            {
                return __runtimeFunctions.GetInstance(instancePtr);
            }
            return null;
        }


        static async Task<string> ExecuteObjectProperty(RxPlatformRuntimeBase whose, int index, object? value)
        {
            if (__runtimeFunctions.ExecuteObjectRuntime != null && whose.__nativeObjectPtr != IntPtr.Zero)
            {
                return await __runtimeFunctions.ExecuteObjectRuntime(whose.RxType, whose.__nativeObjectPtr, index, value);
            }
            // Placeholder for property write logic
            await Task.CompletedTask;
            return string.Empty;
        }
    }

    public class RxPlatformStructRuntime : RxPlatformRuntimeBase
    {
        override internal byte RxType { get { return 9;/*rx_struct_type*/ } }

    }
    public class RxPlatformEventRuntime : RxPlatformRuntimeBase
    {
        override internal byte RxType { get { return 13;/*rx_event_type*/ } }

        protected void __FireEvent(string value, string queue, bool state, bool remove)
        {
            if (__runtimeFunctions.FireEvent != null && __nativeObjectPtr != IntPtr.Zero)
            {
                __runtimeFunctions.FireEvent(__nativeObjectPtr, value, queue, state, remove);
            }
        }

    }
    
    public class RxPlatformDisplayRuntime : RxPlatformRuntimeBase
    {
        override internal byte RxType { get { return 14;/*rx_display_type*/ } }

    }
}