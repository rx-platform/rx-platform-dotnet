using ENSACO.RxPlatform.Attributes;
using ENSACO.RxPlatform.Hosting.Internal;
using ENSACO.RxPlatform.Hosting.Model;
using ENSACO.RxPlatform.Hosting.Model.Algorithms;
using ENSACO.RxPlatform.Model;
using ENSACO.RxPlatform.Model.System;
using ENSACO.RxPlatform.Runtime;
using RxPlatform.Hosting.Interface;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ENSACO.RxPlatform.Hosting.Runtime
{
    internal static class RxRuntimeRegistrator
    {
        static async Task<bool> RxPlatformCreateRuntime(rx_item_type rxType, RxNodeId id, RxNodeId parentId, object prototype, string name, string path, string def)
        {
            RxPlatformObject.Instance.WriteLogDebug("PlatformRuntimeTypes.InternalLibraryCreateRuntime", 100
                , $"Creating Runtime {prototype.GetType().FullName} with name {name}, path {path}.");

            if (PlatformHostMain.api.CreateRuntime != null)
            {
                Exception? result = await PlatformHostMain.api.CreateRuntime(rxType, "", id, parentId, name, path, 0x10000, 0, 0, def);
                if (result == null)
                {
                    RxPlatformObject.Instance.WriteLogTrace("PlatformRuntimeTypes.InternalLibraryCreateRuntime", 110
                        , $"Created Runtime {prototype.GetType().FullName} with name {name}, path {path}.");
                    
                    return true;
                }
                else
                {
                    RxPlatformObject.Instance.WriteLogError("PlatformRuntimeTypes.InternalLibraryCreateRuntime", 120
                        , $"Failed to create Runtime {prototype.GetType().FullName} with name {name}, path {path}:{result.Message}");
                }
            }
            return false;
        }
        static async Task<bool> CreateRuntime<T>(T attr, rx_item_type rxType, object prototype
            , string name, string path, RxNodeId id
            , HostedPlatformLibrary lib, Dictionary<RxNodeId, PlatformTypeData<T>> dict)
            where T : RxPlatformTypeAttribute
        {
            RxNodeId parentId = RxNodeId.NullId;

            PlatformTypeData<T>? typeData = null;
            lock (RxMetaData.Instance.TypesLock)
            {
                if (dict.TryGetValue(attr.NodeId, out typeData))
                {
                    parentId = typeData.Meta.id;
                }
            }
            if (lib == null)
            {
                RxPlatformObject.Instance.WriteLogError("PlatformRuntimeTypes.CreateRuntime", 10
                    , $"Failed to create Runtime for object {prototype.GetType().FullName} because calling assembly is not registered.");
                return false;
            }
            if (typeData == null)
            {
                if (RxMetaExtracter.ParseType(prototype.GetType()))
                {
                    lock (RxMetaData.Instance.TypesLock)
                    {
                        if (dict.TryGetValue(attr.NodeId, out typeData))
                        {
                            parentId = typeData.Meta.id;
                        }
                    }
                }
                if (typeData == null)
                {
                    RxPlatformObject.Instance.WriteLogError("PlatformRuntimeTypes.CreateRuntime", 10
                    , $"Failed to create Runtime for object {prototype.GetType().FullName} because its type is not registered.");
                    return false;
                }
            }
            
            if (!parentId.IsNull() && lib != null)
            {
                StringBuilder stream = new StringBuilder();

                if (!RxRuntimeDefinitionCreater.GetInstanceData(attr, prototype, stream))
                {
                    return false;
                }
                stream.AppendLine(RxRuntimeDefinitionCreater.CreateOverrides(prototype, lib));
                stream.Append($@"
}}
}}");
                if (await RxPlatformCreateRuntime(rxType, id, parentId, prototype, name, path, stream.ToString()))
                {
                    lock (RxMetaData.Instance.TypesLock)
                    {
                        if (!RxMetaData.Instance.RuntimeObjects.ContainsKey(lib))
                        {
                            RxMetaData.Instance.RuntimeObjects.Add(lib, new HashSet<PlatformInstanceData>());
                        }
                        var instData = new PlatformInstanceData()
                        {
                            id = id,
                            path = $"{path}/{name}",
                            rxType = rxType
                        };
                        RxMetaData.Instance.RuntimeObjects[lib].Add(instData);
                        RxMetaData.Instance.RegisteredObjects.Add(prototype, instData);
                    }
                    return true;
                }
            }
            return false;
        }
        // object is special case because of runtime objects
        static async Task<bool> CreateRuntime(RxPlatformObjectType attr, rx_item_type rxType, object prototype
            , string name, string path, RxNodeId id, HostedPlatformLibrary lib)
        {
            RxNodeId parentId = RxNodeId.NullId;

            PlatformTypeData<RxPlatformObjectType>? typeData = null;
            lock (RxMetaData.Instance.TypesLock)
            {
                if (RxMetaData.Instance.ObjectTypes.TryGetValue(attr.NodeId, out typeData))
                {
                    parentId = typeData.Meta.id;
                }
            }
            if (typeData == null)
            {
                if (RxMetaExtracter.ParseType(prototype.GetType()))
                {
                    lock (RxMetaData.Instance.TypesLock)
                    {
                        if (RxMetaData.Instance.ObjectTypes.TryGetValue(attr.NodeId, out typeData))
                        {
                            parentId = typeData.Meta.id;
                        }
                    }
                }
                if (typeData == null)
                {
                    RxPlatformObject.Instance.WriteLogError("PlatformRuntimeTypes.CreateRuntime", 10
                    , $"Failed to create Runtime for object {prototype.GetType().FullName} because its type is not registered.");
                    return false;
                }
            }

            if (!parentId.IsNull() && lib != null)
            {
                StringBuilder stream = new StringBuilder();

                if (!RxRuntimeDefinitionCreater.GetInstanceData(attr, prototype, stream))
                {
                    return false;
                }
                stream.AppendLine(JsonSerializer.Serialize(prototype, prototype.GetType()));
                stream.Append($@"
}}
}}");
                if (await RxPlatformCreateRuntime(rxType, id, parentId, prototype, name, path, stream.ToString()))
                {
                    lock (RxMetaData.Instance.TypesLock)
                    {
                        if (!RxMetaData.Instance.RuntimeObjects.ContainsKey(lib))
                        {
                            RxMetaData.Instance.RuntimeObjects.Add(lib, new HashSet<PlatformInstanceData>());
                        }
                        var instData = new PlatformInstanceData()
                        {
                            id = id,
                            path = $"{path}/{name}",
                            rxType = rxType
                        };
                        RxMetaData.Instance.RuntimeObjects[lib].Add(instData);
                        RxMetaData.Instance.RegisteredObjects.Add(prototype, instData);
                    }
                    return true;
                }
            }
            return false;
        }
        // functions called by the managed side to register/unregister/create/delete runtimes
        internal static async Task<bool> CreateRuntime(object prototype, string name, string path
            , RxNodeId id, Assembly assembly)
        {
            HostedPlatformLibrary? lib = null;
            lock (RxMetaData.Instance.TypesLock)
            {
                if (!RxMetaData.Instance.HostedLibraries.TryGetValue(assembly, out lib))
                {
                    RxPlatformObject.Instance.WriteLogError("PlatformRuntimeTypes.CreateRuntime", 10
                        , $"Failed to create Runtime for object {prototype.GetType().FullName} because calling assembly {assembly.FullName} is not registered.");
                    return false;
                }
            }
            if (lib == null)
            {
                RxPlatformObject.Instance.WriteLogError("PlatformRuntimeTypes.CreateRuntime", 10
                    , $"Failed to create Runtime for object {prototype.GetType().FullName} because calling assembly {assembly.FullName} is not registered.");
                return false;
            }
            if (!path.StartsWith('/'))
            {
                path = $"/sys/{RxPlatformObject.Instance.GetPluginName()}/{lib.GetPluginName()}{(string.IsNullOrEmpty(path) ? "" : "/")}{path}";
                
            }
            if (id.IsNull())
            {
                id = new RxNodeId(Guid.NewGuid(), 999);
            }
            
            Type type = prototype.GetType();
            var attrObj = type.GetCustomAttribute<RxPlatformObjectType>();
            if (attrObj != null)
            {
                return await CreateRuntime(attrObj, rx_item_type.rx_object, prototype, name, path, id, lib);
            }
            var portAttr = type.GetCustomAttribute<RxPlatformPortType>();
            if (portAttr != null)
            {
                return await CreateRuntime<RxPlatformPortType>(portAttr, rx_item_type.rx_port, prototype, name, path, id, lib
                    , RxMetaData.Instance.PortTypes);

            }
            var appAttr = type.GetCustomAttribute<RxPlatformApplicationType>();
            if (appAttr != null)
            {
                return await CreateRuntime<RxPlatformApplicationType>(appAttr, rx_item_type.rx_application, prototype, name, path, id, lib
                    , RxMetaData.Instance.ApplicationTypes);
            }
            var domainAttr = type.GetCustomAttribute<RxPlatformDomainType>();
            if (domainAttr != null)
            {
                return await CreateRuntime<RxPlatformDomainType>(domainAttr, rx_item_type.rx_domain, prototype, name, path, id, lib
                    , RxMetaData.Instance.DomainTypes);
            }
            return false;
        }
        internal static async Task DeleteRuntimes(HostedPlatformLibrary lib)
        {
            if (PlatformHostMain.api.DeleteRuntime != null)
            {
                HashSet<PlatformInstanceData>? instances = null;
                lock (RxMetaData.Instance.TypesLock)
                {
                    if (RxMetaData.Instance.RuntimeObjects.TryGetValue(lib, out instances))
                    {
                        RxMetaData.Instance.RuntimeObjects.Remove(lib);
                    }
                }
                if (instances != null)
                {
                    foreach (PlatformInstanceData inst in instances) 
                    {
                        Exception? result = await PlatformHostMain.api.DeleteRuntime(inst.rxType, "", inst.id);
                        if (result == null)
                        {
                            RxPlatformObject.Instance.WriteLogDebug("PlatformRuntimeTypes.UnregisterRuntimes", 100
                                , $"Deleted Runtime object with id {inst.id} at path {inst.path}.");
                        }
                        else
                        {
                            RxPlatformObject.Instance.WriteLogError("PlatformRuntimeTypes.UnregisterRuntimes", 110
                                , $"Failed to delete Runtime object with id {inst.id} at path {inst.path}:{result.Message}");
                        }
                    }
                }
            }
        }
        internal static async Task<RxPlatformRuntimeBase?> RegisterRuntime(byte type, object prototype, string name, string path, RxNodeId id)
        {
            RxPlatformObjectRuntime? runtimeInstance = null;
            // we have to have an id that is valid
            if (id.IsNull())
            {
                id = new RxNodeId(Guid.NewGuid(), 999);
            }
            var result = await CreateRuntime(prototype, name, path, id, prototype.GetType().Assembly);
            if(result)
            {
                lock (RxMetaData.Instance.TypesLock)
                {
                    if (RxMetaData.Instance.ObjectRuntimes.NodeIdDict.TryGetValue(id, out var rtData))
                    {
                        runtimeInstance = rtData.objectRuntime;
                    }
                }
            }
            return runtimeInstance;
        }
        internal static Task UnregisterRuntime(byte typeId, Type type, RxPlatformRuntimeBase instance, string name, string path, RxNodeId id)
        {
            return Task.CompletedTask;
        }


        // functions called by the native side bind/unbind runtimes

        static internal void BindObject(rx_item_type type, RxNodeId nodeId, RxNodeId typeId, IntPtr nativePtr)
        {
            switch (type)
            {
                case rx_item_type.rx_object:
                    {
                        RuntimeConstructionData typeConstructor;
                        bool found = false;
                        lock (RxMetaData.Instance.TypesLock)
                        {
                            found = RxMetaData.Instance.ObjectRuntimes.RegisteredConstructors.TryGetValue(typeId, out typeConstructor);
                        }
                        if (found && typeConstructor.constructor != null)
                        {
                            RxPlatformObjectRuntime? managedObj = typeConstructor.constructor() as RxPlatformObjectRuntime;
                            if(managedObj != null)
                            {
                                Action? startedMethod = null;
                                if (typeConstructor.startMethod != null)
                                    startedMethod = (Action)Delegate.CreateDelegate(typeof(Action), managedObj, typeConstructor.startMethod);

                                Action? stoppedMethod = null;
                                if (typeConstructor.stopMethod != null)
                                    stoppedMethod = (Action)Delegate.CreateDelegate(typeof(Action), managedObj, typeConstructor.stopMethod);

                                PlatformRuntimeData<RxPlatformObjectRuntime> runtimeData = new PlatformRuntimeData<RxPlatformObjectRuntime>()
                                {
                                    objectRuntime = managedObj,
                                    nativeRuntimePtr = nativePtr,
                                    nodeId = nodeId,
                                    startedMethod = startedMethod,
                                    stoppedMethod = stoppedMethod
                                };
                                
                                lock (RxMetaData.Instance.TypesLock)
                                {
                                    RxMetaData.Instance.ObjectRuntimes.ManagedDict.Add(managedObj, runtimeData);
                                    RxMetaData.Instance.ObjectRuntimes.NativeDict.Add(nativePtr, runtimeData);
                                    RxMetaData.Instance.ObjectRuntimes.NodeIdDict.Add(nodeId, runtimeData);
                                }
                                managedObj.__BindObject((IntPtr)nativePtr);
                                RxPlatformObject.Instance.WriteLogDebug("PlatformRuntimeTypes.BindObject", 90
                                    , $"Created managed object type for native object pointer {nativePtr}.");
                            }
                            else
                            {
                                RxPlatformObject.Instance.WriteLogError("PlatformRuntimeTypes.BindObject", 110
                                    , $"Failed to bind native object pointer {nativePtr} unable to create managed object with type id {typeId}.");
                            }
                        }
                        else
                        {
                            RxPlatformObject.Instance.WriteLogError("PlatformRuntimeTypes.BindObject", 110
                                , $"Failed to bind native object pointer {nativePtr} because no managed object is registered with id {typeId}.");

                        }
                    }
                    break;
                default:
                    {
                        RxPlatformObject.Instance.WriteLogError("PlatformRuntimeTypes.BindObject", 120
                            , $"Failed to bind native object pointer {nativePtr} because type {type} is not supported.");
                    }
                    break;
            }
        }
        static internal void UnbindObject(rx_item_type type, RxNodeId nodeId, RxNodeId typeId, IntPtr nativePtr)
        {
            switch (type)
            {
                case rx_item_type.rx_object:
                    {
                        bool success = false;
                        lock (RxMetaData.Instance.TypesLock)
                        {
                            if (RxMetaData.Instance.ObjectRuntimes.NativeDict.TryGetValue(nativePtr, out var rtData))
                            {
                                success = true;
                                if (rtData.objectRuntime != null)
                                {
                                    rtData.objectRuntime.__BindObject(IntPtr.Zero);
                                    if(rtData.stoppedMethod != null)
                                    {
                                        rtData.stoppedMethod();
                                    }
                                    RxMetaData.Instance.ObjectRuntimes.ManagedDict.Remove(rtData.objectRuntime);
                                }
                                RxMetaData.Instance.ObjectRuntimes.NodeIdDict.Remove(rtData.nodeId);
                                RxMetaData.Instance.ObjectRuntimes.NativeDict.Remove(nativePtr);
                              }
                        }
                        if (success)
                        {
                            RxPlatformObject.Instance.WriteLogDebug("PlatformRuntimeTypes.BindObject", 90
                                    , $"Removed managed object type for native object pointer {nativePtr}.");

                        }
                        else
                        {
                            RxPlatformObject.Instance.WriteLogError("PlatformRuntimeTypes.BindObject", 110
                                , $"Failed to unbind native object pointer {nativePtr} unable to find managed object with pointer.");
                        }
                    }
                    break;
                default:
                    {
                        RxPlatformObject.Instance.WriteLogError("PlatformRuntimeTypes.BindObject", 120
                            , $"Failed to bind native object pointer {nativePtr} because type {type} is not supported.");
                    }
                    break;
            }
        }
    }
}