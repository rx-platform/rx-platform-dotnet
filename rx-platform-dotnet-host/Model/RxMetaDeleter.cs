using ENSACO.RxPlatform.Hosting.Common;
using ENSACO.RxPlatform.Hosting.Interface;
using ENSACO.RxPlatform.Hosting.Internal;
using ENSACO.RxPlatform.Model;
using System.Reflection;

namespace ENSACO.RxPlatform.Hosting.Model
{
    internal static class RxMetaDeleter
    {
        struct DeletingTypeInfo
        {
            public rx_item_type type;
            public RxNodeId id;
            public string fullPath;
        }
        internal static void DeleteAssemblyTypes(LibraryPlatformTypes types, HostedPlatformLibrary hostLib, Assembly assembly)
        {
            if(PlatformHostMain.api.DeleteType == null)
            {
                return;
            }
            string module = hostLib.GetPluginName();
            List<DeletingTypeInfo> toDelete = new List<DeletingTypeInfo>();
            lock (RxMetaData.Instance.TypesLock)
            {

                RxMetaData.Instance.HostedLibraries.Remove(assembly);
                
                foreach (var typeName in types.objectTypes)
                {
                    if (RxMetaData.Instance.ObjectTypes.TryGetValue(typeName, out var objType))
                    {
                        if (objType.Meta.runtimeType && RxMetaData.Instance.ObjectRuntimes.RegisteredConstructors.ContainsKey(objType.Meta.id))
                        {
                            RxMetaData.Instance.ObjectRuntimes.RegisteredConstructors.Remove(objType.Meta.id);
                        }
                        RxMetaData.Instance.ObjectTypes.Remove(typeName);
                        toDelete.Add(new DeletingTypeInfo
                        {
                            type = rx_item_type.rx_object_type
                            , id = objType.Meta.id
                            , fullPath = $"{objType.Meta.path}/{objType.Meta.name}"
                        });                        
                    }
                }
                foreach (var typeName in types.applicationTypes)
                {
                    if (RxMetaData.Instance.ApplicationTypes.TryGetValue(typeName, out var objType))
                    {
                        if (objType.Meta.whose == hostLib)
                        {
                            RxMetaData.Instance.ApplicationTypes.Remove(typeName);
                            toDelete.Add(new DeletingTypeInfo
                            {
                                type = rx_item_type.rx_application_type,
                                id = objType.Meta.id,
                                fullPath = $"{objType.Meta.path}/{objType.Meta.name}"
                            });
                        }
                    }
                }
                foreach (var typeName in types.domainTypes)
                {
                    if (RxMetaData.Instance.DomainTypes.TryGetValue(typeName, out var objType))
                    {
                        if (objType.Meta.whose == hostLib)
                        {
                            RxMetaData.Instance.DomainTypes.Remove(typeName);
                            toDelete.Add(new DeletingTypeInfo
                            {
                                type = rx_item_type.rx_domain_type,
                                id = objType.Meta.id,
                                fullPath = $"{objType.Meta.path}/{objType.Meta.name}"
                            });
                        }
                    }
                }
                foreach (var typeName in types.portTypes)
                {
                    if (RxMetaData.Instance.PortTypes.TryGetValue(typeName, out var objType))
                    {
                        if (objType.Meta.whose == hostLib)
                        {
                            RxMetaData.Instance.PortTypes.Remove(typeName);
                            toDelete.Add(new DeletingTypeInfo
                            {
                                type = rx_item_type.rx_port_type,
                                id = objType.Meta.id,
                                fullPath = $"{objType.Meta.path}/{objType.Meta.name}"
                            });
                        }
                    }
                }

                foreach (var typeNane in types.variableTypes)
                {
                    if (RxMetaData.Instance.VariableTypes.TryGetValue(typeNane, out var objType))
                    {
                        if (objType.Meta.whose == hostLib)
                        {
                            RxMetaData.Instance.VariableTypes.Remove(typeNane);
                            toDelete.Add(new DeletingTypeInfo
                            {
                                type = rx_item_type.rx_variable_type,
                                id = objType.Meta.id,
                                fullPath = $"{objType.Meta.path}/{objType.Meta.name}"
                            });
                        }
                    }
                }
                foreach (var typeName in types.structTypes)
                {
                    if (RxMetaData.Instance.StructTypes.TryGetValue(typeName, out var objType))
                    {
                        if (objType.Meta.whose == hostLib)
                        {
                            RxMetaData.Instance.StructTypes.Remove(typeName);
                            toDelete.Add(new DeletingTypeInfo
                            {
                                type = rx_item_type.rx_struct_type,
                                id = objType.Meta.id,
                                fullPath = $"{objType.Meta.path}/{objType.Meta.name}"
                            });
                        }
                    }
                }
                foreach (var typeName in types.sourceTypes)
                {
                    if (RxMetaData.Instance.SourceTypes.TryGetValue(typeName, out var objType))
                    {
                        if (objType.Meta.whose == hostLib)
                        {
                            if (objType.Meta.runtimeType && RxMetaData.Instance.SourceRuntimes.RegisteredConstructors.ContainsKey(objType.Meta.id))
                            {
                                RxMetaData.Instance.SourceRuntimes.RegisteredConstructors.Remove(objType.Meta.id);
                            }
                            RxMetaData.Instance.SourceTypes.Remove(typeName);
                            toDelete.Add(new DeletingTypeInfo
                            {
                                type = rx_item_type.rx_source_type,
                                id = objType.Meta.id,
                                fullPath = $"{objType.Meta.path}/{objType.Meta.name}"
                            });
                        }
                    }
                }
                foreach (var typeName in types.filterTypes)
                {
                    if (RxMetaData.Instance.FilterTypes.TryGetValue(typeName, out var objType))
                    {
                        if (objType.Meta.whose == hostLib)
                        {
                            RxMetaData.Instance.FilterTypes.Remove(typeName);
                            toDelete.Add(new DeletingTypeInfo
                            {
                                type = rx_item_type.rx_filter_type,
                                id = objType.Meta.id,
                                fullPath = $"{objType.Meta.path}/{objType.Meta.name}"
                            });
                        }
                    }
                }
                foreach (var typeName in types.eventTypes)
                {
                    if (RxMetaData.Instance.EventTypes.TryGetValue(typeName, out var objType))
                    {
                        if (objType.Meta.whose == hostLib)
                        {
                            if (objType.Meta.runtimeType && RxMetaData.Instance.EventRuntimes.RegisteredConstructors.ContainsKey(objType.Meta.id))
                            {
                                RxMetaData.Instance.EventRuntimes.RegisteredConstructors.Remove(objType.Meta.id);
                            }
                            RxMetaData.Instance.EventTypes.Remove(typeName);
                            toDelete.Add(new DeletingTypeInfo
                            {
                                type = rx_item_type.rx_event_type,
                                id = objType.Meta.id,
                                fullPath = $"{objType.Meta.path}/{objType.Meta.name}"
                            });
                        }
                    }
                }
                foreach (var typeName in types.mapperTypes)
                {
                    if (RxMetaData.Instance.MapperTypes.TryGetValue(typeName, out var objType))
                    {
                        if (objType.Meta.whose == hostLib)
                        {
                            if (objType.Meta.runtimeType && RxMetaData.Instance.MapperRuntimes.RegisteredConstructors.ContainsKey(objType.Meta.id))
                            {
                                RxMetaData.Instance.MapperRuntimes.RegisteredConstructors.Remove(objType.Meta.id);
                            }
                            RxMetaData.Instance.MapperTypes.Remove(typeName);
                            toDelete.Add(new DeletingTypeInfo
                            {
                                type = rx_item_type.rx_mapper_type,
                                id = objType.Meta.id,
                                fullPath = $"{objType.Meta.path}/{objType.Meta.name}"
                            });
                        }
                    }
                }
                foreach (var typeName in types.dataTypes)
                {
                    if (RxMetaData.Instance.DataTypes.TryGetValue(typeName, out var objType))
                    {
                        if (objType.Meta.whose == hostLib)
                        {
                            RxMetaData.Instance.DataTypes.Remove(typeName);
                            toDelete.Add(new DeletingTypeInfo
                            {
                                type = rx_item_type.rx_data_type,
                                id = objType.Meta.id,
                                fullPath = $"{objType.Meta.path}/{objType.Meta.name}"
                            });

                        }
                    }
                }
            }
            foreach (var del in toDelete)
            {
                unsafe
                {
                    rx_node_id_struct nodeIdStruct = CommonInterface.CreateNodeIdFromRxNodeId(del.id);
                    PlatformHostMain.api.DeleteType(del.type, module, &nodeIdStruct);

                    RxPlatformObject.Instance.WriteLogTrace("RxMetaDeleter", 0
                        , $"Removed type {del.fullPath}.");

                }
            }
        }

    }
}