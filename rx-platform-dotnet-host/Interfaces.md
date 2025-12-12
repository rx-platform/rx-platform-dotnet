### C# Callback interfaces

___

These are interfaces for Managed->Native callbacks used by the .NET host.
They are defined in cs file <b>HostInterface.cs</b> in the 
namespace <b>ENSACO.RxPlatform.Hosting.CallbackInterface</b>.


```csharp


internal unsafe struct dotnet_loading_api_t
    {
        internal IntPtr build_type;    // dotnetBuildType_t
        internal IntPtr delete_type;   //dotnetDeleteType_t

        internal IntPtr build_runtime;    // dotnetBuildRuntime_t
        internal IntPtr delete_runtime;    // dotnetDeleteRuntime_t

        internal IntPtr result_callback; // dotnetResultCallback_t
        internal IntPtr void_callback; // dotnetResultCallback_t

        internal IntPtr write_value;    // dotnetWriteValue_t
    };

#pragma warning restore CS0649

    internal unsafe delegate rx_result_struct dotnetBuildTypeDelegate(
        rx_item_type type
        , [MarshalAs(UnmanagedType.LPStr)] string module
        , rx_node_id_struct* id
        , rx_node_id_struct* parent
        , [MarshalAs(UnmanagedType.LPStr)] string name
        , [MarshalAs(UnmanagedType.LPStr)] string path
        , uint version, uint flags
        , [MarshalAs(UnmanagedType.LPStr)] string def
        , int ownType
        , [MarshalAs(UnmanagedType.LPStr)] string conns);

    internal unsafe delegate rx_result_struct dotnetDeleteTypeDelegate(
        rx_item_type type
        , [MarshalAs(UnmanagedType.LPStr)] string module
        , rx_node_id_struct* id);

    internal unsafe delegate void dotnetCreateRuntimeDelegate(
        UInt64 transId
        , rx_item_type type
        , [MarshalAs(UnmanagedType.LPStr)] string module
        , rx_node_id_struct* id
        , rx_node_id_struct* parent
        , [MarshalAs(UnmanagedType.LPStr)] string name
        , [MarshalAs(UnmanagedType.LPStr)] string path
        , uint version, uint flags
        , [MarshalAs(UnmanagedType.LPStr)] string def
        , IntPtr instance
        , IntPtr callback /*dotnetRuntimeResultDelegate*/);

    internal unsafe delegate void dotnetDeleteRuntimeDelegate(
        UInt64 transId
        , rx_item_type type
        , [MarshalAs(UnmanagedType.LPStr)] string module
        , rx_node_id_struct* id
        , IntPtr callback /*dotnetRuntimeResultDelegate*/);

    
    internal unsafe delegate void dotnetRuntimeResultDelegate(UInt64 transId
        , rx_result_struct result);


    internal unsafe delegate void dotnetResultCallbackDelegate(
        UInt32 transId,
        rx_result_struct result);
    internal unsafe delegate void dotnetVoidCallbackDelegate(UInt32 transId);


    internal unsafe delegate void dotnetWriteValueDelegate(
        UInt64 transId
        , nint instance
        , typed_value_type value
        , IntPtr callback /*dotnetRuntimeResultDelegate*/);

```


### C++ Callback interfaces definitions
___
These are the C++ definitions of the above interfaces.
They are defined in the header file <b>dotnet_api.h</b> as Extern "C" functions


```cpp
typedef rx_result_struct(*dotnetBuildType_t)(rx_item_type type, const char* module, const rx_node_id_struct_t* id, const rx_node_id_struct_t* parent
	, const char* name, const char* path, uint32_t version, uint32_t flags, const char* def, int ownType
	, const char* conns);

rx_result_struct dotnetBuildType(rx_item_type type, const char* module, const rx_node_id_struct_t* id, const rx_node_id_struct_t* parent
	, const char* name, const char* path, uint32_t version, uint32_t flags, const char* def, int ownType
	, const char* conns);


typedef rx_result_struct(*dotnetDeleteType_t)(rx_item_type type, const char* module, const rx_node_id_struct_t* id);

rx_result_struct dotnetDeleteType(rx_item_type type, const char* module, const rx_node_id_struct_t* id);


typedef void(*dotnetRuntimeCallback_t)(uint64_t transId, rx_result_struct result);

typedef void(*dotnetResultCallback_t)(uint32_t transId, rx_result_struct result);
typedef void(*dotnetVoidCallback_t)(uint32_t transId);

typedef void(*dotnetBuildRuntime_t)(uint64_t transId, rx_item_type type, const char* module, const rx_node_id_struct_t* id, const rx_node_id_struct_t* parent
	, const char* name, const char* path, uint32_t version, uint32_t flags, const char* def, void* instance, dotnetRuntimeCallback_t callback);


typedef void(*dotnetWriteValue_t)(
	uint32_t transId
	, void* instance
	, typed_value_type value
	, dotnetRuntimeCallback_t callback /*dotnetRuntimeResultDelegate*/);

void dotnetBuildRuntime(uint64_t transId, rx_item_type type, const char* module, const rx_node_id_struct_t* id, const rx_node_id_struct_t* parent
	, const char* name, const char* path, uint32_t version, uint32_t flags, const char* def, void* instance, dotnetRuntimeCallback_t callback);

typedef void(*dotnetDeleteRuntime_t)(uint64_t transId, rx_item_type type, const char* module, const rx_node_id_struct_t* id, dotnetRuntimeCallback_t callback);

void dotnetDeleteRuntime(uint64_t transId, rx_item_type type, const char* module, const rx_node_id_struct_t* id, dotnetRuntimeCallback_t callback);

void dotnetResultCallback(uint32_t transId, rx_result_struct result);
void dotnetVoidCallback(uint32_t transId);


void dotnetWriteValue(
	uint32_t transId
	, void* instance
	, typed_value_type value
	, dotnetRuntimeCallback_t callback/*dotnetRuntimeResultDelegate*/);

typedef struct dotnet_loading_api_t
{
	dotnetBuildType_t build_type;
	dotnetDeleteType_t delete_type;

	dotnetBuildRuntime_t build_runtime;
	dotnetDeleteRuntime_t delete_runtime;

	dotnetResultCallback_t result_callback;
	dotnetVoidCallback_t void_callback;

	dotnetWriteValue_t write_value;

} dotnet_loading_api;

```


## C# Interface functions

___

These are interfaces for Native->Managed functions used by the Native Host.
They are defined in cs file <b>PlatformHostMain.cs</b> in the namespace <b>ENSACO.RxPlatform.Hosting</b>
as static methods with the attribute [UnmanagedCallersOnly] in class <b>PlatformHostMain</b>

```csharp
    
    [UnmanagedCallersOnly()]
    public unsafe static rx_result_struct InitPlatformHosting(IntPtr api6,
        IntPtr apiPtr,
        uint host_stream_version,
        uint* plugin_stream_version,
        IntPtr assemblies, uint count, string_value_struct* version);

    [UnmanagedCallersOnly()]
    public static rx_result_struct StartPlatformHosting();

    [UnmanagedCallersOnly()]
    public unsafe static void BindObject(rx_item_type type, rx_node_id_struct* node_id, IntPtr runtime);
     
    [UnmanagedCallersOnly()]
    public unsafe static void UnbindObject(rx_item_type type, rx_node_id_struct* node_id, IntPtr runtime);

    [UnmanagedCallersOnly()]
    public unsafe static rx_result_struct BuildHostingTypes(char* lib);
        
    [UnmanagedCallersOnly()]
    public unsafe static void DeinitLibrary(char* lib, uint transId);
     
    [UnmanagedCallersOnly()]
    public unsafe static void DeinitPlatformHosting(ulong transId);

``````

### C++ Managed Interface functions definitions
___

These are the C++ definitions of the above interfaces.
These definitions are in the header file <b>dotnet_api.h</b>.
```cpp


typedef rx_result_struct(CORECLR_DELEGATE_CALLTYPE* init_hosting_entry_point_fn)(const dotnet_plugin_platform_api_t* api, dotnet_loading_api_t* apiPtr, uint32_t host_stream_version, uint32_t* dotnet_stream_version, const char** assemblies, uint32_t count, string_value_struct* version);
typedef rx_result_struct(CORECLR_DELEGATE_CALLTYPE* start_hosting_entry_point_fn)();
typedef rx_result_struct(CORECLR_DELEGATE_CALLTYPE* bind_object_entry_point_fn)(rx_item_type type, const rx_node_id_struct* node_id, void* runtime);
typedef rx_result_struct(CORECLR_DELEGATE_CALLTYPE* build_library_hosting_entry_point_fn)(const char* lib);
typedef void (CORECLR_DELEGATE_CALLTYPE* deinit_library_hosting_entry_point_fn)(const char* lib, uint32_t transId);
typedef void (CORECLR_DELEGATE_CALLTYPE* deinit_hosting_entry_point_fn)(uint32_t transId);

```
Pointer to these functions are obtained using <b>coreclr_get_delegate</b> function and are placed in the file <b>dotnet_api.h</b> header file.
These pointers are placed inside the class <b>dotnet_host</b> which is defined in <b>rx_dotnet_host.h</b>.
