using ENSACO.RxPlatform.Attributes;
using ENSACO.RxPlatform.Hosting.Common;
using ENSACO.RxPlatform.Hosting.Model.Code;
using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace ENSACO.RxPlatform.Hosting.Model
{
    public class RxMemoryCompiler
    {
        internal static string ValueToSourceCode(object? value, Type type, int array)
        {
            if (array >= 0)
            {
                if (array == 0 || value == null)
                {
                    return $"new {type.FullName}[0]";
                }
                else
                {
                    StringBuilder tempStream = new StringBuilder();
                    tempStream.Append($"new {type.FullName}[]{{");
                    IEnumerable? arr = value as IEnumerable;
                    if (arr != null)
                    {
                        bool first = true;
                        foreach (var item in arr)
                        {
                            if (!first)
                            {
                                tempStream.Append(",");
                            }
                            tempStream.Append(ValueToSourceCode(item, type, -1));
                            first = false;
                        }
                    }
                    tempStream.Append("}");
                    return tempStream.ToString();
                }
            }
            else
            {
                if (value == null)
                {
                    return "null";
                }
                if (type == typeof(string))
                {
                    return $"\"{value.ToString()?.Replace("\"", "\\\"")}\"";
                }
                else if (type == typeof(char))
                {
                    return $"'{value}'";
                }
                else if (type == typeof(bool))
                {
                    return value.ToString()!.ToLower();
                }
                else if (type.IsEnum)
                {
                    return $"{type.FullName}.{value}";
                }
                else if (type.GetCustomAttribute<RxPlatformDataType>() != null)
                {
                    return $"new {type.FullName}()";
                }
                if (type == typeof(Guid))
                {
                    return value != null ? $"Guid.Parse(\"{value.ToString()}\")" : "null";
                }
                if (type == typeof(DateTime))
                {
                    return value != null ? $"DateTime.Parse(\"{value.ToString()}\")" : "null";
                }
                else
                {
                    return string.Format(CultureInfo.InvariantCulture, "{0}", value);
                }
            }
        }
        internal const string overridePostfix = "__rxImplementation";
        private static void GenerateTypeHeader(string typeName, string typeNamespace, StringBuilder stream)
        {
            string namespaceName = typeNamespace + overridePostfix;
            stream.Append(@"
namespace " + namespaceName + $@"
{{

public class {typeName} : {typeNamespace}.{typeName}
{{");

        }
        private static void GenerateTypeFooter(StringBuilder stream)
        {

            stream.Append(@"
}
}
");
        }
        static void GenerateTypeStructsCode(RxStructCodeData[] structs, StringBuilder stream)
        {
            if (structs == null || structs.Length == 0)
                return;

            int index = 0;
            foreach (var structData in structs)
            {
                if (structData.codeType == null)
                    continue;
                string structFullType = structData.isNullAble ? structData.codeType + "?" : structData.codeType;
                
                stream.Append($@"
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private {structFullType} {structData.name}____rxImplementation = new {structData.codeType}();
    public override {structFullType} {structData.name} 
    {{
        get 
        {{
            return {structData.name}____rxImplementation;
        }}
    }}
");
                index++;
            }
        }
        static void GenerateTypePropertiesCode(RxPropertyCodeData[] properties, RxOwnRelationCodeData[] relations, StringBuilder stream, out int index)
        {
            index = 0;
            if (properties.Length == 0 && (relations == null || relations.Length == 0))
                return;

            foreach (var prop in properties)
            {
                if (prop.codeType == null)
                    continue;

            //    System.Diagnostics.Debug.Assert(prop.name != "Priority");

                string propFullType = prop.isNullAble ? prop.codeType + "?" : prop.codeType;
                string? eventDelName = prop.eventName;
                if (eventDelName != null)
                {
                    stream.Append($@"
     public override event {eventDelName}? On{prop.name}Change;
");
                }
                stream.Append($@"
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private {propFullType} {prop.name}____rxImplementation = {prop.defaultValue};
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private object {prop.name}____rxLock = new object();
    public override {propFullType} {prop.name} 
    {{
        get 
        {{
            lock({prop.name}____rxLock)
            {{
                return {prop.name}____rxImplementation;
            }}
        }}");
                if (prop.canWrite)
                {
                    if (!prop.jsonValue)
                    {
                        stream.Append($@"
        {prop.setModifier} set
        {{
            if(value != null)
            {{
                __WriteProperty({index}, value);
            }}
        }}
    }}
");
                    }
                    else
                    {
                        stream.Append($@"
        {prop.setModifier} set
        {{
                if (value != null)
                {{
                    string strVal = JsonSerializer.Serialize<{prop.codeType}>(value);
                    __WriteProperty({index}, strVal);
                }}
        }}
    }}
");
                    }
                }
                else
                {
                    stream.Append(@"
    }
");
                }
                if (prop.writeMethod)
                {
                    stream.Append($@"
    public async override Task<bool> Write{prop.name}({prop.codeType} value)
    {{
        return await __WriteProperty({index}, value);
    }}
");
                }
                index++;
            }
            if (relations != null && relations.Length > 0)
            {
                HashSet<string> generatedTypeConnectedEvents = new HashSet<string>();
                HashSet<string> generatedTypeDisconnectedEvents = new HashSet<string>();
                foreach (var prop in relations)
                {
                    if (prop.codeType == null)
                        continue;
                    string simpleTypeName = prop.codeType.Contains(".") ? prop.codeType.Substring(prop.codeType.LastIndexOf(".") + 1) : prop.codeType;
                    string propFullType = prop.isNullAble ? prop.codeType + "?" : prop.codeType;
                    string? eventDelName = prop.eventName;
                    string? connectedEventDelName = prop.connectedEventName;
                    string? disconnectedEventDelName = prop.disconnectedEventName;
                    string? typeConnectedEventDelName = prop.typeConnectedEventName;
                    string? typeDisconnectedEventDelName = prop.typeDisconnectedEventName;
                    if (eventDelName != null)
                    {
                        stream.Append($@"
     public override event {eventDelName}? On{prop.name}Change;
");
                    }
                    if (connectedEventDelName != null)
                    {
                        stream.Append($@"
     public override event {connectedEventDelName}? On{prop.name}Connected;
");
                    }
                    if (disconnectedEventDelName != null)
                    {
                        stream.Append($@"
     public override event {disconnectedEventDelName}? On{prop.name}Disconnected;
");
                    }

                    if (typeConnectedEventDelName != null && !generatedTypeConnectedEvents.Contains(simpleTypeName))
                    {
                        stream.Append($@"
     public override event {typeConnectedEventDelName}? On{simpleTypeName}Connected;
");
                        generatedTypeConnectedEvents.Add(simpleTypeName);
                    }
                    if (typeDisconnectedEventDelName != null && !generatedTypeDisconnectedEvents.Contains(simpleTypeName))
                    {
                        stream.Append($@"
     public override event {typeDisconnectedEventDelName}? On{simpleTypeName}Disconnected;
");
                        generatedTypeDisconnectedEvents.Add(simpleTypeName);
                    }
                    stream.Append($@"
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private {propFullType} {prop.name}____rxImplementation = {prop.defaultValue};
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private object {prop.name}____rxLock = new object();
    public override {propFullType} {prop.name} 
    {{
        get 
        {{
            lock({prop.name}____rxLock)
            {{
                return {prop.name}____rxImplementation;
            }}
        }}");
                    if (prop.canWrite)
                    {
                        stream.Append($@"
        {prop.setModifier} set
        {{
                if (value != null)
                {{
                    string strVal = value.Path;
                    __WriteProperty({index}, strVal);
                }}
        }}
    }}
");
                    }
                    else
                    {
                        stream.Append(@"
    }
");
                    }
                    index++;
                }
            }
            stream.Append(@"
    protected override void __rxValueCallback(int index, object? value)
    {
        switch(index)
        {");
            index = 0;
            foreach (var prop in properties)
            {
                if (prop.codeType == null)
                    continue;

                string propFullType = prop.isNullAble ? prop.codeType + "?" : prop.codeType;
                string eventStr = "";
                if (prop.eventName != null)
                {
                    eventStr = $@"
                    On{prop.name}Change?.Invoke(myValue);
";
                }
                if (!prop.jsonValue)
                {
                    stream.Append($@"
            case {index}:
                {{
                    {propFullType} myValue = null;
                    lock({prop.name}____rxLock)
                    {{
                        try
                        {{
                            if(value == null)
                            {{
                                {prop.name}____rxImplementation = null;
                            }}
                            else
                            {{
                                myValue = ({propFullType})value;
                                {prop.name}____rxImplementation = myValue;
                            }}
                        }}
                        catch(Exception)
                        {{
                            {prop.name}____rxImplementation = null;
                            myValue = null;
                        }}
                     }}
                    {eventStr}
                 }}
                break;");

                }
                else
                {
                    stream.Append($@"

            case {index}:
                {{
                    {propFullType} myValue = null;
                    lock({prop.name}____rxLock)
                    {{
                        try
                        {{                        
                            string? strValue = value as string;
                            if(strValue != null)
                            {{
                                  myValue = JsonSerializer.Deserialize<{prop.codeType}>(strValue, __jsonContext);
                            }}
                            {prop.name}____rxImplementation = myValue;   
                        }}
                        catch(Exception)
                        {{
                            {prop.name}____rxImplementation = null;
                            myValue = null;
                        }}
                    }}
                    {eventStr}
                }}
                break;");

                }
                index++;
            }
            if (relations != null && relations.Length > 0)
            {
                foreach (var prop in relations)
                {
                    if (prop.codeType == null)
                        continue;
                    string simpleTypeName = prop.codeType.Contains(".") ? prop.codeType.Substring(prop.codeType.LastIndexOf(".") + 1) : prop.codeType;
                    string propFullType = prop.isNullAble ? prop.codeType + "?" : prop.codeType;
                    string eventStr = "";
                    string eventBeforeStr = "";
                    if (prop.eventName != null)
                    {
                        eventStr = $@"
                    On{prop.name}Change?.Invoke(myValue);
";
                    }
                    if (prop.connectedEventName != null)
                    {
                        eventStr += $@"
                        if(connectValue != null)
                        {{
                            On{prop.name}Connected?.Invoke(connectValue);
                        }}
";
                    }
                    if (prop.disconnectedEventName != null)
                    {
                        eventBeforeStr += $@"
                        if(disconnectValue != null)
                        {{
                            On{prop.name}Disconnected?.Invoke(disconnectValue);
                        }}
";
                    }
                    if (prop.typeConnectedEventName != null)
                    {
                        eventStr += $@"
                        if(connectValue != null)
                        {{
                            On{simpleTypeName}Connected?.Invoke(""{prop.name}"", connectValue);
                        }}
";
                    }
                    if (prop.typeDisconnectedEventName != null)
                    {
                        eventBeforeStr += $@"
                        if(disconnectValue != null)
                        {{
                            On{simpleTypeName}Disconnected?.Invoke(""{prop.name}"", disconnectValue);
                        }}
";
                    }
                    stream.Append($@"
            case {index}:
                {{
                    {propFullType} disconnectValue = null;
                    {propFullType} connectValue = null;
                    {propFullType} myValue = null;
");
                    if(eventBeforeStr != "")
                    {
                        stream.Append($@"
                    lock ({prop.name}____rxLock)
                    {{
                        disconnectValue = {prop.name}____rxImplementation;
                    }} 
");
                        stream.Append(eventBeforeStr);
                    }
                    stream.Append($@"
                    lock ({prop.name}____rxLock)
                    {{
                        disconnectValue = {prop.name}____rxImplementation;
                        try
                        {{
                            if(value == null)
                            {{
                                {prop.name}____rxImplementation = null;
                            }}
                            else
                            {{
                                ulong val = (ulong)value;
                                if(val == 0)
                                {{
                                    {prop.name}____rxImplementation = null;
                                }}
                                else
                                {{
                                    myValue = __GetInstance((nint)val) as {prop.codeType};   
                                    connectValue = myValue;
                                    {prop.name}____rxImplementation = myValue;   
                                }}
                            }}
                        }}
                        catch(Exception)
                        {{
                            {prop.name}____rxImplementation = null;
                            myValue = null;
                            connectValue = null;
                        }}
                     }}
                    {eventStr}
                 }}
                break;");
                    index++;
                }
            }
            stream.Append(@"
        }
    }");
        }
////////////////////////////////////////////////////////////////////////////////////
// this is for conversion of SourceWrite and MappedChanged methods
///////////////////////////////////////////////////////////////////////////////////
        static readonly rx_value_t[] boolValues = { rx_value_t.Bool
            , rx_value_t.Int8, rx_value_t.Int16, rx_value_t.Int32, rx_value_t.Int64
            , rx_value_t.UInt8, rx_value_t.UInt16, rx_value_t.UInt32, rx_value_t.UInt64
            , rx_value_t.Float, rx_value_t.Double
            , rx_value_t.String, rx_value_t.Time, rx_value_t.Uuid
         };


        static readonly rx_value_t[] int8Values = { 
            rx_value_t.Int8, rx_value_t.Int16, rx_value_t.Int32, rx_value_t.Int64
            , rx_value_t.Float, rx_value_t.Double
         };
        static readonly rx_value_t[] int16Values = {
            rx_value_t.Int16, rx_value_t.Int32, rx_value_t.Int64
            , rx_value_t.Float, rx_value_t.Double, rx_value_t.String
         };
        static readonly rx_value_t[] int32Values = {
            rx_value_t.Int32, rx_value_t.Int64
            , rx_value_t.Double, rx_value_t.String
         };
        static readonly rx_value_t[] int64Values = {
           rx_value_t.Int64, rx_value_t.String
         };

        static readonly rx_value_t[] uint8Values = {
            rx_value_t.UInt8, rx_value_t.UInt16, rx_value_t.UInt32, rx_value_t.UInt64
            , rx_value_t.Float, rx_value_t.Double, rx_value_t.String
         };
        static readonly rx_value_t[] uint16Values = {
            rx_value_t.UInt16, rx_value_t.UInt32, rx_value_t.UInt64
            , rx_value_t.Float, rx_value_t.Double, rx_value_t.String
         };
        static readonly rx_value_t[] uint32Values = {
            rx_value_t.UInt32, rx_value_t.UInt64
            , rx_value_t.Double, rx_value_t.String
         };
        static readonly rx_value_t[] uint64Values = {
            rx_value_t.UInt64, rx_value_t.String
         };

        static readonly rx_value_t[] floatValues = {
            rx_value_t.Float, rx_value_t.Double, rx_value_t.String
         };
        static readonly rx_value_t[] doubleValues = {
            rx_value_t.Double, rx_value_t.String
         };

        static readonly rx_value_t[] stringValues = {
            rx_value_t.String,
            rx_value_t.Double, rx_value_t.Float,
            rx_value_t.UInt64, rx_value_t.Int64,
            rx_value_t.UInt32, rx_value_t.Int32,
            rx_value_t.UInt16, rx_value_t.Int16,
            rx_value_t.UInt8, rx_value_t.Int8,
            rx_value_t.Bool, rx_value_t.Time, rx_value_t.Uuid
         };

        static readonly rx_value_t[] timeValues = {
            rx_value_t.Time, rx_value_t.String
         };

        static readonly rx_value_t[] uuidValues = {
            rx_value_t.Uuid, rx_value_t.String,

         };

////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////////
        static byte[] GetConvertedTypesDelegates(bool[] definedTypes)
        {
            byte[] ret = new byte[18];
            foreach (var dt in boolValues)//1
            {
                if (definedTypes[(byte)dt])
                {
                    ret[1] = (byte)dt;
                    break;
                }
            }
            foreach (var dt in int8Values)//2
            {
                if (definedTypes[(byte)dt])
                {
                    ret[2] = (byte)dt;
                    break;
                }
            }
            foreach (var dt in uint8Values)//3
            {
                if (definedTypes[(byte)dt])
                {
                    ret[3] = (byte)dt;
                    break;
                }
            }
            foreach (var dt in int16Values)//4
            {
                if (definedTypes[(byte)dt])
                {
                    ret[4] = (byte)dt;
                    break;
                }
            }
            foreach (var dt in uint16Values)//5
            {
                if (definedTypes[(byte)dt])
                {
                    ret[5] = (byte)dt;
                    break;
                }
            }
            foreach (var dt in int32Values)//6
            {
                if (definedTypes[(byte)dt])
                {
                    ret[6] = (byte)dt;
                    break;
                }
            }
            foreach (var dt in uint32Values)//7
            {
                if (definedTypes[(byte)dt])
                {
                    ret[7] = (byte)dt;
                    break;
                }
            }
            foreach (var dt in int64Values)//8
            {
                if (definedTypes[(byte)dt])
                {
                    ret[8] = (byte)dt;
                    break;
                }
            }
            foreach (var dt in uint64Values)//9
            {
                if (definedTypes[(byte)dt])
                {
                    ret[9] = (byte)dt;
                    break;
                }
            }
            foreach (var dt in floatValues)//10
            {
                if (definedTypes[(byte)dt])
                {
                    ret[10] = (byte)dt;
                    break;
                }
            }
            foreach (var dt in doubleValues)//11
            {
                if (definedTypes[(byte)dt])
                {
                    ret[11] = (byte)dt;
                    break;
                }
            }
            // zero for complex type
            foreach (var dt in stringValues)//13
            {
                if (definedTypes[(byte)dt])
                {
                    ret[13] = (byte)dt;
                    break;
                }
            }
            foreach (var dt in timeValues)//14
            {
                if (definedTypes[(byte)dt])
                {
                    ret[14] = (byte)dt;
                    break;
                }
            }
            foreach (var dt in uuidValues)//15
            {
                if (definedTypes[(byte)dt])
                {
                    ret[15] = (byte)dt;
                    break;
                }
            }
            // zero for bytes type 
            if(definedTypes[(byte)rx_value_t.Struct])//17
            {
                ret[17] = (byte)rx_value_t.Struct;
            }
            return ret.ToArray();
        }
        internal static void GenerateSourceTypeSourceCode(PlatformTypeBuildMeta<RxPlatformSourceType> type, StringBuilder stream)
        {

            string typeName = type.name;
            if (type.codeNamespace == null)
                return;

            int index = 0;
            GenerateTypeHeader(typeName, type.codeNamespace, stream);
            GenerateTypePropertiesCode(type.definedProperties, type.definedRelations, stream, out index);
            GenerateTypeStructsCode(type.definedStructs, stream);

            if (type.sourceWriteMethods != null)
            {
                bool hadOne = false;
                bool[] types = new bool[Enum.GetValues(typeof(rx_value_t)).Length];
                foreach (var methodData in type.sourceWriteMethods)
                {
                    hadOne = true;
                    types[(int)methodData.typeCode] = true;
                }
                if (hadOne)
                {
                    byte[] delegates = GetConvertedTypesDelegates(types);
                    stream.Append($@"
    public override byte __GetWriteConvert(byte input)
    {{
        byte[] delegates = {{ {string.Join(", ", delegates)} }};
        if(input < delegates.Length)
        {{
            return delegates[input];
        }}
        return 0;
    }}
");
                }
            }

            GenerateTypeFooter(stream);
        }

        internal static void GenerateStructTypeSourceCode(PlatformTypeBuildMeta<RxPlatformStructType> type, StringBuilder stream)
        {

            string typeName = type.name;
            if (type.codeNamespace == null)
                return;

            int index = 0;
            GenerateTypeHeader(typeName, type.codeNamespace, stream);
            GenerateTypePropertiesCode(type.definedProperties, type.definedRelations, stream, out index);
            GenerateTypeStructsCode(type.definedStructs, stream);

            GenerateTypeFooter(stream);
        }
        internal static void GenerateMapperTypeSourceCode(PlatformTypeBuildMeta<RxPlatformMapperType> type, StringBuilder stream)
        {

            string typeName = type.name;
            if (type.codeNamespace == null)
                return;

            int index = 0;
            GenerateTypeHeader(typeName, type.codeNamespace, stream);
            GenerateTypePropertiesCode(type.definedProperties, type.definedRelations, stream, out index);
            GenerateTypeStructsCode(type.definedStructs, stream);

            GenerateTypeFooter(stream);
        }
        internal static void GenerateEventTypeSourceCode(PlatformTypeBuildMeta<RxPlatformEventType> type, StringBuilder stream)
        {

            string typeName = type.name;
            if (type.codeNamespace == null)
                return;

           

            GenerateTypeHeader(typeName, type.codeNamespace, stream);

            stream.Append($@"
    public {typeName}() : base()
    {{
        Fire += (value) =>
        {{
            string val = JsonSerializer.Serialize(value);
            __FireEvent(val, """", false, false);
        }};
    }}
");
            int index = 0;
            GenerateTypePropertiesCode(type.definedProperties, type.definedRelations, stream, out index);
            GenerateTypeStructsCode(type.definedStructs, stream);

            GenerateTypeFooter(stream);
        }

        internal static void GenerateDisplayTypeSourceCode(PlatformTypeBuildMeta<RxPlatformDisplayType> type, StringBuilder stream)
        {

            string typeName = type.name;
            if (type.codeNamespace == null)
                return;

            int index = 0;
            GenerateTypeHeader(typeName, type.codeNamespace, stream);
            GenerateTypePropertiesCode(type.definedProperties, type.definedRelations, stream, out index);
            GenerateTypeStructsCode(type.definedStructs, stream);

            GenerateTypeFooter(stream);
        }
        static private void GenerateOwnMethodCode(RxOwnMethodCodeData method, string resultPrefix, string resultSuffix, StringBuilder stream)
        {
            if (method.name == null)
                return;

            string awaitString = "";
            string argSufix = "";
            if (method.isAsync)
                awaitString = "await";
            if(method.isStructArgument)
                argSufix = ".Value";
            bool isVoid = method.resultType == "void" || method.resultType == typeof(Task).FullName;
            if (string.IsNullOrEmpty(method.argumentType) && isVoid)
            {
                //////////////////////////////////////////////////////////////////////////
                /// void, void method
                //////////////////////////////////////////////////////////////////////////
                stream.Append($@"
                case ""{method.name}"":
                    {{
                        {awaitString} {method.name}();
                        return {resultPrefix}""{{ }}""{resultSuffix};
                    }}
");
            }
            else if (!string.IsNullOrEmpty(method.argumentType) && isVoid)
            {
                //////////////////////////////////////////////////////////////////////////
                /// non-void, void method
                //////////////////////////////////////////////////////////////////////////
                if (method.isNullAbleArgument)
                {
                    stream.Append($@"
                case ""{method.name}"":
                    {{
                        {method.argumentType}? argObj = JsonSerializer.Deserialize<{method.argumentType}>(args, __jsonContext);
                        {awaitString} {method.name}(argObj);
                        return {resultPrefix}""{{ }}""{resultSuffix};
                    }}
");
                }
                else
                {// non-null-able argument
                    
                        stream.Append($@"
                case ""{method.name}"":
                    {{
                        {method.argumentType}? argObj = JsonSerializer.Deserialize<{method.argumentType}>(args, __jsonContext);
                        if(argObj == null)
                        {{
                            argObj = new {method.argumentType}();
                        }}
                        {awaitString} {method.name}(argObj{argSufix});
                        return {resultPrefix}""{{ }}""{resultSuffix};
                    }}
");
                }
            }
            else if (string.IsNullOrEmpty(method.argumentType) && !isVoid)
            {
                //////////////////////////////////////////////////////////////////////////
                /// void, non-void method
                //////////////////////////////////////////////////////////////////////////
                if (method.isNullAbleResult)
                {
                    stream.Append($@"
                case ""{method.name}"":
                    {{
                        var result = {awaitString} {method.name}();
                        if(result == null)
                        {{
                            return ""{{ }}"";
                        }}
                        else
                        {{
                            return {resultPrefix}JsonSerializer.Serialize<{method.resultType}>(result, __jsonContext){resultSuffix};
                        }}
                    }}
");
                }
                else
                {
                    stream.Append($@"
                case ""{method.name}"":
                    {{
                        var result = {awaitString} {method.name}();
                        return {resultPrefix}JsonSerializer.Serialize<{method.resultType}>(result, __jsonContext){resultSuffix};
                    }}
");
                }
            }

            else if (!string.IsNullOrEmpty(method.argumentType) && !isVoid)
            {
                //////////////////////////////////////////////////////////////////////////
                /// non-void, non-void method
                //////////////////////////////////////////////////////////////////////////

                if (method.isNullAbleResult)
                {
                    if (method.isNullAbleArgument)
                    {
                        string methodArgFull = method.isNullAbleArgument ? method.argumentType + "?" : method.argumentType;
                        stream.Append($@"
                case ""{method.name}"":
                    {{
                        {method.argumentType}? argObj = JsonSerializer.Deserialize<{method.argumentType}>(args, __jsonContext);
                        var result = {awaitString} {method.name}(argObj);
                        if(result == null)
                        {{
                            return {resultPrefix}""{{ }}""{resultSuffix};
                        }}
                        else
                        {{
                            return {resultPrefix}JsonSerializer.Serialize<{method.resultType}>(result, __jsonContext){resultSuffix};
                        }}
                    }}
                    break;
");
                    }
                    else
                    {// non-null-able argument

                        stream.Append($@"
                case ""{method.name}"":
                    {{
                        {method.argumentType}? argObj = JsonSerializer.Deserialize<{method.argumentType}>(args, __jsonContext);
                        if(argObj == null)
                        {{
                            argObj = new {method.argumentType}();
                        }}
                        var result = {awaitString} {method.name}(argObj{argSufix});
                        if(result == null)
                        {{
                            return {resultPrefix}""{{ }}""{resultSuffix};
                        }}
                        else
                        {{
                            return {resultPrefix}JsonSerializer.Serialize<{method.resultType}>(result, __jsonContext){resultSuffix};
                        }}
                    }}
");
                    }
                }
                else
                {// non-null-able result
                    if (method.isNullAbleArgument)
                    {
                        stream.Append($@"
                case ""{method.name}"":
                    {{
                        var result = {awaitString} {method.name}();
                        return {resultPrefix}(JsonSerializer.Serialize<{method.resultType}>(result, __jsonContext){resultSuffix};
                    }}
                    break;
");
                    }
                    else
                    {// non-null-able argument

                        stream.Append($@"
                case ""{method.name}"":
                    {{
                        {method.argumentType}? argObj = JsonSerializer.Deserialize<{method.argumentType}>(args, __jsonContext);
                        if(argObj == null)
                        {{
                            argObj = new {method.argumentType}();
                        }}
                        var result = {awaitString} {method.name}(argObj{argSufix});
                        return {resultPrefix}(JsonSerializer.Serialize<{method.resultType}>(result, __jsonContext){resultSuffix});
                    }}
                    break;
");
                    }
                }
            }
        }
        static private void GenerateCallableMethodCode(RxCallableMethodCodeData method, int index, StringBuilder stream)
        {
            if (method.name == null)
                return;


            string resultPostfix = method.isStructResult ? ".Value" : "";

            bool isVoid = method.resultType == typeof(Task).FullName;
            if (string.IsNullOrEmpty(method.argumentType) && isVoid)
            {
                //////////////////////////////////////////////////////////////////////////
                /// void, void method
                //////////////////////////////////////////////////////////////////////////

                stream.Append($@"
    public override async {method.resultType} {method.name}()
    {{
            await __ExecuteProperty({index}, null);
    }}
");
            }
            else if (!string.IsNullOrEmpty(method.argumentType) && isVoid)
            {
                //////////////////////////////////////////////////////////////////////////
                /// non-void, void method
                //////////////////////////////////////////////////////////////////////////

            }
            else if (string.IsNullOrEmpty(method.argumentType) && !isVoid)
            {
                //////////////////////////////////////////////////////////////////////////
                /// void, non-void method
                //////////////////////////////////////////////////////////////////////////
                stream.Append($@"
    public override async Task<{method.resultType}> {method.name}()
    {{
            var jsonStr = await __ExecuteProperty({index}, null);
            {method.resultType}? resultObj = JsonSerializer.Deserialize<{method.resultType}>(jsonStr, __jsonContext);
            if(resultObj == null)
            {{
                throw new Exception($""Failed to deserialize result of method {method.name}"");
            }}
            return resultObj{resultPostfix};
    }}
");
            }

            else if (!string.IsNullOrEmpty(method.argumentType) && !isVoid)
            {
                //////////////////////////////////////////////////////////////////////////
                /// non-void, non-void method
                //////////////////////////////////////////////////////////////////////////
                ///string structPostfix = method.isStructArgument ? ".Value" : "";
                stream.Append($@"
    public override async Task<{method.resultType}> {method.name}({method.argumentType} args)
    {{
            var inStr = JsonSerializer.Serialize(args, __jsonContext);
            if(string.IsNullOrEmpty(inStr))
            {{
                inStr = ""{{ }}"";
            }}
            var jsonStr = await __ExecuteProperty({index}, inStr);  
            {method.resultType}? resultObj = JsonSerializer.Deserialize<{method.resultType}>(jsonStr, __jsonContext);
            if(resultObj == null)
            {{
                throw new Exception($""Failed to deserialize result of method {method.name}"");
            }}
            return resultObj{resultPostfix};
    }}
");

            }

        }
        internal static void GenerateObjectTypeSourceCode(PlatformTypeBuildMeta<RxPlatformObjectType> type, StringBuilder stream)
        {
            string typeName = type.name;
            if (type.codeNamespace == null)
                return;

            int index = 0;
            GenerateTypeHeader(typeName, type.codeNamespace, stream);
            GenerateTypePropertiesCode(type.definedProperties, type.definedRelations, stream, out index);
            GenerateTypeStructsCode(type.definedStructs, stream);

            if (type.definedMethods.Length > 0)
            {
                string asyncString = "";
                string resultPrefix = "Task.FromResult(";
                string resultSuffix = ")";
                foreach (var method in type.definedMethods)
                {
                    if (method.isAsync)
                    {
                        asyncString = "async";
                        resultPrefix = "";
                        resultSuffix = "";
                        break;
                    }
                }
                stream.Append($@"
    public {asyncString} override Task<string> __rxExecuteMethod(string method, string args)
    {{
        switch(method)
        {{
");
                foreach (var method in type.definedMethods)
                {
                    GenerateOwnMethodCode(method, resultPrefix, resultSuffix, stream);
                }

                stream.Append($@"
        }}
        throw new Exception($""Method {{method}} not found"");
    }}
");
            }

            if (type.callableMethods.Length > 0)
            {
                foreach (var method in type.callableMethods)
                {
                    GenerateCallableMethodCode(method, index, stream);
                    index++;
                }
            }
            GenerateTypeFooter(stream);
        }


        internal static void GenerateMonitoredObjectTypeSourceCode(PlatformMonitoredTypeBuildMeta type, StringBuilder stream)
        {
            if(type.type == null)
                return;

            string typeName = type.type.Name;
            if (type.codeNamespace == null)
                return;

            int index = 0;
            GenerateTypeHeader(typeName, type.codeNamespace, stream);
            GenerateTypePropertiesCode(type.definedProperties, type.definedRelations, stream, out index);
            GenerateTypeStructsCode(type.definedStructs, stream);


            if (type.callableMethods.Length > 0)
            {
                foreach (var method in type.callableMethods)
                {
                    GenerateCallableMethodCode(method, index, stream);
                    index++;
                }
            }
            GenerateTypeFooter(stream);
        }

    }
}