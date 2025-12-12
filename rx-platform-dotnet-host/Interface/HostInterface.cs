using ENSACO.RxPlatform.Hosting.Internal;
using ENSACO.RxPlatform.Model;
using ENSACO.RxPlatform.Hosting.Common;
using ENSACO.RxPlatform.Hosting.Threading;
using System.Runtime.InteropServices;

namespace ENSACO.RxPlatform.Hosting.Interface
{
    // we only get a pointer to this structure from native code, so we don't need to initialize it ourselves
#pragma warning disable CS0649
    internal unsafe struct dotnet_loading_api_t
    {
        internal IntPtr write_log;    // dotnetWriteLog_t

        internal IntPtr build_type;    // dotnetBuildType_t
        internal IntPtr delete_type;   //dotnetDeleteType_t

        internal IntPtr build_runtime;    // dotnetBuildRuntime_t
        internal IntPtr delete_runtime;    // dotnetDeleteRuntime_t

        internal IntPtr result_callback; // dotnetResultCallback_t
        internal IntPtr void_callback; // dotnetResultCallback_t

        internal IntPtr write_value;    // dotnetWriteValue_t

        internal IntPtr execute_done;    // dotnetExecuteDone_t

        internal IntPtr source_write_done;    // dotnetSourceWriteDone_t

        internal IntPtr source_change;    // dotnetSourceChange_t
    };

#pragma warning restore CS0649
    internal unsafe delegate void dotnetWriteLogDelegate(int type
        , [MarshalAs(UnmanagedType.LPStr)] string library
        , [MarshalAs(UnmanagedType.LPStr)] string source
        , ushort level, [MarshalAs(UnmanagedType.LPStr)] string code
        , [MarshalAs(UnmanagedType.LPStr)] string message);


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
        , [MarshalAs(UnmanagedType.LPStr)] string conns
        , [MarshalAs(UnmanagedType.LPStr)] string codeInfo);

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
        rx_item_type itemType
        , UInt64 transId
        , nint instance
        , UIntPtr idx
        , typed_value_type value
        , IntPtr callback /*dotnetRuntimeResultDelegate*/);


    internal unsafe delegate void dotnetExecuteDoneDelegate(
        UInt64 transId
        , nint instance
        , [MarshalAs(UnmanagedType.LPStr)] string value
        , rx_result_struct result);

    internal unsafe delegate void dotnetSourceWriteDoneDelegate(
        UInt64 transId
        , nint instance
        , rx_result_struct result);


    internal unsafe delegate void dotnetSourceChangeDelegate(
        nint instance
        , full_value_type value);

    internal class InitDataAPI
    {
        internal unsafe void Init(ref dotnet_loading_api_t api)
        {
            WriteLog = (dotnetWriteLogDelegate)Marshal.GetDelegateForFunctionPointer(api.write_log, typeof(dotnetWriteLogDelegate));

            BuildType = (dotnetBuildTypeDelegate)Marshal.GetDelegateForFunctionPointer(api.build_type, typeof(dotnetBuildTypeDelegate));

            DeleteType = (dotnetDeleteTypeDelegate)Marshal.GetDelegateForFunctionPointer(api.delete_type, typeof(dotnetDeleteTypeDelegate));

            InternalCreateRuntime = (dotnetCreateRuntimeDelegate)Marshal.GetDelegateForFunctionPointer(api.build_runtime, typeof(dotnetCreateRuntimeDelegate));
            InternalDeleteRuntime = (dotnetDeleteRuntimeDelegate)Marshal.GetDelegateForFunctionPointer(api.delete_runtime, typeof(dotnetDeleteRuntimeDelegate));

            InternalResultCallback = (dotnetResultCallbackDelegate)Marshal.GetDelegateForFunctionPointer(api.result_callback, typeof(dotnetResultCallbackDelegate));
            InternalVoidCallback = (dotnetVoidCallbackDelegate)Marshal.GetDelegateForFunctionPointer(api.void_callback, typeof(dotnetVoidCallbackDelegate));

            WriteValue = (dotnetWriteValueDelegate)Marshal.GetDelegateForFunctionPointer(api.write_value, typeof(dotnetWriteValueDelegate));

            ExecuteDone = (dotnetExecuteDoneDelegate)Marshal.GetDelegateForFunctionPointer(api.execute_done, typeof(dotnetExecuteDoneDelegate));
            SourceWriteDone = (dotnetSourceWriteDoneDelegate)Marshal.GetDelegateForFunctionPointer(api.source_write_done, typeof(dotnetSourceWriteDoneDelegate));
            SourceChange = (dotnetSourceChangeDelegate)Marshal.GetDelegateForFunctionPointer(api.source_change, typeof(dotnetSourceChangeDelegate));

  
        }

        internal dotnetWriteLogDelegate? WriteLog { get; set; }

        internal dotnetWriteValueDelegate? WriteValue { get; set; }

        internal dotnetBuildTypeDelegate? BuildType { get; set; }

        internal dotnetDeleteTypeDelegate? DeleteType { get; set; }

        internal unsafe Task<Exception?> CreateRuntime(rx_item_type type
            , [MarshalAs(UnmanagedType.LPStr)] string module
            , RxNodeId id
            , RxNodeId parent
            , string name
            , string path
            , uint version
            , uint flags
            , IntPtr instance
            , string def)
        {
            if (InternalCreateRuntime == null)
                return Task.FromResult<Exception?>(new Exception("CreateRuntime delegate is not initialized!"));

            rx_node_id_struct nodeid = CommonInterface.CreateNodeIdFromRxNodeId(id);
            rx_node_id_struct parentnodeid = CommonInterface.CreateNodeIdFromRxNodeId(parent);

            var task = HostThreadingSynchronizator.AppendExceptioned();

            InternalCreateRuntime(task.TransId, type, module
                   , &nodeid, &parentnodeid, name, path, version, 0, def
                   , instance, task.CallbackPtr
                   );

            CommonInterface.rx_destory_node_id(&nodeid);
            CommonInterface.rx_destory_node_id(&parentnodeid);

            return task.Task;
        }

        private dotnetCreateRuntimeDelegate? InternalCreateRuntime { get; set; }


        internal unsafe Task<Exception?> DeleteRuntime(rx_item_type type
            , string module
            , RxNodeId id)
        {
            if (InternalDeleteRuntime == null)
                return Task.FromResult<Exception?>(new Exception("CreateRuntime delegate is not initialized!"));


            var task = HostThreadingSynchronizator.AppendExceptioned();

            rx_node_id_struct nodeid = CommonInterface.CreateNodeIdFromRxNodeId(id);


            InternalDeleteRuntime(
                    task.TransId
                    , type
                    , module
                    , &nodeid
                    , task.CallbackPtr
                    );
            CommonInterface.rx_destory_node_id(&nodeid);

            return task.Task;
        }
        private dotnetDeleteRuntimeDelegate? InternalDeleteRuntime { get; set; }


        private dotnetResultCallbackDelegate? InternalResultCallback { get; set; }
        private dotnetVoidCallbackDelegate? InternalVoidCallback { get; set; }

        internal unsafe void ResultCallback(uint transId, Exception? exc = null)
        {
            if (InternalResultCallback != null)
            {
                rx_result_struct result = CommonInterface.CreateResultFromException(exc);
                InternalResultCallback(transId, result);
            }
        }

        internal void VoidCallback(uint transId)
        {
            if (InternalVoidCallback != null)
            {
                InternalVoidCallback(transId);
            }
        }

        internal dotnetExecuteDoneDelegate? ExecuteDone { get; set; } = null;
        internal dotnetSourceWriteDoneDelegate? SourceWriteDone { get; set; } = null;
        internal dotnetSourceChangeDelegate? SourceChange { get; set; } = null;
    }
}