using ENSACO.RxPlatform.Hosting.Common;
using ENSACO.RxPlatform.Model;
using System.Runtime.InteropServices;
using System.Text;

namespace ENSACO.RxPlatform.Hosting.Common
{
    internal unsafe static class CommonInterface
    {
        public static rx_result_struct CreateErrorResult(string msg, Exception? exc = null, uint code = 1)
        {
            StringBuilder stream = new StringBuilder();
            if(!string.IsNullOrEmpty(msg))
                stream.Append($"{msg};");
            while (exc != null)
            {
                if (stream.Length > 0)
                    stream.Append(";");
                stream.Append(exc.Message);
                exc = exc.InnerException;
            }
            rx_result_struct result = new rx_result_struct();
            rx_init_result_struct_with_error(&result, 1, stream.ToString(), -1);
            return result;
        }
        public static rx_result_struct CreateResultFromException(Exception? exc, uint code = 1)
        {
            if (exc == null)
            {
                rx_result_struct result = new rx_result_struct();
                rx_init_result_struct(&result);
                return result;
            }
            else
            {
                StringBuilder stream = new StringBuilder();
                while (exc != null)
                {
                    if (stream.Length > 0)
                        stream.Append(";");
                    stream.Append(exc.Message);
                    exc = exc.InnerException;
                }
                rx_result_struct result = new rx_result_struct();
                rx_init_result_struct_with_error(&result, 1, stream.ToString(), -1);
                return result;
            }            
        }
        public static rx_node_id_struct CreateNodeIdFromGuid(Guid guid)
        {
            rx_node_id_struct node_id = new rx_node_id_struct();
            byte[] data = guid.ToByteArray();
            rx_uuid_t node_uuid;
            unsafe
            {
                fixed (byte* pData = data)
                {
                    node_uuid = new rx_uuid_t();
                    for (int i = 0; i < 16; i++)
                    {
                        node_uuid.bytes[i] = data[i];
                    }
                    rx_init_uuid_node_id(&node_id, &node_uuid, 999);
                }
            }
            return node_id;
        }

        public static rx_node_id_struct CreateNodeIdFromRxNodeId(RxNodeId id)
        {
            rx_node_id_struct node_id = new rx_node_id_struct();
            switch (id.NodeType)
            {
                case RxNodeIdType.Numeric:
                    unsafe
                    {
                        rx_init_int_node_id(&node_id, id.IntValue, id.Namespace);
                    }
                    break;
                case RxNodeIdType.String:
                    unsafe
                    {
                        if (id.StringValue != null)
                        {
                            rx_init_string_node_id(&node_id, id.StringValue, -1, id.Namespace);
                        }
                        else
                        {
                            rx_init_string_node_id(&node_id, "", 0, id.Namespace);
                        }
                    }
                    break;
                case RxNodeIdType.Uuid:
                    {
                        byte[] data = id.UuidValue.ToByteArray();
                        rx_uuid_t node_uuid;
                        unsafe
                        {
                            fixed (byte* pData = data)
                            {
                                node_uuid = new rx_uuid_t();
                                for (int i = 0; i < 16; i++)
                                {
                                    node_uuid.bytes[i] = data[i];
                                }
                                rx_init_uuid_node_id(&node_id, &node_uuid, id.Namespace);
                            }
                        }
                    }
                    break;
                case RxNodeIdType.Bytes:
                    {
                        byte[]? data = id.BytesValue;
                        unsafe
                        {
                            if (data != null)
                            {
                                fixed (byte* pData = data)
                                {
                                    rx_init_bytes_node_id(&node_id, (nint)pData, (ulong)data.Length, id.Namespace);
                                }
                            }
                            else
                            {
                                rx_init_bytes_node_id(&node_id, nint.Zero, 0, id.Namespace);
                            }
                        }
                    }
                    break;
            }
            
            return node_id;
        }

        public static RxNodeId CreateRxNodeIdFromNodeId(rx_node_id_struct id)
        {
            rx_node_id_struct node_id = new rx_node_id_struct();
            switch ((RxNodeIdType)id.node_type)
            {
                case RxNodeIdType.Numeric:
                    return new RxNodeId(node_id.value.int_value, id.namespace_index);

                case RxNodeIdType.String:
                    {
                        string strValue = "";
                        unsafe
                        {
                            nint cstr = rx_c_str(&id.value.string_value);
                            if (cstr != nint.Zero)
                            {
                                var temp = Marshal.PtrToStringUTF8(cstr);
                                if (temp != null)
                                    strValue = temp;
                            }
                        }
                        return new RxNodeId(strValue, id.namespace_index);
                    }
                case RxNodeIdType.Uuid:
                    {
                        var gid = new Guid();
                        unsafe
                        {                            
                            if(id.value.uuid_value.bytes != null)
                                gid = new Guid(new Span<byte>(id.value.uuid_value.bytes, 16));
                        }
                        return new RxNodeId(gid, id.namespace_index);
                    }
                case RxNodeIdType.Bytes:
                    {
                        var aid = new byte[0];
                        unsafe
                        {
                            ulong size = 0;
                            byte* ptr = (byte*)CommonInterface.rx_c_ptr(&id.value.bstring_value, (ulong*)&size);
                            if (ptr != null)
                                aid = new Span<byte>(ptr, (int)size).ToArray();
                        }
                        return new RxNodeId(aid, id.namespace_index);
                    }
            
            }
            return new RxNodeId();
        }
        public static rx_node_id_struct CreateNodeIdFromInt(uint value)
        {
            rx_node_id_struct node_id = new rx_node_id_struct();
            unsafe
            {
                rx_init_int_node_id(&node_id, value, 1);
            }
            return node_id;
        }

        static internal unsafe Exception? GetExceptionFromResult(rx_result_struct* ret)
        {
            if (CommonInterface.rx_result_ok(ret) == 0)
            {
                ulong sz = CommonInterface.rx_result_errors_count(ret);
                if (sz == 0)
                {
                    return new Exception("Unknown error");
                }
                StringBuilder stream = new StringBuilder();
                for (ulong i = 0; i < sz; i++)
                {
                    uint code = 0;
                    string? msg = Marshal.PtrToStringUTF8(CommonInterface.rx_result_get_error(ret, i, &code));
                    if (msg != null)
                        stream.AppendLine(msg);
                }
                ;
                return new Exception(stream.ToString());
            }
            return null;
        }
#if _WINDOWS
        const string common_dll = "rx-common.dll";
#elif _LINUX
        // Code specific to Linux
        const string common_dll = "rx-common.so";
#else
        // Code specific to macOS
        const string common_dll = "rx-common";//.dylib";
#endif

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint rx_get_new_transaction_id();

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint rx_get_new_handle();

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern nint rx_heap_alloc(ulong size);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rx_heap_free(nint ptr);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern nint rx_heap_realloc(nint ptr, ulong size);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong rx_heap_house_keeping();

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong rx_get_values_count();

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rx_heap_status(nint buffer, ulong* total, ulong* used, ulong* trigger, ulong* alloc);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rx_init_common_library(rx_platform_init_data* init_data);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern void rx_process_stopping();

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern void rx_deinit_common_library();

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rx_is_debug_instance();

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint rx_border_rand(uint min, uint max);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong rx_os_page_size();

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern nint rx_allocate_os_memory(ulong size);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern void rx_deallocate_os_memory(nint p, ulong size);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern nint rx_load_library([MarshalAs(UnmanagedType.LPStr)] string path);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern nint rx_get_func_address(nint module_handle, [MarshalAs(UnmanagedType.LPStr)] string name);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern void rx_unload_library(nint module_handle);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rx_match_pattern([MarshalAs(UnmanagedType.LPStr)] string str, [MarshalAs(UnmanagedType.LPStr)] string pattern, int case_sensitive);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rx_os_get_system_time(rx_time_struct* st);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rx_os_to_local_time(rx_time_struct* st);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rx_os_to_utc_time(rx_time_struct* st);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong rx_os_get_ms(rx_time_struct* st);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rx_os_split_time(rx_time_struct* st, rx_full_time* full);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rx_os_collect_time(rx_full_time* full, rx_time_struct* st);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong rx_get_tick_count();

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong rx_get_us_ticks();

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern void rx_ms_sleep(uint timeout);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern void rx_us_sleep(ulong timeout);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern void rx_slim_lock_create(nint plock);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern void rx_slim_lock_destroy(nint plock);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern void rx_slim_lock_aquire(nint plock);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern void rx_slim_lock_release(nint plock);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern void rx_rw_slim_lock_create(nint plock);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern void rx_rw_slim_lock_destroy(nint plock);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern void rx_rw_slim_lock_aquire_reader(nint plock);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern void rx_rw_slim_lock_release_reader(nint plock);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern void rx_rw_slim_lock_aquire_writter(nint plock);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern void rx_rw_slim_lock_release_writter(nint plock);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint rx_handle_wait(nint what, uint timeout);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint rx_handle_wait_us(nint what, ulong timeout);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint rx_handle_wait_for_multiple(nint what, ulong count, uint timeout);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern nint rx_event_create(int initialy_set);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rx_event_destroy(nint hndl);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rx_event_set(nint hndl);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern void rx_aquire_weak_reference(nint data);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern void rx_release_weak_reference(nint data);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern nint rx_lock_weak_reference(nint data);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rx_parse_value_type_name([MarshalAs(UnmanagedType.LPStr)] string strtype, nint type);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern nint rx_get_value_type_name(byte type);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rx_init_string_value_struct(string_value_struct* data, [MarshalAs(UnmanagedType.LPStr)] string val, int count);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern nint rx_c_str(string_value_struct* data);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rx_copy_string_value(string_value_struct* dest, string_value_struct* src);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern void rx_destory_string_value_struct(string_value_struct* data);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rx_init_bytes_value_struct(bytes_value_struct* data, nint bytes, ulong len);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern nint rx_c_ptr(bytes_value_struct* data, ulong* size);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rx_copy_bytes_value(bytes_value_struct* dest, bytes_value_struct* src);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern void rx_destory_bytes_value_struct(bytes_value_struct* data);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rx_uuid_to_string(rx_uuid_t* uuid, nint str);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rx_string_to_uuid([MarshalAs(UnmanagedType.LPStr)] string str, rx_uuid_t* uuid);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rx_is_null_uuid(rx_uuid_t* uuid);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rx_init_null_node_id(rx_node_id_struct* data);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rx_init_int_node_id(rx_node_id_struct* data, uint id, ushort namesp);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rx_init_string_node_id(rx_node_id_struct* data, [MarshalAs(UnmanagedType.LPStr)] string id, int count, ushort namesp);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rx_init_uuid_node_id(rx_node_id_struct* data, rx_uuid_t* uuid, ushort namesp);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rx_init_bytes_node_id(rx_node_id_struct* data, nint bytes, ulong len, ushort namesp);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rx_copy_node_id(rx_node_id_struct* data, rx_node_id_struct* src);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rx_move_node_id(rx_node_id_struct* data, rx_node_id_struct* src);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rx_compare_node_ids(rx_node_id_struct* left, rx_node_id_struct* right);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rx_is_null_node_id(rx_node_id_struct* data);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rx_node_id_to_string(rx_node_id_struct* data, string_value_struct* str);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rx_node_id_from_string(rx_node_id_struct* data, [MarshalAs(UnmanagedType.LPStr)] string str);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern void rx_destory_node_id(rx_node_id_struct* data);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rx_init_node_id_value(nint val, rx_node_id_struct* data);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rx_copy_result_struct(rx_result_struct* res, rx_result_struct* src);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rx_move_result_struct(rx_result_struct* res, rx_result_struct* src);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rx_result_ok(rx_result_struct* res);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong rx_result_errors_count(rx_result_struct* res);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern nint rx_result_get_error(rx_result_struct* res, ulong idx, uint* code);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern void rx_destroy_result_struct(rx_result_struct* res);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern void rx_init_meta_data(rx_meta_data_struct* what);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern void rx_deinit_meta_data(rx_meta_data_struct* what);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rx_init_values_array_struct(values_array_struct* data, nint values, ulong len);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern void rx_destory_values_array_struct(values_array_struct* data);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rx_init_null_reference(rx_reference_struct* refStruct);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rx_init_path_reference(rx_reference_struct* refStruct, [MarshalAs(UnmanagedType.LPStr)] string path, int count);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rx_init_id_reference(rx_reference_struct* refStruct, rx_node_id_struct* id);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rx_copy_reference(rx_reference_struct* refStruct, rx_reference_struct* src);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rx_move_reference(rx_reference_struct* refStruct, rx_reference_struct* src);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rx_compare_references(rx_reference_struct* left, rx_reference_struct* right);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rx_is_null_reference(rx_reference_struct* refStruct);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rx_reference_to_string(rx_reference_struct* data, string_value_struct* str);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rx_reference_from_string(rx_reference_struct* data, [MarshalAs(UnmanagedType.LPStr)] string str);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern void rx_deinit_reference(rx_reference_struct* refStruct);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern void rx_init_result_struct(rx_result_struct* res);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rx_init_result_struct_with_error(rx_result_struct* res, uint code, [MarshalAs(UnmanagedType.LPStr)] string text, int count);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rx_init_result_struct_with_errors(rx_result_struct* res, nint codes, nint texts, ulong errors_count);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rx_result_add_error(rx_result_struct* res, uint code, [MarshalAs(UnmanagedType.LPStr)] string text, int count);


        // rx_init_lock_reference
        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern void rx_init_lock_reference(
            lock_reference_struct* data, // lock_reference_struct*
            nint target,
            lock_reference_def_struct* def // lock_reference_def_struct*
        );

        // rx_init_lock_reference2
        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern nint rx_init_lock_reference2(
            nint target,
            lock_reference_def_struct2* def // lock_reference_def_struct2*
        );

        // rx_aquire_lock_reference
        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern void rx_aquire_lock_reference(
            lock_reference_struct* data // lock_reference_struct*
        );

        // rx_release_lock_reference
        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern void rx_release_lock_reference(
            lock_reference_struct* data // lock_reference_struct*
        );


        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern void rx_destroy_value(ref typed_value_type val);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rx_init_null_value(ref typed_value_type val);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rx_init_bool_value(ref typed_value_type val, byte data);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rx_init_int8_value(ref typed_value_type val, sbyte data);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rx_init_uint8_value(ref typed_value_type val, byte data);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rx_init_int16_value(ref typed_value_type val, short data);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rx_init_uint16_value(ref typed_value_type val, ushort data);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rx_init_int32_value(ref typed_value_type val, int data);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rx_init_uint32_value(ref typed_value_type val, uint data);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rx_init_int64_value(ref typed_value_type val, long data);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rx_init_uint64_value(ref typed_value_type val, ulong data);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rx_init_float_value(ref typed_value_type val, float data);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rx_init_double_value(ref typed_value_type val, double data);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rx_init_complex_value(ref typed_value_type val, complex_value_struct data);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rx_init_struct_value(ref typed_value_type val, nint data, ulong count);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rx_init_struct_value_with_ptrs(ref typed_value_type val, nint data, ulong count);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rx_init_array_value(ref typed_value_type val, byte type, nint data, ulong count);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rx_init_array_value_with_ptrs(ref typed_value_type val, byte type, nint data, ulong count);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rx_init_array_value_with_move(ref typed_value_type val, byte type, nint data, ulong count);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rx_init_array_value_with_ptrs_move(ref typed_value_type val, byte type, nint data, ulong count);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rx_init_bool_array_value(ref typed_value_type val, nint data, ulong count);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rx_init_int8_array_value(ref typed_value_type val, nint data, ulong count);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rx_init_uint8_array_value(ref typed_value_type val, nint data, ulong count);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rx_init_int16_array_value(ref typed_value_type val, nint data, ulong count);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rx_init_uint16_array_value(ref typed_value_type val, nint data, ulong count);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rx_init_int32_array_value(ref typed_value_type val, nint data, ulong count);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rx_init_uint32_array_value(ref typed_value_type val, nint data, ulong count);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rx_init_int64_array_value(ref typed_value_type val, nint data, ulong count);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rx_init_uint64_array_value(ref typed_value_type val, nint data, ulong count);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rx_init_float_array_value(ref typed_value_type val, nint data, ulong count);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rx_init_double_array_value(ref typed_value_type val, nint data, ulong count);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rx_init_complex_array_value(ref typed_value_type val, nint data, ulong count);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rx_init_string_value(ref typed_value_type val, string data, int count);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rx_init_bytes_value(ref typed_value_type val, nint data, ulong count);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rx_init_uuid_value(ref typed_value_type val, ref rx_uuid_t data);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rx_init_time_value(ref typed_value_type val, rx_time_struct data);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rx_init_node_id_value(ref typed_value_type val, ref rx_node_id_struct data);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rx_init_string_array_value(ref typed_value_type val, nint data, ulong size);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rx_init_uuid_array_value(ref typed_value_type val, nint data, ulong count);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rx_init_time_array_value(ref typed_value_type val, nint data, ulong count);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rx_init_bytes_array_value(ref typed_value_type val, nint data, nint sizes, ulong count);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rx_init_node_id_array_value(ref typed_value_type val, nint data, ulong count);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rx_init_struct_array_value(ref typed_value_type val, nint data, ulong count);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rx_init_struct_array_value_with_ptrs(ref typed_value_type val, nint data, ulong count);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rx_parse_string(ref typed_value_type val, string data);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern void rx_assign_value(ref typed_value_type val, ref typed_value_type right);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern void rx_copy_value(ref typed_value_type val, ref typed_value_type right);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern void rx_move_value(ref typed_value_type val, ref typed_value_type right);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rx_convert_value(ref typed_value_type val, byte type);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rx_compare_values(ref typed_value_type val, ref typed_value_type right);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rx_get_array_value(ulong index, ref typed_value_type val, ref typed_value_type right);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rx_is_null_value(ref typed_value_type val);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rx_is_float_value(ref typed_value_type val);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rx_is_complex_value(ref typed_value_type val);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rx_is_numeric_value(ref typed_value_type val);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rx_is_integer_value(ref typed_value_type val);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rx_is_unassigned_value(ref typed_value_type val);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rx_is_bool_value(ref typed_value_type val);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rx_is_string_value(ref typed_value_type val);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rx_is_bytes_value(ref typed_value_type val);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rx_is_array_value(ref typed_value_type val);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rx_get_array_size(ref typed_value_type val, out ulong size);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rx_is_struct(ref typed_value_type val);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rx_get_struct_size(ref typed_value_type val, out ulong size);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rx_get_struct_value(ulong idx, ref typed_value_type out_val, ref typed_value_type val);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rx_get_sub_struct_value(ulong idx_count, nint idxs, ref typed_value_type out_val, ref typed_value_type val);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rx_set_sub_struct_value(ulong idx_count, nint idxs, ref typed_value_type in_val, ref typed_value_type val);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rx_is_sub_struct(ulong idx_count, nint idxs, ref typed_value_type val);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rx_get_sub_struct_size(ulong idx_count, nint idxs, ref typed_value_type val, out ulong size);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rx_is_sub_array_value(ulong idx_count, nint idxs, ref typed_value_type val);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rx_get_sub_array_size(ulong idx_count, nint idxs, ref typed_value_type val, out ulong size);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rx_get_float_value(ref typed_value_type val, ulong idx, out double value, out byte type);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rx_get_complex_value(ref typed_value_type val, ulong idx, out complex_value_struct value);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rx_get_integer_value(ref typed_value_type val, ulong idx, out long value, out byte type);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rx_get_unassigned_value(ref typed_value_type val, ulong idx, out ulong value, out byte type);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rx_get_bool_value(ref typed_value_type val, ulong idx, out int value);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rx_get_string_value(ref typed_value_type val, ulong idx, out string_value_struct value);

        [DllImport(common_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int rx_get_bytes_value(ref typed_value_type val, ulong idx, out bytes_value_struct value);

    }
}
