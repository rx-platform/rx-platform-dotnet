using ENSACO.RxPlatform.Attributes;
using ENSACO.RxPlatform.Hosting.Internal;
using ENSACO.RxPlatform.Hosting.Model.Code;
using ENSACO.RxPlatform.Hosting.Model.Items;
using ENSACO.RxPlatform.Model;
using ENSACO.RxPlatform.Runtime;
using System.Reflection;
using System.Runtime.InteropServices;

namespace ENSACO.RxPlatform.Hosting.Model
{


    internal struct PlatformRuntimeData<T>
    {
        internal RxNodeId nodeId;
       // internal T? objectRuntime;
        internal IntPtr nativeRuntimePtr;
        internal Action? startedMethod;
        internal Action? stoppedMethod;
    }
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



    struct RuntimeConstructionData
    {
        internal Func<object?> constructor;
        internal MethodInfo? startMethod;
        internal MethodInfo? stopMethod;
    }

    class PlatformTypeRuntime<T> where T : RxPlatformRuntimeBase
    {
        public PlatformTypeRuntime() { }

        internal Dictionary<RxNodeId, RuntimeConstructionData> RegisteredConstructors = new Dictionary<RxNodeId, RuntimeConstructionData>();
        internal Dictionary<RxNodeId, PlatformRuntimeData<T>> NodeIdDict = new Dictionary<RxNodeId, PlatformRuntimeData<T>>();
        internal Dictionary<IntPtr, PlatformRuntimeData<T>> NativeDict = new Dictionary<IntPtr, PlatformRuntimeData<T>>();
        internal Dictionary<T, PlatformRuntimeData<T>> ManagedDict  = new Dictionary<T, PlatformRuntimeData<T>>();

    }

    class PlatformTypeData<T> where T : RxPlatformTypeAttribute
    {
        internal PlatformTypeMeta<T> Meta = new PlatformTypeMeta<T>();
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
        internal RxOwnMethodCodeData[] definedMethods;
        internal RxCallableMethodCodeData[] callableMethods;
        internal RxOwnRelationCodeData[] definedRelations;
        internal HostedPlatformLibrary? whose;
        internal bool valid;
        internal Type? type;
        internal string? codeNamespace;
        internal string? runtimeConnections;
        internal string? initialValues;
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

        internal Dictionary<RxNodeId, PlatformTypeBuildMeta<RxPlatformStruct>> StructTypes
            = new Dictionary<RxNodeId, PlatformTypeBuildMeta<RxPlatformStruct>>();
        internal Dictionary<RxNodeId, PlatformTypeBuildMeta<RxPlatformMapper>> MapperTypes
            = new Dictionary<RxNodeId, PlatformTypeBuildMeta<RxPlatformMapper>>();
        internal Dictionary<RxNodeId, PlatformTypeBuildMeta<RxPlatformVariable>> VariableTypes
            = new Dictionary<RxNodeId, PlatformTypeBuildMeta<RxPlatformVariable>>();
        internal Dictionary<RxNodeId, PlatformTypeBuildMeta<RxPlatformSource>> SourceTypes
            = new Dictionary<RxNodeId, PlatformTypeBuildMeta<RxPlatformSource>>();
        internal Dictionary<RxNodeId, PlatformTypeBuildMeta<RxPlatformEvent>> EventTypes
            = new Dictionary<RxNodeId, PlatformTypeBuildMeta<RxPlatformEvent>>();
        internal Dictionary<RxNodeId, PlatformTypeBuildMeta<RxPlatformFilter>> FilterTypes
            = new Dictionary<RxNodeId, PlatformTypeBuildMeta<RxPlatformFilter>>();

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

        internal Dictionary<object, PlatformInstanceData> RegisteredObjects = new Dictionary<object, PlatformInstanceData>();

        internal object TypesLock = new object();
        internal Dictionary<RxNodeId, PlatformTypeData<RxPlatformObjectType>> ObjectTypes
            = new Dictionary<RxNodeId, PlatformTypeData<RxPlatformObjectType>>();
        internal PlatformTypeRuntime<RxPlatformObjectRuntime> ObjectRuntimes
            = new PlatformTypeRuntime<RxPlatformObjectRuntime>();

        internal Dictionary<RxNodeId, PlatformTypeData<RxPlatformApplicationType>> ApplicationTypes
            = new Dictionary<RxNodeId, PlatformTypeData<RxPlatformApplicationType>>();
        internal Dictionary<RxNodeId, PlatformTypeData<RxPlatformDomainType>> DomainTypes
            = new Dictionary<RxNodeId, PlatformTypeData<RxPlatformDomainType>>();
        internal Dictionary<RxNodeId, PlatformTypeData<RxPlatformPortType>> PortTypes
            = new Dictionary<RxNodeId, PlatformTypeData<RxPlatformPortType>>();


        internal Dictionary<RxNodeId, PlatformTypeData<RxPlatformStruct>> StructTypes
            = new Dictionary<RxNodeId, PlatformTypeData<RxPlatformStruct>>();

        internal Dictionary<RxNodeId, PlatformTypeData<RxPlatformMapper>> MapperTypes
            = new Dictionary<RxNodeId, PlatformTypeData<RxPlatformMapper>>();
        internal PlatformTypeRuntime<RxPlatformMapperRuntime> MapperRuntimes
            = new PlatformTypeRuntime<RxPlatformMapperRuntime>();

        internal Dictionary<RxNodeId, PlatformTypeData<RxPlatformVariable>> VariableTypes
            = new Dictionary<RxNodeId, PlatformTypeData<RxPlatformVariable>>();

        internal Dictionary<RxNodeId, PlatformTypeData<RxPlatformSource>> SourceTypes
            = new Dictionary<RxNodeId, PlatformTypeData<RxPlatformSource>>();
        internal PlatformTypeRuntime<RxPlatformSourceRuntime> SourceRuntimes
            = new PlatformTypeRuntime<RxPlatformSourceRuntime>();

        internal Dictionary<RxNodeId, PlatformTypeData<RxPlatformEvent>> EventTypes
            = new Dictionary<RxNodeId, PlatformTypeData<RxPlatformEvent>>();
        internal PlatformTypeRuntime<RxPlatformEventRuntime> EventRuntimes
            = new PlatformTypeRuntime<RxPlatformEventRuntime>();

        internal Dictionary<RxNodeId, PlatformTypeData<RxPlatformFilter>> FilterTypes
            = new Dictionary<RxNodeId, PlatformTypeData<RxPlatformFilter>>();


        internal Dictionary<RxNodeId, PlatformTypeData<RxPlatformDataType>> DataTypes
            = new Dictionary<RxNodeId, PlatformTypeData<RxPlatformDataType>>();
    }
}