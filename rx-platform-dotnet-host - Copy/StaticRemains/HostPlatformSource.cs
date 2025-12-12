using ENSACO.RxPlatform.Hosting.Internal;
using RxPlatform.Hosting.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace RxPlatform.Hosting.StaticRemains
{

    public unsafe abstract class PlatformSourceBase : HostPlatformRuntime
    {

        const uint RX_IO_DATA_INPUT_MASK = 0x01;
        const uint RX_IO_DATA_OUTPUT_MASK = 0x02;

        static unsafe plugin_source_def_struct* source_def;
        internal unsafe plugin_source_runtime_struct4_t* runtime = null;

        static PlatformSourceBase()
        {
            source_def = (plugin_source_def_struct*)Marshal.AllocHGlobal(sizeof(plugin_source_def_struct));
            // Fix for CS8902: Use function pointer syntax for UnmanagedCallersOnly methods
            source_def->init_source = (nint)(delegate* unmanaged[Cdecl]<void*, void*, byte, rx_result_struct>)&ABIStubs.rx_init_source_stub;
            source_def->start_source = (nint)(delegate* unmanaged[Cdecl]<void*, void*, rx_result_struct>)&ABIStubs.rx_start_source_stub;
            source_def->stop_source = (nint)(delegate* unmanaged[Cdecl]<void*, rx_result_struct>)&ABIStubs.rx_stop_source_stub;
            source_def->deinit_source = (nint)(delegate* unmanaged[Cdecl]<void*, rx_result_struct>)&ABIStubs.rx_deinit_source_stub;
            source_def->write_source = (nint)(delegate* unmanaged[Cdecl]<void*, ulong, int, ulong, typed_value_type, void*, rx_result_struct >)&ABIStubs.rx_write_source_stub; ;

        }
        public PlatformSourceBase()
        {
            
            runtime = (plugin_source_runtime_struct4_t*)Marshal.AllocHGlobal(sizeof(plugin_source_runtime_struct4_t));

            runtime->def = source_def;
            runtime->host = null;
            runtime->host_def = null;
            runtime->io_data = RX_IO_DATA_INPUT_MASK | RX_IO_DATA_OUTPUT_MASK;

            Init(&runtime->anchor);
        }
        internal void SourceWrite(ulong id, int test, ulong identity, typed_value_type val, void* ctx)
        {
            if (CommonInterface.rx_is_bool_value(ref val) != 0)
            {
                int uval = 0;
                if (CommonInterface.rx_get_bool_value(ref val, 0, out uval) > 0)
                    Task.Run(() =>
                    {
                        try
                        {
                            SourceWriteReceived(id, test != 0, uval != 0);
                        }
                        catch (Exception ex)
                        {
                            SendWriteResult(id, ex);
                            return;
                        }
                    });
            }
            else if (CommonInterface.rx_is_unassigned_value(ref val) != 0)
            {
                ulong uval = 0;
                byte type = 0;
                if (CommonInterface.rx_get_unassigned_value(ref val, 0, out uval, out type) > 0)
                    Task.Run(() =>
                    {
                        try
                        {
                            SourceWriteReceived(id, test != 0, uval);
                        }
                        catch (Exception ex)
                        {
                            SendWriteResult(id, ex);
                            return;
                        }
                    });
            }
            else if (CommonInterface.rx_is_integer_value(ref val) != 0)
            {
                long uval = 0;
                byte type = 0;
                if (CommonInterface.rx_get_integer_value(ref val, 0, out uval, out type) > 0)
                    Task.Run(() =>
                    {
                        try
                        {
                            SourceWriteReceived(id, test != 0, uval);
                        }
                        catch (Exception ex)
                        {
                            SendWriteResult(id, ex);
                            return;
                        }
                    });
            }
            else if (CommonInterface.rx_is_float_value(ref val) != 0)
            {
                double uval = 0;
                byte type = 0;
                if (CommonInterface.rx_get_float_value(ref val, 0, out uval, out type) > 0)
                    Task.Run(() =>
                    {
                        try
                        {
                            SourceWriteReceived(id, test != 0, uval);
                        }
                        catch (Exception ex)
                        {
                            SendWriteResult(id, ex);
                            return;
                        }
                    });
            }
            else if (CommonInterface.rx_is_string_value(ref val) != 0)
            {
                string_value_struct uval;
                if (CommonInterface.rx_get_string_value(ref val, 0, out uval) > 0)
                {
                    Task.Run(() =>
                    {
                        try
                        {
                            string_value_struct copy = uval;
                            SourceWriteReceived(id, test != 0, Marshal.PtrToStringAnsi(CommonInterface.rx_c_str(&copy)));
                            CommonInterface.rx_destory_string_value_struct(&copy);
                        }
                        catch (Exception ex)
                        {
                            SendWriteResult(id, ex);
                            return;
                        }
                    });
                }
            }
        }
        public void SendWriteResult(ulong id, Exception? ex = null)
        {
            rx_result_struct res;
            if(ex== null)
                CommonInterface.rx_init_result_struct(&res);
            else
                CommonInterface.rx_init_result_struct_with_error(&res, 1, ex.Message, -1);
            Marshal.GetDelegateForFunctionPointer<rx_result_update_source_t>(runtime->host_def->result_update_source)(runtime->host, res, id);
        }
        void DoUnhandledWrite(ulong id)
        {
            rx_result_struct res;
            CommonInterface.rx_init_result_struct_with_error(&res, 55, "Not Handled!!!", -1);
            Marshal.GetDelegateForFunctionPointer<rx_result_update_source_t>(runtime->host_def->result_update_source)(runtime->host, res, id);
        }
        protected virtual void SourceWriteReceived(ulong id, bool test, bool val)
        {
            DoUnhandledWrite(id);
        }
        protected virtual void SourceWriteReceived(ulong id, bool test, double val)
        {
            DoUnhandledWrite(id);
        }
        protected virtual void SourceWriteReceived(ulong id, bool test, ulong val)
        {
            DoUnhandledWrite(id);
        }
        protected virtual void SourceWriteReceived(ulong id, bool test, long val)
        {
            DoUnhandledWrite(id);
        }
        protected virtual void SourceWriteReceived(ulong id, bool test, string? val)
        {
            DoUnhandledWrite(id);
        }
        internal void Initialize(nint ctx, rx_value_t type_id)
        {
            PlatformInitContext context = new PlatformInitContext(ctx);
            //InitializeSource(context, type_id);
        }
        bool IsStarted()
        {
            lock (initLock)
            {
                return initialized;
            }
        }
        void SetStarted()
        {
            lock (initLock)
            {
                initialized = true;
            }
        }
        void SetStopped()
        {
            lock (initLock)
            {
                initialized = false;
            }
        }
        bool initialized = false;
        object initLock = new object();
        internal void Start(nint ctx)
        {
            SetStarted();
            PlatformStartContext context = new PlatformStartContext(ctx);
        }

        public virtual void StopSource()
        {
        }
        internal void Stop()
        {
            StopSource();
            SetStopped();
        }
        public virtual void DeinitializeSource()
        {
        }
        internal void Deinitialize()
        {
            DeinitializeSource();
        }
        full_value_type PrepareValue()
        {
            full_value_type val = new full_value_type();
            val.quality = 0;
            val.time = new rx_time_struct() { t_value = (ulong)DateTime.UtcNow.ToFileTimeUtc() };

            return val;
        }
        public void UpdateSource(bool value)
        {
            if (IsStarted())
            {
                full_value_type val = PrepareValue();
                if (HostValuesConvertor.ConvertToRxValue(value, ref val.value))
                    Marshal.GetDelegateForFunctionPointer<rx_update_source_t>(runtime->host_def->update_source)(runtime->host, val);
            }
        }

        public void UpdateSource(byte value)
        {
            if (IsStarted())
            {
                full_value_type val = PrepareValue();
                if (HostValuesConvertor.ConvertToRxValue(value, ref val.value))
                    Marshal.GetDelegateForFunctionPointer<rx_update_source_t>(runtime->host_def->update_source)(runtime->host, val);
            }
        }

        public void UpdateSource(sbyte value)
        {
            if (IsStarted())
            {
                full_value_type val = PrepareValue();
                if (HostValuesConvertor.ConvertToRxValue(value, ref val.value))
                    Marshal.GetDelegateForFunctionPointer<rx_update_source_t>(runtime->host_def->update_source)(runtime->host, val);
            }
        }
        public void UpdateSource(short value)
        {
            if (IsStarted())
            {
                full_value_type val = PrepareValue();
                if (HostValuesConvertor.ConvertToRxValue(value, ref val.value))
                    Marshal.GetDelegateForFunctionPointer<rx_update_source_t>(runtime->host_def->update_source)(runtime->host, val);
            }
        }
        public void UpdateSource(ushort value)
        {
            if (IsStarted())
            {
                full_value_type val = PrepareValue();
                if (HostValuesConvertor.ConvertToRxValue(value, ref val.value))
                    Marshal.GetDelegateForFunctionPointer<rx_update_source_t>(runtime->host_def->update_source)(runtime->host, val);
            }
        }
        public void UpdateSource(int value)
        {
            if (IsStarted())
            {
                full_value_type val = PrepareValue();
                if (HostValuesConvertor.ConvertToRxValue(value, ref val.value))
                    Marshal.GetDelegateForFunctionPointer<rx_update_source_t>(runtime->host_def->update_source)(runtime->host, val);
            }
        }
        public void UpdateSource(uint value)
        {
            if (IsStarted())
            {
                full_value_type val = PrepareValue();
                if (HostValuesConvertor.ConvertToRxValue(value, ref val.value))
                    Marshal.GetDelegateForFunctionPointer<rx_update_source_t>(runtime->host_def->update_source)(runtime->host, val);
            }
        }
        public void UpdateSource(long value)
        {
            if (IsStarted())
            {
                full_value_type val = PrepareValue();
                if (HostValuesConvertor.ConvertToRxValue(value, ref val.value))
                    Marshal.GetDelegateForFunctionPointer<rx_update_source_t>(runtime->host_def->update_source)(runtime->host, val);
            }
        }
        public void UpdateSource(ulong value)
        {
            if (IsStarted())
            {
                full_value_type val = PrepareValue();
                if (HostValuesConvertor.ConvertToRxValue(value, ref val.value))
                    Marshal.GetDelegateForFunctionPointer<rx_update_source_t>(runtime->host_def->update_source)(runtime->host, val);
            }
        }
        public void UpdateSource(float value)
        {
            if (IsStarted())
            {
                full_value_type val = PrepareValue();
                if (HostValuesConvertor.ConvertToRxValue(value, ref val.value))
                    Marshal.GetDelegateForFunctionPointer<rx_update_source_t>(runtime->host_def->update_source)(runtime->host, val);
            }
        }
        public void UpdateSource(double value)
        {
            if (IsStarted())
            {
                full_value_type val = PrepareValue();
                if (HostValuesConvertor.ConvertToRxValue(value, ref val.value))
                    Marshal.GetDelegateForFunctionPointer<rx_update_source_t>(runtime->host_def->update_source)(runtime->host, val);
            }
        }
        public void UpdateSource(string value)
        {
            if (IsStarted())
            {
                full_value_type val = PrepareValue();
                if (HostValuesConvertor.ConvertToRxValue(value, ref val.value))
                    Marshal.GetDelegateForFunctionPointer<rx_update_source_t>(runtime->host_def->update_source)(runtime->host, val);
            }
        }
        public abstract string GetSourceName();
        public virtual string GetSourceDirectory()
        {
            return "";
        }
        public abstract Guid GetSourceId();
        public virtual Guid GetParentId()
        {
            return Guid.Empty;
        }

        static readonly byte[] c_def_EmptySource = new byte[] {
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
        };

        public virtual byte[] GetSourceDefinition(out uint version)
        {
            version = 0x20008;
            return c_def_EmptySource;
        }        
    }
}
