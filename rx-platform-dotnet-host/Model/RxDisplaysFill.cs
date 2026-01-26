using ENSACO.RxPlatform.Attributes;
using ENSACO.RxPlatform.Hosting.Common;
using ENSACO.RxPlatform.Hosting.Interface;
using ENSACO.RxPlatform.Hosting.Model.Items;
using ENSACO.RxPlatform.Hosting.Reflection;
using ENSACO.RxPlatform.Model;
using System.Reflection;
using System.Runtime.InteropServices;

namespace ENSACO.RxPlatform.Hosting.Model.Algorithms
{

    internal class RxDisplaysFill : IRxMetaAlgorithm
    {
        private List<RxDisplayDataItem>? GetItems(PropertyInfo[] properties, object instance)
        {
            var items = new List<RxDisplayDataItem>();
            foreach (var prop in properties)
            {

                Type? propType = ReflectionHelpers.GetNullableType(prop);
                if (propType != null)
                {
                    continue;// no null-able types
                }
                Type? enumType = ReflectionHelpers.GetEnumerableElement(prop.PropertyType);
                if (enumType != null)
                {
                    continue;// no enumerable types
                }
                if (prop.CanWrite)
                {
                    continue;// no read/write properties
                }
                propType = prop.PropertyType;
                if (propType == null)
                {
                    continue;
                }
                var attr = propType.GetCustomAttribute<RxPlatformDisplayType>(false);
                if (attr != null)
                {
                    string? targetId = null;
                    unsafe
                    {
                        string_value_struct nodeIdStr;
                        rx_node_id_struct nodeId = CommonInterface.CreateNodeIdFromRxNodeId(attr.NodeId);
                        if (CommonInterface.rx_node_id_to_string(&nodeId, &nodeIdStr) > 0)
                        {
                            targetId = Marshal.PtrToStringUTF8(CommonInterface.rx_c_str(&nodeIdStr));
                            CommonInterface.rx_destory_string_value_struct(&nodeIdStr);
                        }
                        CommonInterface.rx_destory_node_id(&nodeId);
                    }
                    if (!string.IsNullOrEmpty(targetId))
                    {
                        RxDisplayDataItem item = new RxDisplayDataItem
                        {
                            name = prop.Name,
                            target = new RXHostReferenceId { id = targetId },
                            sim = true,
                            proc = true,
                            input = true
                        };
                        items.Add(item);
                    }
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
                if(instance == null)
                {
                    objType.valid = false;
                    continue;
                }
                var props = ReflectionHelpers.GetDisplayPropertyInfos(objType.type);
                var items = GetItems(props, instance);
                if(items==null)
                {
                    objType.valid = false;
                    continue;
                }
                objType.displays = items.ToArray();
                data[kvp.Key] = objType;
            }
        }
        public void FillTypes(PlatformTypeBuildData data)
        {
            FillTypes(data.VariableTypes);
            FillTypes(data.StructTypes);

            FillTypes(data.ObjectTypes);
            FillTypes(data.PortTypes);
            FillTypes(data.DomainTypes);
            FillTypes(data.ApplicationTypes);

        }
    }
    
}