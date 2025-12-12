using ENSACO.RxPlatform.Hosting;
using ENSACO.RxPlatform.Hosting.Internal;
using ENSACO.RxPlatform.Hosting.Model;
using ENSACO.RxPlatform.Model;
using ENSACO.RxPlatform.Runtime;
using RxPlatform.Hosting.Interface;
using RxPlatform.Hosting.StaticRemains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RxPlatform.Hosting.Runtime
{

    internal unsafe class PlatformInitContext
    {
        nint context;
        public PlatformInitContext(nint ctx)
        {
            context = ctx;
        }
        public string GetConfigValue(string key, string defaultValue)
        {
            nint keyPtr = Marshal.StringToHGlobalAnsi(key);
            typed_value_type val = new typed_value_type();
            rx_result_struct result = PlatformABI.platformABI.prxInitCtxGetLocalValue((void*)context, key, &val);

            Marshal.FreeHGlobal(keyPtr);
            if (CommonInterface.rx_result_ok(&result) == 0)
            {
                return defaultValue;
            }
            string? res = default;
            if (!HostValuesConvertor.ConvertValueFromRxString(&val, ref res) || res == null)
            {
                return defaultValue;
            }

            return res;
        }
        public uint GetConfigValue(string key, uint defaultValue)
        {
            nint keyPtr = Marshal.StringToHGlobalAnsi(key);
            typed_value_type val = new typed_value_type();
            rx_result_struct result = PlatformABI.platformABI.prxInitCtxGetLocalValue((void*)context, key, &val);

            Marshal.FreeHGlobal(keyPtr);
            if (CommonInterface.rx_result_ok(&result) == 0)
            {
                return defaultValue;
            }
            ulong res = default;
            if (!HostValuesConvertor.ConvertValueFromRxUint(&val, ref res))
            {
                return defaultValue;
            }

            return (uint)res;
        }

        public ulong GetConfigValue(string key, ulong defaultValue)
        {
            nint keyPtr = Marshal.StringToHGlobalAnsi(key);
            typed_value_type val = new typed_value_type();
            rx_result_struct result = PlatformABI.platformABI.prxInitCtxGetLocalValue((void*)context, key, &val);

            Marshal.FreeHGlobal(keyPtr);
            if (CommonInterface.rx_result_ok(&result) == 0)
            {
                return defaultValue;
            }
            ulong res = default;
            if (!HostValuesConvertor.ConvertValueFromRxUint(&val, ref res))
            {
                return defaultValue;
            }

            return res;
        }

        public double GetConfigValue(string key, double defaultValue)
        {
            nint keyPtr = Marshal.StringToHGlobalAnsi(key);
            typed_value_type val = new typed_value_type();
            rx_result_struct result = PlatformABI.platformABI.prxInitCtxGetLocalValue((void*)context, key, &val);

            Marshal.FreeHGlobal(keyPtr);
            if (CommonInterface.rx_result_ok(&result) == 0)
            {
                return defaultValue;
            }
            double res = default;
            if (!HostValuesConvertor.ConvertValueFromRxFloat(&val, ref res))
            {
                return defaultValue;
            }

            return res;
        }

        public bool GetConfigValue(string key, bool defaultValue)
        {
            nint keyPtr = Marshal.StringToHGlobalAnsi(key);
            typed_value_type val = new typed_value_type();
            rx_result_struct result = PlatformABI.platformABI.prxInitCtxGetLocalValue((void*)context, key, &val);

            Marshal.FreeHGlobal(keyPtr);
            if (CommonInterface.rx_result_ok(&result) == 0)
            {
                return defaultValue;
            }
            bool res = default;
            if (!HostValuesConvertor.ConvertValueFromRxBool(&val, ref res))
            {
                return defaultValue;
            }

            return res;
        }
        public unsafe string?[] GetSourceValuesString(Guid id, string path)
        {
            values_array_struct data;
            rx_node_id_struct node_id = CommonInterface.CreateNodeIdFromGuid(id);

            var ret = PlatformABI.platformABI.prxInitCtxGetSourceValues((void*)context
                    , &node_id, path, &data);

            var exception = CommonInterface.GetExceptionFromResult(&ret);
            if (exception != null)
                throw exception;

            typed_value_type* values = (typed_value_type*)data.values;

            string?[] retVals = new string?[data.size];
            for (ulong i = 0; i < data.size; i++)
            {
                HostValuesConvertor.ConvertValueFromRxString(&values[i], ref retVals[i]);
            }
            CommonInterface.rx_destory_values_array_struct(&data);
            return retVals;
        }
        public unsafe double[] GetSourceValuesFloat(Guid id, string path)
        {
            values_array_struct data;
            rx_node_id_struct node_id = CommonInterface.CreateNodeIdFromGuid(id);

            var ret = PlatformABI.platformABI.prxInitCtxGetSourceValues((void*)context
                    , &node_id, path, &data);

            var exception = CommonInterface.GetExceptionFromResult(&ret);
            if (exception != null)
                throw exception;

            typed_value_type* values = (typed_value_type*)data.values;

            double[] retVals = new double[data.size];
            for (ulong i = 0; i < data.size; i++)
            {
                HostValuesConvertor.ConvertValueFromRxFloat(&values[i], ref retVals[i]);
            }
            CommonInterface.rx_destory_values_array_struct(&data);
            return retVals;
        }
        public unsafe bool[] GetSourceValuesBool(Guid id, string path)
        {
            values_array_struct data;
            rx_node_id_struct node_id = CommonInterface.CreateNodeIdFromGuid(id);

            var ret = PlatformABI.platformABI.prxInitCtxGetSourceValues((void*)context
                    , &node_id, path, &data);

            var exception = CommonInterface.GetExceptionFromResult(&ret);
            if (exception != null)
                throw exception;

            typed_value_type* values = (typed_value_type*)data.values;

            bool[] retVals = new bool[data.size];
            for (ulong i = 0; i < data.size; i++)
            {
                HostValuesConvertor.ConvertValueFromRxBool(&values[i], ref retVals[i]);
            }
            CommonInterface.rx_destory_values_array_struct(&data);
            return retVals;
        }
        public unsafe long[] GetSourceValuesInt(Guid id, string path)
        {
            values_array_struct data;
            rx_node_id_struct node_id = CommonInterface.CreateNodeIdFromGuid(id);

            var ret = PlatformABI.platformABI.prxInitCtxGetSourceValues((void*)context
                    , &node_id, path, &data);

            var exception = CommonInterface.GetExceptionFromResult(&ret);
            if (exception != null)
                throw exception;

            typed_value_type* values = (typed_value_type*)data.values;

            long[] retVals = new long[data.size];
            for (ulong i = 0; i < data.size; i++)
            {
                HostValuesConvertor.ConvertValueFromRxInt(&values[i], ref retVals[i]);
            }
            CommonInterface.rx_destory_values_array_struct(&data);
            return retVals;
        }
        public unsafe ulong[] GetSourceValuesUInt(Guid id, string path)
        {
            values_array_struct data;
            rx_node_id_struct node_id = CommonInterface.CreateNodeIdFromGuid(id);

            var ret = PlatformABI.platformABI.prxInitCtxGetSourceValues((void*)context
                    , &node_id, path, &data);

            var exception = CommonInterface.GetExceptionFromResult(&ret);
            if (exception != null)
                throw exception;

            typed_value_type* values = (typed_value_type*)data.values;

            ulong[] retVals = new ulong[data.size];
            for (ulong i = 0; i < data.size; i++)
            {
                HostValuesConvertor.ConvertValueFromRxUint(&values[i], ref retVals[i]);
            }
            CommonInterface.rx_destory_values_array_struct(&data);
            return retVals;
        }
    }
    internal unsafe class PlatformStartContext
    {
        nint context;
        public PlatformStartContext(nint ctx)
        {
            context = ctx;
        }
    }
    internal class RuntimeRepository
    {
        internal void RegisterObject(RxNodeId nodeId, RxPlatformObjectRuntime objectRuntime)
        {
            lock (this)
            {
                if (nodeIdDict.ContainsKey(nodeId))
                {
                    throw new Exception($"Runtime data for node id {nodeId.ToString()} is already registered.");
                }
                else
                {
                    PlatformRuntimeData<RxPlatformObjectRuntime> data = new PlatformRuntimeData<RxPlatformObjectRuntime>();
                    data.objectRuntime = objectRuntime;
                    data.nodeId = nodeId;
                    data.nativeRuntimePtr = IntPtr.Zero;
                    nodeIdDict[nodeId] = data;
                    managedDict[objectRuntime] = data;
                }
            }
        }
        internal void UnregisterObject(RxNodeId nodeId, RxPlatformObjectRuntime objectRuntime)
        {
            lock (this)
            {
                PlatformRuntimeData<RxPlatformObjectRuntime> data;
                if (managedDict.TryGetValue(objectRuntime, out data))
                {
                    bool hasZeroing = data.nativeRuntimePtr != IntPtr.Zero;
                    data.objectRuntime = null;
                    if (hasZeroing)
                    {
                        data.nativeRuntimePtr = IntPtr.Zero;
                    }
                    managedDict.Remove(objectRuntime);
                    nodeIdDict.Remove(nodeId);

                    if (hasZeroing)
                    {
                        objectRuntime.__BindObject(IntPtr.Zero);
                    }
                }
                else
                {
                    throw new Exception($"No runtime data found for node id {nodeId.ToString()}");
                }
            }
        }
        internal void BindObject(RxNodeId nodeId, nint nativePtr)
        {
            lock (this)
            {
                if (nativeDict.ContainsKey(nativePtr))
                {
                    return;
                }
                PlatformRuntimeData<RxPlatformObjectRuntime> data;
                if (nodeIdDict.TryGetValue(nodeId, out data))
                {
                    if( data.nativeRuntimePtr != IntPtr.Zero)
                    {
                        throw new Exception($"Runtime data for node id {nodeId.ToString()} is already bound to native pointer {data.nativeRuntimePtr.ToString("X")}.");
                    }
                    data.nativeRuntimePtr = nativePtr;
                    if (data.objectRuntime != null)
                    {
                        data.objectRuntime.__BindObject((IntPtr)nativePtr);
                        managedDict[data.objectRuntime] = data;
                        nodeIdDict[nodeId] = data;
                    }
                    nativeDict.Add((IntPtr)nativePtr, data);
                }
                else
                {
                    var tempPtr = PlatformHostMain.CreateObject(nodeId);
                    if (tempPtr != null)
                    {
                        data.nativeRuntimePtr = nativePtr;
                        data.objectRuntime = tempPtr;
                        data.objectRuntime.__BindObject((IntPtr)nativePtr);
                        managedDict[data.objectRuntime] = data;
                        nodeIdDict[nodeId] = data;
                        nativeDict.Add((IntPtr)nativePtr, data);

                    }
                }
            }
            RxPlatformObject.Instance.WriteLogDebug("RuntimeRepository.BindObject", 200, $".NET Host:Bind object for node id {nodeId.ToString()} and native ptr {nativePtr.ToString("X")}");

        }

        internal IntPtr GetNativePtr(RxPlatformObjectRuntime whose)
        {
            lock (this)
            {
                PlatformRuntimeData<RxPlatformObjectRuntime> data;
                if (managedDict.TryGetValue(whose, out data))
                {
                    return data.nativeRuntimePtr;
                }
                else
                {
                    return IntPtr.Zero;
                }
            }
        }
        internal RxPlatformObjectRuntime? GetObject(IntPtr whose)
        {
            lock (this)
            {
                PlatformRuntimeData<RxPlatformObjectRuntime> data;
                if (nativeDict.TryGetValue(whose, out data))
                {
                    return data.objectRuntime;
                }
                else
                {
                    return null;
                }
            }
        }
        internal void UnbindObject(RxNodeId nodeId, nint nativePtr)
        {
            lock (this)
            {
                if (!nativeDict.ContainsKey(nativePtr))
                {
                    return;
                }
                PlatformRuntimeData<RxPlatformObjectRuntime> data;
                if (nodeIdDict.TryGetValue(nodeId, out data))
                {
                    if (data.nativeRuntimePtr == (IntPtr)nativePtr)
                    {
                        data.nativeRuntimePtr = IntPtr.Zero;
                        if (data.objectRuntime != null)
                        {
                            data.objectRuntime.__BindObject(IntPtr.Zero);
                            managedDict[data.objectRuntime] = data;
                        }
                        nativeDict.Remove((IntPtr)nativePtr);
                        nodeIdDict[nodeId] = data;
                    }
                }
                else
                {
                    throw new Exception($"No runtime data found for node id {nodeId.ToString()}");
                }
            }
            RxPlatformObject.Instance.WriteLogDebug("RuntimeRepository.UnbindObject", 200, $".NET Host:Unbind object for node id {nodeId.ToString()} and native ptr {nativePtr.ToString("X")}");
        }

        public RuntimeRepository() { }
        Dictionary<RxNodeId, PlatformRuntimeData<RxPlatformObjectRuntime>> nodeIdDict = new Dictionary<RxNodeId, PlatformRuntimeData<RxPlatformObjectRuntime>>();
        Dictionary<IntPtr, PlatformRuntimeData<RxPlatformObjectRuntime>> nativeDict = new Dictionary<IntPtr, PlatformRuntimeData<RxPlatformObjectRuntime>>();
        Dictionary<RxPlatformObjectRuntime, PlatformRuntimeData<RxPlatformObjectRuntime>> managedDict = new Dictionary<RxPlatformObjectRuntime, PlatformRuntimeData<RxPlatformObjectRuntime>>();
    }
}
