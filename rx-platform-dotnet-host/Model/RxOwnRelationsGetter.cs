using ENSACO.RxPlatform.Attributes;
using ENSACO.RxPlatform.Hosting.Model.Code;
using ENSACO.RxPlatform.Hosting.Model.Items;
using ENSACO.RxPlatform.Hosting.Reflection;
using ENSACO.RxPlatform.Model;
using ENSACO.RxPlatform.Runtime;
using System.Reflection;
using System.Text;

namespace ENSACO.RxPlatform.Hosting.Model.Algorithms
{
    internal class RxOwnRelationsGetter : IRxMetaAlgorithm
    {
        private List<RxOwnRelationCodeData> GetItems(Type type, PropertyInfo[] properties, object instance, ref string? runtimeConnections)
        {
            var items = new List<RxOwnRelationCodeData>();
            StringBuilder connectionsBuilder = new StringBuilder();
            foreach (var prop in properties)
            {
                if (ReflectionHelpers.IsVirtual(prop))
                {
                    bool nullable = false;
                    string? propTypeName = prop.PropertyType.FullName;
                    if (propTypeName == null)
                        propTypeName = prop.Name;
                    Type? propType = ReflectionHelpers.GetNullableType(prop);
                    if (propType != null)
                    {
                        nullable = true;
                        propTypeName = propType.FullName;
                        if (propTypeName == null)
                            propTypeName = prop.Name;
                    }
                    else
                    {
                        propType = prop.PropertyType;
                    }
                    string simpleTypeName = propTypeName.Contains(".") ? propTypeName.Substring(propTypeName.LastIndexOf(".") + 1) : propTypeName;

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
                    string defaultValue = "default";
                    defaultValue = RxMemoryCompiler.ValueToSourceCode(prop.GetValue(instance), propType, -1);

                    RxOwnRelationCodeData data = new RxOwnRelationCodeData()
                    {
                        name = prop.Name,
                        isNullAble = nullable,
                        codeType = propTypeName,
                        defaultValue = defaultValue,
                        itemId = prop.Name,
                        eventName = ReflectionHelpers.EventType(type, prop, null),
                        connectedEventName = ReflectionHelpers.EventType(type, prop, $"On{prop.Name}Connected"),
                        disconnectedEventName = ReflectionHelpers.EventType(type, prop, $"On{prop.Name}Disconnected"),
                        typeConnectedEventName = ReflectionHelpers.EventTypeType(type, prop, $"On{simpleTypeName}Connected"),
                        typeDisconnectedEventName = ReflectionHelpers.EventTypeType(type, prop, $"On{simpleTypeName}Disconnected"),
                        canWrite = prop.CanWrite && prop.SetMethod != null && !initOnly,
                        setModifier = hasPrivateSetter ? "protected" : ""

                    };
                    if (connectionsBuilder.Length > 0)
                        connectionsBuilder.Append(";");
                    connectionsBuilder.Append(prop.Name);
                    items.Add(data);
                }
            }
            runtimeConnections = connectionsBuilder.ToString();
            return items;
        }
        private void FillTypes<T>(Dictionary<RxNodeId, PlatformTypeBuildMeta<T>> data) where T : RxPlatformTypeAttribute
        {
            foreach (var kvp in data)
            {
                if (!kvp.Value.valid)
                    continue;
                if (!kvp.Value.definedType || !kvp.Value.runtimeType)
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
                Type? instanceType = objType.type;
                Dictionary<string, PropertyInfo> propertyInfos = new Dictionary<string, PropertyInfo>();
                while (instanceType != null)
                {
                    var props = ReflectionHelpers.GetRelationsPropertyInfos(instanceType, true);
                    if (props != null)
                    {
                        foreach (var m in props)
                        {
                            if (!propertyInfos.ContainsKey(m.Name))
                            {
                                propertyInfos.Add(m.Name, m);
                            }
                        }
                    }
                    instanceType = instanceType.BaseType;
                    if (instanceType == null || (instanceType.BaseType != null && instanceType.BaseType == typeof(RxPlatformRuntimeBase)))
                    {
                        break;
                    }
                }
                var relations = GetItems(objType.type, propertyInfos.Values.ToArray(), instance
                    , ref objType.relationValues);
                if (relations == null)
                {
                    objType.valid = false;
                    continue;
                }
                objType.definedRelations = relations.ToArray();
                data[kvp.Key] = objType;
            }
        }

        private void FillTypes(Dictionary<RxNodeId, PlatformMonitoredTypeBuildMeta> data) 
        {
            foreach (var kvp in data)
            {
                if (!kvp.Value.valid)
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
                Type? instanceType = objType.type;
                Dictionary<string, PropertyInfo> propertyInfos = new Dictionary<string, PropertyInfo>();
                while (instanceType != null)
                {
                    var props = ReflectionHelpers.GetRelationsPropertyInfos(instanceType, true);
                    if (props != null)
                    {
                        foreach (var m in props)
                        {
                            if (!propertyInfos.ContainsKey(m.Name))
                            {
                                propertyInfos.Add(m.Name, m);
                            }
                        }
                    }
                    instanceType = instanceType.BaseType;
                    if (instanceType == null || (instanceType.BaseType != null && instanceType.BaseType == typeof(RxPlatformRuntimeBase)))
                    {
                        break;
                    }
                }
                var relations = GetItems(objType.type, propertyInfos.Values.ToArray(), instance
                    , ref objType.relationValues);
                if (relations == null)
                {
                    objType.valid = false;
                    continue;
                }
                objType.definedRelations = relations.ToArray();
                data[kvp.Key] = objType;
            }
        }
        public void FillTypes(PlatformTypeBuildData data)
        {
            FillTypes(data.ObjectTypes);
            FillTypes(data.PortTypes);
            FillTypes(data.DomainTypes);
            FillTypes(data.ApplicationTypes);

            FillTypes(data.MonitoredObjects);
            FillTypes(data.MonitoredObjects);

        }
    }
}