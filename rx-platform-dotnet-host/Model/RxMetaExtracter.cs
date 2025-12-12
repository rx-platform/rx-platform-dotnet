using ENSACO.RxPlatform.Attributes;
using ENSACO.RxPlatform.Hosting.Internal;
using ENSACO.RxPlatform.Hosting.Model.Algorithms;
using ENSACO.RxPlatform.Hosting.Model.Items;
using ENSACO.RxPlatform.Hosting.Reflection;
using ENSACO.RxPlatform.Model;
using ENSACO.RxPlatform.Runtime;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;

namespace ENSACO.RxPlatform.Hosting.Model
{
    internal struct LibraryPlatformTypes
    {
        internal List<RxNodeId> objectTypes;
        internal List<RxNodeId> applicationTypes;
        internal List<RxNodeId> domainTypes;
        internal List<RxNodeId> portTypes;

        internal List<RxNodeId> structTypes;
        internal List<RxNodeId> mapperTypes;
        internal List<RxNodeId> variableTypes;
        internal List<RxNodeId> sourceTypes;
        internal List<RxNodeId> eventTypes;
        internal List<RxNodeId> filterTypes;

        internal List<RxNodeId> dataTypes;
    }
    internal static class RxMetaExtracter
    {
        static void AddRxType<T>(T attr, Dictionary<RxNodeId, PlatformTypeBuildMeta<T>> dict, Type type, bool defined, bool runtime, HostedPlatformLibrary? hostLib)
            where T : RxPlatformTypeAttribute
        {
            RxNodeId parentId = new RxNodeId();
            if(!attr.ParentId.IsNull())
            {
                parentId = attr.ParentId;
            }
            else
            {
                Type? baseType = type.BaseType;
                if(baseType!=null)
                {
                    var baseAttr = baseType.GetCustomAttribute<T>();
                    if(baseAttr != null)
                    {
                        parentId = baseAttr.NodeId;
                    }
                }
            }

            string path = attr.Directory;
            if(!path.StartsWith('/'))
            {
                if(hostLib != null)
                {
                    path = $"/sys/{RxPlatformObject.Instance.GetPluginName()}/{hostLib.GetPluginName()}{(string.IsNullOrEmpty(attr.Directory) ? "" : "/")}{attr.Directory}";
                }
                else
                {
                    path = $"/sys/{RxPlatformObject.Instance.GetPluginName()}/undefined{(string.IsNullOrEmpty(attr.Directory) ? "" : "/")}{attr.Directory}";
                }
            }
            string typeName = type.Name;
            if(type.IsGenericType)
            {
                typeName = typeName.Substring(0, typeName.IndexOf('`'));
            }
            string name = string.IsNullOrEmpty(attr.Name) ? typeName : attr.Name;

            var platformTypeData = new PlatformTypeBuildMeta<T>
            {
                id = attr.NodeId,
                path = path,
                name = name,
                defaultConstructor = null,
                attribute = attr,
                definedType = defined,
                runtimeType = runtime,
                runtimeConstructor = null,
                startMethod = null,
                stopMethod = null,
                whose = hostLib,
                valid = true,
                type = type,
                parentId = parentId
            };
            dict.Add(platformTypeData.id, platformTypeData);
        }
        static void AddRxType(RxPlatformDataType attr, Dictionary<RxNodeId, PlatformDataTypeBuildMeta> dict, Type type, bool defined, bool runtime, HostedPlatformLibrary? hostLib)
        {
            RxNodeId parentId = new RxNodeId();
            if (!attr.ParentId.IsNull())
            {
                parentId = attr.ParentId;
            }
            else
            {
                Type? baseType = type.BaseType;
                if (baseType != null)
                {
                    var baseAttr = baseType.GetCustomAttribute<RxPlatformDataType>();
                    if (baseAttr != null)
                    {
                        parentId = baseAttr.NodeId;
                    }
                }
            }
            string path;
            if(!attr.Directory.StartsWith('/'))
            {
                if (hostLib != null)
                {
                    path = $"/sys/{RxPlatformObject.Instance.GetPluginName()}/{hostLib.GetPluginName()}{(string.IsNullOrEmpty(attr.Directory) ? "" : "/")}{attr.Directory}";
                }
                else
                {
                    path = $"/sys/{RxPlatformObject.Instance.GetPluginName()}/undefined{(string.IsNullOrEmpty(attr.Directory) ? "" : "/")}{attr.Directory}";
                }
            }
            else
            {
                path = attr.Directory;
            }

            var platformTypeData = new PlatformDataTypeBuildMeta
            {
                id = attr.NodeId,
                path = path,
                name = string.IsNullOrEmpty(attr.Name) ? type.Name : attr.Name,
                defaultConstructor = null,
                attribute = attr,
                definedType = defined,
                runtimeType = runtime,
                whose = hostLib,
                valid = true,
                type = type,
                parentId = parentId
            };
            dict.Add(platformTypeData.id, platformTypeData);
        }
        static PlatformTypeData<T> ConvertData<T>(PlatformTypeBuildMeta<T> buildMeta)
            where T : RxPlatformTypeAttribute
        {
            return new PlatformTypeData<T>
            {
                Meta = new PlatformTypeMeta<T>()
                {
                    whose = buildMeta.whose,
                    startMethod = buildMeta.startMethod,
                    stopMethod = buildMeta.stopMethod,

                    path = buildMeta.path,
                    name = buildMeta.name,
                    id = buildMeta.id,
                    definedType = buildMeta.definedType,
                    runtimeType = buildMeta.runtimeType,
                }
            };
        }
        static PlatformTypeData<RxPlatformDataType> ConvertData(PlatformDataTypeBuildMeta buildMeta)
        {
            return new PlatformTypeData<RxPlatformDataType>
            {
                Meta = new PlatformTypeMeta<RxPlatformDataType>()
                {
                    whose = buildMeta.whose,

                    path = buildMeta.path,
                    name = buildMeta.name,
                    id = buildMeta.id,
                    definedType = buildMeta.definedType,
                    runtimeType = buildMeta.runtimeType,
                }
            };
        }

        private static PlatformTypeBuildData FillTypes(Type[] types, out bool hadOne, HostedPlatformLibrary? hostLib)
        {            
            hadOne = false;
            PlatformTypeBuildData tempData = new PlatformTypeBuildData();
            foreach (var type in types)
            {
                bool runtime = type.GetCustomAttribute<RxPlatformRuntimeAttribute>() != null;
                bool defined = runtime || type.GetCustomAttribute<RxPlatformDeclareAttribute>() != null;

                if (type.IsClass)
                {
                    var objAttr = type.GetCustomAttribute<RxPlatformObjectType>();
                    if (objAttr != null)
                    {
                        AddRxType<RxPlatformObjectType>(objAttr, tempData.ObjectTypes, type, defined, runtime, hostLib);
                        hadOne = true;
                    }
                    var appAttr = type.GetCustomAttribute<RxPlatformApplicationType>();
                    if (appAttr != null)
                    {
                        AddRxType(appAttr, tempData.ApplicationTypes, type, defined, runtime, hostLib);
                        hadOne = true;
                    }
                    var domainAttr = type.GetCustomAttribute<RxPlatformDomainType>();
                    if (domainAttr != null)
                    {
                        AddRxType(domainAttr, tempData.DomainTypes, type, defined, runtime, hostLib);
                        hadOne = true;
                    }
                    var portAttr = type.GetCustomAttribute<RxPlatformPortType>();
                    if (portAttr != null)
                    {
                        AddRxType(portAttr, tempData.PortTypes, type, defined, runtime, hostLib);
                        hadOne = true;
                    }

                    var structAttr = type.GetCustomAttribute<RxPlatformStructType>();
                    if (structAttr != null)
                    {
                        AddRxType(structAttr, tempData.StructTypes, type, defined, runtime, hostLib);
                        hadOne = true;
                    }
                    var mapperAttr = type.GetCustomAttribute<RxPlatformMapperType>();
                    if (mapperAttr != null)
                    {
                        AddRxType(mapperAttr, tempData.MapperTypes, type, defined, runtime, hostLib);
                        hadOne = true;
                    }
                    var variableAttr = type.GetCustomAttribute<RxPlatformVariableType>();
                    if (variableAttr != null)
                    {
                        AddRxType(variableAttr, tempData.VariableTypes, type, defined, runtime, hostLib);
                        hadOne = true;
                    }
                    var sourceAttr = type.GetCustomAttribute<RxPlatformSourceType>();
                    if (sourceAttr != null)
                    {
                        AddRxType(sourceAttr, tempData.SourceTypes, type, defined, runtime, hostLib);
                        hadOne = true;
                    }
                    var eventAttr = type.GetCustomAttribute<RxPlatformEventType>();
                    if (eventAttr != null)
                    {
                        AddRxType(eventAttr, tempData.EventTypes, type, defined, runtime, hostLib);
                        hadOne = true;
                    }
                    var filterAttr = type.GetCustomAttribute<RxPlatformFilterType>();
                    if (filterAttr != null)
                    {
                        AddRxType(filterAttr, tempData.FilterTypes, type, defined, runtime, hostLib);
                        hadOne = true;
                    }
                    var dataAttr = type.GetCustomAttribute<RxPlatformDataType>();
                    if (dataAttr != null)
                    {
                        AddRxType(dataAttr, tempData.DataTypes, type, defined, runtime, hostLib);
                        hadOne = true;
                    }
                    else if (!hadOne && (defined || runtime))
                    {
                        RxPlatformObject.Instance.WriteLogWarining("", 200, $"Type {type.FullName} is not a class and does not have any valid type attribute.");
                    }
                }
                else
                {

                    var dataAttr = type.GetCustomAttribute<RxPlatformDataType>();
                    if (dataAttr != null)
                    {
                        AddRxType(dataAttr, tempData.DataTypes, type, defined, runtime, hostLib);
                        hadOne = true;
                    }
                    else if (defined || runtime)
                    {
                        RxPlatformObject.Instance.WriteLogWarining("", 200, $"Type {type.FullName} is not a class and does not have a RxPlatformDataType attribute.");
                    }
                }

            }
            return tempData;
        }
        internal static bool ParseType(Type type)
        {
            HostedPlatformLibrary? hostLib = null;
            PlatformTypeBuildData tempData = FillTypes([type], out bool hadOne, hostLib);

            if (hadOne)
            {
                foreach (var algorithm in SimplifiedAlgorithms)
                {
                    algorithm.FillTypes(tempData);
                }
                lock (RxMetaData.Instance.TypesLock)
                {
                    foreach (var kvp in tempData.ObjectTypes)
                    {
                        if (!kvp.Value.valid)
                            continue;

                        RxMetaData.Instance.ObjectTypes.Add(kvp.Key
                            , ConvertData<RxPlatformObjectType>(kvp.Value));
                     
                    }
                    foreach (var kvp in tempData.ApplicationTypes)
                    {
                        if (!kvp.Value.valid)
                            continue;
                        RxMetaData.Instance.ApplicationTypes.Add(kvp.Key
                            , ConvertData<RxPlatformApplicationType>(kvp.Value));
                    }
                    foreach (var kvp in tempData.DomainTypes)
                    {
                        if (!kvp.Value.valid)
                            continue;;
                        RxMetaData.Instance.DomainTypes.Add(kvp.Key
                            , ConvertData<RxPlatformDomainType>(kvp.Value));
                    }
                    foreach (var kvp in tempData.PortTypes)
                    {
                        if (!kvp.Value.valid)
                            continue;
                        RxMetaData.Instance.PortTypes.Add(kvp.Key
                            , ConvertData<RxPlatformPortType>(kvp.Value));
                    }


                    foreach (var kvp in tempData.StructTypes)
                    {
                        if (!kvp.Value.valid)
                            continue;
                        RxMetaData.Instance.StructTypes.Add(kvp.Key
                            , ConvertData<RxPlatformStructType>(kvp.Value));
                    }
                    foreach (var kvp in tempData.MapperTypes)
                    {
                        if (!kvp.Value.valid)
                            continue;
                        RxMetaData.Instance.MapperTypes.Add(kvp.Key
                            , ConvertData<RxPlatformMapperType>(kvp.Value));
                    }
                    foreach (var kvp in tempData.VariableTypes)
                    {
                        if (!kvp.Value.valid)
                            continue;
                        RxMetaData.Instance.VariableTypes.Add(kvp.Key
                            , ConvertData<RxPlatformVariableType>(kvp.Value));
                    }
                    foreach (var kvp in tempData.SourceTypes)
                    {
                        if (!kvp.Value.valid)
                            continue;
                        RxMetaData.Instance.SourceTypes.Add(kvp.Key
                            , ConvertData<RxPlatformSourceType>(kvp.Value));
                    }
                    foreach (var kvp in tempData.EventTypes)
                    {
                        if (!kvp.Value.valid)
                            continue;
                        RxMetaData.Instance.EventTypes.Add(kvp.Key
                            , ConvertData<RxPlatformEventType>(kvp.Value));

                    }
                    foreach (var kvp in tempData.FilterTypes)
                    {
                        if (!kvp.Value.valid)
                            continue;
                        RxMetaData.Instance.FilterTypes.Add(kvp.Key
                            , ConvertData<RxPlatformFilterType>(kvp.Value));
                    }

                    foreach (var kvp in tempData.DataTypes)
                    {
                        if (!kvp.Value.valid)
                            continue;
                        RxMetaData.Instance.DataTypes.Add(kvp.Key
                            , ConvertData(kvp.Value));
                    }
                }
                return true;
            }
            return false;
        }
        internal static LibraryPlatformTypes ParseAssembly(Assembly assembly, HostedPlatformLibrary hostLib)
        {
            LibraryPlatformTypes ret = new LibraryPlatformTypes
            {
                objectTypes = new List<RxNodeId>(),
                applicationTypes = new List<RxNodeId>(),
                domainTypes = new List<RxNodeId>(),
                portTypes = new List<RxNodeId>(),
                structTypes = new List<RxNodeId>(),
                mapperTypes = new List<RxNodeId>(),
                variableTypes = new List<RxNodeId>(),
                sourceTypes = new List<RxNodeId>(),
                eventTypes = new List<RxNodeId>(),
                filterTypes = new List<RxNodeId>(),
                dataTypes = new List<RxNodeId>()
            };

            PlatformTypeBuildData tempData = FillTypes(assembly.GetTypes(), out bool hadOne, hostLib);

            if (hadOne)
            {
                foreach (var algorithm in Algorithms)
                {
                    algorithm.FillTypes(tempData);
                }
                RxTypesGenerator.GeneratePlatformTypes(tempData, hostLib);
                RxAssemblyGenerator.GenerateAssembly(tempData, hostLib);
                lock (RxMetaData.Instance.TypesLock)
                {
                    RxMetaData.Instance.HostedLibraries.Add(assembly, hostLib);

                    foreach (var kvp in tempData.ObjectTypes)
                    {
                        if(!kvp.Value.valid)
                            continue;

                        ret.objectTypes.Add(kvp.Key);
                        RxMetaData.Instance.ObjectTypes.Add(kvp.Key
                            , ConvertData<RxPlatformObjectType>(kvp.Value));

                        if (kvp.Value.runtimeType && kvp.Value.runtimeConstructor != null)
                        {
                            byte[]? initialValues = null;
                            if (kvp.Value.defaultConstructor != null)
                            {
                                RxPlatformObjectRuntime? def = kvp.Value.defaultConstructor() as RxPlatformObjectRuntime;
                                if (def != null)
                                {
                                    MemoryStream ms = new MemoryStream();
                                    Utf8JsonWriter writer = new Utf8JsonWriter(ms);
                                    def.__rxStructSerialize(writer);
                                    initialValues = ms.ToArray();
                                    string debug = Encoding.UTF8.GetString(initialValues);
                                }
                            }
                            RuntimeConstructionData data = new RuntimeConstructionData
                            {
                                constructor = kvp.Value.runtimeConstructor,
                                startMethod = kvp.Value.startMethod,
                                stopMethod = kvp.Value.stopMethod,
                                initialValues = initialValues
                            };
                            RxMetaData.Instance.ObjectRuntimes.RegisteredConstructors.Add(kvp.Key, data);
                        }
                    }
                    foreach (var kvp in tempData.ApplicationTypes)
                    {
                        if (!kvp.Value.valid)
                            continue;
                        ret.applicationTypes.Add(kvp.Key);
                        RxMetaData.Instance.ApplicationTypes.Add(kvp.Key
                            , ConvertData<RxPlatformApplicationType>(kvp.Value));
                    }
                    foreach (var kvp in tempData.DomainTypes)
                    {
                        if (!kvp.Value.valid)
                            continue;
                        ret.domainTypes.Add(kvp.Key);
                        RxMetaData.Instance.DomainTypes.Add(kvp.Key
                            , ConvertData<RxPlatformDomainType>(kvp.Value));
                    }
                    foreach (var kvp in tempData.PortTypes)
                    {
                        if (!kvp.Value.valid)
                            continue;
                        ret.portTypes.Add(kvp.Key);
                        RxMetaData.Instance.PortTypes.Add(kvp.Key
                            , ConvertData<RxPlatformPortType>(kvp.Value));
                    }


                    foreach (var kvp in tempData.StructTypes)
                    {
                        if (!kvp.Value.valid)
                            continue;
                        ret.structTypes.Add(kvp.Key);
                        RxMetaData.Instance.StructTypes.Add(kvp.Key
                            , ConvertData<RxPlatformStructType>(kvp.Value));

                        if (kvp.Value.runtimeType && kvp.Value.runtimeConstructor != null)
                        {
                            byte[]? initialValues = null;
                            if (kvp.Value.defaultConstructor!=null)
                            {
                                RxPlatformStructRuntime? def = kvp.Value.defaultConstructor() as RxPlatformStructRuntime;
                                if(def!=null)
                                {
                                    MemoryStream ms = new MemoryStream();
                                    Utf8JsonWriter writer = new Utf8JsonWriter(ms);
                                    def.__rxStructSerialize(writer);
                                    initialValues = ms.ToArray();
                                }
                            }
                            RuntimeConstructionData data = new RuntimeConstructionData
                            {
                                constructor = kvp.Value.runtimeConstructor,
                                startMethod = kvp.Value.startMethod,
                                stopMethod = kvp.Value.stopMethod,
                                initialValues = initialValues
                            };
                            RxMetaData.Instance.StructRuntimes.RegisteredConstructors.Add(kvp.Key, data);
                        }
                    }
                    foreach (var kvp in tempData.MapperTypes)
                    {
                        if (!kvp.Value.valid)
                            continue;
                        ret.mapperTypes.Add(kvp.Key);
                        RxMetaData.Instance.MapperTypes.Add(kvp.Key
                            , ConvertData<RxPlatformMapperType>(kvp.Value));

                        if (kvp.Value.runtimeType && kvp.Value.runtimeConstructor != null)
                        {
                            byte[]? initialValues = null;
                            if (kvp.Value.defaultConstructor != null)
                            {
                                RxPlatformMapperRuntime? def = kvp.Value.defaultConstructor() as RxPlatformMapperRuntime;
                                if (def != null)
                                {
                                    MemoryStream ms = new MemoryStream();
                                    Utf8JsonWriter writer = new Utf8JsonWriter(ms);
                                    def.__rxStructSerialize(writer);
                                    initialValues = ms.ToArray();
                                }
                            }
                            RuntimeConstructionData data = new RuntimeConstructionData
                            {
                                constructor = kvp.Value.runtimeConstructor,
                                startMethod = kvp.Value.startMethod,
                                stopMethod = kvp.Value.stopMethod,
                                initialValues = initialValues
                            };
                            RxMetaData.Instance.MapperRuntimes.RegisteredConstructors.Add(kvp.Key, data);
                        }
                    }
                    foreach (var kvp in tempData.VariableTypes)
                    {
                        if (!kvp.Value.valid)
                            continue;
                        ret.variableTypes.Add(kvp.Key);
                        RxMetaData.Instance.VariableTypes.Add(kvp.Key
                            , ConvertData<RxPlatformVariableType>(kvp.Value));
                    }
                    foreach (var kvp in tempData.SourceTypes)
                    {
                        if (!kvp.Value.valid)
                            continue;
                        ret.sourceTypes.Add(kvp.Key);
                        RxMetaData.Instance.SourceTypes.Add(kvp.Key
                            , ConvertData<RxPlatformSourceType>(kvp.Value));

                        if (kvp.Value.runtimeType && kvp.Value.runtimeConstructor != null)
                        {
                            byte[]? initialValues = null;
                            if (kvp.Value.defaultConstructor != null)
                            {
                                RxPlatformSourceRuntime? def = kvp.Value.defaultConstructor() as RxPlatformSourceRuntime;
                                if (def != null)
                                {
                                    MemoryStream ms = new MemoryStream();
                                    Utf8JsonWriter writer = new Utf8JsonWriter(ms);
                                    def.__rxStructSerialize(writer);
                                    initialValues = ms.ToArray();
                                }
                            }
                            RuntimeConstructionData data = new RuntimeConstructionData
                            {
                                constructor = kvp.Value.runtimeConstructor,
                                startMethod = kvp.Value.startMethod,
                                stopMethod = kvp.Value.stopMethod,
                                sourceWriteMethods = kvp.Value.sourceWriteMethods,
                                initialValues = initialValues
                            };
                            RxMetaData.Instance.SourceRuntimes.RegisteredConstructors.Add(kvp.Key, data);
                        }
                    }
                    foreach (var kvp in tempData.EventTypes)
                    {
                        if (!kvp.Value.valid)
                            continue;
                        ret.eventTypes.Add(kvp.Key);
                        RxMetaData.Instance.EventTypes.Add(kvp.Key
                            , ConvertData<RxPlatformEventType>(kvp.Value));

                        if (kvp.Value.runtimeType && kvp.Value.runtimeConstructor != null)
                        {
                            byte[]? initialValues = null;
                            if (kvp.Value.defaultConstructor != null)
                            {
                                RxPlatformEventRuntime? def = kvp.Value.defaultConstructor() as RxPlatformEventRuntime;
                                if (def != null)
                                {
                                    MemoryStream ms = new MemoryStream();
                                    Utf8JsonWriter writer = new Utf8JsonWriter(ms);
                                    def.__rxStructSerialize(writer);
                                    initialValues = ms.ToArray();
                                }
                            }
                            RuntimeConstructionData data = new RuntimeConstructionData
                            {
                                constructor = kvp.Value.runtimeConstructor,
                                startMethod = kvp.Value.startMethod,
                                stopMethod = kvp.Value.stopMethod,
                                initialValues = initialValues
                            };
                            RxMetaData.Instance.EventRuntimes.RegisteredConstructors.Add(kvp.Key, data);
                        }
                    }
                    foreach (var kvp in tempData.FilterTypes)
                    {
                        if (!kvp.Value.valid)
                            continue;
                        ret.filterTypes.Add(kvp.Key);
                        RxMetaData.Instance.FilterTypes.Add(kvp.Key
                            , ConvertData<RxPlatformFilterType>(kvp.Value));
                    }

                    foreach (var kvp in tempData.DataTypes)
                    {
                        if (!kvp.Value.valid)
                            continue;
                        ret.dataTypes.Add(kvp.Key);
                        RxMetaData.Instance.DataTypes.Add(kvp.Key
                            , ConvertData(kvp.Value));
                    }
                }
            }
            return ret;
        }

        internal static readonly List<IRxMetaAlgorithm> Algorithms = new List<IRxMetaAlgorithm>
        {
            new RxInitialDataFill(),
            new RxItemsFill(),
            new RxSourcesFill(),
            new RxMappersFill(),
            new RxFiltersFill(),
            new RxPropertiesGetter(),
            new RxStructsGetter(),
            new RxOwnMethodsGetter(),
            new RxCallableMethodsGetter(),
            new RxSourceModelGetter(),
            new RxRelationsGetter(),
            new RxOwnRelationsGetter()
        };
        internal static readonly List<IRxMetaAlgorithm> SimplifiedAlgorithms = new List<IRxMetaAlgorithm>
        {
            new RxInitialDataFill(),
            new RxItemsFill(),
            new RxSourcesFill(),
            new RxMappersFill(),
            new RxFiltersFill(),
            new RxRelationsGetter()
        };

    }
}