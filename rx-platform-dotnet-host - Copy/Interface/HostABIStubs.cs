using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using RxPlatform.Hosting.StaticRemains;

namespace RxPlatform.Hosting.Interface
{
    internal unsafe static class ABIStubs
    {
        // Source ABI stubs
        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        public static void rx_get_code_info_stub(void* whose, sbyte* name, string_value_struct* info)
        {
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        public static rx_result_struct rx_init_source_stub(void* whose, void* ctx, byte value_type)
        {
            unsafe {
                try
                {
                    GCHandle handle = GCHandle.FromIntPtr((nint)whose);
                    PlatformSourceBase? src = handle.Target as PlatformSourceBase;
                    if (src != null)
                    {
                        src.Initialize((nint)ctx, (rx_value_t)value_type);
                    }
                }
                catch (Exception ex)
                {
                    rx_result_struct result = new rx_result_struct();
                    CommonInterface.rx_init_result_struct_with_error(&result, 1, ex.Message, -1);
                    return result;
                }
            }
            return default;
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        public static rx_result_struct rx_start_source_stub(void* whose, void* ctx)
        {
            unsafe
            {
                try
                {
                    GCHandle handle = GCHandle.FromIntPtr((nint)whose);
                    PlatformSourceBase? src = handle.Target as PlatformSourceBase;
                    if (src != null)
                    {
                        src.Start((nint)ctx);
                    }
                }
                catch (Exception ex)
                {
                    rx_result_struct result = new rx_result_struct();
                    CommonInterface.rx_init_result_struct_with_error(&result, 1, ex.Message, -1);
                    return result;
                }
            }
            return default;
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        public static rx_result_struct rx_stop_source_stub(void* whose)
        {
            unsafe
            {
                try
                {
                    GCHandle handle = GCHandle.FromIntPtr((nint)whose);
                    PlatformSourceBase? src = handle.Target as PlatformSourceBase;
                    if (src != null)
                    {
                        src.Stop();
                    }
                }
                catch (Exception ex)
                {
                    rx_result_struct result = new rx_result_struct();
                    CommonInterface.rx_init_result_struct_with_error(&result, 1, ex.Message, -1);
                    return result;
                }
            }
            return default;
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        public static rx_result_struct rx_deinit_source_stub(void* whose)
        {
            unsafe
            {
                try
                {
                    GCHandle handle = GCHandle.FromIntPtr((nint)whose);
                    PlatformSourceBase? src = handle.Target as PlatformSourceBase;
                    if (src != null)
                    {
                        src.Deinitialize();
                    }
                }
                catch (Exception ex)
                {
                    rx_result_struct result = new rx_result_struct();
                    CommonInterface.rx_init_result_struct_with_error(&result, 1, ex.Message, -1);
                    return result;
                }
            }
            return default;
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        public static rx_result_struct rx_write_source_stub(void* whose, ulong id, int test, ulong identity, typed_value_type val, void* ctx)
        {
            unsafe
            {
                try
                {
                    GCHandle handle = GCHandle.FromIntPtr((nint)whose);
                    PlatformSourceBase? src = handle.Target as PlatformSourceBase;
                    if (src != null)
                    {
                        src.SourceWrite(id, test, identity, val, ctx);
                    }
                }
                catch (Exception ex)
                {
                    rx_result_struct result = new rx_result_struct();
                    CommonInterface.rx_init_result_struct_with_error(&result, 1, ex.Message, -1);
                    return result;
                }
            }
            return default;
        }

        // Mapper ABI stubs
        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        public static rx_result_struct rx_init_mapper_stub(void* whose, void* ctx, byte value_type)
        {
            return default;
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        public static rx_result_struct rx_start_mapper_stub(void* whose, void* ctx)
        {
            return default;
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        public static rx_result_struct rx_stop_mapper_stub(void* whose)
        {
            return default;
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        public static rx_result_struct rx_deinit_mapper_stub(void* whose)
        {
            return default;
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        public static void rx_mapped_value_changed_stub(void* whose, void* val, void* ctx)
        {
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        public static void rx_mapper_result_received_stub(void* whose, rx_result_struct result, ulong id, void* ctx)
        {
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        public static void rx_mapper_execute_result_received_stub(void* whose, rx_result_struct result, ulong id, typed_value_type out_val, void* ctx)
        {
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        public static void rx_mapper_event_fired_stub(void* whose, ulong id, int test, ulong identity, void* data, sbyte* queue, int state_machine, int remove, void* ctx)
        {
        }

        // Filter ABI stubs
        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        public static rx_result_struct rx_init_filter_stub(void* whose, void* ctx)
        {
            return default;
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        public static rx_result_struct rx_start_filter_stub(void* whose, void* ctx)
        {
            return default;
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        public static rx_result_struct rx_stop_filter_stub(void* whose)
        {
            return default;
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        public static rx_result_struct rx_deinit_filter_stub(void* whose)
        {
            return default;
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        public static rx_result_struct rx_filter_input_stub(void* whose, void* val)
        {
            return default;
        }   

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        public static rx_result_struct rx_filter_output_stub(void* whose, typed_value_type* val)
        {
            return default;
        }

        // Struct ABI stubs
        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        public static rx_result_struct rx_init_struct_stub(void* whose, void* ctx)
        {
            return default;
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        public static rx_result_struct rx_start_struct_stub(void* whose, void* ctx)
        {
            return default;
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        public static rx_result_struct rx_stop_struct_stub(void* whose)
        {
            return default;
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        public static rx_result_struct rx_deinit_struct_stub(void* whose)
        {
            return default;
        }

        // Variable ABI stubs
        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        public static rx_result_struct rx_init_variable_stub(void* whose, void* ctx)
        {
            return default;
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        public static rx_result_struct rx_start_variable_stub(void* whose, void* ctx)
        {
            return default;
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        public static rx_result_struct rx_stop_variable_stub(void* whose)
        {
            return default;
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        public static rx_result_struct rx_deinit_variable_stub(void* whose)
        {
            return default;
        }

        // Event ABI stubs
        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        public static rx_result_struct rx_init_event_stub(void* whose, void* ctx)
        {
            return default;
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        public static rx_result_struct rx_start_event_stub(void* whose, void* ctx)
        {
            return default;
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        public static rx_result_struct rx_stop_event_stub(void* whose)
        {
            return default;
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        public static rx_result_struct rx_deinit_event_stub(void* whose)
        {
            return default;
        }

        // DataType ABI stubs
        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        public static rx_result_struct rx_init_data_type_stub(void* whose, void* ctx, bytes_value_struct* data)
        {
            return default;
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        public static rx_result_struct rx_start_data_type_stub(void* whose, void* ctx)
        {
            return default;
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        public static rx_result_struct rx_stop_data_type_stub(void* whose)
        {
            return default;
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        public static rx_result_struct rx_deinit_data_type_stub(void* whose)
        {
            return default;
        }

        // Method ABI stubs
        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        public static rx_result_struct rx_init_method_stub(void* whose, void* ctx)
        {
            return default;
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        public static rx_result_struct rx_start_method_stub(void* whose, void* ctx)
        {
            return default;
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        public static rx_result_struct rx_stop_method_stub(void* whose)
        {
            return default;
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        public static rx_result_struct rx_deinit_method_stub(void* whose)
        {
            return default;
        }
        public static rx_result_struct rx_execute_method_stub(void* whose, ulong id, int test, ulong identity, typed_value_type val, void* ctx)
        {
            return default;
        }

        public static rx_result_struct rx_init_program_stub(void* whose, void* ctx)
        {
            return default;
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        public static rx_result_struct rx_start_program_stub(void* whose, void* ctx)
        {
            return default;
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        public static rx_result_struct rx_stop_program_stub(void* whose)
        {
            return default;
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        public static rx_result_struct rx_deinit_program_stub(void* whose)
        {
            return default;
        }

        // Display ABI stubs
        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        public static rx_result_struct rx_init_display_stub(void* whose, void* ctx)
        {
            return default;
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        public static rx_result_struct rx_start_display_stub(void* whose, void* ctx)
        {
            return default;
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        public static rx_result_struct rx_stop_display_stub(void* whose)
        {
            return default;
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        public static rx_result_struct rx_deinit_display_stub(void* whose)
        {
            return default;
        }

        // Object ABI stubs
        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        public static rx_result_struct rx_init_object_stub(void* whose, void* ctx)
        {
            return default;
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        public static rx_result_struct rx_start_object_stub(void* whose, void* ctx)
        {
            return default;
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        public static rx_result_struct rx_stop_object_stub(void* whose)
        {
            return default;
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        public static rx_result_struct rx_deinit_object_stub(void* whose)
        {
            return default;
        }

        // Domain ABI stubs
        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        public static rx_result_struct rx_init_domain_stub(void* whose, void* ctx)
        {
            return default;
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        public static rx_result_struct rx_start_domain_stub(void* whose, void* ctx)
        {
            return default;
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        public static rx_result_struct rx_stop_domain_stub(void* whose)
        {
            return default;
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        public static rx_result_struct rx_deinit_domain_stub(void* whose)
        {
            return default;
        }

        // Application ABI stubs
        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        public static rx_result_struct rx_init_application_stub(void* whose, void* ctx)
        {
            return default;
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        public static rx_result_struct rx_start_application_stub(void* whose, void* ctx)
        {
            return default;
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        public static rx_result_struct rx_stop_application_stub(void* whose)
        {
            return default;
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        public static rx_result_struct rx_deinit_application_stub(void* whose)
        {
            return default;
        }

        // Port ABI stubs
        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        public static rx_result_struct rx_init_port_stub(void* whose, void* ctx)
        {
            return default;
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        public static rx_result_struct rx_start_port_stub(void* whose, void* ctx)
        {
            return default;
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        public static rx_result_struct rx_stop_port_stub(void* whose)
        {
            return default;
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        public static rx_result_struct rx_deinit_port_stub(void* whose)
        {
            return default;
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        public static void rx_stack_assembled_stub(void* whose)
        {
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        public static void rx_stack_disassembled_stub(void* whose)
        {
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        public static void rx_extract_bind_address_stub(void* whose, byte* binder_data, ulong binder_data_size, void* local_addr, void* remote_addr)
        {
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        public static void rx_destroy_endpoint_stub(void* whose, void* endpoint)
        {
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        public static void* rx_construct_listener_endpoint_stub(void* whose, void* local_address, void* remote_address)
        {
            return null;
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        public static void* rx_construct_initiator_endpoint_stub(void* whose)
        {
            return null;
        }

        // Relation ABI stubs
        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        public static rx_result_struct rx_init_relation_stub(void* whose, void* ctx)
        {
            return default;
        }
    }
}
