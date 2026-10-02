using ENSACO.RxPlatform.Attributes;
using ENSACO.RxPlatform.Hosting.Common;
using ENSACO.RxPlatform.Hosting.Interface;
using ENSACO.RxPlatform.Hosting.Model.Items;
using ENSACO.RxPlatform.Hosting.Reflection;
using ENSACO.RxPlatform.Model;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json.Nodes;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ENSACO.RxPlatform.Hosting.Model.Algorithms
{
    
    internal class RxItemsFill : IRxMetaAlgorithm
    {
        private void FillVariableData(ref RxVariableConstructionData data, object[] attributes)
        {
            foreach (var attribute in attributes)
            {
                if (attribute is RxPlatformVariableSourceAttribute)
                {
                    var src = attribute as RxPlatformVariableSourceAttribute;
                    if(src!=null)
                        data.sourceAttributes.Add(src);
                }
                else if(attribute is RxPlatformVariableMapperAttribute)
                {
                    var src = attribute as RxPlatformVariableMapperAttribute;
                    if (src != null)
                        data.mapperAttributes.Add(src);
                }
                else if (attribute is RxPlatformFilterAttribute)
                {
                    var src = attribute as RxPlatformFilterAttribute;
                    if (src != null)
                        data.filterAttributes.Add(src);
                }
            }
            string varName = data.GetVariableName();
        }
        private List<RxMetaItem>? GetItems(PropertyInfo[] properties, PropertyInfo[]? valueProperties, object instance, PlatformTypeBuildData buildData, ref JsonObject varInit)
        {
            var items = new List<RxMetaItem>();
            System.Diagnostics.Debug.Assert(valueProperties == null || valueProperties.Length == properties.Length);
            int idx = 0;
            foreach (var prop in properties)
            {
                if(prop.GetCustomAttribute<RxPlatformIgnoreAttribute>(false) != null
                    || prop.GetCustomAttribute<RxPlatformRelationAttribute>(false) != null)
                {
                    idx++;
                    continue;
                }
                PropertyInfo valueProp = prop;
                if (valueProperties != null)
                {
                    valueProp = valueProperties[idx];
                }
                idx++;
                object? value = null;
                try
                {
                    value = valueProp.GetValue(instance);
                }
                catch
                {
                    value = null;
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
                object[] attributes = prop.GetCustomAttributes(false);
                RxVariableConstructionData data = new RxVariableConstructionData();

                FillVariableData(ref data, attributes);

                var varData = RxMetaExtracter.GetVariableType(data, buildData);
                string? varId = null;
                if (varData != null && !varData.Item1.IsNull())
                {
                    varId = varData.Item1.ToString();
                    if(varData.Item2 != null)
                    {
                        varInit[prop.Name] = varData.Item2;
                    }
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

                        if (!string.IsNullOrEmpty(varId))
                        {
                            RxHostBlockVariableItem item = new RxHostBlockVariableItem()
                            {
                                name = prop.Name,
                                target = new RXHostReferenceId { id = varId },
                                array = -1,
                                ro = initOnly || !prop.CanWrite || hasPrivateSetter,
                                datatype = new RXHostReferenceId { id = targetId },
                            };
                            items.Add(item);
                        }
                        else if (!initOnly)
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
                    var structAttr = propType.GetCustomAttribute<RxPlatformStructType>(false);
                    if (structAttr != null)
                    {
                        if(value!=null)
                        {
                            var attr1 = value.GetType().GetCustomAttribute<RxPlatformStructType>(false);
                            if(attr1!= null)
                            {
                                structAttr = attr1;
                            }
                        }

                        string? targetId = null;
                        unsafe
                        {
                            string_value_struct nodeIdStr;
                            rx_node_id_struct nodeId = CommonInterface.CreateNodeIdFromRxNodeId(structAttr.NodeId);
                            if (CommonInterface.rx_node_id_to_string(&nodeId, &nodeIdStr) > 0)
                            {
                                targetId = Marshal.PtrToStringUTF8(CommonInterface.rx_c_str(&nodeIdStr));
                                CommonInterface.rx_destory_string_value_struct(&nodeIdStr);
                            }
                            CommonInterface.rx_destory_node_id(&nodeId);
                        }
                        if (!string.IsNullOrEmpty(targetId))
                        {
                            RxHostStructItem item = new RxHostStructItem()
                            {
                                name = prop.Name,
                                target = new RXHostReferenceId { id = targetId },
                                array = array
                            };
                            items.Add(item);
                        }
                    }
                    else
                    {
                        var varAttr = propType.GetCustomAttribute<RxPlatformVariableType>(false);
                        if (varAttr != null)
                        {
                            string? targetId = null;
                            unsafe
                            {
                                string_value_struct nodeIdStr;
                                rx_node_id_struct nodeId = CommonInterface.CreateNodeIdFromRxNodeId(varAttr.NodeId);
                                if (CommonInterface.rx_node_id_to_string(&nodeId, &nodeIdStr) > 0)
                                {
                                    targetId = Marshal.PtrToStringUTF8(CommonInterface.rx_c_str(&nodeIdStr));
                                    CommonInterface.rx_destory_string_value_struct(&nodeIdStr);
                                }
                                CommonInterface.rx_destory_node_id(&nodeId);
                            }
                            if (!string.IsNullOrEmpty(targetId))
                            {
                                RxHostVariableItem item = new RxHostVariableItem()
                                {
                                    name = prop.Name,
                                    target = new RXHostReferenceId { id = targetId },
                                    array = -1,
                                    ro = initOnly || !prop.CanWrite || hasPrivateSetter
                                };
                                item.value = ReflectionHelpers.GetVariableValue(prop, propType, value, array);
                                items.Add(item);
                            }
                        }
                        else
                        {
                            var eventAttr = propType.GetCustomAttribute<RxPlatformEventType>(false);
                            if (eventAttr != null)
                            {
                                var attrType = eventAttr.GetType();
                                if (attrType.IsConstructedGenericType)
                                {
                                    string? targetId = null;
                                    string? argumentId = null;
                                    unsafe
                                    {
                                        Type argType = attrType.GenericTypeArguments[0];
                                        if (argType != null)
                                        {
                                            var dtAttribute = argType.GetCustomAttribute<RxPlatformDataType>(false);
                                            if (dtAttribute != null)
                                            {
                                                string_value_struct argIdStr;
                                                rx_node_id_struct argId = CommonInterface.CreateNodeIdFromRxNodeId(dtAttribute.NodeId);
                                                if (CommonInterface.rx_node_id_to_string(&argId, &argIdStr) > 0)
                                                {
                                                    argumentId = Marshal.PtrToStringUTF8(CommonInterface.rx_c_str(&argIdStr));
                                                    CommonInterface.rx_destory_string_value_struct(&argIdStr);
                                                }
                                                CommonInterface.rx_destory_node_id(&argId);
                                            }
                                        }
                                        string_value_struct nodeIdStr;
                                        rx_node_id_struct nodeId = CommonInterface.CreateNodeIdFromRxNodeId(eventAttr.NodeId);
                                        if (CommonInterface.rx_node_id_to_string(&nodeId, &nodeIdStr) > 0)
                                        {
                                            targetId = Marshal.PtrToStringUTF8(CommonInterface.rx_c_str(&nodeIdStr));
                                            CommonInterface.rx_destory_string_value_struct(&nodeIdStr);
                                        }
                                        CommonInterface.rx_destory_node_id(&nodeId);
                                    }
                                    if (!string.IsNullOrEmpty(targetId) && !string.IsNullOrEmpty(argumentId))
                                    {
                                        RxHostEventItem item = new RxHostEventItem()
                                        {
                                            name = prop.Name,
                                            target = new RXHostReferenceId { id = targetId },
                                            args = new RXHostReferenceId { id = argumentId },

                                        };
                                        items.Add(item);
                                    }
                                }
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(varId))
                                {
                                    RxHostVariableItem item = new RxHostVariableItem()
                                    {
                                        name = prop.Name,
                                        target = new RXHostReferenceId { id = varId },
                                        array = -1,
                                        ro = initOnly || !prop.CanWrite || hasPrivateSetter
                                    };
                                    item.value = ReflectionHelpers.GetValue(prop, propType, value, array);
                                    items.Add(item);
                                }
                                else if (!initOnly)
                                {
                                    RxHostPropItem item = new RxHostPropItem
                                    {
                                        name = prop.Name,
                                        ro = !prop.CanWrite || hasPrivateSetter,
                                        array = -1
                                    };

                                    item.value = ReflectionHelpers.GetValue(prop, propType, value, array);
                                    items.Add(item);
                                }
                                else
                                {
                                    RxHostConstItem item = new RxHostConstItem
                                    {
                                        name = prop.Name,
                                        ro = initOnly,
                                        array = -1
                                    };

                                    item.value = ReflectionHelpers.GetValue(prop, propType, value, array);
                                    items.Add(item);
                                }
                            }
                        }
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


                    item.value = ReflectionHelpers.GetValue(prop, propType, value, array);

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
                object? instance = objType.defaultConstructor(null);
                if (instance == null)
                {
                    objType.valid = false;
                    continue;
                }
                var props = ReflectionHelpers.GetSimplePropertyInfos(objType.type, false);
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
        private void FillTypes<T>(Dictionary<RxNodeId, PlatformTypeBuildMeta<T>> data, PlatformTypeBuildData buildData) where T : RxPlatformTypeAttribute
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
                object? instance = objType.defaultConstructor(null);
                if(instance == null)
                {
                    objType.valid = false;
                    continue;
                }
                PropertyInfo[]? valProperties = null;
                if (objType.type.IsGenericType)
                {
                    valProperties = ReflectionHelpers.GetSimplePropertyInfos(objType.type.MakeGenericType(new Type[] { typeof(int) }), true);
                }
                var props = ReflectionHelpers.GetSimplePropertyInfos(objType.type, true);
                JsonObject varInit = new JsonObject();
                var items = GetItems(props, valProperties, instance, buildData, ref varInit);
                if(items==null)
                {
                    objType.valid = false;
                    continue;
                }
                objType.items = items.ToArray();
                if(varInit.Count>0)
                {
                    objType.VariableOverrideData = varInit;
                }
                data[kvp.Key] = objType;
            }
        }
        public void FillTypes(PlatformTypeBuildData data)
        {
            FillTypes(data.DataTypes);

            FillTypes(data.EventTypes, data);
            FillTypes(data.SourceTypes, data);
            FillTypes(data.MapperTypes, data);
            FillTypes(data.FilterTypes, data);
            FillTypes(data.VariableTypes, data);
            FillTypes(data.StructTypes, data);
            FillTypes(data.DisplayTypes, data);

            FillTypes(data.ObjectTypes, data);
            FillTypes(data.PortTypes, data);
            FillTypes(data.DomainTypes, data);
            FillTypes(data.ApplicationTypes, data);

        }
    }
    
}