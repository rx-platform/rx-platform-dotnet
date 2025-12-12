using ENSACO.RxPlatform.Attributes;
using ENSACO.RxPlatform.Hosting.Internal;
using ENSACO.RxPlatform.Hosting.Model.Items;
using ENSACO.RxPlatform.Runtime;
using RxPlatform.Hosting.Interface;
using System.Collections;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace ENSACO.RxPlatform.Hosting.Reflection
{

    internal static class ReflectionHelpers
    {       

        static internal Func<object>? CreateConstructorFunc(Type type)
        {
            ConstructorInfo? ctor = type.GetConstructor(Type.EmptyTypes);
            if (ctor == null)
                return null;

            NewExpression newExp = Expression.New(ctor);
            var lambda = Expression.Lambda<Func<object>>(newExp);
            return lambda.Compile();
        }
        internal static RxHostValueType GetValue(PropertyInfo prop, Type propType, object? value)
        {
            RxHostValueType rxValue = new RxHostValueType();
            if (propType == typeof(bool))
            {
                rxValue.type = rx_value_t.Bool;
                if (value == null)
                    rxValue.val = false;
                else
                    rxValue.val = (bool?)value;
            }
            else if (propType == typeof(string))
            {
                rxValue.type = rx_value_t.String;
                if (value == null)
                    rxValue.val = "";
                else
                    rxValue.val = (string?)value;
            }
            else if (propType == typeof(float))
            {
                rxValue.type = rx_value_t.Float;
                if (value == null)
                    rxValue.val = 0.0f;
                else
                    rxValue.val = (float?)value;
            }
            else if (propType == typeof(double))
            {
                rxValue.type = rx_value_t.Double;
                if (value == null)
                    rxValue.val = 0.0;
                else
                    rxValue.val = (double?)value;
            }
            else if (propType == typeof(char))
            {
                rxValue.type = rx_value_t.Int8;
                if (value == null)
                    rxValue.val = (sbyte)0;
                else
                    rxValue.val = (sbyte?)(sbyte)(char?)value;
            }
            else if (propType == typeof(DateTime))
            {
                rxValue.type = rx_value_t.Int64;
                if (value == null)
                    rxValue.val = null;
                else
                    rxValue.val = (DateTime?)value;
            }
            else if (propType == typeof(Guid))
            {
                rxValue.type = rx_value_t.Uuid;
                Guid guid;
                if (value == null)
                    guid = Guid.Empty;
                else
                    guid = (Guid)value;

                rxValue.val = guid.ToString();
            }
            else if (propType == typeof(sbyte))
            {
                rxValue.type = rx_value_t.Int8;
                if (value == null)
                    rxValue.val = (sbyte)0;
                else
                    rxValue.val = (sbyte?)value;
            }
            else if (propType == typeof(short))
            {
                rxValue.type = rx_value_t.Int16;
                if (value == null)
                    rxValue.val = (short)0;
                else
                    rxValue.val = (short?)value;
            }
            else if (propType == typeof(int))
            {
                rxValue.type = rx_value_t.Int32;
                if (value == null)
                    rxValue.val = (int)0;
                else
                    rxValue.val = (int?)value;
            }
            else if (propType == typeof(long))
            {
                rxValue.type = rx_value_t.Int64;
                if (value == null)
                    rxValue.val = (long)0;
                else
                    rxValue.val = (long?)value;
            }
            else if (propType == typeof(byte))
            {
                rxValue.type = rx_value_t.UInt8;
                if (value == null)
                    rxValue.val = (byte)0;
                else
                    rxValue.val = (byte?)value;
            }
            else if (propType == typeof(ushort))
            {
                rxValue.type = rx_value_t.UInt16;
                if (value == null)
                    rxValue.val = (ushort)0;
                else
                    rxValue.val = (ushort?)value;
            }
            else if (propType == typeof(uint))
            {
                rxValue.type = rx_value_t.UInt32;
                if (value == null)
                    rxValue.val = (uint)0;
                else
                    rxValue.val = (uint?)value;
            }
            else if (propType == typeof(ulong))
            {
                rxValue.type = rx_value_t.UInt64;
                if (value == null)
                    rxValue.val = (ulong)0;
                else
                    rxValue.val = (ulong?)value;
            }
            else
            {
                throw new Exception($"Unsupported property type {prop.PropertyType.FullName} for property {prop.Name}");
            }
            return rxValue;
        }
        static bool IsEnumerableType(Type type)
        {
            return (type.Name != nameof(String)
                && type.GetInterface(nameof(IEnumerable)) != null);
        }
        internal static Type? GetEnumerableElement(Type type)
        {
            if (type.IsArray)
            {
                return type.GetElementType();
            }
            var genericType = type.GetInterface(nameof(IEnumerable));
            if (genericType != null)
            {
                var genericArgs = genericType.GetGenericArguments();
                if (genericArgs.Length == 1)
                {
                    return genericArgs[0];
                }
            }
            return null;
        }
        static internal ConstructorInfo? GetDefaultConstructor(Type type)
        {
            var constructors = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance);
            foreach (var constructor in constructors)
            {
                var parameters = constructor.GetParameters();
                if (parameters.Length == 0)
                {
                    return constructor;
                }
            }
            return null;
        }
        static private bool IsSimpleType(Type type)
        {
            return
                type.IsPrimitive ||
                new Type[]
                {
                    typeof(string),
                    typeof(decimal),
                    typeof(DateTime),
                    typeof(Guid)
                }.Contains(type);
        }
        static internal Type? GetNullableType(PropertyInfo prop)
        {
            Type? propType = Nullable.GetUnderlyingType(prop.PropertyType);
            if (propType == null)
            {
                NullabilityInfoContext context = new NullabilityInfoContext();
                var info = context.Create(prop);
                if(info.WriteState  == NullabilityState.Nullable || info.ReadState == NullabilityState.Nullable)
                {
                    propType = prop.PropertyType;
                }
            }
            return propType;
        }
        static internal Type? GetNullableType(ParameterInfo prop)
        {
            Type? propType = Nullable.GetUnderlyingType(prop.ParameterType);
            if (propType == null)
            {
                NullabilityInfoContext context = new NullabilityInfoContext();
                var info = context.Create(prop);
                if (info.WriteState == NullabilityState.Nullable || info.ReadState == NullabilityState.Nullable)
                {
                    propType = prop.ParameterType;
                }
            }
            return propType;
        }
        static internal bool IsVirtual(PropertyInfo prop)
        {
            var getMethod = prop.GetGetMethod();
            if (getMethod != null && getMethod.IsVirtual && !getMethod.IsFinal)
            {
                return true;
            }
            return false;
        }
        static internal bool IsVirtual(MethodInfo method)
        {
            if (method != null && method.IsVirtual && !method.IsFinal)
            {
                return true;
            }
            return false;
        }
        static internal PropertyInfo[] GetRelationsPropertyInfos(Type type)
        {
            List<PropertyInfo> ret = new List<PropertyInfo>();
            var propertyInfos = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var prop in propertyInfos)
            {
                var relAttr = prop.GetCustomAttribute<RxPlatformRelationAttribute>();
                if (relAttr != null)
                {
                    ret.Add(prop);
                    continue;
                }
            }
            return ret.ToArray();
        }
        static internal string? EventType(Type type, PropertyInfo prop)
        {
            var eventName = $"On{prop.Name}Change";
            var eventInfo =  type.GetEvent(eventName);
            if (eventInfo != null && eventInfo.EventHandlerType != null)
            {
                string? typeName = eventInfo.EventHandlerType.FullName;
                if (eventInfo.EventHandlerType.IsGenericType)
                {
                    Type[] genericArguments = eventInfo.EventHandlerType.GetGenericArguments();
                    if (genericArguments.Length == 1 && genericArguments[0] == prop.PropertyType
                        && eventInfo.EventHandlerType.FullName != null)
                    {
                        var idx = eventInfo.EventHandlerType.FullName.IndexOf('`');
                        if (idx != -1)
                        {
                            typeName = $"{eventInfo.EventHandlerType.FullName.Substring(0, idx)}<{genericArguments[0].FullName}?>";
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
                var method = eventInfo.EventHandlerType.GetMethod("Invoke");
                if(method!=null && method.ReturnType == typeof(void))
                {
                    var parameters = method.GetParameters();
                    if(parameters.Length==1)
                    {
                        if(parameters[0].ParameterType == prop.PropertyType)
                        {
                            return typeName;
                        }
                    }
                }
            }
            return null;
        }
        static internal bool HasWriteMethod(Type type, PropertyInfo prop)
        {
            var methodName = $"Write{prop.Name}";
            var methodInfo = type.GetMethod(methodName);
            if (methodInfo != null && methodInfo.ReturnType == typeof(Task) && IsVirtual(methodInfo))
            {
                var parameters = methodInfo.GetParameters();
                if (parameters.Length == 1)
                {
                    if (parameters[0].ParameterType == prop.PropertyType)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        static internal PropertyInfo[] GetSimplePropertyInfos(Type type)
        {

            List<PropertyInfo> ret = new List<PropertyInfo>();
            var propertyInfos = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var prop in propertyInfos)
            {
                if (prop.DeclaringType == type)
                {
                    Type? propType = Nullable.GetUnderlyingType(prop.PropertyType);
                    if(propType == null)
                    {
                        propType = prop.PropertyType;
                    }
                    if (IsSimpleType(propType))
                    {
                        ret.Add(prop);
                    }
                    else if(IsEnumerableType(prop.PropertyType))
                    {
                        var itemType = GetEnumerableElement(prop.PropertyType);
                        if (itemType != null)
                        {
                            if (IsSimpleType(itemType))
                            {
                                ret.Add(prop);
                            }
                            else
                            {
                                if (null != itemType.GetCustomAttribute<RxPlatformDataType>())
                                {
                                    ret.Add(prop);
                                }
                            }
                        }
                    }
                    else
                    {
                        if(null != prop.PropertyType.GetCustomAttribute<RxPlatformDataType>())
                        {
                            ret.Add(prop);
                        }
                    }
                }
            }
            return ret.ToArray();
        }
        static internal Tuple<string?, RxPlatformObjectRuntime?> CreateObjectFromCode(ConstructorInfo? constructor)
        {
            var ret = new Tuple<string?, RxPlatformObjectRuntime?>(null, null);
            if (constructor != null)
            {
                if(constructor.DeclaringType!=null)
                {
                    Console.WriteLine($"*******{constructor.DeclaringType.FullName}\r\n*****");
                }
                var instance = constructor.Invoke(null) as RxPlatformObjectRuntime;
                if (instance != null)
                {
                    // You may want to return a tuple with actual values here
                    // For now, returning a tuple with nulls as in the original code
                    return new Tuple<string?, RxPlatformObjectRuntime?>(null, instance);
                }
            }
            return ret;
        }
        internal static bool IsRxDataType(Type type)
        {
            if(type == typeof(void))
            {
                return true;
            }
            var attr = type.GetCustomAttribute<RxPlatformDataType>(false);
            if (attr != null)
            {
                return true;
            }
            return false;
        }
        internal static MethodInfo[] GetDefinedMethods(Type type)
        {
            List<MethodInfo> ret = new List<MethodInfo>();
            var methodInfos = type.GetMethods(BindingFlags.Public | BindingFlags.Instance);
            foreach (var method in methodInfos)
            {
                if(method.DeclaringType != type)
                {
                    continue;
                }
                if(method.IsSpecialName || method.Name == "Started" || method.Name == "Stopping")
                {
                    continue;
                }
                if(method.GetCustomAttribute<RxPlatformMethod>()!=null)
                {
                    continue;
                }
                Type? paramType = null;
                Type? returnType = null;
                var parmsInfo = method.GetParameters();
                if(parmsInfo==null || parmsInfo.Length == 0)
                {
                    paramType = typeof(void);
                }
                if (parmsInfo!=null && parmsInfo.Length == 1 )
                {
                    paramType = Nullable.GetUnderlyingType(parmsInfo[0].ParameterType);
                }

                returnType = Nullable.GetUnderlyingType(method.ReturnType);
                if(returnType==null)
                {
                    returnType = typeof(void);
                }
                if (returnType != null && IsRxDataType(returnType) && paramType != null && IsRxDataType(paramType))
                {
                    ret.Add(method);
                }
            }
            return ret.ToArray();
        }
        static internal string BuildMethods(MethodInfo[] methods)
        {
            StringBuilder builder = new StringBuilder();
            foreach (var method in methods)
            {
                Type? paramType = null;
                Type? resultType = null;
                var parmsInfo = method.GetParameters();
                if (parmsInfo == null || parmsInfo.Length == 0)
                {
                    paramType = typeof(void);
                }
                else if (parmsInfo != null && parmsInfo.Length == 1)
                {
                    paramType = Nullable.GetUnderlyingType(parmsInfo[0].ParameterType);
                }

                resultType = Nullable.GetUnderlyingType(method.ReturnType);
                if(resultType==null)
                {
                    resultType = typeof(void);
                }
                if(paramType!=null && resultType!=null)
                { 
                    
                    string? nodeIdIn = null;
                    if (paramType == typeof(void))
                    {
                        unsafe
                        {
                            string_value_struct nodeIdStr;
                            rx_node_id_struct nodeId = CommonInterface.CreateNodeIdFromInt(HostPlatformIds.RX_CLASS_DATA_BASE_ID);
                            if (CommonInterface.rx_node_id_to_string(&nodeId, &nodeIdStr) > 0)
                            {
                                nodeIdIn = Marshal.PtrToStringUTF8(CommonInterface.rx_c_str(&nodeIdStr));
                                CommonInterface.rx_destory_string_value_struct(&nodeIdStr);
                            }
                            CommonInterface.rx_destory_node_id(&nodeId);
                        }
                    }
                    else
                    {
                        var attrIn = paramType.GetCustomAttribute<RxPlatformDataType>(false);
                        if (attrIn != null)
                        {
                            nodeIdIn = attrIn.NodeId.ToString();
                        }
                    }
                    if (nodeIdIn != null)
                    {
                        if (resultType != null)
                        {

                            string? nodeIdOut = null;
                            if (resultType == typeof(void))
                            {
                                unsafe
                                {
                                    string_value_struct nodeIdStr;
                                    rx_node_id_struct nodeId = CommonInterface.CreateNodeIdFromInt(HostPlatformIds.RX_CLASS_DATA_BASE_ID);
                                    if (CommonInterface.rx_node_id_to_string(&nodeId, &nodeIdStr) > 0)
                                    {
                                        nodeIdOut = Marshal.PtrToStringUTF8(CommonInterface.rx_c_str(&nodeIdStr));
                                        CommonInterface.rx_destory_string_value_struct(&nodeIdStr);
                                    }
                                    CommonInterface.rx_destory_node_id(&nodeId);
                                }
                            }
                            else
                            {
                                var attrIn = resultType.GetCustomAttribute<RxPlatformDataType>(false);
                                if (attrIn != null)
                                {
                                    nodeIdOut = attrIn.NodeId.ToString();
                                }
                            }
                            if (nodeIdOut != null)
                            {
                                string? targetIdStr = null;
                                unsafe
                                {
                                    string_value_struct nodeIdStr;
                                    rx_node_id_struct nodeId = CommonInterface.CreateNodeIdFromInt(HostPlatformIds.RX_DOTNET_METHOD_TYPE_ID);
                                    if (CommonInterface.rx_node_id_to_string(&nodeId, &nodeIdStr) > 0)
                                    {
                                        targetIdStr = Marshal.PtrToStringUTF8(CommonInterface.rx_c_str(&nodeIdStr));
                                        CommonInterface.rx_destory_string_value_struct(&nodeIdStr);
                                    }
                                    CommonInterface.rx_destory_node_id(&nodeId);
                                }

                                RxMethodDataItem methodDataItem = new RxMethodDataItem
                                {
                                    name = method.Name,
                                    inType = new RXHostReferenceId { id = nodeIdIn },
                                    outType = new RXHostReferenceId { id = nodeIdOut },
                                    description = "",
                                    target = new RXHostReferenceId { id = targetIdStr }

                                };

                                if (builder.Length > 0)
                                {
                                    builder.Append(", ");
                                }
                                string jsonString = JsonSerializer.Serialize(methodDataItem);
                                builder.AppendLine(jsonString);
                            }
                        }
                    }
                }
            }
            return builder.ToString();
        }

        internal static bool IsAsyncMethod(MethodInfo method)
        {
            bool returnValue = method.GetCustomAttribute(typeof(AsyncStateMachineAttribute)) != null;
            if(!returnValue)
            {
                if (method.ReturnType == typeof(Task) || (method.ReturnType.IsGenericType && method.ReturnType.GetGenericTypeDefinition() == typeof(Task<>)))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return returnValue;
        }
    }
}
