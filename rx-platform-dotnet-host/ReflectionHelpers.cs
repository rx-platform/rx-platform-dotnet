using ENSACO.RxPlatform.Attributes;
using ENSACO.RxPlatform.Hosting.Common;
using ENSACO.RxPlatform.Hosting.Model.Items;
using ENSACO.RxPlatform.Runtime;
using System.Collections;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace ENSACO.RxPlatform.Hosting.Reflection
{

    internal static class ReflectionHelpers
    {
        private static Func<T> CreateInstance<T>() where T : new()
        {
            // This pattern can be cached in a static dictionary for performance if needed.

            // 1. Get the constructor information for the type T with no parameters.
            ConstructorInfo constructorInfo = typeof(T).GetConstructor(Type.EmptyTypes);

            if (constructorInfo == null)
            {
                // If the type is a value type (like int or a struct), use Expression.Default
                if (typeof(T).IsValueType)
                {
                    return Expression.Lambda<Func<T>>(Expression.Default(typeof(T))).Compile();
                }
                throw new InvalidOperationException($"The type {typeof(T).Name} must have a parameterless constructor.");
            }

            // 2. Create a NewExpression that represents calling the constructor.
            NewExpression newExpression = Expression.New(constructorInfo);

            // 3. Create a Lambda expression, compile it into a delegate, and return it.
            Func<T> function = Expression.Lambda<Func<T>>(newExpression).Compile();
            return function;
        }
        static internal Func<object>? CreateConstructorFunc(Type type)
        {
            
            if (type.IsGenericType)
            {
                Type instType = type.MakeGenericType(new Type[] { typeof(int) });

                ConstructorInfo? ctor = instType.GetConstructor(Type.EmptyTypes);
                if (ctor == null)
                    return null;

                NewExpression newExp = Expression.New(ctor);
                var lambda = Expression.Lambda<Func<object>>(newExp);
                return lambda.Compile();
            }
            else
            {
                ConstructorInfo? ctor = type.GetConstructor(Type.EmptyTypes);
                if (ctor == null)
                {
                    if (type.IsValueType)
                    {
                        var lambda = Expression.Lambda<Func<object>>(Expression.Convert(Expression.Default(type), typeof(object)));
                        return lambda.Compile();
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    NewExpression newExp = Expression.New(type);
                    var lambda = Expression.Lambda<Func<object>>(newExp);
                    return lambda.Compile();
                }
            }
        }
        internal static RxHostValueType GetVariableValue(PropertyInfo prop, Type propType, object? value)
        {
            RxHostValueType rxValue = new RxHostValueType();
            if(value!=null)
            {
                var varProp = propType.GetProperty("_");
                if (varProp != null)
                {
                    var varVal = varProp.GetValue(value);
                    rxValue = GetValue(varProp, varProp.PropertyType, varVal);
                }
            }
            // Implement logic to extract variable value from the property
            return rxValue;
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
        static private bool IsVariableType(Type type)
        {
            if(type.IsGenericType && type.GetCustomAttribute<RxPlatformVariableType>()!=null)
            {
                return true;
            }
            return false;
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

        static internal bool IsNullable(PropertyInfo prop)
        {
            return GetNullableType(prop) != null;
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
        static internal PropertyInfo[] GetAllRelationsPropertyInfos(Type type)
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
                if (!IsVirtual(prop))
                {
                    continue;
                }
                Type? propType = Nullable.GetUnderlyingType(prop.PropertyType);
                if (propType == null)
                {
                    propType = prop.PropertyType;
                }
                if (propType != null && propType.IsSubclassOf(typeof(RxPlatformObjectRuntime)))
                {
                    ret.Add(prop);
                    continue;
                }
            }
            return ret.ToArray();
        }
        static internal PropertyInfo[] GetRelationsPropertyInfos(Type type, bool ownOnly)
        {
            List<PropertyInfo> ret = new List<PropertyInfo>();
            var propertyInfos = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var prop in propertyInfos)
            {
                if(prop.DeclaringType != type)
                {
                    continue;
                }
                if (!ownOnly)
                {
                    var relAttr = prop.GetCustomAttribute<RxPlatformRelationAttribute>();
                    if (relAttr != null)
                    {
                        ret.Add(prop);
                        continue;
                    }
                }
                if(!IsVirtual(prop))
                {
                    continue;
                }
                Type? propType = Nullable.GetUnderlyingType(prop.PropertyType);
                if (propType == null)
                {
                    propType = prop.PropertyType;
                }
                if (propType != null && propType.IsSubclassOf(typeof(RxPlatformObjectRuntime)))
                {
                    ret.Add(prop);
                    continue;
                }
            }
            return ret.ToArray();
        }
        static internal string? RxResultType(Type type)
        {
            string? typeName = type.FullName;
            if (type.IsGenericType && typeName != null)
            {
                Type[] genericArguments = type.GetGenericArguments();
                if (genericArguments.Length == 1)
                {
                    var idx = typeName.IndexOf('`');
                    if (idx != -1)
                    {
                        typeName = $"{typeName.Substring(0, idx)}<{genericArguments[0].FullName}?>";
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            return typeName;
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
            if (methodInfo != null && methodInfo.ReturnType == typeof(Task<bool>) && IsVirtual(methodInfo))
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
        static internal PropertyInfo[] GetSimplePropertyInfos(Type type, bool includeStructs)
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
                            else if(IsVariableType(itemType))
                            {
                                ret.Add(prop);
                            }
                            else
                            {
                                if (null != itemType.GetCustomAttribute<RxPlatformDataType>())
                                {
                                    ret.Add(prop);
                                }
                                else if (includeStructs && null != prop.PropertyType.GetCustomAttribute<RxPlatformStructType>())
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
                        else if (IsVariableType(prop.PropertyType))
                        {
                            ret.Add(prop);
                        }
                        else if (includeStructs && null != prop.PropertyType.GetCustomAttribute<RxPlatformStructType>())
                        {
                            ret.Add(prop);
                        }
                    }
                }
            }
            return ret.ToArray();
        }

        static internal PropertyInfo[] GetSourcePropertyInfos(Type type)
        {
            List<PropertyInfo> ret = new List<PropertyInfo>();
            var propertyInfos = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var prop in propertyInfos)
            {
                if (prop.DeclaringType == type)
                {
                    Type? propType = Nullable.GetUnderlyingType(prop.PropertyType);
                    if (propType == null)
                    {
                        propType = prop.PropertyType;
                    }
                    if (!IsSimpleType(propType))
                    {
                        if (null != prop.PropertyType.GetCustomAttribute<RxPlatformSourceType>())
                        {
                            ret.Add(prop);
                        }
                    }
                }
            }
            return ret.ToArray();
        }

        static internal PropertyInfo[] GetMapperPropertyInfos(Type type)
        {
            List<PropertyInfo> ret = new List<PropertyInfo>();
            var propertyInfos = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var prop in propertyInfos)
            {
                if (prop.DeclaringType == type)
                {
                    Type? propType = Nullable.GetUnderlyingType(prop.PropertyType);
                    if (propType == null)
                    {
                        propType = prop.PropertyType;
                    }
                    if (!IsSimpleType(propType))
                    {
                        if (null != prop.PropertyType.GetCustomAttribute<RxPlatformMapperType>())
                        {
                            ret.Add(prop);
                        }
                    }
                }
            }
            return ret.ToArray();
        }
        static internal PropertyInfo[] GetFilterPropertyInfos(Type type)
        {
            List<PropertyInfo> ret = new List<PropertyInfo>();
            var propertyInfos = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var prop in propertyInfos)
            {
                if (prop.DeclaringType == type)
                {
                    Type? propType = Nullable.GetUnderlyingType(prop.PropertyType);
                    if (propType == null)
                    {
                        propType = prop.PropertyType;
                    }
                    if (!IsSimpleType(propType))
                    {
                        if (null != prop.PropertyType.GetCustomAttribute<RxPlatformFilterType>())
                        {
                            ret.Add(prop);
                        }
                    }
                }
            }
            return ret.ToArray();
        }
        static internal PropertyInfo[] GetStructPropertyInfos(Type type)
        {
            List<PropertyInfo> ret = new List<PropertyInfo>();
            var propertyInfos = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var prop in propertyInfos)
            {
                if (prop.DeclaringType == type)
                {
                    Type? propType = Nullable.GetUnderlyingType(prop.PropertyType);
                    if (propType == null)
                    {
                        propType = prop.PropertyType;
                    }
                    if (!IsSimpleType(propType))
                    {
                        if (null != prop.PropertyType.GetCustomAttribute<RxPlatformStructType>())
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
                if(method.GetCustomAttribute<RxPlatformMethodType>()!=null)
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

        internal static bool IsRxPlatformResultDelegate(Type type)
        {
            if(type.IsSubclassOf(typeof(Delegate)))
            {
                MethodInfo? invokeMethod = type.GetMethod("Invoke");
                if (invokeMethod != null)
                {
                    var parameters = invokeMethod.GetParameters();
                    if (parameters.Length == 1)
                    {
                        var ttempType = GetNullableType(parameters[0]);
                        if(ttempType != null && ttempType == typeof(Exception))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        internal static MethodInfo[] GetSourceWriteMethods(Type type)
        {
            List<MethodInfo> ret = new List<MethodInfo>();
            var methodInfos = type.GetMethods(BindingFlags.Public | BindingFlags.Instance);
            foreach (var method in methodInfos)
            {
                if (method.DeclaringType != type)
                {
                    continue;
                }
                if (method.Name == "SourceWrite" 
                    && method.ReturnType == typeof(void))
                {
                    var paramsInfo = method.GetParameters();
                    if(paramsInfo.Length != 2)
                    {
                        continue;
                    }

                    Type paramType = paramsInfo[0].ParameterType;
                    Type param2Type = paramsInfo[1].ParameterType;
                    if(IsRxPlatformResultDelegate(param2Type))
                    {
                        ret.Add(method);
                    }
                }
            }
            return ret.ToArray();
        }
    }
}
