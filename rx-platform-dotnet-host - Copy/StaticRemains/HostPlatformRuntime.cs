using ENSACO.RxPlatform.Hosting.Internal;
using RxPlatform.Hosting.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace RxPlatform.Hosting.StaticRemains
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
    public unsafe class HostPlatformRuntime
    {
        static unsafe lock_reference_def_struct* anchor_definition;

        //private delegate void DestroyDelegateType(IntPtr anchor);
        //private static readonly DestroyDelegateType DestroyObjectDelegate = new DestroyDelegateType(DestroyObject);
        //private static readonly IntPtr DestroyDelegatePtr = Marshal.GetFunctionPointerForDelegate(DestroyObjectDelegate);

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        static unsafe void DestroyObject(nint anchor)
        {
            if (anchor != nint.Zero)
            {
                GCHandle handle = GCHandle.FromIntPtr(anchor);
                if(handle.Target == null)
                {
                    return;
                }
                myObjects.Remove(handle.Target);
                handle.Free();
            }
        }


        static unsafe HostPlatformRuntime()
        {
            anchor_definition = (lock_reference_def_struct*)Marshal.AllocHGlobal(sizeof(lock_reference_def_struct));
            anchor_definition->destroy_reference = (nint)(delegate* unmanaged[Cdecl]<nint, void>)&DestroyObject;

        }
        protected HostPlatformRuntime()
        {
        }
        GCHandle handle = default;
        static HashSet<object> myObjects = new HashSet<object>();
        protected void Init(lock_reference_struct* anchor)
        {
            myObjects.Add(this);
            handle = GCHandle.Alloc(this, GCHandleType.Normal);
            CommonInterface.rx_init_lock_reference(anchor, GCHandle.ToIntPtr(handle), anchor_definition);
        }
        ~HostPlatformRuntime()
        {
        }
    }
}
