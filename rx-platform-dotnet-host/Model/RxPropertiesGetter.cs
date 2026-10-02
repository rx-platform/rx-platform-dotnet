using ENSACO.RxPlatform.Attributes;
using ENSACO.RxPlatform.Hosting.Internal;
using ENSACO.RxPlatform.Hosting.Model.Code;
using ENSACO.RxPlatform.Hosting.Reflection;
using ENSACO.RxPlatform.Model;
using ENSACO.RxPlatform.Runtime;
using System.Reflection;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ENSACO.RxPlatform.Hosting.Model.Algorithms
{

    internal class RxPropertiesGetter : IRxMetaAlgorithm
    {
        private List<RxPropertyCodeData>? GetItems(Type type, PropertyInfo[] properties, object instance, ref string? runtimeConnections, ref string? initialData)
        {
            StringBuilder connectionsBuilder = new StringBuilder();
            StringBuilder initialDataBuilder = new StringBuilder();
            var items = new List<RxPropertyCodeData>();
            foreach (var prop in properties)
            {
                bool hasCode = false;
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
                    }
                    else
                    {
                        propType = prop.PropertyType;
                    }
                    propTypeName = propType.FullName;
                    if (propTypeName == null)
                        propTypeName = prop.Name;

                    int array = -1;
                    Type? enumType = ReflectionHelpers.GetEnumerableElement(prop.PropertyType);
                    if (enumType != null)
                    {
                        propType = enumType;
                        array = 0;
                    }
                    bool hasPrivateSetter = false;
                    bool initOnly = false;
                    if (nullable)
                    {
                        if (prop.SetMethod != null)
                        {
                            if (!prop.SetMethod.IsPublic)
                            {
                                hasPrivateSetter = true;
                            }
                            var requiredModifiers = prop.SetMethod.ReturnParameter.GetRequiredCustomModifiers();
                            initOnly = requiredModifiers.Contains(typeof(System.Runtime.CompilerServices.IsExternalInit));
                        }
                    }
                    else
                    {
                        if (prop.SetMethod != null)
                        {
                            var requiredModifiers = prop.SetMethod.ReturnParameter.GetRequiredCustomModifiers();
                            initOnly = requiredModifiers.Contains(typeof(System.Runtime.CompilerServices.IsExternalInit));
                            if (!initOnly)
                            {
                                RxPlatformObject.Instance.WriteLogWarning("RxPropertiesGetter", 0, $"Property {prop.Name} of type {type.FullName} is not null-able and has setter skipping code generation for it.");
                                hasCode = false;
                            }
                        }

                    }
                    string defaultValue = "default";
                    defaultValue = RxMemoryCompiler.ValueToSourceCode(prop.GetValue(instance), propType, array);

                    RxPropertyCodeData data = new RxPropertyCodeData()
                    {
                        name = prop.Name,
                        isNullAble = nullable,
                        codeType = propTypeName,
                        defaultValue = defaultValue,
                        itemId = prop.Name,
                        eventName = nullable ? ReflectionHelpers.EventType(type, prop, null) : null,
                        writeMethod = nullable ? ReflectionHelpers.HasWriteMethod(type, prop) : false,
                        jsonValue = (null != prop.PropertyType.GetCustomAttribute<RxPlatformDataType>()),
                        canWrite = prop.CanWrite && prop.SetMethod != null && !initOnly,
                        setModifier = hasPrivateSetter ? "protected" : ""

                    };
                    if (connectionsBuilder.Length > 0)
                        connectionsBuilder.Append(";");
                    connectionsBuilder.Append(prop.Name);
                    items.Add(data);
                    hasCode = true;
                }
                if (!hasCode)
                {
                    bool createInitData = false;
                    if (prop.SetMethod == null)
                    {
                        createInitData = true;
                    }
                    else if (!prop.SetMethod.IsPrivate)
                    {
                        var requiredModifiers = prop.SetMethod.ReturnParameter.GetRequiredCustomModifiers();
                        bool initOnly = requiredModifiers.Contains(typeof(System.Runtime.CompilerServices.IsExternalInit));
                        if (initOnly)
                        {
                            createInitData = true;
                        }
                        else
                        {
                            RxPlatformObject.Instance.WriteLogWarning("RxPropertiesGetter", 0, $"Property {prop.Name} of type {type.FullName} is virtual but does not have a init setter, skipping code generation for it.");
                        }
                    }

                    if (createInitData)
                    {
                        if (initialDataBuilder.Length > 0)
                            initialDataBuilder.Append(";");
                        initialDataBuilder.Append(prop.Name);
                    }
                }
            }
            initialData = initialDataBuilder.ToString();
            runtimeConnections = connectionsBuilder.ToString();
            return items;
        }
        private void FillTypes<T>(Dictionary<RxNodeId, PlatformTypeBuildMeta<T>> data) where T : RxPlatformTypeAttribute
        {
            foreach (var kvp in data)
            {
                if (!kvp.Value.valid)
                    continue;
                if (!kvp.Value.runtimeType)
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
                    var props = ReflectionHelpers.GetSimplePropertyInfos(instanceType, false);
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
                var items = GetItems(objType.type, propertyInfos.Values.ToArray(), instance
                    , ref objType.runtimeConnections, ref objType.initialValues);
                if (items == null)
                {
                    objType.valid = false;
                    continue;
                }
                
                objType.definedProperties = items.ToArray();
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
                    var props = ReflectionHelpers.GetSimplePropertyInfos(instanceType, false);
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
                var items = GetItems(objType.type, propertyInfos.Values.ToArray(), instance
                    , ref objType.runtimeConnections, ref objType.initialValues);
                if (items == null)
                {
                    objType.valid = false;
                    continue;
                }

                objType.definedProperties = items.ToArray();
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
            FillTypes(data.DisplayTypes);

            FillTypes(data.ObjectTypes);
            FillTypes(data.PortTypes);
            FillTypes(data.DomainTypes);
            FillTypes(data.ApplicationTypes);

            FillTypes(data.MonitoredObjects);
            FillTypes(data.MonitoredStructs);

        }

    }
}