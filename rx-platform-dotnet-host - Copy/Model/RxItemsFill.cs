using ENSACO.RxPlatform.Attributes;
using ENSACO.RxPlatform.Hosting.Model.Items;
using ENSACO.RxPlatform.Hosting.Reflection;
using ENSACO.RxPlatform.Model;
using RxPlatform.Hosting.Interface;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;

namespace ENSACO.RxPlatform.Hosting.Model.Algorithms
{

    internal class RxItemsFill : IRxMetaAlgorithm
    {
        private List<RxMetaItem>? GetItems(PropertyInfo[] properties, object instance)
        {
            var items = new List<RxMetaItem>();
            foreach (var prop in properties)
            {
                object? value = null;
                try
                {
                    value = prop.GetValue(instance);
                }
                catch
                {
                    return null;
                }
                int array = -1;
                Type? propType = ReflectionHelpers.GetNullableType(prop);
                if (propType == null)
                {
                    propType = prop.PropertyType;
                }
                Type? enumType = ReflectionHelpers.GetEnumerableElement(prop.PropertyType);
                if (enumType != null)
                {
                    propType = enumType;
                    array = 0;
                }
                bool readOnly = !prop.CanWrite;
                bool hasPrivateSetter = false;
                bool initOnly = false;
                if (prop.SetMethod != null)
                {
                    if (!prop.SetMethod.IsPublic)
                    {
                        hasPrivateSetter = true;
                    }
                    var requiredModifiers = prop.SetMethod.ReturnParameter.GetRequiredCustomModifiers();
                    initOnly = requiredModifiers.Contains(typeof(System.Runtime.CompilerServices.IsExternalInit));
                }
                if (propType == null)
                {
                    continue;
                }
                var attr = propType.GetCustomAttribute<RxPlatformDataType>(false);
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
                        if (!initOnly)
                        {
                            RxHostPropBlockItem item = new RxHostPropBlockItem
                            {
                                name = prop.Name,
                                datatype = new RXHostReferenceId { id = targetId },
                                ro = !prop.CanWrite || hasPrivateSetter,
                                array = array,
                            };
                            items.Add(item);
                        }
                        else
                        {
                            RxHostConstBlockItem item = new RxHostConstBlockItem
                            {
                                name = prop.Name,
                                datatype = new RXHostReferenceId { id = targetId },
                                array = array,
                                ro = initOnly
                            };
                            items.Add(item);
                        }
                    }
                }
                else
                {
                    if (!initOnly)
                    {
                        RxHostPropItem item = new RxHostPropItem
                        {
                            name = prop.Name,
                            ro = !prop.CanWrite || hasPrivateSetter,
                            array = array
                        };

                        item.value = ReflectionHelpers.GetValue(prop, propType, value);
                        items.Add(item);
                    }
                    else
                    {
                        RxHostConstItem item = new RxHostConstItem
                        {
                            name = prop.Name,
                            ro = initOnly,
                            array = array
                        };

                        item.value = ReflectionHelpers.GetValue(prop, propType, value);
                        items.Add(item);
                    }
                }
            }
            return items;
        }
        private List<RxDataItem>? GetDataItems(PropertyInfo[] properties, object instance)
        {
            var items = new List<RxDataItem>();
            foreach (var prop in properties)
            {
                int array = -1;

                Type? propType = Nullable.GetUnderlyingType(prop.PropertyType);
                if (propType == null)
                {
                    propType = prop.PropertyType;
                }

                Type? enumType = ReflectionHelpers.GetEnumerableElement(prop.PropertyType);
                if (enumType != null)
                {
                    propType = enumType;
                    array = 0;
                }
                var attr = propType.GetCustomAttribute<RxPlatformDataType>(false);
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

                        RxHostBlockDataItem item = new RxHostBlockDataItem
                        {
                            name = prop.Name,
                            target = new RXHostReferenceId { id = targetId },
                            array = array
                        };
                        object? value = null;
                        try
                        {
                            value = prop.GetValue(instance);
                        }
                        catch
                        {

                        }
                        items.Add(item);
                    }
                }
                else
                {

                    RxHostSimpleDataItem item = new RxHostSimpleDataItem
                    {
                        name = prop.Name,
                    };
                    object? value = null;
                    try
                    {
                        value = prop.GetValue(instance);
                    }
                    catch
                    {
                    }


                    item.value = ReflectionHelpers.GetValue(prop, propType, value);

                    items.Add(item);
                }
            }
            return items;
        }
        private void FillTypes(Dictionary<RxNodeId, PlatformDataTypeBuildMeta> data)
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
                var props = ReflectionHelpers.GetSimplePropertyInfos(objType.type);
                var items = GetDataItems(props, instance);
                if (items == null)
                {
                    objType.valid = false;
                    continue;
                }
                objType.items = items.ToArray();
                data[kvp.Key] = objType;
            }
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
                var props = ReflectionHelpers.GetSimplePropertyInfos(objType.type);
                var items = GetItems(props, instance);
                if(items==null)
                {
                    objType.valid = false;
                    continue;
                }
                objType.items = items.ToArray();
                data[kvp.Key] = objType;
            }
        }
        public void FillTypes(PlatformTypeBuildData data)
        {
            FillTypes(data.DataTypes);

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