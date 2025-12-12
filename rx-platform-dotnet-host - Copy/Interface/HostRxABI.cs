using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace RxPlatform.Hosting.Interface
{


    internal unsafe delegate void reference_destroy_func_t(void* reference);

    // Delegate types for function pointers in platform_api2
    internal unsafe delegate void rxWriteLogDelegate(ulong plugin, int type, [MarshalAs(UnmanagedType.LPStr)] string library, [MarshalAs(UnmanagedType.LPStr)] string source, ushort level, [MarshalAs(UnmanagedType.LPStr)] string code, [MarshalAs(UnmanagedType.LPStr)] string message);
    internal unsafe delegate rx_result_struct rxRegisterItemDelegate(ulong plugin, byte item_type, [MarshalAs(UnmanagedType.LPStr)] string name, [MarshalAs(UnmanagedType.LPStr)] string path, rx_node_id_struct* id, rx_node_id_struct* parent, uint version, rx_time_struct modified, uint stream_version, byte* data, ulong count);
    internal unsafe delegate rx_result_struct rxRegisterRuntimeItemDelegate(ulong plugin, byte item_type, [MarshalAs(UnmanagedType.LPStr)] string name, [MarshalAs(UnmanagedType.LPStr)] string path, rx_node_id_struct* id, rx_node_id_struct* parent, uint version, rx_time_struct modified, uint stream_version, byte* data, ulong count);
    internal unsafe delegate void rxLockRuntimeManagerDelegate();
    internal unsafe delegate void rxUnlockRuntimeManagerDelegate();

    internal unsafe delegate rx_result_struct rxRegisterSourceRuntimeDelegate(ulong plugin, rx_node_id_struct* id, plugin_source_register_data construct_data);
    internal unsafe delegate rx_result_struct rxRegisterMapperRuntimeDelegate(ulong plugin, rx_node_id_struct* id, rx_mapper_constructor_t construct_func);
    internal unsafe delegate rx_result_struct rxRegisterFilterRuntimeDelegate(ulong plugin, rx_node_id_struct* id, rx_filter_constructor_t construct_func);
    internal unsafe delegate rx_result_struct rxRegisterStructRuntimeDelegate(ulong plugin, rx_node_id_struct* id, rx_struct_constructor_t construct_func);
    internal unsafe delegate rx_result_struct rxRegisterVariableRuntimeDelegate(ulong plugin, rx_node_id_struct* id, rx_variable_constructor_t construct_func);
    internal unsafe delegate rx_result_struct rxRegisterEventRuntimeDelegate(ulong plugin, rx_node_id_struct* id, rx_event_constructor_t construct_func);
    internal unsafe delegate rx_result_struct rxRegisterMethodRuntimeDelegate(ulong plugin, rx_node_id_struct* id, rx_method_constructor_t construct_func);
    internal unsafe delegate rx_result_struct rxRegisterProgramRuntimeDelegate(ulong plugin, rx_node_id_struct* id, rx_program_constructor_t construct_func);
    internal unsafe delegate rx_result_struct rxRegisterDisplayRuntimeDelegate(ulong plugin, rx_node_id_struct* id, rx_display_constructor_t construct_func);
    internal unsafe delegate rx_result_struct rxRegisterObjectRuntimeDelegate(ulong plugin, rx_node_id_struct* id, plugin_object_register_data construct_data);
    internal unsafe delegate rx_result_struct rxRegisterApplicationRuntimeDelegate(ulong plugin, rx_node_id_struct* id, plugin_application_register_data construct_data);
    internal unsafe delegate rx_result_struct rxRegisterDomainRuntimeDelegate(ulong plugin, rx_node_id_struct* id, plugin_domain_register_data construct_data);
    internal unsafe delegate rx_result_struct rxRegisterPortRuntimeDelegate(ulong plugin, rx_node_id_struct* id, plugin_port_register_data construct_data);
    internal unsafe delegate rx_result_struct rxRegisterRelationRuntimeDelegate(ulong plugin, rx_node_id_struct* id, plugin_relation_register_data construct_data);
    
    internal unsafe delegate rx_result_struct rxInitCtxBindItemDelegate(void* ctx, [MarshalAs(UnmanagedType.LPStr)] string path, uint* handle, void** rt_ctx, bind_callback_data* callback);
    internal unsafe delegate sbyte* rxInitCtxGetCurrentPathDelegate(void* ctx);
    internal unsafe delegate rx_result_struct rxInitCtxGetLocalValueDelegate(void* ctx, [MarshalAs(UnmanagedType.LPStr)] string path, typed_value_type* val);
    internal unsafe delegate rx_result_struct rxInitCtxSetLocalValueDelegate(void* ctx, [MarshalAs(UnmanagedType.LPStr)] string path, typed_value_type val);
    internal unsafe delegate rx_result_struct rxInitCtxGetMappingValuesDelegate(void* ctx, rx_node_id_struct* id, [MarshalAs(UnmanagedType.LPStr)] string path, values_array_struct* vals);
    internal unsafe delegate rx_result_struct rxInitCtxGetSourceValuesDelegate(void* ctx, rx_node_id_struct* id, [MarshalAs(UnmanagedType.LPStr)] string path, values_array_struct* vals);
    internal unsafe delegate void rxInitCtxGetItemMetaDelegate(void* ctx, rx_node_id_struct** id, [MarshalAs(UnmanagedType.LPStr)] ref string path, [MarshalAs(UnmanagedType.LPStr)] ref string name);

    internal unsafe delegate rx_result_struct rxInitCtxGetDataTypeDelegate(int version, void* ctx, [MarshalAs(UnmanagedType.LPStr)] string path, string_value_struct* data);

    internal unsafe delegate sbyte* rxStartCtxGetCurrentPathDelegate(void* ctx);
    internal unsafe delegate rx_result_struct rxStartCtxGetLocalValueDelegate(void* ctx, [MarshalAs(UnmanagedType.LPStr)] string path, typed_value_type* val);
    internal unsafe delegate rx_result_struct rxStartCtxSetLocalValueDelegate(void* ctx, [MarshalAs(UnmanagedType.LPStr)] string path, typed_value_type val);
    internal unsafe delegate rx_result_struct rxStartCtxSubscribeRelationDelegate(void* ctx, [MarshalAs(UnmanagedType.LPStr)] string name, relation_subscriber_data* callback);

    internal unsafe delegate nint rxRegisterStorageTypeDelegate(ulong plugin, [MarshalAs(UnmanagedType.LPStr)] string prefix, rx_storage_constructor_t construct_func);

    internal unsafe delegate void rxGetCodeInfoT(void* whose, [MarshalAs(UnmanagedType.LPStr)] string name, string_value_struct* info);

    internal unsafe delegate void relation_connected_callback_t(void* target, [MarshalAs(UnmanagedType.LPStr)] string name, rx_node_id_struct* id);
    internal unsafe delegate void relation_disconnected_callback_t(void* target, [MarshalAs(UnmanagedType.LPStr)] string name);

    internal unsafe delegate uint rxStartCtxCreateTimerDelegate(void* ctx, int type, plugin_job_struct* job, uint period);
    internal unsafe delegate rx_result_struct rxCtxGetValueDelegate(void* ctx, uint handle, typed_value_type* val);
    internal unsafe delegate rx_result_struct rxCtxSetValueDelegate(void* ctx, uint handle, typed_value_type val);
    internal unsafe delegate void rxCtxSetAsyncPendingDelegate(void* ctx, uint handle, typed_value_type val);
    // Add these delegates for platform_runtime_api6 function pointers
    internal unsafe delegate rx_result_struct rxInitCtxConnectItemDelegate(void* ctx, [MarshalAs(UnmanagedType.LPStr)] string path, uint rate, uint* handle, void** rt_ctx, nint callback);
    internal unsafe delegate rx_result_struct rxCtxWriteConnectedDelegate(void* ctx, uint handle, typed_value_type val, ulong trans_id);
    internal unsafe delegate rx_result_struct rxCtxExecuteConnectedDelegate(void* ctx, uint handle, typed_value_type val, ulong trans_id);
    internal unsafe delegate rx_result_struct rxRegisterDataTypeRuntimeDelegate(ulong plugin, rx_node_id_struct* id, rx_data_type_constructor_t construct_func);
    internal unsafe delegate rx_result_struct rxCtxWriteBindedDelegate(void* ctx, uint handle, typed_value_type val, ulong trans_id);
    internal unsafe delegate ulong rxGetNewUniqueIdDelegate();


    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct platform_general_api
    {
        public nint pWriteLog; //rxWriteLogDelegate
        public nint pRegisterItem; //rxRegisterItemDelegate
        public nint prxRegisterRuntimeItem; //rxRegisterRuntimeItemDelegate 
        public nint prxLockRuntimeManager; //rxLockRuntimeManagerDelegate
        public nint prxUnlockRuntimeManager; //rxUnlockRuntimeManagerDelegate 
    }
    

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct platform_runtime_api6                  
    {
        public nint prxRegisterSourceRuntime;
        public nint prxRegisterMapperRuntime;
        public nint prxRegisterMapperRuntime3;
        public nint prxRegisterFilterRuntime;
        public nint prxRegisterStructRuntime;
        public nint prxRegisterVariableRuntime;
        public nint prxRegisterEventRuntime;
        public nint prxRegisterMethodRuntime;
        public nint prxRegisterProgramRuntime;
        public nint prxRegisterDisplayRuntime;
        public nint prxRegisterObjectRuntime;
        public nint prxRegisterApplicationRuntime;
        public nint prxRegisterDomainRuntime;
        public nint prxRegisterPortRuntime;
        public nint prxRegisterRelationRuntime;
        public nint prxInitCtxBindItem;
        public nint prxInitCtxGetCurrentPath;
        public nint prxInitCtxGetLocalValue;
        public nint prxInitCtxSetLocalValue;
        public nint prxInitCtxGetMappingValues;
        public nint prxInitCtxGetSourceValues;
        public nint prxInitCtxGetItemMeta;
        public nint prxStartCtxGetCurrentPath;
        public nint prxStartCtxCreateTimer;
        public nint prxStartCtxGetLocalValue;
        public nint prxStartCtxSetLocalValue;
        public nint prxStartCtxSubscribeRelation;
        public nint prxCtxGetValue;
        public nint prxCtxSetValue;
        public nint prxCtxSetAsyncPending;
        public nint prxInitCtxConnectItem;
        public nint prxCtxWriteConnected;
        public nint prxCtxExecuteConnected;
        public nint prxInitCtxGetDataType;
        public nint prxRegisterDataTypeRuntime;
        public nint prxCtxWriteBinded;
        public nint rxGetNewUniqueId;
    }

    internal unsafe struct PlatformABITranslated
    {
        // general api
        public rxWriteLogDelegate pWriteLog;
        public rxRegisterItemDelegate pRegisterItem;
        public rxRegisterRuntimeItemDelegate prxRegisterRuntimeItem;
        public rxLockRuntimeManagerDelegate prxLockRuntimeManager;
        public rxUnlockRuntimeManagerDelegate prxUnlockRuntimeManager;

        // runtime api 6
        public rxRegisterSourceRuntimeDelegate prxRegisterSourceRuntime;
        public rxRegisterMapperRuntimeDelegate prxRegisterMapperRuntime;
        public rxRegisterFilterRuntimeDelegate prxRegisterFilterRuntime;
        public rxRegisterStructRuntimeDelegate prxRegisterStructRuntime;
        public rxRegisterVariableRuntimeDelegate prxRegisterVariableRuntime;
        public rxRegisterEventRuntimeDelegate prxRegisterEventRuntime;
        public rxRegisterMethodRuntimeDelegate prxRegisterMethodRuntime;
        public rxRegisterProgramRuntimeDelegate prxRegisterProgramRuntime;
        public rxRegisterDisplayRuntimeDelegate prxRegisterDisplayRuntime;
        public rxRegisterObjectRuntimeDelegate prxRegisterObjectRuntime;
        public rxRegisterApplicationRuntimeDelegate prxRegisterApplicationRuntime;
        public rxRegisterDomainRuntimeDelegate prxRegisterDomainRuntime;
        public rxRegisterPortRuntimeDelegate prxRegisterPortRuntime;
        public rxRegisterRelationRuntimeDelegate prxRegisterRelationRuntime;
        public rxInitCtxBindItemDelegate prxInitCtxBindItem;
        public rxInitCtxGetCurrentPathDelegate prxInitCtxGetCurrentPath;
        public rxInitCtxGetLocalValueDelegate prxInitCtxGetLocalValue;
        public rxInitCtxSetLocalValueDelegate prxInitCtxSetLocalValue;
        public rxInitCtxGetMappingValuesDelegate prxInitCtxGetMappingValues;
        public rxInitCtxGetSourceValuesDelegate prxInitCtxGetSourceValues;
        public rxInitCtxGetItemMetaDelegate prxInitCtxGetItemMeta;
        public rxStartCtxGetCurrentPathDelegate prxStartCtxGetCurrentPath;
        public rxStartCtxCreateTimerDelegate prxStartCtxCreateTimer;
        public rxStartCtxGetLocalValueDelegate prxStartCtxGetLocalValue;
        public rxStartCtxSetLocalValueDelegate prxStartCtxSetLocalValue;
        public rxStartCtxSubscribeRelationDelegate prxStartCtxSubscribeRelation;
        public rxCtxGetValueDelegate prxCtxGetValue;
        public rxCtxSetValueDelegate prxCtxSetValue;
        public rxCtxSetAsyncPendingDelegate prxCtxSetAsyncPending;
        public rxInitCtxConnectItemDelegate prxInitCtxConnectItem;
        public rxCtxWriteConnectedDelegate prxCtxWriteConnected;
        public rxCtxExecuteConnectedDelegate prxCtxExecuteConnected;
        public rxInitCtxGetDataTypeDelegate prxInitCtxGetDataType;
        public rxRegisterDataTypeRuntimeDelegate prxRegisterDataTypeRuntime;
        public rxCtxWriteBindedDelegate prxCtxWriteBinded;
        public rxGetNewUniqueIdDelegate rxGetNewUniqueId;
    }


    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct platform_api6
    {
        public platform_general_api general;
        public platform_runtime_api6 runtime;
        //public platform_storage_api storage;
    }

    internal unsafe delegate void rx_runtime_register_func_t(rx_node_id_struct* id, lock_reference_struct* reference);
    internal unsafe delegate void rx_runtime_unregister_func_t(rx_node_id_struct* id);

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct plugin_source_register_data
    {
        public nint constructor; // rx_source_constructor_t
        public nint register_func;
        public nint unregister_func;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct plugin_object_register_data
    {
        public rx_object_constructor_t constructor;
        public rx_runtime_register_func_t register_func;
        public rx_runtime_unregister_func_t unregister_func;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct plugin_application_register_data
    {
        public rx_application_constructor_t constructor;
        public rx_runtime_register_func_t register_func;
        public rx_runtime_unregister_func_t unregister_func;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct plugin_domain_register_data
    {
        public rx_domain_constructor_t constructor;
        public rx_runtime_register_func_t register_func;
        public rx_runtime_unregister_func_t unregister_func;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct plugin_port_register_data
    {
        public rx_port_constructor_t constructor;
        public rx_runtime_register_func_t register_func;
        public rx_runtime_unregister_func_t unregister_func;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct plugin_relation_register_data
    {
        public rx_relation_constructor_t constructor;
        public rx_runtime_register_func_t register_func;
        public rx_runtime_unregister_func_t unregister_func;
    }


    // --- Constructor function pointer types from rx_abi.h ---

    // Source
    internal unsafe delegate plugin_source_runtime_struct4_t* rx_source_constructor_t();

    // Mapper
    internal unsafe delegate plugin_mapper_runtime_struct_t* rx_mapper_constructor_t();

    // Mapper3
    internal unsafe delegate plugin_mapper_runtime_struct3_t* rx_mapper_constructor3_t();

    // Filter
    internal unsafe delegate plugin_filter_runtime_struct_t* rx_filter_constructor_t();

    // Struct
    internal unsafe delegate plugin_struct_runtime_struct_t* rx_struct_constructor_t();

    // Variable
    internal unsafe delegate plugin_variable_runtime_struct_t* rx_variable_constructor_t();

    // Event
    internal unsafe delegate plugin_event_runtime_struct_t* rx_event_constructor_t();

    // Object
    internal unsafe delegate plugin_object_runtime_struct_t* rx_object_constructor_t();

    // Application
    internal unsafe delegate plugin_application_runtime_struct_t* rx_application_constructor_t();

    // Domain
    internal unsafe delegate plugin_domain_runtime_struct_t* rx_domain_constructor_t();

    // Port
    internal unsafe delegate plugin_port_runtime_struct_t* rx_port_constructor_t();

    // Relation
    internal unsafe delegate plugin_relation_runtime_struct_t* rx_relation_constructor_t();

    // DataType
    internal unsafe delegate plugin_data_type_runtime_struct_t* rx_data_type_constructor_t();

    // Method
    internal unsafe delegate plugin_method_runtime_struct_t* rx_method_constructor_t();

    // Program
    internal unsafe delegate plugin_program_runtime_struct_t* rx_program_constructor_t();

    // Display
    internal unsafe delegate plugin_display_runtime_struct_t* rx_display_constructor_t();

    // Storage
    internal unsafe delegate plugin_storage_struct_t* rx_storage_constructor_t();

    // --- plugin_*runtime_struct_t types from rx_abi.h ---

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct plugin_source_runtime_struct4_t
    {
        public lock_reference_struct anchor;
        public void* host;
        public plugin_source_def_struct* def; // plugin_source_def_struct*
        public host_source_def_struct4* host_def; // host_source_def_struct*
        public uint io_data;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct plugin_mapper_runtime_struct_t
    {
        public lock_reference_struct anchor;
        public void* host;
        public void* def; // plugin_mapper_def_struct*
        public void* host_def; // host_mapper_def_struct*
        public uint io_data;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct plugin_mapper_runtime_struct3_t
    {
        public lock_reference_struct anchor;
        public void* host;
        public void* def; // plugin_mapper_def_struct3*
        public void* host_def; // host_mapper_def_struct3*
        public uint io_data;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct plugin_mapper_runtime_struct4_t
    {
        public lock_reference_struct anchor;
        public void* host;
        public void* def; // plugin_mapper_def_struct3*
        public void* host_def; // host_mapper_def_struct4*
        public uint io_data;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct plugin_filter_runtime_struct_t
    {
        public lock_reference_struct anchor;
        public void* host;
        public void* def; // plugin_filter_def_struct*
        public void* host_def; // host_filter_def_struct*
        public uint io_data;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct plugin_struct_runtime_struct_t
    {
        public lock_reference_struct anchor;
        public void* host;
        public void* def; // plugin_struct_def_struct*
        public void* host_def; // host_struct_def_struct*
        public uint io_data;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct plugin_variable_runtime_struct_t
    {
        public lock_reference_struct anchor;
        public void* host;
        public void* def; // plugin_variable_def_struct*
        public void* host_def; // host_variable_def_struct*
        public uint io_data;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct plugin_event_runtime_struct_t
    {
        public lock_reference_struct anchor;
        public void* host;
        public void* def; // plugin_event_def_struct*
        public void* host_def; // host_event_def_struct*
        public uint io_data;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct plugin_data_type_runtime_struct_t
    {
        public lock_reference_struct anchor;
        public void* host;
        public void* def; // plugin_data_type_def_struct*
        public void* host_def; // host_data_type_def_struct*
    }

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct plugin_method_runtime_struct_t
    {
        public lock_reference_struct anchor;
        public void* host;
        public void* def; // plugin_method_def_struct*
        public void* host_def; // host_method_def_struct*
    }

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct plugin_program_runtime_struct_t
    {
        public lock_reference_struct anchor;
        public void* host;
        public void* def; // plugin_program_def_struct*
        public void* host_def; // host_program_def_struct*
        public uint io_data;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct plugin_display_runtime_struct_t
    {
        public lock_reference_struct anchor;
        public void* host;
        public void* def; // plugin_display_def_struct*
        public void* host_def; // host_display_def_struct*
        public uint io_data;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct plugin_object_runtime_struct_t
    {
        public lock_reference_struct anchor;
        public void* host;
        public void* def; // plugin_object_def_struct*
        public void* host_def; // host_object_def_struct*
        public uint io_data;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct plugin_object_runtime_struct2_t
    {
        public lock_reference_struct anchor;
        public lock_reference_struct host;
        public void* def; // plugin_object_def_struct*
        public void* host_def; // host_object_def_struct*
        public uint io_data;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct plugin_domain_runtime_struct_t
    {
        public lock_reference_struct anchor;
        public void* host;
        public void* def; // plugin_domain_def_struct*
        public void* host_def; // host_domain_def_struct*
        public uint io_data;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct plugin_application_runtime_struct_t
    {
        public lock_reference_struct anchor;
        public void* host;
        public void* def; // plugin_application_def_struct*
        public void* host_def; // host_application_def_struct*
        public uint io_data;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct plugin_port_runtime_struct_t
    {
        public lock_reference_struct anchor;
        public void* host;
        public void* def; // plugin_port_def_struct*
        public void* host_def; // host_port_def_struct*
        public uint io_data;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct plugin_relation_runtime_struct_t
    {
        public lock_reference_struct anchor;
        public void* host;
        public void* def; // plugin_relation_def_struct*
        public void* host_def; // host_relation_def_struct*
        public uint io_data;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct plugin_storage_struct_t
    {
        public lock_reference_struct anchor;
        public void* host;
        public void* def; // plugin_storage_def_struct*
        public void* host_def; // host_storage_def_struct*
    }

    internal unsafe delegate void bind_callback_func_t(void* target, typed_value_type* value);

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct bind_callback_data
    {
        public void* target;
        public bind_callback_func_t callback;
    }

    internal unsafe delegate void plugin_job_process_func_t(void* job);

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct plugin_job_struct
    {
        public lock_reference_struct anchor;
        public plugin_job_process_func_t process;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct relation_subscriber_data
    {
        public void* target;
        public relation_connected_callback_t connected_callback;
        public relation_disconnected_callback_t disconnected_callback;
    }
    // Delegate definitions for storage ABI
    internal unsafe delegate rx_result_struct rx_init_storage_t(void* whose, sbyte* reference);
    internal unsafe delegate rx_result_struct rx_deinit_storage_t(void* whose);
    internal unsafe delegate rx_result_struct construct_storage_connection_t(void* whose);
    internal unsafe delegate rx_result_struct rx_post_storage_job_t(void* whose, int type, plugin_job_struct* job, uint period);

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct plugin_storage_def_struct
    {
        public rx_get_code_info_t code_info; // rx_get_code_info_t (delegate, can be defined if needed)
        public rx_init_storage_t init_storage;
        public rx_deinit_storage_t deinit_storage;
        public construct_storage_connection_t construct_storage_connection;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct host_storage_def_struct
    {
        public rx_post_storage_job_t post_job;
    }

    internal unsafe delegate void rx_get_code_info_t(void* whose, [MarshalAs(UnmanagedType.LPStr)] string name, string_value_struct* info);

    // Source ABI delegates
    internal unsafe delegate rx_result_struct rx_init_source_t(void* whose, void* ctx, byte value_type);
    internal unsafe delegate rx_result_struct rx_start_source_t(void* whose, void* ctx);
    internal unsafe delegate rx_result_struct rx_stop_source_t(void* whose);
    internal unsafe delegate rx_result_struct rx_deinit_source_t(void* whose);
    internal unsafe delegate rx_result_struct rx_write_source_t(void* whose, ulong id, int test, ulong identity, typed_value_type val, void* ctx);

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct plugin_source_def_struct
    {
        public nint code_info;//rx_get_code_info_t
        public nint init_source; // rx_init_source_t
        public nint start_source; //rx_start_source_t
        public nint stop_source;//rx_stop_source_t 
        public nint deinit_source;//rx_deinit_source_t
        public nint write_source;//rx_write_source_t
    }

    // Delegate types for host_source_def_struct_t
    internal unsafe delegate rx_result_struct rx_post_job_t(void* whose, int type, plugin_job_struct* job, uint period);
    internal unsafe delegate uint rx_create_timer_t(void* whose, int type, plugin_job_struct* job, uint period);
    internal unsafe delegate void rx_start_timer_t(void* whose, uint timer, uint period);
    internal unsafe delegate void rx_suspend_timer_t(void* whose, uint timer);
    internal unsafe delegate void rx_destroy_timer_t(void* whose, uint timer);

    internal unsafe delegate rx_result_struct rx_update_source_t(void* whose, full_value_type val);
    internal unsafe delegate void rx_result_update_source_t(void* whose, rx_result_struct result, ulong id);
    internal unsafe delegate void rx_source_get_method_model_t(void* whose, bytes_value_struct* data);

    // host_runtime_def_struct
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct host_runtime_def_struct
    {
        public nint post_job;         // rx_post_job_t
        public nint create_timer;     // rx_create_timer_t
        public nint start_timer;      // rx_start_timer_t
        public nint suspend_timer;    // rx_suspend_timer_t
        public nint destroy_timer;    // rx_destroy_timer_t
    }

    // host_source_def_struct
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct host_source_def_struct4
    {
        public host_runtime_def_struct runtime;
        public nint update_source;         // rx_update_source_t
        public nint result_update_source;  // rx_result_update_source_t
        public nint source_data_model; //rx_source_get_method_model_t
    }

    // Supporting struct for full_value_type (from rx_common.h)
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct full_value_type
    {
        public typed_value_type value;
        public rx_time_struct time;
        public uint quality;
        public uint origin;
    }


    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct host_object_def_struct
    {
        // From rx_abi.h:
        // typedef struct host_object_def_struct_t
        // {
        //     rx_post_object_job_t post_job;
        // } host_object_def_struct;
        public rx_post_object_job_t post_job;
    }

    // Add the delegate types if not already present:
    internal unsafe delegate rx_result_struct rx_post_source_job_t(void* whose, int type, plugin_job_struct* job, uint period);
    internal unsafe delegate rx_result_struct rx_post_object_job_t(void* whose, int type, plugin_job_struct* job, uint period);

    // Mapper ABI delegates
    internal unsafe delegate rx_result_struct rx_init_mapper_t(void* whose, void* ctx, byte value_type);
    internal unsafe delegate rx_result_struct rx_start_mapper_t(void* whose, void* ctx);
    internal unsafe delegate rx_result_struct rx_stop_mapper_t(void* whose);
    internal unsafe delegate rx_result_struct rx_deinit_mapper_t(void* whose);
    internal unsafe delegate void rx_mapped_value_changed_t(void* whose, void* val, void* ctx);
    internal unsafe delegate void rx_mapper_result_received_t(void* whose, rx_result_struct result, ulong id, void* ctx);
    internal unsafe delegate void rx_mapper_execute_result_received_t(void* whose, rx_result_struct result, ulong id, typed_value_type out_val, void* ctx);
    internal unsafe delegate void rx_mapper_event_fired_t(void* whose, ulong id, int test, ulong identity, void* data, sbyte* queue, int state_machine, int remove, void* ctx);

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct plugin_mapper_def_struct
    {
        public rx_get_code_info_t code_info;
        public rx_init_mapper_t init_mapper;
        public rx_start_mapper_t start_mapper;
        public rx_stop_mapper_t stop_mapper;
        public rx_deinit_mapper_t deinit_mapper;
        public rx_mapped_value_changed_t mapped_value_changed;
        public rx_mapper_result_received_t mapper_result_received;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct plugin_mapper_def_struct3
    {
        public rx_get_code_info_t code_info;
        public rx_init_mapper_t init_mapper;
        public rx_start_mapper_t start_mapper;
        public rx_stop_mapper_t stop_mapper;
        public rx_deinit_mapper_t deinit_mapper;
        public rx_mapped_value_changed_t mapped_value_changed;
        public rx_mapper_result_received_t mapper_result_received;
        public rx_mapper_execute_result_received_t mapper_execute_result_received;
        public rx_mapper_event_fired_t mapper_event_fired;
    }

    // Filter ABI delegates
    internal unsafe delegate rx_result_struct rx_init_filter_t(void* whose, void* ctx);
    internal unsafe delegate rx_result_struct rx_start_filter_t(void* whose, void* ctx);
    internal unsafe delegate rx_result_struct rx_stop_filter_t(void* whose);
    internal unsafe delegate rx_result_struct rx_deinit_filter_t(void* whose);
    internal unsafe delegate rx_result_struct rx_filter_input_t(void* whose, void* val);
    internal unsafe delegate rx_result_struct rx_filter_output_t(void* whose, typed_value_type* val);

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct plugin_filter_def_struct
    {
        public rx_get_code_info_t code_info;
        public rx_init_filter_t init_filter;
        public rx_start_filter_t start_filter;
        public rx_stop_filter_t stop_filter;
        public rx_deinit_filter_t deinit_filter;
        public rx_filter_input_t filter_input;
        public rx_filter_output_t filter_output;
    }

    // Struct ABI delegates
    internal unsafe delegate rx_result_struct rx_init_struct_t(void* whose, void* ctx);
    internal unsafe delegate rx_result_struct rx_start_struct_t(void* whose, void* ctx);
    internal unsafe delegate rx_result_struct rx_stop_struct_t(void* whose);
    internal unsafe delegate rx_result_struct rx_deinit_struct_t(void* whose);

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct plugin_struct_def_struct
    {
        public rx_get_code_info_t code_info;
        public rx_init_struct_t init_struct;
        public rx_start_struct_t start_struct;
        public rx_stop_struct_t stop_struct;
        public rx_deinit_struct_t deinit_struct;
    }

    // Variable ABI delegates
    internal unsafe delegate rx_result_struct rx_init_variable_t(void* whose, void* ctx);
    internal unsafe delegate rx_result_struct rx_start_variable_t(void* whose, void* ctx);
    internal unsafe delegate rx_result_struct rx_stop_variable_t(void* whose);
    internal unsafe delegate rx_result_struct rx_deinit_variable_t(void* whose);

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct plugin_variable_def_struct
    {
        public rx_get_code_info_t code_info;
        public rx_init_variable_t init_variable;
        public rx_start_variable_t start_variable;
        public rx_stop_variable_t stop_variable;
        public rx_deinit_variable_t deinit_variable;
    }

    // Event ABI delegates
    internal unsafe delegate rx_result_struct rx_init_event_t(void* whose, void* ctx);
    internal unsafe delegate rx_result_struct rx_start_event_t(void* whose, void* ctx);
    internal unsafe delegate rx_result_struct rx_stop_event_t(void* whose);
    internal unsafe delegate rx_result_struct rx_deinit_event_t(void* whose);

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct plugin_event_def_struct
    {
        public rx_get_code_info_t code_info;
        public rx_init_event_t init_event;
        public rx_start_event_t start_event;
        public rx_stop_event_t stop_event;
        public rx_deinit_event_t deinit_event;
    }

    // DataType ABI delegates
    internal unsafe delegate rx_result_struct rx_init_data_type_t(void* whose, void* ctx, bytes_value_struct* data);
    internal unsafe delegate rx_result_struct rx_start_data_type_t(void* whose, void* ctx);
    internal unsafe delegate rx_result_struct rx_stop_data_type_t(void* whose);
    internal unsafe delegate rx_result_struct rx_deinit_data_type_t(void* whose);

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct plugin_data_type_def_struct
    {
        public rx_get_code_info_t code_info;
        public rx_init_data_type_t init_data_type;
        public rx_start_data_type_t start_data_type;
        public rx_stop_data_type_t stop_data_type;
        public rx_deinit_data_type_t deinit_data_type;
    }

    // Method ABI delegates
    internal unsafe delegate rx_result_struct rx_init_method_t(void* whose, void* ctx);
    internal unsafe delegate rx_result_struct rx_start_method_t(void* whose, void* ctx);
    internal unsafe delegate rx_result_struct rx_stop_method_t(void* whose);
    internal unsafe delegate rx_result_struct rx_deinit_method_t(void* whose);
    internal unsafe delegate rx_result_struct rx_execute_method_t(void* whose, ulong id, int test, ulong identity, typed_value_type val, void* ctx);

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct plugin_method_def_struct
    {
        public rx_get_code_info_t code_info;
        public rx_init_method_t init_method;
        public rx_start_method_t start_method;
        public rx_stop_method_t stop_method;
        public rx_deinit_method_t deinit_method;
        public rx_execute_method_t execute_method;
    }

    // Program ABI delegates
    internal unsafe delegate rx_result_struct rx_init_program_t(void* whose, void* ctx);
    internal unsafe delegate rx_result_struct rx_start_program_t(void* whose, void* ctx);
    internal unsafe delegate rx_result_struct rx_stop_program_t(void* whose);
    internal unsafe delegate rx_result_struct rx_deinit_program_t(void* whose);

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct plugin_program_def_struct
    {
        public rx_get_code_info_t code_info;
        public rx_init_program_t init_program;
        public rx_start_program_t start_program;
        public rx_stop_program_t stop_program;
        public rx_deinit_program_t deinit_program;
    }

    // Display ABI delegates
    internal unsafe delegate rx_result_struct rx_init_display_t(void* whose, void* ctx);
    internal unsafe delegate rx_result_struct rx_start_display_t(void* whose, void* ctx);
    internal unsafe delegate rx_result_struct rx_stop_display_t(void* whose);
    internal unsafe delegate rx_result_struct rx_deinit_display_t(void* whose);

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct plugin_display_def_struct
    {
        public rx_get_code_info_t code_info;
        public rx_init_display_t init_display;
        public rx_start_display_t start_display;
        public rx_stop_display_t stop_display;
        public rx_deinit_display_t deinit_display;
    }

    // Object ABI delegates
    internal unsafe delegate rx_result_struct rx_init_object_t(void* whose, void* ctx);
    internal unsafe delegate rx_result_struct rx_start_object_t(void* whose, void* ctx);
    internal unsafe delegate rx_result_struct rx_stop_object_t(void* whose);
    internal unsafe delegate rx_result_struct rx_deinit_object_t(void* whose);

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct plugin_object_def_struct
    {
        public rx_get_code_info_t code_info;
        public rx_init_object_t init_object;
        public rx_start_object_t start_object;
        public rx_stop_object_t stop_object;
        public rx_deinit_object_t deinit_object;
    }

    // Domain ABI delegates
    internal unsafe delegate rx_result_struct rx_init_domain_t(void* whose, void* ctx);
    internal unsafe delegate rx_result_struct rx_start_domain_t(void* whose, void* ctx);
    internal unsafe delegate rx_result_struct rx_stop_domain_t(void* whose);
    internal unsafe delegate rx_result_struct rx_deinit_domain_t(void* whose);

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct plugin_domain_def_struct
    {
        public rx_get_code_info_t code_info;
        public rx_init_domain_t init_domain;
        public rx_start_domain_t start_domain;
        public rx_stop_domain_t stop_domain;
        public rx_deinit_domain_t deinit_domain;
    }

    // Application ABI delegates
    internal unsafe delegate rx_result_struct rx_init_application_t(void* whose, void* ctx);
    internal unsafe delegate rx_result_struct rx_start_application_t(void* whose, void* ctx);
    internal unsafe delegate rx_result_struct rx_stop_application_t(void* whose);
    internal unsafe delegate rx_result_struct rx_deinit_application_t(void* whose);

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct plugin_application_def_struct
    {
        public rx_get_code_info_t code_info;
        public rx_init_application_t init_application;
        public rx_start_application_t start_application;
        public rx_stop_application_t stop_application;
        public rx_deinit_application_t deinit_application;
    }

    // Port ABI delegates
    internal unsafe delegate rx_result_struct rx_init_port_t(void* whose, void* ctx);
    internal unsafe delegate rx_result_struct rx_start_port_t(void* whose, void* ctx);
    internal unsafe delegate rx_result_struct rx_stop_port_t(void* whose);
    internal unsafe delegate rx_result_struct rx_deinit_port_t(void* whose);
    internal unsafe delegate void rx_stack_assembled_t(void* whose);
    internal unsafe delegate void rx_stack_disassembled_t(void* whose);
    internal unsafe delegate void rx_extract_bind_address_t(void* whose, byte* binder_data, ulong binder_data_size, void* local_addr, void* remote_addr);
    internal unsafe delegate void rx_destroy_endpoint_t(void* whose, void* endpoint);
    internal unsafe delegate void* rx_construct_listener_endpoint_t(void* whose, void* local_address, void* remote_address);
    internal unsafe delegate void* rx_construct_initiator_endpoint_t(void* whose);

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct plugin_port_def_struct
    {
        public rx_get_code_info_t code_info;
        public rx_init_port_t init_port;
        public rx_start_port_t start_port;
        public rx_stop_port_t stop_port;
        public rx_deinit_port_t deinit_port;
        public rx_stack_assembled_t stack_assembled;
        public rx_stack_disassembled_t stack_disassembled;
        public rx_extract_bind_address_t extract_bind_address;
        public rx_destroy_endpoint_t destroy_endpoint;
        public rx_construct_listener_endpoint_t construct_listener_endpoint;
        public rx_construct_initiator_endpoint_t construct_initiator_endpoint;
    }

    // Relation ABI delegates
    internal unsafe delegate rx_result_struct rx_init_relation_t(void* whose, void* ctx);
    internal unsafe delegate rx_result_struct rx_start_relation_t(void* whose, void* ctx, int target);
    internal unsafe delegate rx_result_struct rx_stop_relation_t(void* whose, int target);
    internal unsafe delegate rx_result_struct rx_deinit_relation_t(void* whose);
    internal unsafe delegate rx_result_struct rx_make_target_relation_t(void* whose, void** target);
    internal unsafe delegate rx_result_struct rx_relation_connected_t(void* whose, rx_node_id_struct* from, rx_node_id_struct* to);
    internal unsafe delegate rx_result_struct rx_relation_disconnected_t(void* whose, rx_node_id_struct* from, rx_node_id_struct* to);

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct plugin_relation_def_struct
    {
        public rx_get_code_info_t code_info;
        public rx_init_relation_t init_relation;
        public rx_start_relation_t start_relation;
        public rx_stop_relation_t stop_relation;
        public rx_deinit_relation_t deinit_relation;
        public rx_make_target_relation_t make_target_relation;
        public rx_relation_connected_t relation_connected;
        public rx_relation_disconnected_t relation_disconnected;
    }




}