using ENSACO.RxPlatform.Attributes;
using ENSACO.RxPlatform.Hosting.Common;
using ENSACO.RxPlatform.Hosting.Interface;
using ENSACO.RxPlatform.Hosting.Internal;
using ENSACO.RxPlatform.Hosting.Model.Items;
using ENSACO.RxPlatform.Model;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace ENSACO.RxPlatform.Hosting.Model
{
    internal static class RxTypesGenerator
    {
        static bool GenerateDataType(
            string name
            , string path
            , RxNodeId nodeId
            , RxNodeId parentNodeId
            , RxDataItem[] items
            , string overrides
            , HostedPlatformLibrary lib)
        {

            RxPlatformObject.Instance.WriteLogDebug("RxTypesGenerator", 100
                , $"Building type {path}/{name}.");

            if (PlatformHostMain.api.BuildType == null)
            {
                RxPlatformObject.Instance.WriteLogError("RxTypesGenerator", 200
                    , $"Error building Type {path}/{name}: API not initialized.");
                return false;
            }
            var classDescription = "";

            string def = @" 
{
    ""def"": {
        ""items"": [
";
            bool first = true;
            foreach (var item in items)
            {
                string itemsStr = JsonSerializer.Serialize(item, item.GetType(), PlatformHostMain.JsonContext);
                if (!first)
                {
                    def += ",";
                }
                else
                {
                    first = false;
                }
                def += itemsStr;
            }
            def += $@"]
,
        ""overrides"": {overrides},
        ""description"": """ + classDescription + @"""
    }
}
";
            

            Exception? exception = null;
            unsafe
            {
                rx_node_id_struct id = CommonInterface.CreateNodeIdFromRxNodeId(nodeId);
                rx_node_id_struct parentId = parentNodeId.IsNull() ?
                    CommonInterface.CreateNodeIdFromInt(HostPlatformIds.RX_CLASS_DATA_BASE_ID) :
                    CommonInterface.CreateNodeIdFromRxNodeId(parentNodeId);
                var result = PlatformHostMain.api.BuildType(rx_item_type.rx_data_type
                    , lib.GetPluginName(), &id, &parentId, name, path, 0x10001, 0, def
                    , 0
                    , "", "");
                exception = CommonInterface.GetExceptionFromResult(&result);
                CommonInterface.rx_destroy_result_struct(&result);
                CommonInterface.rx_destory_node_id(&parentId);
                CommonInterface.rx_destory_node_id(&id);
            }


            if (exception != null)
            {
                RxPlatformObject.Instance.WriteLogError("RxTypesGenerator", 200
                    , $"Error building Type {path}/{name}: {exception.Message}");

                return false;
            }
            else
            {
                RxPlatformObject.Instance.WriteLogTrace("RxTypesGenerator", 100
                    , $"Type {path}/{name} has been built.");

            }
            return true;
        }
        static bool GenerateType(
            string codeInfo
            , rx_item_type itemType
            , string name
            , string path
            , RxNodeId nodeId
            , RxNodeId parentNodeId
            , uint defaultParent
            , string overrides
            , RxMetaItem[] items
            , RxMethodDataItem[] methods
            , RxDisplayDataItem[] displays
            , RxSourceDataItem[] sources
            , RxMapperDataItem[] mappers
            , RxRelationDataItem[] relations
            , string connections
            , HostedPlatformLibrary lib)
        {

            RxPlatformObject.Instance.WriteLogDebug("RxTypesGenerator", 100
                , $"Building type {path}/{name}.");

            if (PlatformHostMain.api.BuildType == null)
            {
                RxPlatformObject.Instance.WriteLogError("RxTypesGenerator", 200
                    , $"Error building Type {path}/{name}: API not initialized.");
                return false;
            }

            var classDescription = "";

            string def = @" 
{
    ""def"": {
        ""sealed"": false,
        ""abstract"": false,
        ""items"": [
";
            bool first = true;
            foreach (var item in items)
            {
                string itemsStr = JsonSerializer.Serialize(item, item.GetType(), PlatformHostMain.JsonContext);
                if (!first)
                {
                    def += ",";
                }
                else
                {
                    first = false;
                }
                def += itemsStr;
            }
            def += $@"]
,
        ""overrides"": {overrides},
        ""description"": """ + classDescription + @""",
        ""access"": {
            ""roles"": []
        },
        ""sources"": 
";
            def+= JsonSerializer.Serialize(sources, PlatformHostMain.JsonContext);
            def += @"
,
        ""mappers"":  
";
            def+= JsonSerializer.Serialize(mappers, PlatformHostMain.JsonContext);
            def += @"
,
        ""constructable"": true,
        ""relations"": 
";
            def += JsonSerializer.Serialize(relations, PlatformHostMain.JsonContext);
            def += @"
,
        ""programs"": [],
        ""methods"": 
";
            string methodsStr = JsonSerializer.Serialize(methods, PlatformHostMain.JsonContext);
            def += methodsStr;

            def += @"
,
        ""displays"": [] 
";
            def+= JsonSerializer.Serialize(displays, PlatformHostMain.JsonContext);
            def += @"
}
";

            Exception? exception = null;
            unsafe
            {
                rx_node_id_struct id = CommonInterface.CreateNodeIdFromRxNodeId(nodeId);
                rx_node_id_struct parentId = parentNodeId.IsNull() ?
                    CommonInterface.CreateNodeIdFromInt(defaultParent) :
                    CommonInterface.CreateNodeIdFromRxNodeId(parentNodeId);
                var result = PlatformHostMain.api.BuildType(itemType
                    , lib.GetPluginName(), &id, &parentId, name, path, 0x10001, 0, def
                    , !string.IsNullOrEmpty(codeInfo) ? 1 : 0
                    , connections, codeInfo);
                exception = CommonInterface.GetExceptionFromResult(&result);
                CommonInterface.rx_destroy_result_struct(&result);
                CommonInterface.rx_destory_node_id(&parentId);
                CommonInterface.rx_destory_node_id(&id);
            }


            if (exception != null)
            {
                RxPlatformObject.Instance.WriteLogError("RxTypesGenerator", 200
                    , $"Error building Type {path}/{name}: {exception.Message}");
                
                return false;
            }
            else
            {
                RxPlatformObject.Instance.WriteLogTrace("RxTypesGenerator", 100
                    , $"Type {path}/{name} has been built.");

            }
            return true;
        }
        static bool GenerateObjectType(PlatformTypeBuildMeta<RxPlatformObjectType> type, HostedPlatformLibrary lib)
        {

            if (PlatformHostMain.api.BuildType == null)
                throw new Exception("BuildType function is not available in the API.");

            if (type.attribute != null && type.defaultConstructor != null && type.definedType)
            {
                string codeInfo = "";
                string overrides = "{}";
                if(type.defaultConstructor != null)
                {
                    var tempObj = type.defaultConstructor.Invoke();
                    if (tempObj != null)
                    {
                        if(type.runtimeType)
                            codeInfo = type.codeInfo ?? "";
                        overrides = JsonSerializer.Serialize(tempObj, tempObj.GetType(), PlatformHostMain.JsonContext);
                    }
                }
                if (GenerateType(
                    codeInfo
                    , rx_item_type.rx_object_type
                    , type.name
                    , type.path
                    , type.id
                    , type.parentId
                    , HostPlatformIds.RX_CLASS_OBJECT_BASE_ID
                    , overrides
                    , type.items
                    , type.methods
                    , type.displays
                    , type.sources
                    , type.mappers
                    , type.relations
                    , type.runtimeConnections + "|" + type.initialValues + "|" + type.relationValues
                    , lib))
                    return false;//no changes

                type.valid = false;
            }
            return true;
        }
        static bool GenerateApplicationType(PlatformTypeBuildMeta<RxPlatformApplicationType> type, HostedPlatformLibrary lib)
        {

            if (PlatformHostMain.api.BuildType == null)
                throw new Exception("BuildType function is not available in the API.");

            if (type.attribute != null && type.defaultConstructor != null && type.definedType)
            {
                string overrides = "{}";
                if (type.defaultConstructor != null)
                {
                    var tempObj = type.defaultConstructor.Invoke();
                    if (tempObj != null)
                    {
                        overrides = JsonSerializer.Serialize(tempObj, tempObj.GetType(), PlatformHostMain.JsonContext);
                    }
                }
                if (GenerateType(
                    ""
                    , rx_item_type.rx_application_type
                    , type.name
                    , type.path
                    , type.id
                    , type.parentId
                    , HostPlatformIds.RX_CLASS_APPLICATION_BASE_ID
                    , overrides
                    , type.items
                    , type.methods
                    , type.displays
                    , type.sources
                    , type.mappers
                    , type.relations
                    , type.runtimeConnections + "|" + type.initialValues + "|" + type.relationValues
                    , lib))
                    return false;//no changes

                type.valid = false;
            }
            return true;
        }
        static bool GeneratePortType(PlatformTypeBuildMeta<RxPlatformPortType> type, HostedPlatformLibrary lib)
        {

            if (PlatformHostMain.api.BuildType == null)
                throw new Exception("BuildType function is not available in the API.");

            if (type.attribute != null && type.defaultConstructor != null && type.definedType)
            {
                string overrides = "{}";
                if (type.defaultConstructor != null)
                {
                    var tempObj = type.defaultConstructor.Invoke();
                    if (tempObj != null)
                    {
                        overrides = JsonSerializer.Serialize(tempObj, tempObj.GetType(), PlatformHostMain.JsonContext);
                    }
                }
                if (GenerateType(
                    ""
                    , rx_item_type.rx_port_type
                    , type.name
                    , type.path
                    , type.id
                    , type.parentId
                    , HostPlatformIds.RX_CLASS_PORT_BASE_ID
                    , overrides
                    , type.items
                    , type.methods
                    , type.displays
                    , type.sources
                    , type.mappers
                    , type.relations
                    , type.runtimeConnections + "|" + type.initialValues + "|" + type.relationValues
                    , lib))
                    return false;//no changes

                type.valid = false;
            }
            return true;
        }
        static bool GenerateDomainType(PlatformTypeBuildMeta<RxPlatformDomainType> type, HostedPlatformLibrary lib)
        {

            if (PlatformHostMain.api.BuildType == null)
                throw new Exception("BuildType function is not available in the API.");

            if (type.attribute != null && type.defaultConstructor != null && type.definedType)
            {
                string overrides = "{}";
                if (type.defaultConstructor != null)
                {
                    var tempObj = type.defaultConstructor.Invoke();
                    if (tempObj != null)
                    {
                        overrides = JsonSerializer.Serialize(tempObj, tempObj.GetType(), PlatformHostMain.JsonContext);
                    }
                }
                if (GenerateType(
                    ""
                    , rx_item_type.rx_domain_type
                    , type.name
                    , type.path
                    , type.id
                    , type.parentId
                    , HostPlatformIds.RX_CLASS_DOMAIN_BASE_ID
                    , overrides
                    , type.items
                    , type.methods
                    , type.displays
                    , type.sources
                    , type.mappers
                    , type.relations
                    , type.runtimeConnections + "|" + type.initialValues + "|" + type.relationValues
                    , lib))
                    return false;//no changes

                type.valid = false;
            }
            return true;
        }
        static bool GenerateDataType(PlatformDataTypeBuildMeta type, HostedPlatformLibrary lib)
        {
            if (PlatformHostMain.api.BuildType == null)
                throw new Exception("BuildType function is not available in the API.");

            if (type.attribute != null && type.defaultConstructor != null && type.definedType)
            {
                string overrides = "{}";
                if (type.defaultConstructor != null)
                {
                    var tempObj = type.defaultConstructor.Invoke();
                    if (tempObj != null)
                    {
                        overrides = JsonSerializer.Serialize(tempObj, tempObj.GetType(), PlatformHostMain.JsonContext);
                    }
                }
                if (GenerateDataType(
                    type.name
                    , type.path
                    , type.id
                    , type.parentId
                    , type.items
                    , overrides
                    , lib))
                    return false;//no changes

                type.valid = false;
            }
            return true;
        }
        static void GenerateDataTypes(Dictionary<RxNodeId, PlatformDataTypeBuildMeta> data, HostedPlatformLibrary lib)
        {
            foreach (var kvp in data)
            {
                var objType = kvp.Value;
                if (!objType.valid)
                    continue;
                if (!objType.definedType)
                    continue;
                if(GenerateDataType(objType, lib))
                    data[kvp.Key] = objType;
            }
        }
        static void GenerateObjectTypes(Dictionary<RxNodeId, PlatformTypeBuildMeta<RxPlatformObjectType>> data, HostedPlatformLibrary lib)
        {
            foreach (var kvp in data)
            {
                var objType = kvp.Value;
                if (!objType.valid)
                    continue;
                if (!objType.definedType)
                    continue;
                if(GenerateObjectType(objType, lib))
                    data[kvp.Key] = objType;
            }
        }
        static void GenerateApplicationTypes(Dictionary<RxNodeId, PlatformTypeBuildMeta<RxPlatformApplicationType>> data, HostedPlatformLibrary lib)
        {
            foreach (var kvp in data)
            {
                var appType = kvp.Value;
                if (!appType.valid)
                    continue;
                if (!appType.definedType)
                    continue;
                if (GenerateApplicationType(appType, lib))
                    data[kvp.Key] = appType;
            }
        }
        static void GeneratePortTypes(Dictionary<RxNodeId, PlatformTypeBuildMeta<RxPlatformPortType>> data, HostedPlatformLibrary lib)
        {
            foreach (var kvp in data)
            {
                var portType = kvp.Value;
                if (!portType.valid)
                    continue;
                if (!portType.definedType)
                    continue;
                if (GeneratePortType(portType, lib))
                    data[kvp.Key] = portType;
            }
        }
        static void GenerateDomainTypes(Dictionary<RxNodeId, PlatformTypeBuildMeta<RxPlatformDomainType>> data, HostedPlatformLibrary lib)
        {
            foreach (var kvp in data)
            {
                var domainType = kvp.Value;
                if (!domainType.valid)
                    continue;
                if (!domainType.definedType)
                    continue;
                if (GenerateDomainType(domainType, lib))
                    data[kvp.Key] = domainType;
            }
        }

        static bool GenerateSimpleType(
            bool runtimeType
            , rx_item_type itemType
            , string name
            , string path
            , RxNodeId nodeId
            , RxNodeId parentNodeId
            , uint defaultParent
            , string overrides
            , RxMetaItem[] items
            , RxSourceDataItem[]? sources
            , RxMapperDataItem[]? mappers
            , RxFilterDataItem[]? filters
            , string connections
            , HostedPlatformLibrary lib)
        {


            RxPlatformObject.Instance.WriteLogDebug("RxTypesGenerator", 100
                , $"Building type {path}/{name}.");

            if (PlatformHostMain.api.BuildType == null)
            {
                RxPlatformObject.Instance.WriteLogError("RxTypesGenerator", 200
                    , $"Error building Type {path}/{name}: API not initialized.");
                return false;
            }

            var classDescription = "";

            string def = @" 
{
    ""def"": {
        ""sealed"": false,
        ""abstract"": false,
        ""items"": [
";
            bool first = true;
            foreach (var item in items)
            {
                string itemsStr = JsonSerializer.Serialize(item, item.GetType(), PlatformHostMain.JsonContext);
                if (!first)
                {
                    def += ",";
                }
                else
                {
                    first = false;
                }
                def += itemsStr;
            }
            def += @$"]
,
        ""overrides"": {overrides},
        ""access"": {{
            ""roles"": []
        }},";
            if(mappers!= null)
            {
                def += @"
        ""mappers"": 
";
                string mapperStr = JsonSerializer.Serialize(mappers, PlatformHostMain.JsonContext);
                def += mapperStr + ",";
            }
            if (filters != null)
            {
                def += @"
        ""filters"": 
";
                string filterStr = JsonSerializer.Serialize(filters, PlatformHostMain.JsonContext);
                def += filterStr + ",";
            }

            if (sources != null)
            {
                def += @"
        ""sources"": 
";
                string sourceStr = JsonSerializer.Serialize(sources, PlatformHostMain.JsonContext);
                def += sourceStr + ",";
            }
            def += @"
        ""description"": """ + classDescription + @"""
    }
}
";

            Exception? exception = null;
            unsafe
            {
                rx_node_id_struct id = CommonInterface.CreateNodeIdFromRxNodeId(nodeId);
                rx_node_id_struct parentId = parentNodeId.IsNull() ?
                    CommonInterface.CreateNodeIdFromInt(defaultParent) :
                    CommonInterface.CreateNodeIdFromRxNodeId(parentNodeId);
                var result = PlatformHostMain.api.BuildType(itemType
                    , lib.GetPluginName(), &id, &parentId, name, path, 0x10001, 0, def
                    , runtimeType ? 1 : 0
                    , connections, "");
                exception = CommonInterface.GetExceptionFromResult(&result);
                CommonInterface.rx_destroy_result_struct(&result);
                CommonInterface.rx_destory_node_id(&parentId);
                CommonInterface.rx_destory_node_id(&id);
            }


            if (exception != null)
            {
                RxPlatformObject.Instance.WriteLogError("RxTypesGenerator", 200
                    , $"Error building Type {path}/{name}: {exception.Message}");

                return false;
            }
            else
            {
                RxPlatformObject.Instance.WriteLogTrace("RxTypesGenerator", 100
                    , $"Type {path}/{name} has been built.");

            }
            return true;
        }

        static bool GenerateSourceType(PlatformTypeBuildMeta<RxPlatformSourceType> type, HostedPlatformLibrary lib)
        {

            if (PlatformHostMain.api.BuildType == null)
                throw new Exception("BuildType function is not available in the API.");

            if (type.attribute != null && type.defaultConstructor != null && type.definedType)
            {
                string overrides = "{}";
                if (type.defaultConstructor != null)
                {
                    var tempObj = type.defaultConstructor.Invoke();
                    if (tempObj != null)
                    {
                        overrides = JsonSerializer.Serialize(tempObj, tempObj.GetType(), PlatformHostMain.JsonContext);
                    }
                }
                if (GenerateSimpleType(
                    type.runtimeType
                    , rx_item_type.rx_source_type
                    , type.name
                    , type.path
                    , type.id
                    , type.parentId
                    , HostPlatformIds.RX_DOTNET_SOURCE_TYPE_ID
                    , overrides
                    , type.items
                    , null //type.sources
                    , null //type.mappers
                    , type.filters
                    , type.runtimeConnections + "|" + type.initialValues
                    , lib))
                    return false;//no changes

                type.valid = false;
            }
            return true;
        }

        static bool GenerateDisplayType(PlatformTypeBuildMeta<RxPlatformDisplayType> type, HostedPlatformLibrary lib)
        {

            if (PlatformHostMain.api.BuildType == null)
                throw new Exception("BuildType function is not available in the API.");

            if (type.attribute != null && type.defaultConstructor != null && type.definedType)
            {
                string overrides = "{}";
                if (type.defaultConstructor != null)
                {
                    var tempObj = type.defaultConstructor.Invoke();
                    if (tempObj != null)
                    {
                        overrides = JsonSerializer.Serialize(tempObj, tempObj.GetType(), PlatformHostMain.JsonContext);
                    }
                }
                if (GenerateSimpleType(
                    type.runtimeType
                    , rx_item_type.rx_source_type
                    , type.name
                    , type.path
                    , type.id
                    , type.parentId
                    , HostPlatformIds.RX_DOTNET_SOURCE_TYPE_ID
                    , overrides
                    , type.items
                    , null //type.sources
                    , null //type.mappers
                    , type.filters
                    , type.runtimeConnections + "|" + type.initialValues
                    , lib))
                    return false;//no changes

                type.valid = false;
            }
            return true;
        }
        static void GenerateSourceTypes(Dictionary<RxNodeId, PlatformTypeBuildMeta<RxPlatformSourceType>> data, HostedPlatformLibrary lib)
        {
            foreach (var kvp in data)
            {
                var sourceType = kvp.Value;
                if (!sourceType.valid)
                    continue;
                if (!sourceType.definedType)
                    continue;
                if (GenerateSourceType(sourceType, lib))
                    data[kvp.Key] = sourceType;
            }
        }

        static void GenerateDisplayTypes(Dictionary<RxNodeId, PlatformTypeBuildMeta<RxPlatformDisplayType>> data, HostedPlatformLibrary lib)
        {
            foreach (var kvp in data)
            {
                var displayType = kvp.Value;
                if (!displayType.valid)
                    continue;
                if (!displayType.definedType)
                    continue;
                if (GenerateDisplayType(displayType, lib))
                    data[kvp.Key] = displayType;
            }
        }

        static bool GenerateMapperType(PlatformTypeBuildMeta<RxPlatformMapperType> type, HostedPlatformLibrary lib)
        {

            if (PlatformHostMain.api.BuildType == null)
                throw new Exception("BuildType function is not available in the API.");

            if (type.attribute != null && type.defaultConstructor != null && type.definedType)
            {
                string overrides = "{}";
                if (type.defaultConstructor != null)
                {
                    var tempObj = type.defaultConstructor.Invoke();
                    if (tempObj != null)
                    {
                        overrides = JsonSerializer.Serialize(tempObj, tempObj.GetType(), PlatformHostMain.JsonContext);
                    }
                }
                if (GenerateSimpleType(
                    type.runtimeType
                    , rx_item_type.rx_mapper_type
                    , type.name
                    , type.path
                    , type.id
                    , type.parentId
                    , HostPlatformIds.RX_DOTNET_MAPPER_TYPE_ID
                    , overrides
                    , type.items
                    , null //type.sources
                    , null //type.mappers
                    , type.filters
                    , type.runtimeConnections + "|" + type.initialValues
                    , lib))
                    return false;//no changes

                type.valid = false;
            }
            return true;
        }
        static void GenerateMapperTypes(Dictionary<RxNodeId, PlatformTypeBuildMeta<RxPlatformMapperType>> data, HostedPlatformLibrary lib)
        {
            foreach (var kvp in data)
            {
                var mapperType = kvp.Value;
                if (!mapperType.valid)
                    continue;
                if (!mapperType.definedType)
                    continue;
                if (GenerateMapperType(mapperType, lib))
                    data[kvp.Key] = mapperType;
            }
        }

        static bool GenerateVariableType(PlatformTypeBuildMeta<RxPlatformVariableType> type, HostedPlatformLibrary lib)
        {

            if (PlatformHostMain.api.BuildType == null)
                throw new Exception("BuildType function is not available in the API.");

            if (type.attribute != null && type.defaultConstructor != null && type.definedType)
            {
                string overrides = "{}";
                if (type.defaultConstructor != null)
                {
                    var tempObj = type.defaultConstructor.Invoke();
                    if (tempObj != null)
                    {
                        overrides = JsonSerializer.Serialize(tempObj, tempObj.GetType(), PlatformHostMain.JsonContext);
                        var document = JsonNode.Parse(overrides) as JsonObject;

                        if (document != null)
                        {
                            if (document.ContainsKey("_"))
                                document.Remove("_");
                            MemoryStream memstm = new MemoryStream();
                            Utf8JsonWriter writer = new Utf8JsonWriter(memstm);
                            document.WriteTo(writer);
                            writer.Flush();
                            overrides = Encoding.UTF8.GetString(memstm.ToArray());
                        }
                    }
                }
                if (GenerateSimpleType(
                    type.runtimeType
                    , rx_item_type.rx_variable_type
                    , type.name
                    , type.path
                    , type.id
                    , type.parentId
                    , HostPlatformIds.RX_CLASS_VARIABLE_BASE_ID
                    , overrides
                    , type.items
                    , type.sources
                    , type.mappers
                    , type.filters
                    , type.runtimeConnections + "|" + type.initialValues
                    , lib))
                    return false;//no changes

                type.valid = false;
            }
            return true;
        }
        static void GenerateVariableTypes(Dictionary<RxNodeId, PlatformTypeBuildMeta<RxPlatformVariableType>> data, HostedPlatformLibrary lib)
        {
            foreach (var kvp in data)
            {
                var variableType = kvp.Value;
                if (!variableType.valid)
                    continue;
                if (!variableType.definedType)
                    continue;
                if (GenerateVariableType(variableType, lib))
                    data[kvp.Key] = variableType;
            }
        }
        static bool GenerateEventType(PlatformTypeBuildMeta<RxPlatformEventType> type, HostedPlatformLibrary lib)
        {

            if (PlatformHostMain.api.BuildType == null)
                throw new Exception("BuildType function is not available in the API.");

            if (type.attribute != null && type.defaultConstructor != null && type.definedType)
            {
                string overrides = "{}";
                if (type.defaultConstructor != null)
                {
                    var tempObj = type.defaultConstructor.Invoke();
                    if (tempObj != null)
                    {
                        overrides = JsonSerializer.Serialize(tempObj, tempObj.GetType(), PlatformHostMain.JsonContext);
                    }
                }
                if (GenerateSimpleType(
                    type.runtimeType
                    , rx_item_type.rx_event_type
                    , type.name
                    , type.path
                    , type.id
                    , type.parentId
                    , HostPlatformIds.RX_DOTNET_EVENT_TYPE_ID
                    , overrides
                    , type.items
                    , null //type.sources
                    , null //type.mappers
                    , null //type.filters
                    , type.runtimeConnections + "|" + type.initialValues
                    , lib))
                    return false;//no changes

                type.valid = false;
            }
            return true;
        }
        static void GenerateEventTypes(Dictionary<RxNodeId, PlatformTypeBuildMeta<RxPlatformEventType>> data, HostedPlatformLibrary lib)
        {
            foreach (var kvp in data)
            {
                var eventType = kvp.Value;
                if (!eventType.valid)
                    continue;
                if (!eventType.definedType)
                    continue;
                if (GenerateEventType(eventType, lib))
                    data[kvp.Key] = eventType;
            }
        }
        static bool GenerateFilterType(PlatformTypeBuildMeta<RxPlatformFilterType> type, HostedPlatformLibrary lib)
        {

            if (PlatformHostMain.api.BuildType == null)
                throw new Exception("BuildType function is not available in the API.");

            if (type.attribute != null && type.defaultConstructor != null && type.definedType)
            {
                string overrides = "{}";
                if (type.defaultConstructor != null)
                {
                    var tempObj = type.defaultConstructor.Invoke();
                    if (tempObj != null)
                    {
                        overrides = JsonSerializer.Serialize(tempObj, tempObj.GetType(), PlatformHostMain.JsonContext);
                    }
                }
                if (GenerateSimpleType(
                    type.runtimeType
                    , rx_item_type.rx_event_type
                    , type.name
                    , type.path
                    , type.id
                    , type.parentId
                    , HostPlatformIds.RX_DOTNET_EVENT_TYPE_ID
                    , overrides
                    , type.items
                    , null //type.sources
                    , null //type.mappers
                    , null //type.filters
                    , type.runtimeConnections + "|" + type.initialValues
                    , lib))
                    return false;//no changes

                type.valid = false;
            }
            return true;
        }
        static void GenerateFilterTypes(Dictionary<RxNodeId, PlatformTypeBuildMeta<RxPlatformFilterType>> data, HostedPlatformLibrary lib)
        {
            foreach (var kvp in data)
            {
                var eventType = kvp.Value;
                if (!eventType.valid)
                    continue;
                if (!eventType.definedType)
                    continue;
                if (GenerateFilterType(eventType, lib))
                    data[kvp.Key] = eventType;
            }
        }
        static bool GenerateStructType(PlatformTypeBuildMeta<RxPlatformStructType> type, HostedPlatformLibrary lib)
        {

            if (PlatformHostMain.api.BuildType == null)
                throw new Exception("BuildType function is not available in the API.");

            if (type.attribute != null && type.defaultConstructor != null && type.definedType)
            {
                string overrides = "{}";
                if (type.defaultConstructor != null)
                {
                    var tempObj = type.defaultConstructor.Invoke();
                    if (tempObj != null)
                    {
                        overrides = JsonSerializer.Serialize(tempObj, tempObj.GetType(), PlatformHostMain.JsonContext);
                    }
                }
                if (GenerateSimpleType(
                    type.runtimeType
                    , rx_item_type.rx_struct_type
                    , type.name
                    , type.path
                    , type.id
                    , type.parentId
                    , HostPlatformIds.RX_DOTNET_STRUCT_TYPE_ID
                    , overrides
                    , type.items
                    , type.sources
                    , type.mappers
                    , null //type.filters
                    , type.runtimeConnections + "|" + type.initialValues
                    , lib))
                    return false;//no changes

                type.valid = false;
            }
            return true;
        }
        static void GenerateStructTypes(Dictionary<RxNodeId, PlatformTypeBuildMeta<RxPlatformStructType>> data, HostedPlatformLibrary lib)
        {
            foreach (var kvp in data)
            {
                var eventType = kvp.Value;
                if (!eventType.valid)
                    continue;
                if (!eventType.definedType)
                    continue;
                if (GenerateStructType(eventType, lib))
                    data[kvp.Key] = eventType;
            }
        }
        public static void GeneratePlatformTypes(PlatformTypeBuildData data, HostedPlatformLibrary lib)
        {
            GenerateDataTypes(data.DataTypes, lib);

            GenerateEventTypes(data.EventTypes, lib);
            GenerateSourceTypes(data.SourceTypes, lib);
            GenerateDisplayTypes(data.DisplayTypes, lib);
            GenerateMapperTypes(data.MapperTypes, lib);
            GenerateFilterTypes(data.FilterTypes, lib);
            GenerateVariableTypes(data.VariableTypes, lib);
            GenerateStructTypes(data.StructTypes, lib);

            GenerateObjectTypes(data.ObjectTypes, lib);
            GeneratePortTypes(data.PortTypes, lib);
            GenerateDomainTypes(data.DomainTypes, lib);
            GenerateApplicationTypes(data.ApplicationTypes, lib);

        }
    }

}