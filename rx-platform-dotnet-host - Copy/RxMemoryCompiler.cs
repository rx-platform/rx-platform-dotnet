using ENSACO.RxPlatform.Attributes;
using ENSACO.RxPlatform.Hosting;
using ENSACO.RxPlatform.Hosting.Internal;
using ENSACO.RxPlatform.Hosting.Model;
using ENSACO.RxPlatform.Hosting.Model.Code;
using ENSACO.RxPlatform.Hosting.Reflection;
using ENSACO.RxPlatform.Runtime;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Data;
using System.Numerics;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;
using System.Xml.Linq;

namespace ENSACO.RxPlatform.Hosting
{
    public class RxMemoryCompiler
    {
        internal static string ValueToSourceCode(object? value, Type type)
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
            else
            {
                return value.ToString()!;
            }
        }
        /*internal static CompiledTypeData GenerateObjectTypeSourceCode(Type type, RxPlatformObjectRuntime? instance, RxAssemblyLoadContext? ctx)
        {
            List<bool> HasEvent = new List<bool>();
            List<bool> JsonItems = new List<bool>();
            List<string> ItemIds = new List<string>();
            List<string> ItemTypes = new List<string>();
            var asname = type.Assembly.GetName().Name;
            if (asname == null)
                throw new Exception("Unable to determine assembly name from type.");

            if (ctx == null)
                throw new Exception("Assembly Load Context is null.");

            var properties = ReflectionHelpers.GetSimplePropertyInfos(type);

            CompiledTypeData ret = new CompiledTypeData();
            ret.assemblyName = asname + overridePostfix;
            ret.connections = "";

            StringBuilder initialData = new StringBuilder();

            ret.typeName = type.Name;
            ret.typeNamespace = type.Namespace + overridePostfix;
            string sourceCode = @"
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Text.Json;
namespace " + ret.typeNamespace + @"
{

public class " + ret.typeName + @" : " + type.FullName + @"
{";
            var methods = ReflectionHelpers.GetDefinedMethods(type);

            int index = 0;
            foreach (var prop in properties)
            {
                if (ReflectionHelpers.IsVirtual(prop))
                {
                    string? propTypeName = prop.PropertyType.FullName;
                    if (propTypeName == null)
                        propTypeName = prop.Name;
                    Type? propType = Nullable.GetUnderlyingType(prop.PropertyType);
                    if (propType != null)
                    {
                        propTypeName = propType.FullName;
                        if (propTypeName == null)
                            propTypeName = prop.Name;
                    }
                    else
                    {
                        propType = prop.PropertyType;
                    }
                    string defaultValue = "default";
                    defaultValue = ValueToSourceCode(prop.GetValue(instance), propType);
                    //if (prop.PropertyType == typeof(string))
                    //{
                    //    defaultValue = $@"""{prop.GetValue(instance)}""";
                    //}
                    //else
                    //{
                    //    defaultValue = prop.GetValue(instance)?.ToString() ?? "default";
                    //}
                    string? eventDelName = ReflectionHelpers.EventType(type, prop);
                    HasEvent.Add(eventDelName != null);
                    ItemIds.Add($"{prop.Name}");
                    ItemTypes.Add($"{propTypeName}");
                    bool isComplex = null != prop.PropertyType.GetCustomAttribute<RxPlatformDataType>();
                    JsonItems.Add(isComplex);
                    if (eventDelName != null)
                    {
                        sourceCode += $@"
     public override event {eventDelName}? On{prop.Name}Change;
";
                    }
                    sourceCode += $@"
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private {propTypeName}? {prop.Name}____rxImplementation = {defaultValue};
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private object {prop.Name}____rxLock = new object();
    public override {propTypeName}? {prop.Name} 
    {{
        get 
        {{
            lock({prop.Name}____rxLock)
            {{
                return {prop.Name}____rxImplementation;
            }}
        }}";
                    if (prop.CanWrite
                        && prop.SetMethod != null)
                    {
                        if (!isComplex)
                        {
                            string modifier = "";
                            if (!prop.SetMethod.IsPublic)
                                modifier = "protected";
                            sourceCode += $@"
        {modifier} set
        {{
            WriteProperty({index}, value);
        }}
    }}
";
                        }
                        else
                        {

                            string modifier = "";
                            if (!prop.SetMethod.IsPublic)
                                modifier = "protected";
                            sourceCode += $@"
        {modifier} set
        {{
                if (value == null)
                {{
                    WriteProperty({index}, null);
                    return;
                }}
                else
                {{
                    string strVal = JsonSerializer.Serialize<{ItemTypes[index]}>(value);
                    WriteProperty({index}, strVal);
                }}
        }}
    }}
";
                        }
                    }
                    else
                    {
                        sourceCode += @"
    }
";
                    }
                    index++;

                    if (ret.connections.Length > 0)
                    {
                        ret.connections += ";";
                    }
                    ret.connections += prop.Name;
                }
                else
                {
                    if (initialData.Length > 0)
                    {
                        initialData.Append(";");
                    }
                    initialData.Append(prop.Name);
                }
            }
            sourceCode += @"
    public override void __rxValueCallback(int index, object? value)
    {
        switch(index)
        {";
            index = 0;
            foreach (var id in ItemIds)
            {
                string eventStr = "";
                if (HasEvent[index])
                {
                    eventStr = $@"
                    On{id}Change?.Invoke(myValue);
";
                }

                if (!JsonItems[index])
                {
                    sourceCode += $@"
            case {index}:
                {{
                    {ItemTypes[index]}? myValue = null;
                    lock({id}____rxLock)
                    {{
                        try
                        {{
                            if(value == null)
                            {{
                                {id}____rxImplementation = null;
                            }}
                            else
                            {{
                                myValue = ({ItemTypes[index]}?)value;
                                {id}____rxImplementation = myValue;
                            }}
                        }}
                        catch(Exception)
                        {{
                            {id}____rxImplementation = null;
                            myValue = null;
                        }}
                     }}
                    {eventStr}
                 }}
                break;";

                }
                else
                {
                    sourceCode += $@"

            case {index}:
                {{
                    {ItemTypes[index]}? myValue = null;
                    lock({id}____rxLock)
                    {{
                        try
                        {{                        
                            string? strValue = value as string;
                            if(strValue != null)
                            {{
                                  myValue = JsonSerializer.Deserialize<{ItemTypes[index]}>(strValue);
                            }}
                            {id}____rxImplementation = myValue;   
                        }}
                        catch(Exception)
                        {{
                            {id}____rxImplementation = null;
                            myValue = null;
                        }}
                    }}
                    {eventStr}
                }}
                break;";

                }
                index++;
            }
            sourceCode += @"
        }
    }";

            if (methods.Length > 0)
            {
                sourceCode += $@"
    public async override Task<string> __rxExecuteMethod(string method, string args)
    {{
        switch(method)
        {{
";
                foreach (var method in methods)
                {
                    sourceCode += $@"
                    case ""{method.Name}"":
                        {{
                            {method.Name}();
                            return ""{{ }}"";
                        }}
                        break;
";
                }
                sourceCode += $@"
        }}
        throw new Exception($""Method {{method}} not found"");
    }}
";
            }
            //            sourceCode += @"
            //    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            //    public string[] ObjectPropIds = new string[] { ";
            //            bool first = true;
            //            foreach (var id in ItemIds)
            //            {
            //                if (!first)
            //                {
            //                    sourceCode += ",";
            //                }
            //                else
            //                {
            //                    first = false;
            //                }
            //                sourceCode += $@"""{id}""";
            //            }

            sourceCode += @"
    }
}
        ";
            if (initialData.Length > 0)
            {
                ret.connections += "|" + initialData.ToString();
            }
            ret.soureCode = sourceCode;
            return ret;
        }
        */

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
        static void GenerateTypePropertiesCode(RxPropertyCodeData[] properties, StringBuilder stream)
        {
            if(properties.Length == 0)
                return;

            int index = 0;
            foreach (var prop in properties)
            {
                if (prop.codeType == null)
                    continue;
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
            WriteProperty({index}, value);
        }}
    }}
");
                    }
                    else
                    {
                        stream.Append($@"
        {prop.setModifier} set
        {{
                if (value == null)
                {{
                    WriteProperty({index}, null);
                    return;
                }}
                else
                {{
                    string strVal = JsonSerializer.Serialize<{prop.codeType}>(value);
                    WriteProperty({index}, strVal);
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
                index++;
            }
            stream.Append(@"
    public override void __rxValueCallback(int index, object? value)
    {
        switch(index)
        {");
            index = 0;
            foreach (var prop in properties)
            {
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
                                  myValue = JsonSerializer.Deserialize<{prop.codeType}>(strValue);
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
            stream.Append(@"
        }
    }");
        }
        internal static void GenerateSourceTypeSourceCode(PlatformTypeBuildMeta<RxPlatformSource> type, StringBuilder stream)
        {

            string typeName = type.name;
            if (type.codeNamespace == null)
                return;

            GenerateTypeHeader(typeName, type.codeNamespace, stream);
            GenerateTypePropertiesCode(type.definedProperties, stream);

            GenerateTypeFooter(stream);
        }

        internal static void GenerateMapperTypeSourceCode(PlatformTypeBuildMeta<RxPlatformMapper> type, StringBuilder stream)
        {

            string typeName = type.name;
            if (type.codeNamespace == null)
                return;

            GenerateTypeHeader(typeName, type.codeNamespace, stream);
            GenerateTypePropertiesCode(type.definedProperties, stream);

            GenerateTypeFooter(stream);
        }
        internal static void GenerateEventTypeSourceCode(PlatformTypeBuildMeta<RxPlatformEvent> type, StringBuilder stream)
        {

            string typeName = type.name;
            if (type.codeNamespace == null)
                return;

            GenerateTypeHeader(typeName, type.codeNamespace, stream);
            GenerateTypePropertiesCode(type.definedProperties, stream);

            GenerateTypeFooter(stream);
        }

        internal static void GenerateObjectTypeSourceCode(PlatformTypeBuildMeta<RxPlatformObjectType> type, StringBuilder stream)
        {
            string typeName = type.name;
            if (type.codeNamespace == null)
                return;

            GenerateTypeHeader(typeName, type.codeNamespace, stream);
            GenerateTypePropertiesCode(type.definedProperties, stream);

            if (type.definedMethods.Length > 0)
            {
                string asyncString = "";
                foreach (var method in type.definedMethods)
                {
                    if (method.isAsync)
                    {
                        asyncString = "async";
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
                    if (method.name == null)
                        continue;

                    string awaitString = "";
                    if (method.isAsync)
                        awaitString = "await";
                    if (string.IsNullOrEmpty(method.argumentType) && method.resultType == "void")
                    {
                        //////////////////////////////////////////////////////////////////////////
                        /// void, void method
                        //////////////////////////////////////////////////////////////////////////
                        stream.Append($@"
                case ""{method.name}"":
                    {{
                        {awaitString} {method.name}();
                        return Task.FromResult(""{{ }}"");
                    }}
                    break;
");
                    }
                    else if (!string.IsNullOrEmpty(method.argumentType) && method.resultType == "void")
                    {
                        //////////////////////////////////////////////////////////////////////////
                        /// non-void, void method
                        //////////////////////////////////////////////////////////////////////////

                        stream.Append($@"
                case ""{method.name}"":
                    {{
                        {method.argumentType}? argObj = JsonSerializer.Deserialize<{method.argumentType}>(args);
                        var result = {awaitString} {method.name}(argObj);
                        return  Task.FromResult(""{{ }}"");
                    }}
                    break;
");
                    }
                    else if (string.IsNullOrEmpty(method.argumentType) && method.resultType != "void")
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
                            return  Task.FromResult(JsonSerializer.Serialize<{method.resultType}>(result));
                        }}
                    }}
                    break;
");
                        }
                        else
                        {
                            stream.Append($@"
                case ""{method.name}"":
                    {{
                        var result = {awaitString} {method.name}();
                        return  Task.FromResult(JsonSerializer.Serialize<{method.resultType}>(result));
                    }}
                    break;
");
                        }
                    }

                    else if (!string.IsNullOrEmpty(method.argumentType) && method.resultType != "void")
                    {
                        //////////////////////////////////////////////////////////////////////////
                        /// non-void, non-void method
                        //////////////////////////////////////////////////////////////////////////

                        if (method.isNullAbleResult)
                        {
                            string methodArgFull = method.isNullAbleArgument ? method.argumentType + "?" : method.argumentType;
                            stream.Append($@"
                case ""{method.name}"":
                    {{
                        {method.argumentType}? argObj = JsonSerializer.Deserialize<{method.argumentType}>(args);
                        var result = {awaitString} {method.name}(argObj);
                        if(result == null)
                        {{
                            return Task.FromResult(""{{ }}"");
                        }}
                        else
                        {{
                            return Task.FromResult(JsonSerializer.Serialize<{method.resultType}>(result));
                        }}
                    }}
                    break;
");
                        }
                        else
                        {
                            stream.Append($@"
                case ""{method.name}"":
                    {{
                        var result = {awaitString} {method.name}();
                        return  Task.FromResult(JsonSerializer.Serialize<{method.resultType}>(result));
                    }}
                    break;
");
                        }
                    }
                }
            }

            stream.Append($@"
        }}
        throw new Exception($""Method {{method}} not found"");
    }}
");
            GenerateTypeFooter(stream);
        }
    }
}