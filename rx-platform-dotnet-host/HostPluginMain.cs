using ENSACO.RxPlatform.Attributes;
using ENSACO.RxPlatform.Host;
using ENSACO.RxPlatform.Hosting.Internal;
using ENSACO.RxPlatform.Hosting.Model;
using ENSACO.RxPlatform.Hosting.Runtime;
using System.Reflection;

namespace ENSACO.RxPlatform.Hosting
{
    internal class HostedPlatformLibrary
    {
        string path = "";
        internal string GetPath()
        {
            return path;
        }
        PlatformLibraryInfo? pluginInfo = null;
        internal Assembly? GetAssembly()
        {
            return assembly;
        }
        Assembly? assembly = null;
        MethodInfo? deinitializeMethod = null;
        MethodInfo? startMethod = null;
        RxAssemblyLoadContext? loadContext = null;
        internal RxAssemblyLoadContext? GetLoadContext()
        {
            return loadContext;
        }
        internal void Unload()
        {
            if(loadContext!=null)
            {
                assembly = null;
                deinitializeMethod = null;
                loadContext.Unload();
                GC.Collect();
                GC.WaitForPendingFinalizers();
                loadContext = null;
            }
        }


        string? assemblyName = null;
        static uint pluginVersion = 0x00001;
        byte[] assemblyData = Array.Empty<byte>();

        internal byte[] GetAssemblyData()
        {
            return assemblyData;
        }
        internal string? GetAssemblyName()
        {
            return assemblyName;
        }
        internal bool InitializeAssembly(string pt)
        {

            RxAssemblyLoadContext context = new RxAssemblyLoadContext(pt);

            byte[] buffer = System.IO.File.ReadAllBytes(pt); // Pre-load to avoid file lock issues

            MemoryStream stream = new MemoryStream(buffer);

            var asm = context.LoadFromStream(stream);

            Assembly? temp = null;
            PlatformLibraryInfo? tempInfo = null;
            var types = asm.GetExportedTypes();
            foreach (var type in types)
            {
                var attrs = type.GetCustomAttributes(typeof(RxPlatformLibrary), false);
                if (attrs.Length > 0)
                {
                    var verAttr = type.GetCustomAttribute<RxPlatformLibraryVersion>();
                    if (verAttr != null)
                    {
                        pluginVersion = verAttr.Version;
                    }
                    else
                    {
                        var assVersion = asm.GetName().Version;
                        if (assVersion != null)
                            pluginVersion = (uint)(((ushort)assVersion.Major) << 16) | ((ushort)assVersion.Minor);
                    }
                    var initializeMethod = type.GetMethod("Initialize", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                    startMethod = type.GetMethod("Start", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                    deinitializeMethod = type.GetMethod("Deinitialize", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                    if (initializeMethod != null && deinitializeMethod != null)
                    {
                        object? retVal = initializeMethod.Invoke(null, null);
                        if (retVal != null && retVal is PlatformLibraryInfo)
                        {
                            if (temp != null)
                            {
                                RxPlatformObject.Instance.WriteLogError("HostPluginMain.InitializeAssembly", 100, $"Multiple RxPlatformLibrary attributes found in assembly {Path.GetFileName(asm.GetName().Name)}. Library will not be initialized!");
                                return false;
                            }
                            temp = asm;
                            tempInfo = (PlatformLibraryInfo)retVal;
                            if (string.IsNullOrEmpty(tempInfo.Information))
                                tempInfo.Information = $"{asm.GetName().Name} Ver {asm.GetName().Version}";
                            if (string.IsNullOrEmpty(tempInfo.Name))
                                tempInfo.Name = $"{asm.GetName().Name}".Replace('.', '_');

                        }
                    }
                    else
                    {
                        tempInfo = new PlatformLibraryInfo()
                        {
                            Name = $"{asm.GetName().Name}".Replace('.', '_'),
                            Information = $"{asm.GetName().Name} Ver {asm.GetName().Version}",
                            DefaultDirectory = ""
                        };
                    }
                }
            }
            if (temp == null || tempInfo == null)
            {
                return false;
            }

            assembly = temp;
            pluginInfo = tempInfo;
            path = pt;
            loadContext = context;
            assemblyData = buffer;
            assemblyName = assembly.GetName().Name + ".DynamicTypes" + RxMemoryCompiler.overridePostfix;

            return true;
        }
        internal string GetPluginName()
        {
            if (pluginInfo == null)
                return "";
            else
                return pluginInfo.Name;
        }
        internal string GetPluginInfo()
        {
            if (pluginInfo == null)
                return "Unknown Plugin";
            else
                return pluginInfo.Name + ";" + pluginInfo.Information;
        }
        internal void Deinitialize()
        {
            if(deinitializeMethod != null)
            {
                deinitializeMethod.Invoke(null, null);
            }
        }
        LibraryPlatformTypes myTypes = new LibraryPlatformTypes();
        internal void BuildPlatformTypes()
        {
            if (pluginInfo != null && assembly != null)
            {
                myTypes = RxMetaExtracter.ParseAssembly(assembly, this);
            }
            else
            {
                throw new Exception("Library not initialized!");
            }
        }
        /////////////////////////////////////
        // utilities functions
        /////////////////////////////////////

        internal void GetPathFromType(Type type, RxPlatformTypeAttribute attribute, out string directory, out string name)
        {
            directory = "";
            name = "";
            directory = attribute.Directory;
            name = attribute.Name;
        }

        internal void DeletePlatformTypes()
        {
            if (pluginInfo != null && assembly != null)
            {
                RxMetaDeleter.DeleteAssemblyTypes(myTypes, this, assembly);
            }
            else
            {
                throw new Exception("Library not initialized!");
            }
        }

        
        internal async void StartHosting()
        {
            if (startMethod != null)
            {
                try
                {
                    var awaitable = startMethod.Invoke(null, null);
                    if (awaitable is System.Threading.Tasks.Task task)
                        await task;
                }
                catch(TargetInvocationException ex)
                {
                    if(ex.InnerException!=null)
                        RxPlatformObject.Instance.WriteLogError("HostedPlatformLibrary.StartHosting", 101, $"Exception in Start method: {ex.InnerException.Message}");
                    else
                        RxPlatformObject.Instance.WriteLogError("HostedPlatformLibrary.StartHosting", 101, $"Exception in Start method: {ex.Message}");
                }
                catch (AggregateException ex)
                {
                    if(ex.InnerException!=null)
                        RxPlatformObject.Instance.WriteLogError("HostedPlatformLibrary.StartHosting", 101, $"Exception in Start method: {ex.InnerException.Message}");
                    else
                        RxPlatformObject.Instance.WriteLogError("HostedPlatformLibrary.StartHosting", 101, $"Exception in Start method: {ex.Message}");
                }
                catch (Exception ex)
                {
                    RxPlatformObject.Instance.WriteLogError("HostedPlatformLibrary.StartHosting", 101, $"Exception in Start method: {ex.Message}");
                }
            }
            else
            {
                RxPlatformObject.Instance.WriteLogError("HostedPlatformLibrary.StartHosting", 101, "Library does not have a Start method!");
            }
        }

        internal async Task DeletePlatformObjects()
        {
            if (pluginInfo != null)
            {
                if (deinitializeMethod != null)
                    deinitializeMethod.Invoke(null, null);

                await RxRuntimeRegistrator.DeleteRuntimes(this);
            }
            else
            {
                throw new Exception("Library not initialized!");
            }
        }

    }
}