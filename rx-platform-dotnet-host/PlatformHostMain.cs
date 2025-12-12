using ENSACO.RxPlatform.Hosting.Common;
using ENSACO.RxPlatform.Hosting.Interface;
using ENSACO.RxPlatform.Hosting.Internal;
using ENSACO.RxPlatform.Hosting.Runtime;
using ENSACO.RxPlatform.Hosting.Threading;
using ENSACO.RxPlatform.Model;
using ENSACO.RxPlatform.Runtime;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Loader;
using System.Text.Json;

namespace ENSACO.RxPlatform.Hosting
{
    internal class RxAssemblyLoadContext : AssemblyLoadContext
    {
        private AssemblyDependencyResolver _resolver;

        internal RxAssemblyLoadContext(string pluginPath) : base(isCollectible: true)
        {
            _resolver = new AssemblyDependencyResolver(pluginPath);
        }

        protected override Assembly? Load(AssemblyName assemblyName)
        {
            string? assemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);
            if (assemblyPath != null)
            {
                byte[] buffer = System.IO.File.ReadAllBytes(assemblyPath);

                RxPlatformObject.Instance.WriteLogDebug("RxAssemblyLoadContext.Load", 100, $"Loading assembly from path: {assemblyPath}");

                MemoryStream stream = new MemoryStream(buffer);

                return LoadFromStream(stream);
            }

            return null;
        }
    }
    public static class PlatformHostMain
    {

        internal static JsonSerializerOptions JsonContext = new JsonSerializerOptions
        {
        };
        static Assembly? CurrentDomain_AssemblyResolve(object? sender, ResolveEventArgs args)
        {
            // This callback is executed when an assembly cannot be resolved
            Console.WriteLine($"Attempting to resolve assembly: {args.Name}");

            // Example: Custom logic to find and load the assembly
            // In a real scenario, you might load from a specific path, a database, etc.
            if (args.Name.StartsWith("NonExistentAssembly"))
            {
                Console.WriteLine("Custom logic: Attempting to provide a dummy assembly for demonstration.");
                // In a real application, you might load a specific DLL here
                // For this example, we'll return null to let the original error propagate
                return null;
            }

            return null; // Return null if you cannot resolve the assembly
        }

        static public RxPlatformObjectRuntime? CreateObject(RxNodeId nodeId)
        {
            //foreach (var plugin in plugins)
            //{
            //    var runtimeType = plugin.CreateRuntime(nodeId);
            //    if (runtimeType != null)
            //    {
            //        return runtimeType;
            //    }
            //}
            return null;
        }

        public const string hostName = ".NET Core Hosting";
        static List<HostedPlatformLibrary> plugins = new List<HostedPlatformLibrary>();
        [UnmanagedCallersOnly()]
        public unsafe static rx_result_struct InitPlatformHosting(IntPtr apiPtr,
            uint host_stream_version,
            uint* plugin_stream_version,
            IntPtr assemblies, uint count, string_value_struct* version)
        {

            System.Diagnostics.Debugger.Launch();

            dotnet_loading_api_t napi = Marshal.PtrToStructure<dotnet_loading_api_t>(apiPtr);
            ENSACO.RxPlatform.Host.PlatformLibraryInfo info = new ENSACO.RxPlatform.Host.PlatformLibraryInfo
            {
                Name = hostName,
                Information = $".NET Core Hosting Version {Assembly.GetExecutingAssembly().GetName().Version}",
                DefaultDirectory = "dotnet"
            };

            api.Init(ref napi);


            RxPlatformObjectRuntime.__InitRuntime(
                new RxRuntimeDelegates
                {
                    RegisterRuntimes = RxRuntimeRegistrator.RegisterRuntime,
                    UnregisterRuntimes = RxRuntimeRegistrator.UnregisterRuntime,
                    CreateRuntimes = RxRuntimeRegistrator.CreateRuntime,

                    WriteBoolRuntime = RxRuntimeExecuter.WriteProperty<bool>,
                    WriteInt8Runtime = RxRuntimeExecuter.WriteProperty<sbyte>,
                    WriteInt16Runtime = RxRuntimeExecuter.WriteProperty<short>,
                    WriteInt32Runtime = RxRuntimeExecuter.WriteProperty<int>,
                    WriteInt64Runtime = RxRuntimeExecuter.WriteProperty<long>,
                    WriteUInt8Runtime = RxRuntimeExecuter.WriteProperty<byte>,
                    WriteUInt16Runtime = RxRuntimeExecuter.WriteProperty<ushort>,
                    WriteUInt32Runtime = RxRuntimeExecuter.WriteProperty<uint>,
                    WriteUInt64Runtime = RxRuntimeExecuter.WriteProperty<ulong>,
                    WriteFloatRuntime = RxRuntimeExecuter.WriteProperty<float>,
                    WriteDoubleRuntime = RxRuntimeExecuter.WriteProperty<double>,
                    WriteStringRuntime = RxRuntimeExecuter.WriteProperty<string>,
                    WriteUuidRuntime = RxRuntimeExecuter.WriteProperty<Guid>,
                    WriteDateTimeRuntime = RxRuntimeExecuter.WriteProperty<DateTime>,
                    WriteBytesRuntime = RxRuntimeExecuter.WriteProperty<byte[]>,
                    WriteObjectRuntime = RxRuntimeExecuter.WriteObjectProperty,

                    SourceChangedBool = RxRuntimeExecuter.SourceChanged<bool>,
                    SourceChangedInt8 = RxRuntimeExecuter.SourceChanged<sbyte>,
                    SourceChangedInt16 = RxRuntimeExecuter.SourceChanged<short>,
                    SourceChangedInt32 = RxRuntimeExecuter.SourceChanged<int>,
                    SourceChangedInt64 = RxRuntimeExecuter.SourceChanged<long>,
                    SourceChangedUInt8 = RxRuntimeExecuter.SourceChanged<byte>,
                    SourceChangedUInt16 = RxRuntimeExecuter.SourceChanged<ushort>,
                    SourceChangedUInt32 = RxRuntimeExecuter.SourceChanged<uint>,
                    SourceChangedUInt64 = RxRuntimeExecuter.SourceChanged<ulong>,
                    SourceChangedFloat = RxRuntimeExecuter.SourceChanged<float>,
                    SourceChangedDouble = RxRuntimeExecuter.SourceChanged<double>,
                    SourceChangedString = RxRuntimeExecuter.SourceChanged<string>,
                    SourceChangedUuid = RxRuntimeExecuter.SourceChanged<Guid>,
                    SourceChangedDateTime = RxRuntimeExecuter.SourceChanged<DateTime>,
                    SourceChangedBytes = RxRuntimeExecuter.SourceChanged<byte[]>,
                    SourceChangedObject = RxRuntimeExecuter.SourceChangedObject,
                    SourceChangedBad = RxRuntimeExecuter.SourceChangedBad,

                    GetInstance = RxRuntimeRegistrator.GetInstance
                }
                , PlatformHostMain.JsonContext);


            RxPlatformObject.Instance.WriteLogInfo("PlatformHostMain.InitPlatformHosting", 200, $".NET Core host initialized, version {Assembly.GetExecutingAssembly().GetName().Version}.");


            ProperyValuesSynhronizator.Start();


            * plugin_stream_version = 0x20008; // Version 2.0.8
            rx_result_struct result = new rx_result_struct();

            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

            try
            {
                IntPtr* assemblyArray = (IntPtr*)assemblies;
                for (uint i = 0; i < count; i++)
                {
                    string? path = Marshal.PtrToStringUTF8(assemblyArray[i]);
                    if (path != null)
                    {
                        RxPlatformObject.Instance.WriteLogTrace("PlatformHostMain.InitPlatformHosting", 100, $"Inspecting assembly {path}");

                        HostedPlatformLibrary plugin = new HostedPlatformLibrary();
                        if (plugin.InitializeAssembly(path))
                        {
                            plugins.Add(plugin);
                            RxPlatformObject.Instance.WriteLogInfo("PlatformHostMain.InitPlatformHosting", 0, $"Loaded .NET library: {plugin.GetPluginName()} [{plugin.GetPluginInfo()}].");
                        }
                    }
                }
                CommonInterface.rx_init_string_value_struct(version, $"{hostName} Ver {Assembly.GetExecutingAssembly().GetName().Version}", -1);
            }
            catch (Exception ex)
            {
                result = CommonInterface.CreateResultFromException(ex);
            }
            return result;
        }
        [UnmanagedCallersOnly()]
        public static rx_result_struct StartPlatformHosting()
        {
            rx_result_struct result = new rx_result_struct();

            foreach (var plugin in plugins)
            {
                Task.Run(() =>
                {
                    try
                    {
                        RxPlatformObject.Instance.WriteLogTrace("PlatformHostMain.StartPlatformHosting", 100, $".NET Core starting library {plugin.GetPluginName()}");
                        plugin.StartHosting();
                        RxPlatformObject.Instance.WriteLogTrace("PlatformHostMain.StartPlatformHosting", 100, $".NET Core started library {plugin.GetPluginName()}");

                    }
                    catch (TargetInvocationException ex)
                    {
                        if (ex.InnerException != null)
                            RxPlatformObject.Instance.WriteLogWarining("PlatformHostMain.StartPlatformHosting", 101, $".NET Core failed to start library  {ex.InnerException.Message}");
                        else
                            RxPlatformObject.Instance.WriteLogWarining("PlatformHostMain.StartPlatformHosting", 101, $".NET Core failed to start library  {ex.Message}");
                    }
                    catch (AggregateException ex)
                    {
                        if (ex.InnerException != null)
                            RxPlatformObject.Instance.WriteLogWarining("PlatformHostMain.StartPlatformHosting", 101, $".NET Core failed to start library  {ex.InnerException.Message}");
                        else
                            RxPlatformObject.Instance.WriteLogWarining("PlatformHostMain.StartPlatformHosting", 101, $".NET Core failed to start library  {ex.Message}");
                    }
                    catch (Exception ex)
                    {
                        RxPlatformObject.Instance.WriteLogWarining("PlatformHostMain.StartPlatformHosting", 200, $".NET Core failed to start library {plugin.GetPluginName()}:{ex.Message}");
                    }
                });
            }
            GC.Collect();
            return result;
        }
        internal static InitDataAPI api = new InitDataAPI();




        [UnmanagedCallersOnly()]
        public unsafe static void BindObject(rx_item_type type, rx_node_id_struct* node_id, char* path, rx_node_id_struct* parent_id, IntPtr runtime)
        {
            string subPath = Marshal.PtrToStringUTF8((IntPtr)path) ?? "";
            var nodeId = CommonInterface.CreateRxNodeIdFromNodeId(*node_id);
            var parentId = CommonInterface.CreateRxNodeIdFromNodeId(*parent_id);

            if ((type == rx_item_type.rx_object || type == rx_item_type.rx_struct_type) && nodeId.IsNull() || parentId.IsNull())
            {
                return;
            }
            try
            {
                RxPlatformObject.Instance.WriteLogDebug("PlatformHostMain.BindObject", 100, $".NET Core binding object {nodeId.ToString()}");
                    
                RxRuntimeRegistrator.BindObject(type, nodeId, subPath, parentId, runtime);

                RxPlatformObject.Instance.WriteLogDebug("PlatformHostMain.BindObject", 100, $".NET Core bound object {nodeId.ToString()}");
            }
            catch (Exception ex)
            {
                RxPlatformObject.Instance.WriteLogWarining("PlatformHostMain.BindObject", 200, $".NET Core failed to bind object {nodeId.ToString()}:{ex.Message}");
            }
        }
        [UnmanagedCallersOnly()]
        public unsafe static void UnbindObject(rx_item_type type, rx_node_id_struct* node_id, char* path, rx_node_id_struct* parent_id, IntPtr runtime)
        {
            string subPath = Marshal.PtrToStringUTF8((IntPtr)path) ?? "";
            var nodeId = CommonInterface.CreateRxNodeIdFromNodeId(*node_id);
            var parentId = CommonInterface.CreateRxNodeIdFromNodeId(*parent_id);

            if ((type == rx_item_type.rx_object || type == rx_item_type.rx_struct_type) && nodeId.IsNull())
            {
                return;
            }
            try
            {
                RxRuntimeRegistrator.UnbindObject(type, nodeId, subPath, parentId, runtime);
                RxPlatformObject.Instance.WriteLogDebug("PlatformHostMain.UnbindObject", 100, $".NET Core unbound object {nodeId.ToString()}");
            }
            catch (Exception ex)
            {
                RxPlatformObject.Instance.WriteLogWarining("PlatformHostMain.UnbindObject", 200, $".NET Core failed to unbind object {nodeId.ToString()}:{ex.Message}");
            }
        }

        [UnmanagedCallersOnly()]
        public unsafe static rx_result_struct BuildHostingTypes(char* lib)
        {
            rx_result_struct result = new rx_result_struct();
            string library = Marshal.PtrToStringUTF8((IntPtr)lib) ?? "";
            bool all = string.IsNullOrEmpty(library);

            try
            {
                //build source types first
                foreach (var plugin in plugins)
                {
                    if (all)
                    {
                        plugin.BuildPlatformTypes();
                    }
                    else
                    {
                        plugin.InitializeAssembly(plugin.GetPath());
                        plugin.BuildPlatformTypes();
                        if (!all)
                        {
                            Task.Run(() =>
                            {
                                try
                                {
                                    RxPlatformObject.Instance.WriteLogTrace("PlatformHostMain.StartPlatformHosting", 100, $".NET Core starting library {plugin.GetPluginName()}");
                                    plugin.StartHosting();
                                    RxPlatformObject.Instance.WriteLogTrace("PlatformHostMain.StartPlatformHosting", 100, $".NET Core started library {plugin.GetPluginName()}");

                                }
                                catch (Exception ex)
                                {
                                    RxPlatformObject.Instance.WriteLogWarining("PlatformHostMain.StartPlatformHosting", 200, $".NET Core failed to start library {plugin.GetPluginName()}:{ex.Message}");
                                }
                            });
                        }
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                result = CommonInterface.CreateResultFromException(ex);
            }

            return result;
        }

        static async Task DeinitObjects(HostedPlatformLibrary targetPlugin, uint transId)
        {
            try
            {
                await targetPlugin.DeletePlatformObjects();
            }
            catch (Exception)
            {
                throw;
            }
        }
        private async static void DeinitLibraryInternal(HostedPlatformLibrary targetPlugin, uint transId)
        {
            try
            {
                await DeinitObjects(targetPlugin, transId);
                targetPlugin.Unload();

                RxPlatformObject.Instance.WriteLogInfo("PlatformHostMain.DeinitLibrary", 0
                    , $"Types removed for library {targetPlugin.GetPluginName()} [{targetPlugin.GetPluginInfo()}].");

                api.ResultCallback(transId);
            }
            catch (TargetInvocationException ex)
            {
                api.ResultCallback(transId, ex.InnerException);
            }
            catch (AggregateException ex)
            {
                api.ResultCallback(transId, ex.InnerException);
            }
            catch (Exception ex)
            {
                api.ResultCallback(transId, ex);
            }
        }
        [UnmanagedCallersOnly()]
        public unsafe static void DeinitLibrary(char* lib, uint transId)
        {
            string library = Marshal.PtrToStringUTF8((IntPtr)lib) ?? "";

            HostedPlatformLibrary? targetPlugin = null;

            foreach (var plugin in plugins)
            {

                if (plugin.GetPluginName() == library)
                {
                    targetPlugin = plugin;
                    break;
                }
            }
            if (targetPlugin != null)
            {
                if (targetPlugin.GetAssembly() == null)
                {
                    api.ResultCallback(transId,
                        new Exception($"Library {library} not loaded."));
                }
                else
                {
                    try
                    {
                        targetPlugin.DeletePlatformTypes();

                        Task.Run(() =>
                        {
                            DeinitLibraryInternal(targetPlugin, transId);
                        });

                    }
                    catch (Exception ex)
                    {
                        api.ResultCallback(transId, ex);
                    }
                }
            }
            else
            {
                api.ResultCallback(transId, new Exception($"Library {library} not found."));
            }
        }
        [UnmanagedCallersOnly()]
        public unsafe static void DeinitPlatformHosting(ulong transId)
        {
            ProperyValuesSynhronizator.Stop();

        }
        [UnmanagedCallersOnly()]
        public unsafe static void RuntimeValueChanged(UIntPtr idx, typed_value_type value, rx_item_type type, IntPtr whose)
        {
            RxRuntimeExecuter.RuntimeValueChanged(type, idx, value, whose);
        }
        [UnmanagedCallersOnly()]
        public unsafe static void InitialRuntimeValues(UIntPtr count, char** names, typed_value_type* value, rx_item_type type, IntPtr whose)
        {
            RxRuntimeExecuter.InitialRuntimeValues(type, count, names, value, whose);
        }

        [UnmanagedCallersOnly()]
        public unsafe static rx_result_struct ExecuteMethod(uint transId, nint whose, char* method, char* value)
        {
            try
            {
                string methodStr = Marshal.PtrToStringUTF8((IntPtr)method) ?? "";
                string valueStr = Marshal.PtrToStringUTF8((IntPtr)value) ?? "";
                RxRuntimeExecuter.ExecuteMethod(transId, whose, methodStr, valueStr);

                rx_result_struct result;
                CommonInterface.rx_init_result_struct(&result);
                return result;
            }
            catch(Exception ex)
            {
                return CommonInterface.CreateResultFromException(ex);
            }
        }
        [UnmanagedCallersOnly()]
        public unsafe static void SourceWrite(uint transId, typed_value_type value, nint whose)
        {
            RxRuntimeExecuter.SourceWrite(transId, value, whose);
        }
        [UnmanagedCallersOnly()]
        internal unsafe static void MappedChanged(full_value_type value, nint whose)
        {
            RxRuntimeExecuter.MappedChange(value, whose);
        }
    }
}
