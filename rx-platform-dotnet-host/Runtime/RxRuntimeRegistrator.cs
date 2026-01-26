using ENSACO.RxPlatform.Attributes;
using ENSACO.RxPlatform.Hosting.Common;
using ENSACO.RxPlatform.Hosting.Construction;
using ENSACO.RxPlatform.Hosting.Internal;
using ENSACO.RxPlatform.Hosting.Model;
using ENSACO.RxPlatform.Model;
using ENSACO.RxPlatform.Runtime;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;

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
                stream.AppendLine(RxRuntimeDefinitionCreater.CreateOverrides(prototype, lib));
                stream.Append($@"
}}
}}");
                string def = stream.ToString();
                if (await RxPlatformCreateRuntime(rxType, id, parentId, prototype, name, path, def))
                {
                    var instData = new PlatformInstanceData()
                    {
                        id = id,
                        path = $"{path}/{name}",
                        rxType = rxType
                    };
                    lock (RxMetaData.Instance.TypesLock)
                    {
                        if (!RxMetaData.Instance.RuntimeObjects.ContainsKey(lib))
                        {
                            RxMetaData.Instance.RuntimeObjects.Add(lib, new HashSet<PlatformInstanceData>());
                        }
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
                path = $"/world{(string.IsNullOrEmpty(path) ? "" : "/")}{path}";
                
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
        internal static async Task<RxPlatformObjectRuntime?> RegisterRuntime(byte type, object prototype, string name, string path, RxNodeId id)
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
                    if (RxMetaData.Instance.ObjectRuntimes.NodeIdDict.TryGetValue(id, out var rtData) 
                        && rtData.objectRuntime.IsAllocated)
                    {
                        RxPlatformObjectRuntime? managedObj = rtData.objectRuntime.Target as RxPlatformObjectRuntime;
                        if (managedObj != null)
                        {
                            runtimeInstance = managedObj;
                        }
                    }
                }
            }
            return runtimeInstance;
        }
        internal static Task UnregisterRuntime(byte typeId, Type type, RxPlatformObjectRuntime instance, string name, string path, RxNodeId id)
        {
            return Task.CompletedTask;
        }


        // functions called by the native side bind/unbind runtimes
        static void BindObject(RxNodeId nodeId, string path, RxNodeId typeId, IntPtr nativePtr)
        {

            System.Diagnostics.Debug.Assert(!nodeId.IsNull() && !string.IsNullOrEmpty(path));
            RuntimeConstructionData typeConstructor;
            bool found = false;
            lock (RxMetaData.Instance.TypesLock)
            {
                found = RxMetaData.Instance.ObjectRuntimes.RegisteredConstructors.TryGetValue(typeId, out typeConstructor);
            }
            if (found && typeConstructor.constructor != null)
            {
                RxPlatformObjectRuntime? managedObj = typeConstructor.constructor() as RxPlatformObjectRuntime;
                if (managedObj != null)
                {
                    Action? startedMethod = null;
                    if (typeConstructor.startMethod != null)
                        startedMethod = (Action)Delegate.CreateDelegate(typeof(Action), managedObj, typeConstructor.startMethod);

                    Action? stoppedMethod = null;
                    if (typeConstructor.stopMethod != null)
                        stoppedMethod = (Action)Delegate.CreateDelegate(typeof(Action), managedObj, typeConstructor.stopMethod);

                    PlatformRuntimeData runtimeData = new PlatformRuntimeData
                    {
                        objectRuntime = GCHandle.Alloc(managedObj),
                        nativeRuntimePtr = nativePtr,
                        nodeId = nodeId,
                        startedMethod = startedMethod,
                        stoppedMethod = stoppedMethod,
                        path = path
                    };

                    lock (RxMetaData.Instance.TypesLock)
                    {
                        RxMetaData.Instance.ObjectRuntimes.NativeDict.Add(nativePtr, runtimeData);
                        RxMetaData.Instance.ObjectRuntimes.NodeIdDict.Add(nodeId, runtimeData);

                        RuntimeConstructAlgorithms.AddToConstructionData(rx_item_type.rx_object, nodeId, "", nativePtr, RxMetaData.Instance.RuntimeConstruction);


                    }
                    managedObj.__BindObject((IntPtr)nativePtr, nodeId, path);
                    RxPlatformObject.Instance.WriteLogDebug("PlatformRuntimeTypes.BindObject", 90
                        , $"Created managed object type for native object pointer 0x{nativePtr.ToString("X")}.");
                }
                else
                {
                    RxPlatformObject.Instance.WriteLogError("PlatformRuntimeTypes.BindObject", 110
                        , $"Failed to bind native object pointer 0x{nativePtr.ToString("X")} unable to create managed object with type id {typeId}.");
                }
            }
            else
            {
                RxPlatformObject.Instance.WriteLogError("PlatformRuntimeTypes.BindObject", 110
                    , $"Failed to bind native object pointer 0x{nativePtr.ToString("X")} because no managed object is registered with id {typeId}.");

            }
        }
        static void BindStruct(RxNodeId nodeId, string path, RxNodeId typeId, IntPtr nativePtr)
        {

            System.Diagnostics.Debug.Assert(!nodeId.IsNull() && !string.IsNullOrEmpty(path));
            RuntimeConstructionData typeConstructor;
            RxPlatformObjectRuntime? hostObj = null;
            bool found = false;
            lock (RxMetaData.Instance.TypesLock)
            {
                if (RxMetaData.Instance.ObjectRuntimes.NodeIdDict.TryGetValue(nodeId, out var rtData))
                {
                    if (rtData.objectRuntime.IsAllocated)
                    {
                        hostObj = rtData.objectRuntime.Target as RxPlatformObjectRuntime;
                    }
                }
                found = RxMetaData.Instance.StructRuntimes.RegisteredConstructors.TryGetValue(typeId, out typeConstructor);
            }
            if (found && typeConstructor.constructor != null)
            {
                RxPlatformStructRuntime? managedObj = typeConstructor.constructor() as RxPlatformStructRuntime;
                if (managedObj != null)
                {
                    if (typeConstructor.initialValues != null && typeConstructor.initialValues.Length > 0)
                    {
                        string temp = Encoding.UTF8.GetString(typeConstructor.initialValues);
                        Utf8JsonReader reader = new Utf8JsonReader(typeConstructor.initialValues);
                        managedObj.__rxStructDeserialize(ref reader);
                    }
                    Action? startedMethod = null;
                    if (typeConstructor.startMethod != null)
                        startedMethod = (Action)Delegate.CreateDelegate(typeof(Action), managedObj, typeConstructor.startMethod);

                    Action? stoppedMethod = null;
                    if (typeConstructor.stopMethod != null)
                        stoppedMethod = (Action)Delegate.CreateDelegate(typeof(Action), managedObj, typeConstructor.stopMethod);


                    PlatformRuntimeData runtimeData = new PlatformRuntimeData
                    {
                        objectRuntime = GCHandle.Alloc(managedObj),
                        nativeRuntimePtr = nativePtr,
                        nodeId = nodeId,
                        startedMethod = startedMethod,
                        stoppedMethod = stoppedMethod,
                        path = path
                    };

                    lock (RxMetaData.Instance.TypesLock)
                    {
                        RxMetaData.Instance.StructRuntimes.NativeDict.Add(nativePtr, runtimeData);
                        RuntimeConstructAlgorithms.AddToConstructionData(rx_item_type.rx_struct_type, nodeId, path, nativePtr, RxMetaData.Instance.RuntimeConstruction);

                    }

                    managedObj.__BindObject((IntPtr)nativePtr, nodeId, path);
                    RxPlatformObject.Instance.WriteLogDebug("PlatformRuntimeTypes.BindObject", 90
                        , $"Created managed object type for native struct pointer 0x{nativePtr.ToString("X")}.");
                }
                else
                {
                    RxPlatformObject.Instance.WriteLogError("PlatformRuntimeTypes.BindObject", 110
                        , $"Failed to bind native struct pointer 0x{nativePtr.ToString("X")} unable to create managed object with type id {typeId}.");
                }
            }
            else
            {
                RxPlatformObject.Instance.WriteLogError("PlatformRuntimeTypes.BindObject", 110
                    , $"Failed to bind native struct pointer 0x{nativePtr.ToString("X")} because no managed object is registered with id {typeId}.");

            }
        }
        static void BindSource(RxNodeId nodeId, string path, RxNodeId typeId, IntPtr nativePtr)
        {

            System.Diagnostics.Debug.Assert(!nodeId.IsNull() && !string.IsNullOrEmpty(path));
            RuntimeConstructionData typeConstructor;
            bool found = false;
            lock (RxMetaData.Instance.TypesLock)
            {
                found = RxMetaData.Instance.SourceRuntimes.RegisteredConstructors.TryGetValue(typeId, out typeConstructor);
            }
            if (found && typeConstructor.constructor != null)
            {
                RxPlatformSourceRuntime? managedObj = typeConstructor.constructor() as RxPlatformSourceRuntime;
                if (managedObj != null)
                {
                    Action? startedMethod = null;
                    if (typeConstructor.startMethod != null)
                        startedMethod = (Action)Delegate.CreateDelegate(typeof(Action), managedObj, typeConstructor.startMethod);

                    Action? stoppedMethod = null;
                    if (typeConstructor.stopMethod != null)
                        stoppedMethod = (Action)Delegate.CreateDelegate(typeof(Action), managedObj, typeConstructor.stopMethod);


                    bool[] types = new bool[Enum.GetValues(typeof(rx_value_t)).Length];
                    SourceWriteMethods sourceWrites = new SourceWriteMethods();
                    foreach (var methodData in typeConstructor.sourceWriteMethods!)
                    {
                        types[(int)methodData.typeCode] = true;
                        switch ((rx_value_t)methodData.typeCode)
                        {
                            case rx_value_t.Bool:
                                sourceWrites.writeBoolMethod = (Action<bool, Action<Exception?>>)Delegate.CreateDelegate(typeof(Action<bool, Action<Exception?>>), managedObj, methodData.methodInfo);
                                break;
                            case rx_value_t.Int8:
                                sourceWrites.writeSByteMethod = (Action<sbyte, Action<Exception?>>)Delegate.CreateDelegate(typeof(Action<sbyte, Action<Exception?>>), managedObj, methodData.methodInfo);
                                break;
                            case rx_value_t.Int16:
                                sourceWrites.writeShortMethod = (Action<short, Action<Exception?>>)Delegate.CreateDelegate(typeof(Action<short, Action<Exception?>>), managedObj, methodData.methodInfo);
                                break;
                            case rx_value_t.Int32:
                                sourceWrites.writeIntMethod = (Action<int, Action<Exception?>>)Delegate.CreateDelegate(typeof(Action<int, Action<Exception?>>), managedObj, methodData.methodInfo);
                                break;
                            case rx_value_t.Int64:
                                sourceWrites.writeLongMethod = (Action<long, Action<Exception?>>)Delegate.CreateDelegate(typeof(Action<long, Action<Exception?>>), managedObj, methodData.methodInfo);
                                break;

                            case rx_value_t.UInt8:
                                sourceWrites.writeByteMethod = (Action<byte, Action<Exception?>>)Delegate.CreateDelegate(typeof(Action<byte, Action<Exception?>>), managedObj, methodData.methodInfo);
                                break;
                            case rx_value_t.UInt16:
                                sourceWrites.writeUShortMethod = (Action<ushort, Action<Exception?>>)Delegate.CreateDelegate(typeof(Action<ushort, Action<Exception?>>), managedObj, methodData.methodInfo);
                                break;
                            case rx_value_t.UInt32:
                                sourceWrites.writeUIntMethod = (Action<uint, Action<Exception?>>)Delegate.CreateDelegate(typeof(Action<uint, Action<Exception?>>), managedObj, methodData.methodInfo);
                                break;
                            case rx_value_t.UInt64:
                                sourceWrites.writeULongMethod = (Action<ulong, Action<Exception?>>)Delegate.CreateDelegate(typeof(Action<ulong, Action<Exception?>>), managedObj, methodData.methodInfo);
                                break;

                            case rx_value_t.Float:
                                sourceWrites.writeFloatMethod = (Action<float, Action<Exception?>>)Delegate.CreateDelegate(typeof(Action<float, Action<Exception?>>), managedObj, methodData.methodInfo);
                                break;
                            case rx_value_t.Double:
                                sourceWrites.writeDoubleMethod = (Action<double, Action<Exception?>>)Delegate.CreateDelegate(typeof(Action<double, Action<Exception?>>), managedObj, methodData.methodInfo);
                                break;

                            case rx_value_t.String:
                                sourceWrites.writeStringMethod = (Action<string, Action<Exception?>>)Delegate.CreateDelegate(typeof(Action<string, Action<Exception?>>), managedObj, methodData.methodInfo);
                                break;

                            case rx_value_t.Time:
                                sourceWrites.writeDateTimeMethod = (Action<DateTime, Action<Exception?>>)Delegate.CreateDelegate(typeof(Action<DateTime, Action<Exception?>>), managedObj, methodData.methodInfo);
                                break;
                            case rx_value_t.Uuid:
                                sourceWrites.writeUuidMethod = (Action<Guid, Action<Exception?>>)Delegate.CreateDelegate(typeof(Action<Guid, Action<Exception?>>), managedObj, methodData.methodInfo);
                                break;
                            default:
                                {
                                    sourceWrites.writeJsonMethod = new Tuple<Type, Action<object, Action<Exception?>>?>(
                                            methodData.methodInfo.GetParameters()[0].ParameterType,
                                            (Action<object, Action<Exception?>>)Delegate.CreateDelegate(typeof(Action<object, Action<Exception?>>), managedObj, methodData.methodInfo)
                                        );
                                }
                                break;



                        }
                    }

                    PlatformRuntimeData runtimeData = new PlatformRuntimeData
                    {
                        objectRuntime = GCHandle.Alloc(managedObj),
                        nativeRuntimePtr = nativePtr,
                        nodeId = nodeId,
                        startedMethod = startedMethod,
                        stoppedMethod = stoppedMethod,
                        sourceWriteMethods = sourceWrites,
                        handleRequests = null,
                        path = path
                    };

                    lock (RxMetaData.Instance.TypesLock)
                    {
                        RxMetaData.Instance.SourceRuntimes.NativeDict.Add(nativePtr, runtimeData);
                        RuntimeConstructAlgorithms.AddToConstructionData(rx_item_type.rx_source_type, nodeId, path, nativePtr, RxMetaData.Instance.RuntimeConstruction);
                    }
                    managedObj.__BindObject((IntPtr)nativePtr, nodeId, path);
                    RxPlatformObject.Instance.WriteLogDebug("PlatformRuntimeTypes.BindObject", 90
                        , $"Created managed object type for native source pointer 0x{nativePtr.ToString("X")}.");
                }
                else
                {
                    RxPlatformObject.Instance.WriteLogError("PlatformRuntimeTypes.BindObject", 110
                        , $"Failed to bind native source pointer 0x{nativePtr.ToString("X")} unable to create managed object with type id {typeId}.");
                }
            }
            else
            {
                RxPlatformObject.Instance.WriteLogError("PlatformRuntimeTypes.BindObject", 110
                    , $"Failed to bind native source pointer 0x{nativePtr.ToString("X")} because no managed object is registered with id {typeId}.");

            }
        }
        static internal void BindObject(rx_item_type type, RxNodeId nodeId, string path, RxNodeId typeId, IntPtr nativePtr)
        {
            switch (type)
            {
                case rx_item_type.rx_object:
                    BindObject(nodeId, path, typeId, nativePtr);
                    break;
                case rx_item_type.rx_source_type:
                    BindSource(nodeId, path, typeId, nativePtr);
                    break;
                case rx_item_type.rx_struct_type:
                    BindStruct(nodeId, path, typeId, nativePtr);
                    break;
                default:
                    {
                        RxPlatformObject.Instance.WriteLogError("PlatformRuntimeTypes.BindObject", 120
                            , $"Failed to bind native pointer 0x{nativePtr.ToString("X")} because type {type} is not supported.");
                    }
                    break;
            }
        }
        static internal void UnbindObject(rx_item_type type, RxNodeId nodeId, string path, RxNodeId typeId, IntPtr nativePtr)
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
                                if (rtData.objectRuntime.IsAllocated)
                                {
                                    RxPlatformObjectRuntime? managedObj = rtData.objectRuntime.Target as RxPlatformObjectRuntime;
                                    if(managedObj != null)
                                    {
                                        managedObj.__BindObject(IntPtr.Zero, RxNodeId.NullId, "");
                                        if (rtData.stoppedMethod != null)
                                        {
                                            rtData.stoppedMethod();
                                        }
                                    }
                                }
                                RxMetaData.Instance.ObjectRuntimes.NodeIdDict.Remove(rtData.nodeId);
                                RxMetaData.Instance.ObjectRuntimes.NativeDict.Remove(nativePtr);
                              }
                        }
                        if (success)
                        {
                            RxPlatformObject.Instance.WriteLogDebug("PlatformRuntimeTypes.BindObject", 90
                                    , $"Removed managed object type for native object pointer 0x{nativePtr.ToString("X")}.");

                        }
                        else
                        {
                            RxPlatformObject.Instance.WriteLogError("PlatformRuntimeTypes.BindObject", 110
                                , $"Failed to unbind native object pointer 0x{nativePtr.ToString("X")} unable to find managed object with pointer.");
                        }
                    }
                    break;

                case rx_item_type.rx_source_type:
                    {
                        bool success = false;
                        lock (RxMetaData.Instance.TypesLock)
                        {
                            if (RxMetaData.Instance.SourceRuntimes.NativeDict.TryGetValue(nativePtr, out var rtData))
                            {
                                success = true;
                                if (rtData.objectRuntime.IsAllocated)
                                {
                                    RxPlatformSourceRuntime? managedObj = rtData.objectRuntime.Target as RxPlatformSourceRuntime;
                                    if (managedObj != null)
                                    {
                                        managedObj.__BindObject(IntPtr.Zero, RxNodeId.NullId, "");
                                        if (rtData.stoppedMethod != null)
                                        {
                                            rtData.stoppedMethod();
                                        }
                                    }
                                }
                                RxMetaData.Instance.SourceRuntimes.NativeDict.Remove(nativePtr);
                            }
                        }
                        if (success)
                        {
                            RxPlatformObject.Instance.WriteLogDebug("PlatformRuntimeTypes.BindObject", 90
                                    , $"Removed managed object type for native object pointer 0x{nativePtr.ToString("X")}.");

                        }
                        else
                        {
                            RxPlatformObject.Instance.WriteLogError("PlatformRuntimeTypes.BindObject", 110
                                , $"Failed to unbind native object pointer 0x{nativePtr.ToString("X")} unable to find managed object with pointer.");
                        }
                    }
                    break;


                case rx_item_type.rx_struct_type:
                    {
                        bool success = false;
                        lock (RxMetaData.Instance.TypesLock)
                        {
                            if (RxMetaData.Instance.StructRuntimes.NativeDict.TryGetValue(nativePtr, out var rtData))
                            {
                                success = true;
                                if (rtData.objectRuntime.IsAllocated)
                                {
                                    RxPlatformStructRuntime? managedObj = rtData.objectRuntime.Target as RxPlatformStructRuntime;
                                    if (managedObj != null)
                                    {
                                        managedObj.__BindObject(IntPtr.Zero, RxNodeId.NullId, "");
                                        if (rtData.stoppedMethod != null)
                                        {
                                            rtData.stoppedMethod();
                                        }
                                    }
                                }
                                RxMetaData.Instance.StructRuntimes.NativeDict.Remove(nativePtr);
                            }
                        }
                        if (success)
                        {
                            RxPlatformObject.Instance.WriteLogDebug("PlatformRuntimeTypes.BindObject", 90
                                    , $"Removed managed object type for native object pointer 0x{nativePtr.ToString("X")}.");

                        }
                        else
                        {
                            RxPlatformObject.Instance.WriteLogError("PlatformRuntimeTypes.BindObject", 110
                                , $"Failed to unbind native object pointer 0x{nativePtr.ToString("X")} unable to find managed object with pointer.");
                        }
                    }
                    break;
                default:
                    {
                        RxPlatformObject.Instance.WriteLogError("PlatformRuntimeTypes.BindObject", 120
                            , $"Failed to bind native object pointer 0x{nativePtr.ToString("X")} because type {type} is not supported.");
                    }
                    break;
            }
        }

        internal static RxPlatformObjectRuntime? GetInstance(nint instancePtr)
        {
            RxPlatformObjectRuntime? managedObj = null;
            lock (RxMetaData.Instance.TypesLock)
            {
                if (RxMetaData.Instance.ObjectRuntimes.NativeDict.TryGetValue((IntPtr)instancePtr, out var rtData))
                {
                    if (rtData.objectRuntime.IsAllocated)
                    {
                        managedObj = rtData.objectRuntime.Target as RxPlatformObjectRuntime;
                    }
                }
            }
            return managedObj;
        }
    }
}