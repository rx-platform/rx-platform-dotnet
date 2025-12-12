using ENSACO.RxPlatform.Attributes;
using ENSACO.RxPlatform.Hosting.Common;
using ENSACO.RxPlatform.Hosting.Construction;
using ENSACO.RxPlatform.Hosting.Interface;
using ENSACO.RxPlatform.Hosting.Internal;
using ENSACO.RxPlatform.Hosting.Model;
using ENSACO.RxPlatform.Hosting.Threading;
using ENSACO.RxPlatform.Model;
using ENSACO.RxPlatform.Runtime;
using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ENSACO.RxPlatform.Hosting.Runtime
{
    internal static class RxRuntimeExecuter
    {
        static RxPlatformRuntimeBase? GetRuntime(rx_item_type type, nint whose, ref Action? started, ref Dictionary<string, object> childrenValues, ref RxNodeId id)
        {

            switch (type)
            {
                case rx_item_type.rx_object:
                    {
                        PlatformRuntimeData obj;
                        lock (RxMetaData.Instance.TypesLock)
                        {
                            if (RxMetaData.Instance.ObjectRuntimes.NativeDict.TryGetValue(whose, out obj))
                            {
                                if (obj.objectRuntime.IsAllocated)
                                {
                                    RxPlatformObjectRuntime? objectRuntime = obj.objectRuntime.Target as RxPlatformObjectRuntime;
                                    if (objectRuntime != null)
                                    {
                                        id = obj.nodeId;
                                        started = obj.startedMethod;
                                        if(RuntimeConstructAlgorithms.TryGetConstructionData(obj.nodeId, obj.path, out var constructData) && constructData!=null)
                                        {
                                            foreach(var child in constructData.structs)
                                            {
                                                if (child.Value._nativePtr != nint.Zero)
                                                {
                                                    var childRuntime = GetRuntime(child.Value.type, child.Value._nativePtr);
                                                    if (childRuntime != null)
                                                    {
                                                        childrenValues[child.Key] = childRuntime;
                                                    }
                                                }
                                            }
                                        }
                                        return objectRuntime;
                                    }
                                }
                            }
                        }
                        break;
                    }
                case rx_item_type.rx_source_type:
                    {
                        PlatformRuntimeData src;
                        lock (RxMetaData.Instance.TypesLock)
                        {
                            if (RxMetaData.Instance.SourceRuntimes.NativeDict.TryGetValue(whose, out src))
                            {
                                if (src.objectRuntime.IsAllocated)
                                {
                                    RxPlatformSourceRuntime? sourceRuntime = src.objectRuntime.Target as RxPlatformSourceRuntime;
                                    if (sourceRuntime != null)
                                    {
                                        id = src.nodeId;
                                        started = src.startedMethod;
                                        if (RuntimeConstructAlgorithms.TryGetConstructionData(src.nodeId, src.path, out var constructData) && constructData != null)
                                        {
                                            foreach (var child in constructData.structs)
                                            {
                                                if (child.Value._nativePtr != nint.Zero)
                                                {
                                                    var childRuntime = GetRuntime(child.Value.type, child.Value._nativePtr);
                                                    if (childRuntime != null)
                                                    {
                                                        childrenValues[child.Key] = childRuntime;
                                                    }
                                                }
                                            }
                                        }

                                        return sourceRuntime;
                                    }
                                }
                            }
                        }
                        break;
                    }
                case rx_item_type.rx_struct_type:
                    {
                        PlatformRuntimeData str;
                        lock (RxMetaData.Instance.TypesLock)
                        {
                            if (RxMetaData.Instance.StructRuntimes.NativeDict.TryGetValue(whose, out str))
                            {
                                if (str.objectRuntime.IsAllocated)
                                {
                                    RxPlatformStructRuntime? structRuntime = str.objectRuntime.Target as RxPlatformStructRuntime;
                                    if (structRuntime != null)
                                    {
                                        id = str.nodeId;
                                        started = str.startedMethod;
                                        if (RuntimeConstructAlgorithms.TryGetConstructionData(str.nodeId, str.path, out var constructData) && constructData != null)
                                        {
                                            foreach (var child in constructData.structs)
                                            {
                                                if (child.Value._nativePtr != nint.Zero)
                                                {
                                                    var childRuntime = GetRuntime(child.Value.type, child.Value._nativePtr);
                                                    if (childRuntime != null)
                                                    {
                                                        childrenValues[child.Key] = childRuntime;
                                                    }
                                                }
                                            }
                                        }
                                        return structRuntime;
                                    }
                                }
                            }
                        }
                        break;
                    }
                case rx_item_type.rx_mapper_type:
                    {
                        PlatformRuntimeData map;
                        lock (RxMetaData.Instance.TypesLock)
                        {
                            if (RxMetaData.Instance.MapperRuntimes.NativeDict.TryGetValue(whose, out map))
                            {
                                if (map.objectRuntime.IsAllocated)
                                {
                                    RxPlatformMapperRuntime? objectRuntime = map.objectRuntime.Target as RxPlatformMapperRuntime;
                                    if (objectRuntime != null)
                                    {
                                        id = map.nodeId;
                                        started = map.startedMethod;
                                        if (RuntimeConstructAlgorithms.TryGetConstructionData(map.nodeId, map.path, out var constructData) && constructData != null)
                                        {
                                            foreach (var child in constructData.structs)
                                            {
                                                if (child.Value._nativePtr != nint.Zero)
                                                {
                                                    var childRuntime = GetRuntime(child.Value.type, child.Value._nativePtr);
                                                    if (childRuntime != null)
                                                    {
                                                        childrenValues[child.Key] = childRuntime;
                                                    }
                                                }
                                            }
                                        }
                                        return objectRuntime;
                                    }
                                }
                            }
                        }
                        break;
                    }
                case rx_item_type.rx_event_type:
                    {
                        PlatformRuntimeData evt;
                        lock (RxMetaData.Instance.TypesLock)
                        {
                            if (RxMetaData.Instance.EventRuntimes.NativeDict.TryGetValue(whose, out evt))
                            {
                                if (evt.objectRuntime.IsAllocated)
                                {
                                    RxPlatformEventRuntime? objectRuntime = evt.objectRuntime.Target as RxPlatformEventRuntime;
                                    if (objectRuntime != null)
                                    {
                                        id = evt.nodeId;
                                        started = evt.startedMethod;
                                        if (RuntimeConstructAlgorithms.TryGetConstructionData(evt.nodeId, evt.path, out var constructData) && constructData != null)
                                        {
                                            foreach (var child in constructData.structs)
                                            {
                                                if (child.Value._nativePtr != nint.Zero)
                                                {
                                                    var childRuntime = GetRuntime(child.Value.type, child.Value._nativePtr);
                                                    if (childRuntime != null)
                                                    {
                                                        childrenValues[child.Key] = childRuntime;
                                                    }
                                                }
                                            }
                                        }
                                        return objectRuntime;
                                    }
                                }
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
                        PlatformRuntimeData obj;
                        lock (RxMetaData.Instance.TypesLock)
                        {
                            if (RxMetaData.Instance.ObjectRuntimes.NativeDict.TryGetValue(whose, out obj))
                            {
                                if (obj.objectRuntime.IsAllocated)
                                {
                                    RxPlatformObjectRuntime? objectRuntime = obj.objectRuntime.Target as RxPlatformObjectRuntime;
                                    if (objectRuntime != null)
                                    {
                                        return objectRuntime;
                                    }
                                }
                            }
                        }
                        break;
                    }
                case rx_item_type.rx_source_type:
                    {
                        PlatformRuntimeData src;
                        lock (RxMetaData.Instance.TypesLock)
                        {
                            if (RxMetaData.Instance.SourceRuntimes.NativeDict.TryGetValue(whose, out src))
                            {
                                if (src.objectRuntime.IsAllocated)
                                {
                                    RxPlatformSourceRuntime? sourceRuntime = src.objectRuntime.Target as RxPlatformSourceRuntime;
                                    if (sourceRuntime != null)
                                    {
                                        return sourceRuntime;
                                    }
                                }
                            }
                        }
                        break;
                    }

                case rx_item_type.rx_struct_type:
                    {
                        PlatformRuntimeData src;
                        lock (RxMetaData.Instance.TypesLock)
                        {
                            if (RxMetaData.Instance.StructRuntimes.NativeDict.TryGetValue(whose, out src))
                            {
                                if (src.objectRuntime.IsAllocated)
                                {
                                    RxPlatformStructRuntime? structRuntime = src.objectRuntime.Target as RxPlatformStructRuntime;
                                    if (structRuntime != null)
                                    {
                                        return structRuntime;
                                    }
                                }
                            }
                        }
                        break;
                    }
                case rx_item_type.rx_mapper_type:
                    {
                        PlatformRuntimeData map;
                        lock (RxMetaData.Instance.TypesLock)
                        {
                            if (RxMetaData.Instance.MapperRuntimes.NativeDict.TryGetValue(whose, out map))
                            {
                                if (map.objectRuntime.IsAllocated)
                                {
                                    RxPlatformMapperRuntime? objectRuntime = map.objectRuntime.Target as RxPlatformMapperRuntime;
                                    if (objectRuntime != null)
                                    {
                                        return objectRuntime;
                                    }
                                }
                            }
                        }
                        break;
                    }
                case rx_item_type.rx_event_type:
                    {
                        PlatformRuntimeData evt;
                        lock (RxMetaData.Instance.TypesLock)
                        {
                            if (RxMetaData.Instance.EventRuntimes.NativeDict.TryGetValue(whose, out evt))
                            {
                                if (evt.objectRuntime.IsAllocated)
                                {
                                    RxPlatformEventRuntime? objectRuntime = evt.objectRuntime.Target as RxPlatformEventRuntime;
                                    if (objectRuntime != null)
                                    {
                                        return objectRuntime;
                                    }
                                }
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
            PlatformRuntimeData obj;
            lock (RxMetaData.Instance.TypesLock)
            {
                if (RxMetaData.Instance.ObjectRuntimes.NativeDict.TryGetValue(whose, out obj))
                {
                    if (obj.objectRuntime.IsAllocated)
                    {
                        RxPlatformObjectRuntime? objectRuntime = obj.objectRuntime.Target as RxPlatformObjectRuntime;
                        if (objectRuntime != null)
                        {
                            return objectRuntime;
                        }
                    }
                }
            }
            return null;
        }

        static Tuple<SourceWriteMethods, RxPlatformSourceRuntime?> GetSource(nint whose)
        {
            PlatformRuntimeData obj;
            lock (RxMetaData.Instance.TypesLock)
            {
                if (RxMetaData.Instance.SourceRuntimes.NativeDict.TryGetValue(whose, out obj))
                {
                    if (obj.objectRuntime.IsAllocated)
                    {
                        return new Tuple<SourceWriteMethods, RxPlatformSourceRuntime?>(obj.sourceWriteMethods, obj.objectRuntime.Target as RxPlatformSourceRuntime);
                    }
                }
            }
            return new Tuple<SourceWriteMethods, RxPlatformSourceRuntime?>(new SourceWriteMethods(), null);
        }
        // called by managed runtimes to write property values
        
        internal static async Task<bool> WriteProperty<T>(byte type, nint whose, int index, T value)
        {
            if (PlatformHostMain.api.WriteValue == null)
            {
                RxPlatformObject.Instance.WriteLogWarining("PlatformRuntimeTypes.LibraryWrite", 100
                , $"Write value function is not available. Cannot write value at index {index}.");
                return false;
            }
            var task = HostThreadingSynchronizator.AppendExceptioned();
            typed_value_type rxVal = new typed_value_type();
            ValuesConvertor.ConvertToRxValue(value, out rxVal);
            PlatformHostMain.api.WriteValue((rx_item_type)type, task.TransId, whose, (UIntPtr)index, rxVal, task.CallbackPtr);
            var result = await task.Task;
            if (result != null)
            {
                RxPlatformObject.Instance.WriteLogWarining("PlatformRuntimeTypes.LibraryWrite", 200
                , $"Error writing runtime Object with ptr 0x{whose.ToString("X")}, at index {index}: {result.Message}");
                return false;
            }
            return true;
        }
        internal static async Task<bool> WriteObjectProperty(byte type, nint whose, int index, object value)
        {
            if (PlatformHostMain.api.WriteValue == null)
            {
                RxPlatformObject.Instance.WriteLogWarining("PlatformRuntimeTypes.LibraryWrite", 100
                , $"Write value function is not available. Cannot write value at index {index}.");
                return false;
            }
            var task = HostThreadingSynchronizator.AppendExceptioned();
            typed_value_type rxVal = new typed_value_type();
            ValuesConvertor.ConvertToRxValue(value, out rxVal);
            PlatformHostMain.api.WriteValue((rx_item_type)type, task.TransId, whose, (UIntPtr)index, rxVal, task.CallbackPtr);
            var result = await task.Task;
            if (result != null)
            {
                RxPlatformObject.Instance.WriteLogWarining("PlatformRuntimeTypes.LibraryWrite", 200
                , $"Error writing runtime Object with ptr 0x{whose.ToString("X")}, at index {index}: {result.Message}");
                return false;
            }
            return true;
        }


        // called by native runtimes to notify value changes, execute functions...
        internal unsafe static void RuntimeValueChanged(rx_item_type type, nuint idx, typed_value_type value, nint whose)
        {
            var obj = GetRuntime(type, whose);
            if (obj != null)
            {
                object? objVal = null;
                ValuesConvertor.ConvertValueFromRx(ref value, ref objVal);
                ProperyValuesSynhronizator.RuntimeValueChanged(obj, idx, objVal);
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
                ValuesConvertor.ConvertValueFromRx(ref value[i], ref objVal);
                vals[i] = new Tuple<string, object?>(nameStr, objVal);
            }
            Task.Run(() =>
            {
                Action? started = null;
                Dictionary<string, object> childrenValues = new Dictionary<string, object>();
                RxNodeId id = RxNodeId.NullId;
                var obj = GetRuntime(type, whose, ref started, ref childrenValues, ref id);
                if (obj != null)
                {
                    if(!id.IsNull())
                    {
                        RuntimeConstructAlgorithms.RemoveFromConstructionData(id);
                    }
                    obj.__rxInitialValuesCallback(vals, childrenValues);
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
                    RxPlatformObject.Instance.WriteLogWarining("PlatformRuntimeTypes.ExecuteMethod", 200
                        , $"Error executing method {methodStr}: {ex.Message}");


                    rx_result_struct result = CommonInterface.CreateResultFromException(ex);

                    if (PlatformHostMain.api.ExecuteDone != null)
                        PlatformHostMain.api.ExecuteDone(transId, whose, "{}", result);
                }
            });

        }
        internal static void SourceWrite(uint transId, typed_value_type value, nint whose)
        {
            Action<Exception?> callback = (Exception? exc) =>
            {
                rx_result_struct ret = new rx_result_struct();
                ret.count = 0;
                if (exc != null)
                {
                    ret = CommonInterface.CreateErrorResult(exc.Message);
                }
                if (PlatformHostMain.api.SourceWriteDone != null)
                    PlatformHostMain.api.SourceWriteDone(transId, whose, ret);
            };
            Task.Run(() =>
            {
                try
                {

                    var obj = GetSource(whose);
                    if (obj.Item2 == null)
                    {
                        rx_result_struct result = CommonInterface.CreateErrorResult($"Unable to find source for ptr 0x{whose.ToString("X")} to write value.");
                        if (PlatformHostMain.api.SourceWriteDone != null)
                            PlatformHostMain.api.SourceWriteDone(transId, whose, result);
                        return;
                    }
                    rx_item_type type = (rx_item_type)obj.Item2.__GetWriteConvert((byte)value.value_type);
                    if (CommonInterface.rx_convert_value(ref value, (byte)type) == 0)
                    {
                        rx_result_struct result = CommonInterface.CreateErrorResult($"Unable to convert value to type {(byte)type} for source write.");
                        if (PlatformHostMain.api.SourceWriteDone != null)
                            PlatformHostMain.api.SourceWriteDone(transId, whose, result);
                    }
                    switch (value.value_type)
                    {
                        case rx_value_t.Bool:
                            {
                                if (obj.Item1.writeBoolMethod != null)
                                {
                                    bool val = false;
                                    if (ValuesConvertor.ConvertValueFromRxBool(ref value, ref val))
                                        obj.Item1.writeBoolMethod(val, callback);
                                    else
                                        throw new InvalidCastException($"Unable to convert value for source write.");
                                }
                                else
                                    throw new InvalidCastException($"Unexpected Action is zero!!!");
                            }
                            break;

                        case rx_value_t.Int8:
                            {
                                if (obj.Item1.writeSByteMethod != null)
                                {
                                    sbyte val = 0;
                                    if (ValuesConvertor.ConvertValueFromRxInt(ref value, ref val))
                                        obj.Item1.writeSByteMethod(val, callback);
                                    else
                                        throw new InvalidCastException($"Unable to convert value for source write.");
                                }
                                else
                                    throw new InvalidCastException($"Unexpected Action is zero!!!");

                            }
                            break;
                        case rx_value_t.Int16:
                            {
                                if (obj.Item1.writeShortMethod != null)
                                {
                                    short val = 0;
                                    if (ValuesConvertor.ConvertValueFromRxInt(ref value, ref val))
                                        obj.Item1.writeShortMethod(val, callback);
                                    else
                                        throw new InvalidCastException($"Unable to convert value for source write.");
                                }
                                else
                                    throw new InvalidCastException($"Unexpected Action is zero!!!");
                            }
                            break;
                        case rx_value_t.Int32:
                            {
                                if (obj.Item1.writeIntMethod != null)
                                {
                                    int val = 0;
                                    if (ValuesConvertor.ConvertValueFromRxInt(ref value, ref val))
                                        obj.Item1.writeIntMethod(val, callback);
                                    else
                                        throw new InvalidCastException($"Unable to convert value for source write.");
                                }
                                else
                                    throw new InvalidCastException($"Unexpected Action is zero!!!");
                            }
                            break;
                        case rx_value_t.Int64:
                            {
                                if (obj.Item1.writeLongMethod != null)
                                {
                                    long val = 0;
                                    if (ValuesConvertor.ConvertValueFromRxInt(ref value, ref val))
                                        obj.Item1.writeLongMethod(val, callback);
                                    else
                                        throw new InvalidCastException($"Unable to convert value for source write.");
                                }
                                else
                                    throw new InvalidCastException($"Unexpected Action is zero!!!");
                            }
                            break;
                        case rx_value_t.UInt8:
                            {
                                if (obj.Item1.writeByteMethod != null)
                                {
                                    byte val = 0;
                                    if (ValuesConvertor.ConvertValueFromRxUInt(ref value, ref val))
                                        obj.Item1.writeByteMethod(val, callback);
                                    else
                                        throw new InvalidCastException($"Unable to convert value for source write.");
                                }
                                else
                                    throw new InvalidCastException($"Unexpected Action is zero!!!");
                            }
                            break;
                        case rx_value_t.UInt16:
                            {
                                if (obj.Item1.writeUShortMethod != null)
                                {
                                    ushort val = 0;
                                    if (ValuesConvertor.ConvertValueFromRxUInt(ref value, ref val))
                                        obj.Item1.writeUShortMethod(val, callback);
                                    else
                                        throw new InvalidCastException($"Unable to convert value for source write.");
                                }
                                else
                                    throw new InvalidCastException($"Unexpected Action is zero!!!");
                            }
                            break;
                        case rx_value_t.UInt32:
                            {
                                if (obj.Item1.writeUIntMethod != null)
                                {
                                    uint val = 0;
                                    if (ValuesConvertor.ConvertValueFromRxUInt(ref value, ref val))
                                        obj.Item1.writeUIntMethod(val, callback);
                                    else
                                        throw new InvalidCastException($"Unable to convert value for source write.");
                                }
                                else
                                    throw new InvalidCastException($"Unexpected Action is zero!!!");
                            }
                            break;
                        case rx_value_t.UInt64:
                            {
                                if (obj.Item1.writeULongMethod != null)
                                {
                                    ulong val = 0;
                                    if (ValuesConvertor.ConvertValueFromRxUInt(ref value, ref val))
                                        obj.Item1.writeULongMethod(val, callback);
                                    else
                                        throw new InvalidCastException($"Unable to convert value for source write.");
                                }
                                else
                                    throw new InvalidCastException($"Unexpected Action is zero!!!");
                            }
                            break;
                        case rx_value_t.Float:
                            {
                                if (obj.Item1.writeFloatMethod != null)
                                {
                                    float val = 0;
                                    if (ValuesConvertor.ConvertValueFromRxFloat(ref value, ref val))
                                        obj.Item1.writeFloatMethod(val, callback);
                                    else
                                        throw new InvalidCastException($"Unable to convert value for source write.");
                                }
                                else
                                    throw new InvalidCastException($"Unexpected Action is zero!!!");
                            }
                            break;
                        case rx_value_t.Double:
                            {
                                if (obj.Item1.writeDoubleMethod != null)
                                {
                                    double val = 0;
                                    if (ValuesConvertor.ConvertValueFromRxFloat(ref value, ref val))
                                        obj.Item1.writeDoubleMethod(val, callback);
                                    else
                                        throw new InvalidCastException($"Unable to convert value for source write.");
                                }
                                else
                                    throw new InvalidCastException($"Unexpected Action is zero!!!");
                            }
                            break;
                        case rx_value_t.String:
                            {
                                if (obj.Item1.writeStringMethod != null)
                                {
                                    string? val = null;
                                    if (ValuesConvertor.ConvertValueFromRxString(ref value, ref val))
                                        obj.Item1.writeStringMethod(val ?? "", callback);
                                    else
                                        throw new InvalidCastException($"Unable to convert value for source write.");
                                }
                                else
                                    throw new InvalidCastException($"Unexpected Action is zero!!!");
                            }
                            break;
                        case rx_value_t.Uuid:
                            {
                                if (obj.Item1.writeUuidMethod != null)
                                {
                                    Guid val = Guid.Empty;
                                    if (ValuesConvertor.ConvertValueFromRxUuid(ref value, ref val))
                                        obj.Item1.writeUuidMethod(val, callback);
                                    else
                                        throw new InvalidCastException($"Unable to convert value for source write.");
                                }
                                else
                                    throw new InvalidCastException($"Unexpected Action is zero!!!");
                            }
                            break;
                        case rx_value_t.Struct:
                            {
                                if (obj.Item1.writeJsonMethod.Item2 != null)
                                {
                                    string? strVal = null;
                                    if (ValuesConvertor.ConvertValueFromRxString(ref value, ref strVal) && strVal != null)
                                    {
                                        object? val = JsonSerializer.Deserialize(strVal, obj.Item1.writeJsonMethod.Item1, PlatformHostMain.JsonContext);
                                        if (val != null)
                                        {
                                            obj.Item1.writeJsonMethod.Item2(val, callback);
                                        }
                                        else
                                        {
                                            throw new InvalidCastException($"Unable to deserialize struct value for source write.");
                                        }
                                    }
                                    else
                                    {
                                        throw new InvalidCastException($"Unable to convert value for source write.");
                                    }
                                }
                                else
                                {
                                    throw new InvalidCastException($"Unexpected Action is zero!!!");
                                }
                            }
                            break;
                        default:
                            {
                                throw new InvalidCastException($"Source write for type {value.value_type} is not implemented.");
                            }
                    }
                }
                catch (Exception ex)
                {
                    callback(new InvalidCastException($"Source write for type {value.value_type} is not implemented."));

                    rx_result_struct result = CommonInterface.CreateResultFromException(ex);

                    if (PlatformHostMain.api.SourceWriteDone != null)
                        PlatformHostMain.api.SourceWriteDone(transId, whose, result);
                }
            });

        }

        internal static void MappedChange(full_value_type value, nint whose)
        {
            throw new NotImplementedException();
        }
        static full_value_type PrepareValue()
        {
            full_value_type val = new full_value_type();
            val.quality = 0;
            val.time = new rx_time_struct() { t_value = (ulong)DateTime.UtcNow.ToFileTimeUtc() };

            return val;
        }
        internal static void SourceChanged<T>(nint instance, T value)
        {
            if (PlatformHostMain.api.SourceChange == null)
            {
                return;
            }
            full_value_type val = PrepareValue();
            ValuesConvertor.ConvertToRxValue(value, out val.value);
            PlatformHostMain.api.SourceChange(instance, val);
        }
        internal static void SourceChangedObject(nint instance, object value)
        {
            if (PlatformHostMain.api.SourceChange == null)
            {
                return;
            }
            full_value_type val = PrepareValue();
            ValuesConvertor.ConvertToRxValue(value, out val.value);
            PlatformHostMain.api.SourceChange(instance, val);
        }
        internal static void SourceChangedBad(nint instance)
        {
            if(PlatformHostMain.api.SourceChange==null)
            {
                return;
            }
            full_value_type val = PrepareValue();
            val.quality = 0x80000020;// RX_BAD_QUALITY_FAILURE
            PlatformHostMain.api.SourceChange(instance, val);
        }
    }
}