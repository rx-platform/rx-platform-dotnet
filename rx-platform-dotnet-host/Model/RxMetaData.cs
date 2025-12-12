using ENSACO.RxPlatform.Attributes;
using ENSACO.RxPlatform.Hosting.Construction;
using ENSACO.RxPlatform.Hosting.Internal;
using ENSACO.RxPlatform.Hosting.Model.Code;
using ENSACO.RxPlatform.Hosting.Model.Items;
using ENSACO.RxPlatform.Model;
using ENSACO.RxPlatform.Runtime;
using System.Reflection;
using System.Runtime.InteropServices;

namespace ENSACO.RxPlatform.Hosting.Model
{

    struct PlatformInstanceData
    {
        internal string path;
        internal rx_item_type rxType;
        internal RxNodeId id;
    }
    struct PlatformTypeMeta<T> where T : RxPlatformTypeAttribute
    {
        internal HostedPlatformLibrary? whose;

        internal string path;
        internal string name;
        internal RxNodeId id;
        internal bool definedType;
        internal bool runtimeType;

        internal MethodInfo? startMethod;
        internal MethodInfo? stopMethod;
    }


#pragma warning disable CS0649
    struct SourceWriteMethods
    {
        internal Action<bool, Action<Exception?>>? writeBoolMethod;
        internal Action<sbyte, Action<Exception?>>? writeSByteMethod;
        internal Action<short, Action<Exception?>>? writeShortMethod;
        internal Action<int, Action<Exception?>>? writeIntMethod;
        internal Action<long, Action<Exception?>>? writeLongMethod;
        internal Action<byte, Action<Exception?>>? writeByteMethod;
        internal Action<ushort, Action<Exception?>>? writeUShortMethod;
        internal Action<uint, Action<Exception?>>? writeUIntMethod;
        internal Action<ulong, Action<Exception?>>? writeULongMethod;
        internal Action<float, Action<Exception?>>? writeFloatMethod;
        internal Action<double, Action<Exception?>>? writeDoubleMethod;
        internal Action<Guid, Action<Exception?>>? writeUuidMethod;
        internal Action<string, Action<Exception?>>? writeStringMethod;
        internal Action<DateTime, Action<Exception?>>? writeDateTimeMethod;
        internal Tuple<Type, Action<object, Action<Exception?>>?> writeJsonMethod;
    }

#pragma warning restore CS0649

    internal struct PlatformRuntimeData
    {
        internal string path;
        internal RxNodeId nodeId;
        internal GCHandle objectRuntime;
        internal IntPtr nativeRuntimePtr;
        internal Action? startedMethod;
        internal Action? stoppedMethod;
        internal SourceWriteMethods sourceWriteMethods;
    }
    struct RuntimeConstructionData
    {
        internal Func<object?> constructor;
        internal MethodInfo? startMethod;
        internal MethodInfo? stopMethod;
        internal byte[]? initialValues;
        internal SourceWriteMethodData[]? sourceWriteMethods;
    }

    class PlatformTypeRuntime
    {
        public PlatformTypeRuntime() { }

        internal Dictionary<RxNodeId, RuntimeConstructionData> RegisteredConstructors = new Dictionary<RxNodeId, RuntimeConstructionData>();
        internal Dictionary<RxNodeId, PlatformRuntimeData> NodeIdDict = new Dictionary<RxNodeId, PlatformRuntimeData>();
        internal Dictionary<IntPtr, PlatformRuntimeData> NativeDict = new Dictionary<IntPtr, PlatformRuntimeData>();
      
    }

    class PlatformTypeData<T> where T : RxPlatformTypeAttribute
    {
        internal PlatformTypeMeta<T> Meta = new PlatformTypeMeta<T>();
    }

    struct SourceWriteMethodData
    {
        internal byte typeCode;
        internal MethodInfo methodInfo;
    }
    struct PlatformTypeBuildMeta<T> where T : RxPlatformTypeAttribute
    {
        internal Func<object?>? defaultConstructor;
        //internal ConstructorInfo? defaultConstructor;
        internal RxMetaItem[] items;
        internal RxRelationDataItem[] relations;
        internal RxMethodDataItem[] methods;
        internal RxFilterDataItem[] filters;
        internal RxMapperDataItem[] mappers;
        internal RxSourceDataItem[] sources;
        internal T? attribute;
        internal string path;
        internal string name;
        internal RxNodeId id;
        internal RxNodeId parentId;
        internal bool definedType;
        internal bool runtimeType;
        internal Func<object?>? runtimeConstructor;
        internal MethodInfo? startMethod;
        internal MethodInfo? stopMethod;
        internal RxPropertyCodeData[] definedProperties;
        internal RxStructCodeData[] definedStructs;
        internal RxOwnMethodCodeData[] definedMethods;
        internal RxCallableMethodCodeData[] callableMethods;
        internal RxOwnRelationCodeData[] definedRelations;
        internal HostedPlatformLibrary? whose;
        internal bool valid;
        internal Type? type;
        internal string? codeNamespace;
        internal string? runtimeConnections;
        internal string? initialValues;
        internal string? relationValues;
        internal string? codeInfo;
        internal SourceWriteMethodData[]? sourceWriteMethods;
    }
    struct PlatformDataTypeBuildMeta
    {
        internal Func<object?>? defaultConstructor;
        //internal ConstructorInfo? defaultConstructor;
        internal RxDataItem[] items;
        internal RxPlatformDataType? attribute;
        internal string path;
        internal string name;
        internal RxNodeId id;
        internal RxNodeId parentId;
        internal bool definedType;
        internal bool runtimeType;
        internal HostedPlatformLibrary? whose;
        internal bool valid;
        internal Type? type;
    }

    class PlatformTypeBuildData
    {
        internal Dictionary<RxNodeId, PlatformTypeBuildMeta<RxPlatformObjectType>> ObjectTypes
            = new Dictionary<RxNodeId, PlatformTypeBuildMeta<RxPlatformObjectType>>();
        internal Dictionary<RxNodeId, PlatformTypeBuildMeta<RxPlatformApplicationType>> ApplicationTypes
            = new Dictionary<RxNodeId, PlatformTypeBuildMeta<RxPlatformApplicationType>>();
        internal Dictionary<RxNodeId, PlatformTypeBuildMeta<RxPlatformDomainType>> DomainTypes
            = new Dictionary<RxNodeId, PlatformTypeBuildMeta<RxPlatformDomainType>>();
        internal Dictionary<RxNodeId, PlatformTypeBuildMeta<RxPlatformPortType>> PortTypes
            = new Dictionary<RxNodeId, PlatformTypeBuildMeta<RxPlatformPortType>>();

        internal Dictionary<RxNodeId, PlatformTypeBuildMeta<RxPlatformStructType>> StructTypes
            = new Dictionary<RxNodeId, PlatformTypeBuildMeta<RxPlatformStructType>>();
        internal Dictionary<RxNodeId, PlatformTypeBuildMeta<RxPlatformMapperType>> MapperTypes
            = new Dictionary<RxNodeId, PlatformTypeBuildMeta<RxPlatformMapperType>>();
        internal Dictionary<RxNodeId, PlatformTypeBuildMeta<RxPlatformVariableType>> VariableTypes
            = new Dictionary<RxNodeId, PlatformTypeBuildMeta<RxPlatformVariableType>>();
        internal Dictionary<RxNodeId, PlatformTypeBuildMeta<RxPlatformSourceType>> SourceTypes
            = new Dictionary<RxNodeId, PlatformTypeBuildMeta<RxPlatformSourceType>>();
        internal Dictionary<RxNodeId, PlatformTypeBuildMeta<RxPlatformEventType>> EventTypes
            = new Dictionary<RxNodeId, PlatformTypeBuildMeta<RxPlatformEventType>>();
        internal Dictionary<RxNodeId, PlatformTypeBuildMeta<RxPlatformFilterType>> FilterTypes
            = new Dictionary<RxNodeId, PlatformTypeBuildMeta<RxPlatformFilterType>>();

        internal Dictionary<RxNodeId, PlatformDataTypeBuildMeta> DataTypes
            = new Dictionary<RxNodeId, PlatformDataTypeBuildMeta>();

    }

    internal interface IRxMetaAlgorithm
    {
        void FillTypes(PlatformTypeBuildData data);
    }

    internal class RxMetaData
    {
        static private RxMetaData? instance = null;
        static internal RxMetaData Instance
        {
            get
            {
                if (instance == null)
                    instance = new RxMetaData();
                return instance;
            }
        }

        internal Dictionary<Assembly, HostedPlatformLibrary> HostedLibraries
            = new Dictionary<Assembly, HostedPlatformLibrary>();

        internal Dictionary<HostedPlatformLibrary, HashSet<PlatformInstanceData>> RuntimeObjects
            = new Dictionary<HostedPlatformLibrary, HashSet<PlatformInstanceData>>();

        internal Dictionary<RxNodeId, RxRutimeConstructData> RuntimeConstruction = new Dictionary<RxNodeId, RxRutimeConstructData>();


        internal Dictionary<object, PlatformInstanceData> RegisteredObjects = new Dictionary<object, PlatformInstanceData>();

        internal object TypesLock = new object();
        internal Dictionary<RxNodeId, PlatformTypeData<RxPlatformObjectType>> ObjectTypes
            = new Dictionary<RxNodeId, PlatformTypeData<RxPlatformObjectType>>();
        internal PlatformTypeRuntime ObjectRuntimes
            = new PlatformTypeRuntime();

        internal Dictionary<RxNodeId, PlatformTypeData<RxPlatformApplicationType>> ApplicationTypes
            = new Dictionary<RxNodeId, PlatformTypeData<RxPlatformApplicationType>>();
        internal Dictionary<RxNodeId, PlatformTypeData<RxPlatformDomainType>> DomainTypes
            = new Dictionary<RxNodeId, PlatformTypeData<RxPlatformDomainType>>();
        internal Dictionary<RxNodeId, PlatformTypeData<RxPlatformPortType>> PortTypes
            = new Dictionary<RxNodeId, PlatformTypeData<RxPlatformPortType>>();


        internal Dictionary<RxNodeId, PlatformTypeData<RxPlatformStructType>> StructTypes
            = new Dictionary<RxNodeId, PlatformTypeData<RxPlatformStructType>>();
        internal PlatformTypeRuntime StructRuntimes
            = new PlatformTypeRuntime();

        internal Dictionary<RxNodeId, PlatformTypeData<RxPlatformMapperType>> MapperTypes
            = new Dictionary<RxNodeId, PlatformTypeData<RxPlatformMapperType>>();
        internal PlatformTypeRuntime MapperRuntimes
            = new PlatformTypeRuntime();

        internal Dictionary<RxNodeId, PlatformTypeData<RxPlatformVariableType>> VariableTypes
            = new Dictionary<RxNodeId, PlatformTypeData<RxPlatformVariableType>>();

        internal Dictionary<RxNodeId, PlatformTypeData<RxPlatformSourceType>> SourceTypes
            = new Dictionary<RxNodeId, PlatformTypeData<RxPlatformSourceType>>();
        internal PlatformTypeRuntime SourceRuntimes
            = new PlatformTypeRuntime();

        internal Dictionary<RxNodeId, PlatformTypeData<RxPlatformEventType>> EventTypes
            = new Dictionary<RxNodeId, PlatformTypeData<RxPlatformEventType>>();
        internal PlatformTypeRuntime EventRuntimes
            = new   PlatformTypeRuntime();

        internal Dictionary<RxNodeId, PlatformTypeData<RxPlatformFilterType>> FilterTypes
            = new Dictionary<RxNodeId, PlatformTypeData<RxPlatformFilterType>>();


        internal Dictionary<RxNodeId, PlatformTypeData<RxPlatformDataType>> DataTypes
            = new Dictionary<RxNodeId, PlatformTypeData<RxPlatformDataType>>();
    }
}