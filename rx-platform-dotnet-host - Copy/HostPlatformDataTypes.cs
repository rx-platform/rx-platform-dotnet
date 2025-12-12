using ENSACO.RxPlatform.Attributes;
using ENSACO.RxPlatform.Hosting.Internal;
using ENSACO.RxPlatform.Hosting.Reflection;
using ENSACO.RxPlatform.Hosting.CallbackInterface;
using RxPlatform.Hosting.Interface;
using RxPlatform.Hosting.StaticRemains;
using System.Reflection;
using System.Security.AccessControl;
using ENSACO.RxPlatform.Model;

namespace ENSACO.RxPlatform.Hosting.Types
{
    internal class PlatformDataTypeData
    {
        internal ConstructorInfo? defaultConstructor;
        internal PropertyInfo[] properties = Array.Empty<PropertyInfo>();
        internal HashSet<object> sourceInstances = new HashSet<object>();
        internal RxPlatformDataType? attribute = null;
        internal string path = "";
        internal string name = "";
        internal RxNodeId id = RxNodeId.NullId;
        internal HashSet<RxNodeId> instances = new HashSet<RxNodeId>();
        internal bool definedType;

    }
    internal class LibraryPlatformDataTypes
    {
        Dictionary<Type, PlatformDataTypeData> platformDataTypes = new Dictionary<Type, PlatformDataTypeData>();  
        public LibraryPlatformDataTypes()
        {
        }
        internal bool InitializeType(Type objectType, HostedPlatformLibrary hostLib)
        {
            bool defined = objectType.GetCustomAttribute<RxPlatformDeclareAttribute>() != null;

            var attr = objectType.GetCustomAttribute<RxPlatformDataType>();
            if (attr != null)
            {
                InternalInitializeDataType(objectType, attr, defined);
                return true;
            }
            return false;
        }
        void InternalInitializeDataType(Type dataType, RxPlatformDataType attr, bool defined = false)
        {
            var platformDataTypeData = new PlatformDataTypeData
            {
                id = attr.NodeId,
                path = attr.Directory,
                name = string.IsNullOrEmpty(attr.Name) ? dataType.Name : attr.Name,
                definedType = defined,
                defaultConstructor = ReflectionHelpers.GetDefaultConstructor(dataType),
                properties = ReflectionHelpers.GetSimplePropertyInfos(dataType),
                attribute = attr
            };
            if(platformDataTypeData.defaultConstructor != null)
            {
                platformDataTypes[dataType] = platformDataTypeData;
                RxPlatformObject.Instance.WriteLogDebug("LibraryPlatformDataTypes.InitializeDataType", 100, $"DataType {dataType.FullName} detected.");
            }
            else
            {
                RxPlatformObject.Instance.WriteLogWarning("LibraryPlatformDataTypes.InitializeDataType", 100, $"Type {dataType.FullName} does not have a public default constructor. DataType will not be available.");
            }
        }

        internal unsafe void BuildPlatformTypes(uint version,string library, InitDataAPI api)
        {
            string libName = "";
            var idx = library.IndexOf(';');
            if (idx <= 0)
                throw new ArgumentException("Library id is not valid!!");

            if (api.BuildType == null)
                throw new Exception("BuildType function is not available in the API.");

            libName = library.Substring(0, idx);
            RxPlatformObject.Instance.WriteLogDebug("PlatformDataTypes.BuildPlatformTypes", 100, $"Building data types for library {library}...");
            foreach (var kvp in platformDataTypes)
            {
                var type = kvp.Key;
                var data = kvp.Value;
                if (data.attribute != null && data.defaultConstructor != null)
                {
                    var classDescription = "";
                    object? instance = data.defaultConstructor.Invoke(Array.Empty<object>());
                    string def = @" 
{
    ""def"": {
        ""items"": [
"
                    + @"
        ],
        ""overrides"": {},
        ""description"": """ + classDescription + @"""
    }
}
";
                    rx_node_id_struct parentId = CommonInterface.CreateNodeIdFromInt(HostPlatformIds.RX_CLASS_DATA_BASE_ID);

                    Type? baseType = type.BaseType;
                    if (baseType != null)
                    {
                        var attr = baseType.GetCustomAttribute<RxPlatformDataType>();
                        if(attr!=null && !attr.NodeId.IsNull())
                        {
                            CommonInterface.rx_destory_node_id(&parentId);
                            parentId = CommonInterface.CreateNodeIdFromRxNodeId(attr.NodeId);
                        }
                    }

                    string name = string.IsNullOrEmpty(data.attribute.Name) ? type.Name : data.attribute.Name;
                    string path = $"/sys/{RxPlatformObject.Instance.GetPluginName()}/{libName}"
                        + (string.IsNullOrEmpty(data.attribute.Directory) ? "" : "/" + data.attribute.Directory);
                    data.name = name;
                    data.path = path;
                    data.id= data.attribute.NodeId;
                    platformDataTypes[type] = data;
                    rx_node_id_struct id = CommonInterface.CreateNodeIdFromRxNodeId(data.id);

                    var result = api.BuildType(rx_item_type.rx_data_type
                        , library, &id, &parentId, name, path,version, 0, def
                        , 0, "");
                    var exception = CommonInterface.GetExceptionFromResult(&result);
                    CommonInterface.rx_destroy_result_struct(&result);
                    CommonInterface.rx_destory_node_id(&parentId);
                    CommonInterface.rx_destory_node_id(&id);
                  

                    if (exception != null)
                    {
                        RxPlatformObject.Instance.WriteLogError("LibraryPlatformDataTypes.BuildPlatformTypes", 200
                            , $"Error building DataType {path}/{name}: {exception.Message}");
                        throw exception;
                    }
                    else
                    {
                        RxPlatformObject.Instance.WriteLogTrace("LibraryPlatformDataTypes.BuildPlatformTypes", 100
                            , $"DataType {path}/{name} has been built.");
                    }
                }
            }
        }

        internal unsafe void DeletePlatformTypes(InitDataAPI api)
        {

            if (api.DeleteType == null)
                throw new Exception("BuildType function is not available in the API.");

            foreach (var kvp in platformDataTypes)
            {
                var type = kvp.Key;
                var data = kvp.Value;

                if (data.attribute != null && !data.id.IsNull() && data.definedType)
                {
                    rx_node_id_struct id = CommonInterface.CreateNodeIdFromRxNodeId(data.id);

                    var result = api.DeleteType(rx_item_type.rx_data_type, RxPlatformObject.Instance.GetPluginName(), &id);
                    var exception = CommonInterface.GetExceptionFromResult(&result);
                    CommonInterface.rx_destroy_result_struct(&result);
                    if (exception != null)
                    {
                        RxPlatformObject.Instance.WriteLogError("LibraryPlatformDataTypes.BuildPlatformTypes", 200
                            , $"Error deleting DataType {data.path}/{data.name}: {exception.Message}");
                        throw exception;
                    }
                    else
                    {
                        RxPlatformObject.Instance.WriteLogTrace("LibraryPlatformDataTypes.BuildPlatformTypes", 100
                            , $"DataType {data.path}/{data.name} has been deleted.");
                    }
                }
            }

            platformDataTypes.Clear();
        }
    }
}
