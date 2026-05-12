using ENSACO.RxPlatform.Attributes;
using ENSACO.RxPlatform.Hosting.Common;
using ENSACO.RxPlatform.Hosting.Model.Items;
using ENSACO.RxPlatform.Runtime;
using System.Collections;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace ENSACO.RxPlatform.Hosting.Reflection
{

    internal static class ReflectionHelpers
    {
        internal static Type[] GetBaseRuntimeTypes(Type type)
        {
            List<Type> baseTypes = new List<Type>();
            Type? currentType = type;
            while (currentType != null && currentType != typeof(object))
            {
                baseTypes.Add(currentType);
                currentType = currentType.BaseType;
                if(currentType == null || currentType.BaseType!=null && currentType.BaseType == typeof(RxPlatformRuntimeBase))
                {
                    break;
                }
            }
            if(baseTypes.Count == 0)
            {
                return Array.Empty<Type>();
            }
            Type[] retval = new Type[baseTypes.Count];
            for (int i= baseTypes.Count - 1; i >= 0 ; i--)
            {
                retval[i]= baseTypes[i];
            }
            return retval;
        }
        static internal Func<string?, object?>? CreateConstructorFunc(Type type)
        {
            Func<string?, object?>? ctorFunc = (string? initial) =>
            {
                if(string.IsNullOrEmpty(initial))
                {
                    var defaultCtor = CreateConstructorFuncDefault(type);
                    if (defaultCtor != null)
                    {
                        return defaultCtor();
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    JsonSerializerOptions options = new JsonSerializerOptions
                    {                        
                        UnmappedMemberHandling = JsonUnmappedMemberHandling.Skip,
                        TypeInfoResolver = new DefaultJsonTypeInfoResolver
                        {
                            Modifiers = { (JsonTypeInfo typeInfo) => {
                                foreach (var property in typeInfo.Properties) {
                                    // Logic to check type and ignore if necessary
                                    if (IsElemntToSkip(property))
                                    { 
                                        property.Set = null; // Effectively ignores during deserialization
                                    }
                                }
                            }}
                        }
                    };
                    return JsonSerializer.Deserialize(initial, type, options);
                }
            };
            return ctorFunc;
        }
        static internal Func<object>? CreateConstructorFuncDefault(Type type)
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
                    if(type.IsValueType)
                    {
                        var lambda = Expression.Lambda<Func<object>>(Expression.Convert(Expression.New(ctor), typeof(object)));
                        return lambda.Compile();
                    }
                    else
                    {
                        NewExpression newExp = Expression.New(type);
                        var lambda = Expression.Lambda<Func<object>>(newExp);
                        return lambda.Compile();
                    }
                }
            }
        }
        internal static RxHostValueType GetVariableValue(PropertyInfo prop, Type propType, object? value, int array)
        {
            RxHostValueType rxValue = new RxHostValueType();
            if (value != null)
            {
                var varProp = propType.GetProperty("_");
                if (varProp != null)
                {
                    var varVal = varProp.GetValue(value);
                    rxValue = GetValue(varProp, varProp.PropertyType, varVal, array);
                }
            }
            // Implement logic to extract variable value from the property
            return rxValue;
        }
        private static object? GetSimpleValue(PropertyInfo prop, Type propType, object? value)
        {
            object? rxVal = null;
            switch (propType)
            {
                case Type t when t == typeof(bool):
                    rxVal = value == null ? false : (bool?)value;
                    break;
                case Type t when t == typeof(string):
                    rxVal = value == null ? "" : (string?)value;
                    break;
                case Type t when t == typeof(float):
                    rxVal = value == null ? 0.0f : (float?)value;
                    break;
                case Type t when t == typeof(double):
                    rxVal = value == null ? 0.0 : (double?)value;
                    break;
                case Type t when t == typeof(char):
                    rxVal = value == null ? (sbyte)0 : (sbyte?)(sbyte)(char?)value;
                    break;
                case Type t when t == typeof(DateTime):
                    rxVal = value == null ? null : (DateTime?)value;
                    break;
                case Type t when t == typeof(Guid):
                    rxVal = (value == null ? Guid.Empty : (Guid)value).ToString();
                    break;
                case Type t when t == typeof(sbyte):
                    rxVal = value == null ? (sbyte)0 : (sbyte?)value;
                    break;
                case Type t when t == typeof(short):
                    rxVal = value == null ? (short)0 : (short?)value;
                    break;
                case Type t when t == typeof(int):
                    rxVal = value == null ? (int)0 : (int?)value;
                    break;
                case Type t when t == typeof(long):
                    rxVal = value == null ? (long)0 : (long?)value;
                    break;
                case Type t when t == typeof(byte):
                    rxVal = value == null ? (byte)0 : (byte?)value;
                    break;
                case Type t when t == typeof(ushort):
                    rxVal = value == null ? (ushort)0 : (ushort?)value;
                    break;
                case Type t when t == typeof(uint):
                    rxVal = value == null ? (uint)0 : (uint?)value;
                    break;
                case Type t when t == typeof(ulong):
                    rxVal = value == null ? (ulong)0 : (ulong?)value;
                    break;
                default:
                    throw new Exception($"Unsupported property type {prop.PropertyType.FullName} for property {prop.Name}");
            }
            return rxVal;
        }
        internal static RxHostValueType GetValue(PropertyInfo prop, Type propType, object? value, int array)
        {
            if (array >= 0)
            {
                RxHostValueType rxValue = new RxHostValueType();
                switch (propType)
                {
                    case Type t when t == typeof(bool):
                        {
                            rxValue.type = rx_value_t.Bool | rx_value_t.ArrayFlag;
                            if (value == null || array == 0)
                            {
                                rxValue.val = new bool[0];
                            }
                            else
                            {
                                bool[] arr = new bool[array];
                                int idx = 0;
                                foreach (var item in (IEnumerable)value)
                                {
                                    if (item is bool && idx < array)
                                    {
                                        object? tempVal = GetSimpleValue(prop, propType, (object?)item);
                                        if (tempVal == null)
                                        {
                                            arr[idx] = false;
                                        }
                                        else
                                        {
                                            arr[idx] = (bool)tempVal;
                                        }
                                        idx++;
                                    }
                                    else
                                    {
                                        throw new Exception($"Expected an array of bool but got an array of {item.GetType().FullName} for property {prop.Name}");
                                    }
                                }
                                rxValue.val = arr;
                            }
                        }
                        break;
                    case Type t when t == typeof(string):
                        {
                            rxValue.type = rx_value_t.String | rx_value_t.ArrayFlag;
                            if (value == null || array == 0)
                            {
                                rxValue.val = new string[0];
                            }
                            else
                            {
                                string[] arr = new string[array];
                                int idx = 0;
                                foreach (var item in (IEnumerable)value)
                                {
                                    if (item is string && idx < array)
                                    {
                                        object? tempVal = GetSimpleValue(prop, propType, (object?)item);
                                        if (tempVal == null)
                                        {
                                            arr[idx] = "";
                                        }
                                        else
                                        {
                                            arr[idx] = (string)tempVal;
                                        }
                                        idx++;
                                    }
                                    else
                                    {
                                        throw new Exception($"Expected an array of string but got an array of {item.GetType().FullName} for property {prop.Name}");
                                    }
                                }
                                rxValue.val = arr;
                            }
                        }
                        break;
                    case Type t when t == typeof(float):
                        {
                            rxValue.type = rx_value_t.Float | rx_value_t.ArrayFlag;
                            if (value == null || array == 0)
                            {
                                rxValue.val = new float[0];
                            }
                            else
                            {
                                float[] arr = new float[array];
                                int idx = 0;
                                foreach (var item in (IEnumerable)value)
                                {
                                    if (item is float && idx < array)
                                    {
                                        object? tempVal = GetSimpleValue(prop, propType, (object?)item);
                                        if (tempVal == null)
                                        {
                                            arr[idx] = 0.0f;
                                        }
                                        else
                                        {
                                            arr[idx] = (float)tempVal;
                                        }
                                        idx++;
                                    }
                                    else
                                    {
                                        throw new Exception($"Expected an array of float but got an array of {item.GetType().FullName} for property {prop.Name}");
                                    }
                                }
                                rxValue.val = arr;
                            }
                        }
                        break;
                    case Type t when t == typeof(double):
                        {
                            rxValue.type = rx_value_t.Double | rx_value_t.ArrayFlag;
                            if (value == null || array == 0)
                            {
                                rxValue.val = new double[0];
                            }
                            else
                            {
                                double[] arr = new double[array];
                                int idx = 0;
                                foreach (var item in (IEnumerable)value)
                                {
                                    if (item is double && idx < array)
                                    {
                                        object? tempVal = GetSimpleValue(prop, propType, (object?)item);
                                        if (tempVal == null)
                                        {
                                            arr[idx] = 0.0;
                                        }
                                        else
                                        {
                                            arr[idx] = (double)tempVal;
                                        }
                                        idx++;
                                    }
                                    else
                                    {
                                        throw new Exception($"Expected an array of double but got an array of {item.GetType().FullName} for property {prop.Name}");
                                    }
                                }
                                rxValue.val = arr;
                            }
                        }
                        break;
                    case Type t when t == typeof(sbyte):
                        {
                            rxValue.type = rx_value_t.Int8 | rx_value_t.ArrayFlag;
                            if (value == null || array == 0)
                            {
                                rxValue.val = new sbyte[0];
                            }
                            else
                            {
                                sbyte[] arr = new sbyte[array];
                                int idx = 0;
                                foreach (var item in (IEnumerable)value)
                                {
                                    if (item is sbyte && idx < array)
                                    {
                                        object? tempVal = GetSimpleValue(prop, propType, (object?)item);
                                        if (tempVal == null)
                                        {
                                            arr[idx] = 0;
                                        }
                                        else
                                        {
                                            arr[idx] = (sbyte)tempVal;
                                        }
                                        idx++;
                                    }
                                    else
                                    {
                                        throw new Exception($"Expected an array of sbyte but got an array of {item.GetType().FullName} for property {prop.Name}");
                                    }
                                }
                                rxValue.val = arr;
                            }
                        }
                        break;
                    case Type t when t == typeof(short):
                        {
                            rxValue.type = rx_value_t.Int16 | rx_value_t.ArrayFlag;
                            if (value == null || array == 0)
                            {
                                rxValue.val = new short[0];
                            }
                            else
                            {
                                short[] arr = new short[array];
                                int idx = 0;
                                foreach (var item in (IEnumerable)value)
                                {
                                    if (item is short && idx < array)
                                    {
                                        object? tempVal = GetSimpleValue(prop, propType, (object?)item);
                                        if (tempVal == null)
                                        {
                                            arr[idx] = 0;
                                        }
                                        else
                                        {
                                            arr[idx] = (short)tempVal;
                                        }
                                        idx++;
                                    }
                                    else
                                    {
                                        throw new Exception($"Expected an array of short but got an array of {item.GetType().FullName} for property {prop.Name}");
                                    }
                                }
                                rxValue.val = arr;
                            }
                        }
                        break;
                    case Type t when t == typeof(int):
                        {
                            rxValue.type = rx_value_t.Int32 | rx_value_t.ArrayFlag;
                            if (value == null || array == 0)
                            {
                                rxValue.val = new int[0];
                            }
                            else
                            {
                                int[] arr = new int[array];
                                int idx = 0;
                                foreach (var item in (IEnumerable)value)
                                {
                                    if (item is int && idx < array)
                                    {
                                        object? tempVal = GetSimpleValue(prop, propType, (object?)item);
                                        if (tempVal == null)
                                        {
                                            arr[idx] = 0;
                                        }
                                        else
                                        {
                                            arr[idx] = (int)tempVal;
                                        }
                                        idx++;
                                    }
                                    else
                                    {
                                        throw new Exception($"Expected an array of int but got an array of {item.GetType().FullName} for property {prop.Name}");
                                    }
                                }
                                rxValue.val = arr;
                            }
                        }
                        break;
                    case Type t when t == typeof(long):
                        {
                            rxValue.type = rx_value_t.Int64 | rx_value_t.ArrayFlag;
                            if (value == null || array == 0)
                            {
                                rxValue.val = new long[0];
                            }
                            else
                            {
                                long[] arr = new long[array];
                                int idx = 0;
                                foreach (var item in (IEnumerable)value)
                                {
                                    if (item is long && idx < array)
                                    {
                                        object? tempVal = GetSimpleValue(prop, propType, (object?)item);
                                        if (tempVal == null)
                                        {
                                            arr[idx] = 0;
                                        }
                                        else
                                        {
                                            arr[idx] = (long)tempVal;
                                        }
                                        idx++;
                                    }
                                    else
                                    {
                                        throw new Exception($"Expected an array of long but got an array of {item.GetType().FullName} for property {prop.Name}");
                                    }
                                }
                                rxValue.val = arr;
                            }
                        }
                        break;
                    case Type t when t == typeof(byte):
                        {
                            rxValue.type = rx_value_t.UInt8 | rx_value_t.ArrayFlag;
                            if (value == null || array == 0)
                            {
                                rxValue.val = new byte[0];
                            }
                            else
                            {
                                byte[] arr = new byte[array];
                                int idx = 0;
                                foreach (var item in (IEnumerable)value)
                                {
                                    if (item is byte && idx < array)
                                    {
                                        object? tempVal = GetSimpleValue(prop, propType, (object?)item);
                                        if (tempVal == null)
                                        {
                                            arr[idx] = 0;
                                        }
                                        else
                                        {
                                            arr[idx] = (byte)tempVal;
                                        }
                                        idx++;
                                    }
                                    else
                                    {
                                        throw new Exception($"Expected an array of byte but got an array of {item.GetType().FullName} for property {prop.Name}");
                                    }
                                }
                                rxValue.val = arr;
                            }
                        }
                        break;
                    case Type t when t == typeof(ushort):
                        {
                            rxValue.type = rx_value_t.UInt16 | rx_value_t.ArrayFlag;
                            if (value == null || array == 0)
                            {
                                rxValue.val = new ushort[0];
                            }
                            else
                            {
                                ushort[] arr = new ushort[array];
                                int idx = 0;
                                foreach (var item in (IEnumerable)value)
                                {
                                    if (item is ushort && idx < array)
                                    {
                                        object? tempVal = GetSimpleValue(prop, propType, (object?)item);
                                        if (tempVal == null)
                                        {
                                            arr[idx] = 0;
                                        }
                                        else
                                        {
                                            arr[idx] = (ushort)tempVal;
                                        }
                                        idx++;
                                    }
                                    else
                                    {
                                        throw new Exception($"Expected an array of ushort but got an array of {item.GetType().FullName} for property {prop.Name}");
                                    }
                                }
                                rxValue.val = arr;
                            }
                        }
                        break;
                    case Type t when t == typeof(uint):
                        {
                            rxValue.type = rx_value_t.UInt32 | rx_value_t.ArrayFlag;
                            if (value == null || array == 0)
                            {
                                rxValue.val = new uint[0];
                            }
                            else
                            {
                                uint[] arr = new uint[array];
                                int idx = 0;
                                foreach (var item in (IEnumerable)value)
                                {
                                    if (item is uint && idx < array)
                                    {
                                        object? tempVal = GetSimpleValue(prop, propType, (object?)item);
                                        if (tempVal == null)
                                        {
                                            arr[idx] = 0;
                                        }
                                        else
                                        {
                                            arr[idx] = (uint)tempVal;
                                        }
                                        idx++;
                                    }
                                    else
                                    {
                                        throw new Exception($"Expected an array of uint but got an array of {item.GetType().FullName} for property {prop.Name}");
                                    }
                                }
                                rxValue.val = arr;
                            }
                        }
                        break;
                    case Type t when t == typeof(ulong):
                        {
                            rxValue.type = rx_value_t.UInt64 | rx_value_t.ArrayFlag;
                            if (value == null || array == 0)
                            {
                                rxValue.val = new ulong[0];
                            }
                            else
                            {
                                ulong[] arr = new ulong[array];
                                int idx = 0;
                                foreach (var item in (IEnumerable)value)
                                {
                                    if (item is ulong && idx < array)
                                    {
                                        object? tempVal = GetSimpleValue(prop, propType, (object?)item);
                                        if (tempVal == null)
                                        {
                                            arr[idx] = 0;
                                        }
                                        else
                                        {
                                            arr[idx] = (ulong)tempVal;
                                        }
                                        idx++;
                                    }
                                    else
                                    {
                                        throw new Exception($"Expected an array of ulong but got an array of {item.GetType().FullName} for property {prop.Name}");
                                    }
                                }
                                rxValue.val = arr;
                            }
                        }
                        break;
                    case Type t when t == typeof(DateTime):
                        {
                            rxValue.type = rx_value_t.Time | rx_value_t.ArrayFlag;
                            if (value == null || array == 0)
                            {
                                rxValue.val = new DateTime[0];
                            }
                            else
                            {
                                DateTime[] arr = new DateTime[array];
                                int idx = 0;
                                foreach (var item in (IEnumerable)value)
                                {
                                    if (item is DateTime && idx < array)
                                    {
                                        object? tempVal = GetSimpleValue(prop, propType, (object?)item);
                                        if (tempVal == null)
                                        {
                                            arr[idx] = DateTime.MinValue;
                                        }
                                        else
                                        {
                                            arr[idx] = (DateTime)tempVal;
                                        }
                                        idx++;
                                    }
                                    else
                                    {
                                        throw new Exception($"Expected an array of DateTime but got an array of {item.GetType().FullName} for property {prop.Name}");
                                    }
                                }
                                rxValue.val = arr;
                            }
                        }
                        break;
                    case Type t when t == typeof(Guid):
                        {
                            rxValue.type = rx_value_t.Uuid | rx_value_t.ArrayFlag;
                            if (value == null || array == 0)
                            {
                                rxValue.val = new string[0];
                            }
                            else
                            {
                                string[] arr = new string[array];
                                int idx = 0;
                                foreach (var item in (IEnumerable)value)
                                {
                                    if (item is Guid && idx < array)
                                    {
                                        object? tempVal = GetSimpleValue(prop, propType, (object?)item);
                                        if (tempVal == null)
                                        {
                                            arr[idx] = Guid.Empty.ToString();
                                        }
                                        else
                                        {
                                            arr[idx] = (string)tempVal;
                                        }
                                        idx++;
                                    }
                                    else
                                    {
                                        throw new Exception($"Expected an array of Guid but got an array of {item.GetType().FullName} for property {prop.Name}");
                                    }
                                }
                                rxValue.val = arr;
                            }
                        }
                        break;
                    default:
                        throw new Exception($"Unsupported property type {prop.PropertyType.FullName} for property {prop.Name}");
                }
                return rxValue;
            }
            else
            {
                RxHostValueType rxValue = new RxHostValueType();
                switch (propType)
                {
                    case Type t when t == typeof(bool):
                        rxValue.type = rx_value_t.Bool;
                        rxValue.val = GetSimpleValue(prop, propType, value);
                        break;
                    case Type t when t == typeof(string):
                        rxValue.type = rx_value_t.String;
                        rxValue.val = GetSimpleValue(prop, propType, value);
                        break;
                    case Type t when t == typeof(float):
                        rxValue.type = rx_value_t.Float;
                        rxValue.val = GetSimpleValue(prop, propType, value);
                        break;
                    case Type t when t == typeof(double):
                        rxValue.type = rx_value_t.Double;
                        rxValue.val = GetSimpleValue(prop, propType, value);
                        break;
                    case Type t when t == typeof(char):
                        rxValue.type = rx_value_t.Int8;
                        rxValue.val = GetSimpleValue(prop, propType, value);
                        break;
                    case Type t when t == typeof(DateTime):
                        rxValue.type = rx_value_t.Time;
                        rxValue.val = GetSimpleValue(prop, propType, value);
                        break;
                    case Type t when t == typeof(Guid):
                        rxValue.type = rx_value_t.Uuid;
                        rxValue.val = GetSimpleValue(prop, propType, value);
                        break;
                    case Type t when t == typeof(sbyte):
                        rxValue.type = rx_value_t.Int8;
                        rxValue.val = GetSimpleValue(prop, propType, value);
                        break;
                    case Type t when t == typeof(short):
                        rxValue.type = rx_value_t.Int16;
                        rxValue.val = GetSimpleValue(prop, propType, value);
                        break;
                    case Type t when t == typeof(int):
                        rxValue.type = rx_value_t.Int32;
                        rxValue.val = GetSimpleValue(prop, propType, value);
                        break;
                    case Type t when t == typeof(long):
                        rxValue.type = rx_value_t.Int64;
                        rxValue.val = GetSimpleValue(prop, propType, value);
                        break;
                    case Type t when t == typeof(byte):
                        rxValue.type = rx_value_t.UInt8;
                        rxValue.val = GetSimpleValue(prop, propType, value);
                        break;
                    case Type t when t == typeof(ushort):
                        rxValue.type = rx_value_t.UInt16;
                        rxValue.val = GetSimpleValue(prop, propType, value);
                        break;
                    case Type t when t == typeof(uint):
                        rxValue.type = rx_value_t.UInt32;
                        rxValue.val = GetSimpleValue(prop, propType, value);
                        break;
                    case Type t when t == typeof(ulong):
                        rxValue.type = rx_value_t.UInt64;
                        rxValue.val = GetSimpleValue(prop, propType, value);
                        break;
                    default:
                        throw new Exception($"Unsupported property type {prop.PropertyType.FullName} for property {prop.Name}");
                }
                return rxValue;
            }
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
            if (type.IsGenericType && type.GetCustomAttribute<RxPlatformVariableType>() != null)
            {
                return true;
            }
            return false;
        }
        static private bool IsEventType(Type type)
        {
            if (type.GetCustomAttribute<RxPlatformEventType>() != null)
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
                if (info.WriteState == NullabilityState.Nullable || info.ReadState == NullabilityState.Nullable)
                {
                    propType = prop.PropertyType;
                }
            }
            return propType;
        }
        static internal Type? GetTaskType(Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Task<>))
            {
                return type.GetGenericArguments()[0];
            }
            if (type == typeof(Task))
            {
                return typeof(void); // Or null, depending on your needs
            }
            return null;
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
                var ignoreAttr = prop.GetCustomAttribute<RxPlatformIgnoreAttribute>();
                if (ignoreAttr != null)
                {
                    continue;
                }
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
        static internal bool IsElemntToSkip(JsonPropertyInfo type)
        {
            Type? propType = Nullable.GetUnderlyingType(type.PropertyType);
            if (propType == null)
            {
                propType = type.PropertyType;
            }
            if (propType.IsSubclassOf(typeof(RxPlatformRuntimeBase)))
            {
                return true;
            }
            if (type.AttributeProvider!=null)
            {
                if(type.AttributeProvider.IsDefined(typeof(RxPlatformRelationAttribute), false))
                {
                    return true;
                }
            }
            return false;
        }
        static internal PropertyInfo[] GetRelationsPropertyInfos(Type type, bool ownOnly)
        {
            List<PropertyInfo> ret = new List<PropertyInfo>();
            var propertyInfos = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var prop in propertyInfos)
            {
                var ignoreAttr = prop.GetCustomAttribute<RxPlatformIgnoreAttribute>();
                if (ignoreAttr != null)
                {
                    continue;
                }
                if (prop.DeclaringType != type)
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
            var eventInfo = type.GetEvent(eventName);
            if (eventInfo != null && eventInfo.EventHandlerType != null)
            {
                string? typeName = eventInfo.EventHandlerType.FullName;
                if (eventInfo.EventHandlerType.IsGenericType)
                {
                    Type[] genericArguments = eventInfo.EventHandlerType.GetGenericArguments();
                    if (genericArguments.Length == 1 && genericArguments[0] == prop.PropertyType
                        && eventInfo.EventHandlerType.FullName != null)
                    {
                        string? arg = genericArguments[0].FullName;
                        Type? nulTpe = GetNullableType(prop);
                        if (nulTpe != null)
                        {
                            arg = nulTpe.FullName;
                        }
                        var idx = eventInfo.EventHandlerType.FullName.IndexOf('`');
                        if (idx != -1)
                        {
                            typeName = $"{eventInfo.EventHandlerType.FullName.Substring(0, idx)}<{arg}?>";
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
                var addMethod = eventInfo.GetAddMethod();
                if(addMethod == null || !addMethod.IsPublic || !addMethod.IsVirtual || addMethod.IsFinal)
                {
                    return null;
                }
                var method = eventInfo.EventHandlerType.GetMethod("Invoke");
                if (method != null && method.ReturnType == typeof(void))
                {
                    var parameters = method.GetParameters();
                    if (parameters.Length == 1)
                    {
                        if (parameters[0].ParameterType == prop.PropertyType)
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
                    Type? propType = Nullable.GetUnderlyingType(prop.PropertyType);
                    if (propType == null)
                    {
                        propType = prop.PropertyType;
                    }
                    if (parameters[0].ParameterType == propType)
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
                var ignoreAttr = prop.GetCustomAttribute<RxPlatformIgnoreAttribute>();
                if (ignoreAttr != null)
                {
                    continue;
                }
                if (prop.DeclaringType == type)
                {
                    Type? propType = Nullable.GetUnderlyingType(prop.PropertyType);
                    if (propType == null)
                    {
                        propType = prop.PropertyType;
                    }
                    if (IsSimpleType(propType))
                    {
                        ret.Add(prop);
                    }
                    else if (IsEnumerableType(prop.PropertyType))
                    {
                        var itemType = GetEnumerableElement(prop.PropertyType);
                        if (itemType != null)
                        {
                            if (IsSimpleType(itemType))
                            {
                                ret.Add(prop);
                            }
                            else if (IsVariableType(itemType))
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
                        if (null != prop.PropertyType.GetCustomAttribute<RxPlatformDataType>())
                        {
                            ret.Add(prop);
                        }
                        else if (IsVariableType(prop.PropertyType))
                        {
                            ret.Add(prop);
                        }
                        else if (includeStructs && IsEventType(prop.PropertyType))
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
                var ignoreAttr = prop.GetCustomAttribute<RxPlatformIgnoreAttribute>();
                if (ignoreAttr != null)
                {
                    continue;
                }
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
        static internal PropertyInfo[] GetDisplayPropertyInfos(Type type)
        {
            List<PropertyInfo> ret = new List<PropertyInfo>();
            var propertyInfos = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var prop in propertyInfos)
            {
                var ignoreAttr = prop.GetCustomAttribute<RxPlatformIgnoreAttribute>();
                if (ignoreAttr != null)
                {
                    continue;
                }
                if (prop.DeclaringType == type)
                {
                    Type? propType = Nullable.GetUnderlyingType(prop.PropertyType);
                    if (propType == null)
                    {
                        continue;
                    }
                    propType = prop.PropertyType;
                    if (null != prop.PropertyType.GetCustomAttribute<RxPlatformDisplayType>())
                    {
                        ret.Add(prop);
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
                var ignoreAttr = prop.GetCustomAttribute<RxPlatformIgnoreAttribute>();
                if (ignoreAttr != null)
                {
                    continue;
                }
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
                var ignoreAttr = prop.GetCustomAttribute<RxPlatformIgnoreAttribute>();
                if (ignoreAttr != null)
                {
                    continue;
                }
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
                var ignoreAttr = prop.GetCustomAttribute<RxPlatformIgnoreAttribute>();
                if (ignoreAttr != null)
                {
                    continue;
                }
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
                        else
                        {
                            if (null != prop.PropertyType.GetCustomAttribute<RxPlatformEventType>())
                            {
                                ret.Add(prop);
                            }
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
            if (type == typeof(void))
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
                var ignoreAttr = method.GetCustomAttribute<RxPlatformIgnoreAttribute>();
                if (ignoreAttr != null)
                {
                    continue;
                }
                if (method.DeclaringType != type)
                {
                    continue;
                }
                if (method.IsSpecialName || method.Name == "Started" || method.Name == "Stopping")
                {
                    continue;
                }
                if (method.GetCustomAttribute<RxPlatformMethodType>() != null)
                {
                    continue;
                }
                Type? paramType = null;
                Type? returnType = null;
                var parmsInfo = method.GetParameters();
                if (parmsInfo == null || parmsInfo.Length == 0)
                {
                    paramType = typeof(void);
                }
                if (parmsInfo != null && parmsInfo.Length == 1)
                {
                    paramType = parmsInfo[0].ParameterType;
                    var nullparam = Nullable.GetUnderlyingType(parmsInfo[0].ParameterType);
                    if(nullparam != null)
                        paramType = nullparam;
                }

                returnType = Nullable.GetUnderlyingType(method.ReturnType);
                if (returnType == null)
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
            if (!returnValue)
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
            if (type.IsSubclassOf(typeof(Delegate)))
            {
                MethodInfo? invokeMethod = type.GetMethod("Invoke");
                if (invokeMethod != null)
                {
                    var parameters = invokeMethod.GetParameters();
                    if (parameters.Length == 1)
                    {
                        var ttempType = GetNullableType(parameters[0]);
                        if (ttempType != null && ttempType == typeof(Exception))
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
                var ignoreAttr = method.GetCustomAttribute<RxPlatformIgnoreAttribute>();
                if (ignoreAttr != null)
                {
                    continue;
                }
                if (method.DeclaringType != type)
                {
                    continue;
                }
                if (method.Name == "SourceWrite"
                    && method.ReturnType == typeof(void))
                {
                    var paramsInfo = method.GetParameters();
                    if (paramsInfo.Length != 2)
                    {
                        continue;
                    }

                    Type paramType = paramsInfo[0].ParameterType;
                    Type param2Type = paramsInfo[1].ParameterType;
                    if (IsRxPlatformResultDelegate(param2Type))
                    {
                        ret.Add(method);
                    }
                }
            }
            return ret.ToArray();
        }
        internal static MethodInfo[] GetRequestHandlers(Type type)
        {
            List<MethodInfo> ret = new List<MethodInfo>();
            var methodInfos = type.GetMethods(BindingFlags.Public | BindingFlags.Instance);
            foreach (var method in methodInfos)
            {
                var ignoreAttr = method.GetCustomAttribute<RxPlatformIgnoreAttribute>();
                if (ignoreAttr != null)
                {
                    continue;
                }
                if (method.DeclaringType != type)
                {
                    continue;
                }
                if (method.ReturnType != typeof(Task<HttpResponseMessage>))
                {
                    continue;
                }

                var paramsInfo = method.GetParameters();
                if (paramsInfo == null || paramsInfo.Length != 1)
                    continue;
                if (paramsInfo[0].ParameterType != typeof(HttpRequestMessage))
                    continue;

                ret.Add(method);

                Type paramType = paramsInfo[0].ParameterType;
                Type param2Type = paramsInfo[1].ParameterType;
                if (IsRxPlatformResultDelegate(param2Type))
                {
                    ret.Add(method);
                }
            }
            return ret.ToArray();
        }
    }
}
