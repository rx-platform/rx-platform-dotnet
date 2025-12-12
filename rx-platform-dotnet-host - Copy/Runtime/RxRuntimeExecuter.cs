using ENSACO.RxPlatform.Hosting.Internal;
using ENSACO.RxPlatform.Hosting.Model;
using ENSACO.RxPlatform.Runtime;
using RxPlatform.Hosting.Interface;
using System.Reflection;
using System.Runtime.InteropServices;

namespace ENSACO.RxPlatform.Hosting.Runtime
{
    internal static class RxRuntimeExecuter
    {
        static RxPlatformRuntimeBase? GetRuntime(rx_item_type type, nint whose, ref Action? started)
        {

            switch (type)
            {
                case rx_item_type.rx_object:
                    {
                        PlatformRuntimeData<RxPlatformObjectRuntime> obj;
                        lock (RxMetaData.Instance.TypesLock)
                        {
                            if (RxMetaData.Instance.ObjectRuntimes.NativeDict.TryGetValue(whose, out obj))
                            {
                                started = obj.startedMethod;
                                return obj.objectRuntime;
                            }
                        }
                        break;
                    }
                case rx_item_type.rx_source_type:
                    {
                        PlatformRuntimeData<RxPlatformSourceRuntime> src;
                        lock (RxMetaData.Instance.TypesLock)
                        {
                            if (RxMetaData.Instance.SourceRuntimes.NativeDict.TryGetValue(whose, out src))
                            {
                                started = src.startedMethod;
                                return src.objectRuntime;
                            }
                        }
                        break;
                    }
                case rx_item_type.rx_mapper_type:
                    {
                        PlatformRuntimeData<RxPlatformMapperRuntime> map;
                        lock (RxMetaData.Instance.TypesLock)
                        {
                            if (RxMetaData.Instance.MapperRuntimes.NativeDict.TryGetValue(whose, out map))
                            {
                                started = map.startedMethod;
                                return map.objectRuntime;
                            }
                        }
                        break;
                    }
                case rx_item_type.rx_event_type:
                    {
                        PlatformRuntimeData<RxPlatformEventRuntime> evt;
                        lock (RxMetaData.Instance.TypesLock)
                        {
                            if (RxMetaData.Instance.EventRuntimes.NativeDict.TryGetValue(whose, out evt))
                            {
                                started = evt.startedMethod;
                                return evt.objectRuntime;
                            }
                        }
                        break;
                    }
                default:
                    break;
            }
            return null;
        }

        static RxPlatformRuntimeBase? GetRuntime(rx_item_type type, nint whose)
        {
            
            switch (type)
            {
                case rx_item_type.rx_object:
                    {
                        PlatformRuntimeData<RxPlatformObjectRuntime> obj;
                        lock (RxMetaData.Instance.TypesLock)
                        {
                            if (RxMetaData.Instance.ObjectRuntimes.NativeDict.TryGetValue(whose, out obj))
                            {
                                return obj.objectRuntime;
                            }
                        }
                        break;
                    }
                case rx_item_type.rx_source_type:
                    {
                        PlatformRuntimeData<RxPlatformSourceRuntime> src;
                        lock (RxMetaData.Instance.TypesLock)
                        {
                            if (RxMetaData.Instance.SourceRuntimes.NativeDict.TryGetValue(whose, out src))
                            {
                                return src.objectRuntime;
                            }
                        }
                        break;
                    }
                case rx_item_type.rx_mapper_type:
                    {
                        PlatformRuntimeData<RxPlatformMapperRuntime> map;
                        lock (RxMetaData.Instance.TypesLock)
                        {
                            if (RxMetaData.Instance.MapperRuntimes.NativeDict.TryGetValue(whose, out map))
                            {
                                return map.objectRuntime;
                            }
                        }
                        break;
                    }
                case rx_item_type.rx_event_type:
                    {
                        PlatformRuntimeData<RxPlatformEventRuntime> evt;
                        lock (RxMetaData.Instance.TypesLock)
                        {
                            if (RxMetaData.Instance.EventRuntimes.NativeDict.TryGetValue(whose, out evt))
                            {
                                return evt.objectRuntime;
                            }
                        }
                        break;
                    }
                default:
                    break;
            }
            return null;
        }
        static RxPlatformObjectRuntime? GetObject(nint whose)
        {
            PlatformRuntimeData<RxPlatformObjectRuntime> obj;
            lock (RxMetaData.Instance.TypesLock)
            {
                if (RxMetaData.Instance.ObjectRuntimes.NativeDict.TryGetValue(whose, out obj))
                {
                    return obj.objectRuntime;
                }
            }
            return null;
        }
        // called by managed runtimes to write property values
        internal static async Task<bool> WriteProperty(byte type, nint whose, int index, object? value)
        {
            RxPlatformObject.Instance.WriteLogDebug("PlatformRuntimeTypes.LibraryWrite", 100
                , $"Writing runtime Object with ptr 0x{whose.ToString("X")}, at index {index}.");


            RxPlatformObject.Instance.WriteLogDebug("PlatformRuntimeTypes.LibraryWrite", 100
                , $"Writing runtime Object with name 0x{whose.ToString("X")}, at index {index}.");

            if (PlatformHostMain.api.WriteValue == null)
            {
                RxPlatformObject.Instance.WriteLogError("PlatformRuntimeTypes.LibraryWrite", 100
                , $"Write value function is not available. Cannot write value at index {index}.");
                return false;
            }
            typed_value_type rxVal = new typed_value_type();
            ValuesConvertor.ConvertToRxValue(value, out rxVal);
            PlatformHostMain.api.WriteValue((rx_item_type)type, 99, whose, (UIntPtr)index, rxVal, IntPtr.Zero);

            return true;
        }

        // called by native runtimes to notify value changes, execute functions...
        internal unsafe static void RuntimeValueChanged(rx_item_type type, nuint idx, typed_value_type value, nint whose)
        {
            var obj = GetRuntime(type, whose);
            if (obj != null)
            {
                object? objVal = null;
                ValuesConvertor.ConvertValueFromRx(&value, ref objVal);
                obj.__rxValueCallback((int)idx, objVal);
            }
        }
        internal static unsafe void InitialRuntimeValues(rx_item_type type, nuint count, char** names, typed_value_type* value, nint whose)
        {
            Tuple<string, object?>[] vals = new Tuple<string, object?>[count];
            for (nuint i = 0; i < count; i++)
            {
                char* name = names[i];
                var nameStr = Marshal.PtrToStringAnsi((nint)name) ?? "";
                object? objVal = null;
                ValuesConvertor.ConvertValueFromRx(&value[i], ref objVal);
                vals[i] = new Tuple<string, object?>(nameStr, objVal);
            }
            Task.Run(() =>
            {
                Action? started = null;
                var obj = GetRuntime(type, whose, ref started);
                if (obj != null)
                {
                    
                    obj.__rxInitialValuesCallback(vals);
                    if (started != null)
                        started();
                }
            });
        }

        internal static void ExecuteMethod(uint transId, nint whose, string methodStr, string value)
        {

            Task.Run(async () =>
            {
                try
                {
                    var obj = GetObject(whose);
                    if (obj != null)
                    {
                        await obj.__rxExecuteMethod(methodStr, value);

                        rx_result_struct ret = new rx_result_struct();
                        ret.count = 0;

                        if (PlatformHostMain.api.ExecuteDone != null)
                            PlatformHostMain.api.ExecuteDone(transId, whose, "{}", ret);
                    }
                    else
                    {
                        string error = $"Unable to find object for ptr 0x{whose.ToString("X")} to execute method {methodStr}.";
                        RxPlatformObject.Instance.WriteLogWarning("PlatformRuntimeTypes.ExecuteMethod", 100
                            , error);

                        rx_result_struct result = CommonInterface.CreateErrorResult(error);

                        if (PlatformHostMain.api.ExecuteDone != null)
                            PlatformHostMain.api.ExecuteDone(transId, whose, "{}", result);

                    }
                }
                catch (Exception ex)
                {
                    RxPlatformObject.Instance.WriteLogError("PlatformRuntimeTypes.ExecuteMethod", 200
                        , $"Error executing method {methodStr}: {ex.Message}");


                    rx_result_struct result = CommonInterface.CreateResultFromException(ex);

                    if (PlatformHostMain.api.ExecuteDone != null)
                        PlatformHostMain.api.ExecuteDone(transId, whose, "{}", result);
                }
            });

        }
    }
}