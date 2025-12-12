using ENSACO.RxPlatform.Attributes;
using ENSACO.RxPlatform.Hosting.Common;
using ENSACO.RxPlatform.Hosting.Internal;
using ENSACO.RxPlatform.Hosting.Model.Code;
using ENSACO.RxPlatform.Hosting.Model.Items;
using ENSACO.RxPlatform.Hosting.Reflection;
using ENSACO.RxPlatform.Model;
using System.Reflection;
using System.Runtime.InteropServices;

namespace ENSACO.RxPlatform.Hosting.Model.Algorithms
{
    internal class RxRelationsGetter : IRxMetaAlgorithm
    {
        private List<RxRelationDataItem> GetItems(PropertyInfo[] properties, object instance)
        {
            var items = new List<RxRelationDataItem>();
            foreach (var prop in properties)
            {
                object? value = null;
                try
                {
                    value = prop.GetValue(instance);
                }
                catch
                {
                    continue;
                }
                Type? propType = ReflectionHelpers.GetNullableType(prop);
                if (propType == null)
                {
                    propType = prop.PropertyType;
                }
                bool readOnly = !prop.CanWrite;
                bool initOnly = false;
                if (prop.SetMethod != null)
                {
                    
                    var requiredModifiers = prop.SetMethod.ReturnParameter.GetRequiredCustomModifiers();
                    initOnly = requiredModifiers.Contains(typeof(System.Runtime.CompilerServices.IsExternalInit));
                }
                if (propType == null)
                {
                    continue;
                }
                string? relationTypeId = null;
                string? targetTypeId = null;
                var relationAttr = prop.GetCustomAttribute<RxPlatformRelationAttribute>(false);
                var attr = propType.GetCustomAttribute<RxPlatformObjectType>(false);
                rx_node_id_struct targetType = new rx_node_id_struct();
                if(attr!= null)
                {
                    targetType = CommonInterface.CreateNodeIdFromRxNodeId(attr.NodeId);
                }

                if (relationAttr != null)
                {
                    unsafe
                    {
                        string_value_struct nodeIdStr;
                        rx_node_id_struct nodeId = CommonInterface.CreateNodeIdFromRxNodeId(relationAttr.NodeId);
                        if (CommonInterface.rx_node_id_to_string(&nodeId, &nodeIdStr) > 0)
                        {
                            relationTypeId = Marshal.PtrToStringUTF8(CommonInterface.rx_c_str(&nodeIdStr));
                            CommonInterface.rx_destory_string_value_struct(&nodeIdStr);
                        }
                        CommonInterface.rx_destory_node_id(&nodeId);
                    }
                }
                else
                {
                    rx_node_id_struct nodeId;
                    if (readOnly || initOnly)
                    {
                        nodeId = CommonInterface.CreateNodeIdFromInt(HostPlatformIds.RX_DOTNET_STATIC_RELATION_TYPE_ID);
                    }
                    else
                    {
                        nodeId = CommonInterface.CreateNodeIdFromInt(HostPlatformIds.RX_DOTNET_DYNAMIC_RELATION_TYPE_ID);
                    }
                    unsafe
                    {
                        string_value_struct nodeIdStr;

                        if (CommonInterface.rx_node_id_to_string(&nodeId, &nodeIdStr) > 0)
                        {
                            relationTypeId = Marshal.PtrToStringUTF8(CommonInterface.rx_c_str(&nodeIdStr));
                            CommonInterface.rx_destory_string_value_struct(&nodeIdStr);
                        }
                        CommonInterface.rx_destory_node_id(&nodeId);
                        
                        string_value_struct targetIdStr;

                        if (CommonInterface.rx_node_id_to_string(&targetType, &targetIdStr) > 0)
                        {
                            targetTypeId = Marshal.PtrToStringUTF8(CommonInterface.rx_c_str(&targetIdStr));
                            CommonInterface.rx_destory_string_value_struct(&targetIdStr);
                        }
                        CommonInterface.rx_destory_node_id(&nodeId);
                    }
                }
                if (!string.IsNullOrEmpty(relationTypeId))
                {
                    if(targetTypeId == null)
                    {
                        targetTypeId = "";
                    }
                    RxRelationDataItem data = new RxRelationDataItem()
                    {
                        name = prop.Name,
                        relation = new RXHostReferenceId { id = relationTypeId },
                        target = new RXHostReferenceId { id = targetTypeId }
                    };
                    items.Add(data);
                }

            }
            return items;
        }
        private void FillTypes<T>(Dictionary<RxNodeId, PlatformTypeBuildMeta<T>> data) where T : RxPlatformTypeAttribute
        {
            foreach (var kvp in data)
            {
                if (!kvp.Value.valid)
                    continue;
                if (!kvp.Value.definedType)
                    continue;

                var objType = kvp.Value;
                if (!objType.valid)
                    continue;
                if (objType.type == null || objType.defaultConstructor == null)
                {
                    objType.valid = false;
                    continue;
                }
                object? instance = objType.defaultConstructor();
                if (instance == null)
                {
                    objType.valid = false;
                    continue;
                }
                var props = ReflectionHelpers.GetRelationsPropertyInfos(objType.type, false);
                var relations = GetItems(props, instance);
                if (relations == null)
                {
                    objType.valid = false;
                    continue;
                }
                objType.relations = relations.ToArray();
                data[kvp.Key] = objType;
            }
        }
        public void FillTypes(PlatformTypeBuildData data)
        {

            FillTypes(data.EventTypes);
            FillTypes(data.SourceTypes);
            FillTypes(data.MapperTypes);
            FillTypes(data.FilterTypes);
            FillTypes(data.VariableTypes);
            FillTypes(data.StructTypes);

            FillTypes(data.ObjectTypes);
            FillTypes(data.PortTypes);
            FillTypes(data.DomainTypes);
            FillTypes(data.ApplicationTypes);

        }
    }
}