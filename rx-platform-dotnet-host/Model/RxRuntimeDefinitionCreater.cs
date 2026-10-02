using ENSACO.RxPlatform.Attributes;
using ENSACO.RxPlatform.Hosting.Internal;
using ENSACO.RxPlatform.Hosting.Model.Code;
using ENSACO.RxPlatform.Hosting.Reflection;
using ENSACO.RxPlatform.Model;
using ENSACO.RxPlatform.Model.System;
using ENSACO.RxPlatform.Runtime;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization.Metadata;

namespace ENSACO.RxPlatform.Hosting.Model
{

    internal class RxRuntimeDefinitionCreater
    {
        static RxNodeId ExtractDomain(object? prototype)
        {
            if (prototype != null)
            {
                var type = prototype.GetType();
                var propInfo = type.GetProperty("Domain", BindingFlags.Instance | BindingFlags.Public);
                if (propInfo != null)
                {
                    var propType = propInfo.PropertyType;
                    var attr = propType.GetCustomAttribute<RxPlatformDomainType>();
                    if (attr != null)
                    {
                        lock (RxMetaData.Instance.TypesLock)
                        {

                            if (RxMetaData.Instance.RegisteredObjects.TryGetValue(prototype, out var instanceData))
                            {
                                return instanceData.id;
                            }
                        }
                    }
                }
            }
            return new RxNodeId(HostPlatformIds.RX_DOTNET_DOMAIN_ID, 1);
        }
        static RxNodeId ExtractApp(object? prototype)
        {
            if (prototype != null)
            {
                var type = prototype.GetType();
                var propInfo = type.GetProperty("App", BindingFlags.Instance | BindingFlags.Public);
                if (propInfo != null)
                {
                    var propType = propInfo.PropertyType;
                    var attr = propType.GetCustomAttribute<RxPlatformApplicationType>();
                    if (attr != null)
                    {
                        lock (RxMetaData.Instance.TypesLock)
                        {
                            if (RxMetaData.Instance.RegisteredObjects.TryGetValue(prototype, out var instanceData))
                            {
                                return instanceData.id;
                            }
                        }
                    }
                }
            }
            return new RxNodeId(HostPlatformIds.RX_DOTNET_APPLICATION_ID, 1);
        }

        const string tabs = "    ";
        const string domainPath = "SystemDomain";
        const string appPath = "SystemApp";
        const int processor = -1;// default processor
        const int priority = 3;// standard priority
        const string defaultIdentity = "AA=="; // default identity base64
        internal static bool GetInstanceData<T>(T attr, object? prototype, StringBuilder stream) where T : RxPlatformTypeAttribute
        {
            stream.AppendLine("{");
            stream.AppendLine("\"def\":{");
            stream.AppendLine("\"programs\":[],");
            stream.AppendLine("\"access\":{");
            stream.AppendLine($"{tabs}\"roles\":[]");
            stream.AppendLine("},");
            stream.AppendLine("\"instance\":{");
            if (attr is RxPlatformObjectType)
            {
                if (prototype != null)
                {
                    RxNodeId dom = ExtractDomain(prototype);
                    if (!dom.IsNull())
                    {
                        stream.AppendLine($"{tabs}\"domain\":{{");
                        stream.AppendLine($"{tabs}{tabs}\"id\":\"{dom.ToString()}\"");
                        stream.AppendLine($"{tabs}}}");
                        stream.AppendLine("},");
                        stream.AppendLine("\"overrides\":");
                        return true;
                    }
                }
                stream.AppendLine($"{tabs}\"domain\":{{");
                stream.AppendLine($"{tabs}{tabs}\"path\":\"{domainPath}\"");
                stream.AppendLine($"{tabs}}}");
                stream.AppendLine("},");
                stream.AppendLine("\"overrides\":");
                return true;

            }
            else if (attr is RxPlatformPortType)
            {
                string identity = defaultIdentity;
                stream.AppendLine($"{tabs}\"identity\":\"{identity}\",");

                RxNodeId app = ExtractApp(prototype);
                if (!app.IsNull())
                {
                    stream.AppendLine($"{tabs}\"app\":{{");
                    stream.AppendLine($"{tabs}{tabs}\"id\":\"{app.ToString()}\"");
                    stream.AppendLine($"{tabs}}},");
                    stream.AppendLine($"{tabs}\"sim\":true,");
                    stream.AppendLine($"{tabs}\"proc\":true");
                    stream.AppendLine("},");
                    stream.AppendLine($"\"overrides\":");
                    return true;
                }
                else
                {
                    stream.AppendLine($"{tabs}\"app\":{{");
                    stream.AppendLine($"{tabs}{tabs}\"path\":\"{appPath}\"");
                    stream.AppendLine($"{tabs}}},");
                    stream.AppendLine($"{tabs}\"sim\":true,");
                    stream.AppendLine($"{tabs}\"proc\":true");
                    stream.AppendLine("},");
                    stream.AppendLine("\"overrides\":");
                }
                return true;
            }
            else if (attr is RxPlatformDomainType)
            {
                stream.AppendLine($"{tabs}\"processor\":\"{processor}\",");
                stream.AppendLine($"{tabs}\"priority\":\"{priority}\",");

                RxNodeId app = ExtractApp(prototype);
                if (!app.IsNull())
                {
                    stream.AppendLine($"{tabs}\"app\":{{");
                    stream.AppendLine($"{tabs}{tabs}\"id\":\"{app.ToString()}\"");
                    stream.AppendLine($"{tabs}}}");
                    stream.AppendLine("},");
                    stream.AppendLine("\"overrides\":");
                    return true;
                }
                else
                {
                    stream.AppendLine($"{tabs}\"app\":{{");
                    stream.AppendLine($"{tabs}{tabs}\"path\":\"{appPath}\"");
                    stream.AppendLine($"{tabs}}}");
                    stream.AppendLine("},");
                    stream.AppendLine("\"overrides\":");
                    return true;
                }
            }
            else if (attr is RxPlatformApplicationType)
            {
                string identity = defaultIdentity;
                stream.AppendLine($"{tabs}\"identity\":\"{identity}\",");
                stream.AppendLine($"{tabs}\"processor\":\"{processor}\",");
                stream.AppendLine($"{tabs}\"priority\":\"{priority}\"");
                stream.AppendLine("},");
                stream.AppendLine("\"overrides\":");
                return true;
            }
            // no instance data for other types
            return false;
        }
        private static void FilterAttributeProperties(JsonTypeInfo typeInfo)
        {
            if (typeof(Attribute).IsAssignableFrom(typeInfo.Type))
            {
                for (int i = typeInfo.Properties.Count - 1; i >= 0; i--)
                {
                    var property = typeInfo.Properties[i];

                    // System.Reflection.MemberInfo holds the true DeclaringType
                    if (property.AttributeProvider is System.Reflection.MemberInfo memberInfo)
                    {
                        if (memberInfo.DeclaringType == typeof(Attribute))
                        {
                            typeInfo.Properties.RemoveAt(i);
                        }
                    }
                    // Fallback for fields or parameter-backed properties (like TypeId)
                    else if (property.Name == "TypeId")
                    {
                        typeInfo.Properties.RemoveAt(i);
                    }
                }
            }
        }

        static PropertyInfo[] GetAllRelationsPropertyInfos(Type type)
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
                Type? propType = Nullable.GetUnderlyingType(prop.PropertyType);
                if (propType == null)
                {
                    propType = prop.PropertyType;
                }
                var relAttr = prop.GetCustomAttribute<RxPlatformRelationAttribute>();
                if (relAttr != null)
                {
                    if(propType!=typeof(string))
                        ret.Add(prop);
                    continue;
                }
                if (!ReflectionHelpers.IsVirtual(prop))
                {
                    continue;
                }
                if (propType != null && propType.IsSubclassOf(typeof(RxPlatformObjectRuntime)))
                {
                    ret.Add(prop);
                    continue;
                }
            }
            return ret.ToArray();
        }
        internal static string CreateOverrides(object prototype, HostedPlatformLibrary hostLib)
        {
            string jsonString = JsonSerializer.Serialize(prototype, prototype.GetType(), PlatformHostMain.JsonContext);


            List<PropertyInfo> varProperties = new List<PropertyInfo>();
            var simpleProperties = ReflectionHelpers.GetSimplePropertyInfos(prototype.GetType(), false);
            if (simpleProperties != null && simpleProperties.Length > 0)
            {
                foreach (var prop in simpleProperties)
                {
                    if(prop.GetCustomAttributes<RxPlatformVariableSourceAttribute>().Any()
                        || prop.GetCustomAttributes<RxPlatformVariableMapperAttribute>().Any()
                        || prop.GetCustomAttributes<RxPlatformFilterAttribute>().Any())
                    varProperties.Add(prop);
                }
            }
            var relationProperties = GetAllRelationsPropertyInfos(prototype.GetType());
            if ((relationProperties != null && relationProperties.Length > 0)
                || varProperties.Count > 0)
            {

                var document = JsonNode.Parse(jsonString) as JsonObject;

                if (document != null)
                {
                    if (relationProperties != null)
                    {
                        foreach (var prop in relationProperties)
                        {
                            object? relInstance = prop.GetValue(prototype);
                            document[prop.Name] = "";
                            if (relInstance != null)
                            {
                                PlatformInstanceData instanceData = new PlatformInstanceData();
                                lock (RxMetaData.Instance.TypesLock)
                                {
                                    RxMetaData.Instance.RegisteredObjects.TryGetValue(relInstance, out instanceData);
                                }
                                if (!instanceData.id.IsNull())
                                {
                                    JsonNode? propNode = null;
                                    if (document.TryGetPropertyValue(prop.Name, out propNode))
                                    {
                                        JsonNode newNode = JsonValue.Create(instanceData.path);
                                        document[prop.Name] = newNode;
                                    }
                                }
                                else
                                {
                                    RxPlatformObjectRuntime? runtimeObj = relInstance as RxPlatformObjectRuntime;
                                    if (runtimeObj != null)
                                    {
                                        var tempPath = runtimeObj.Path;
                                        if (!string.IsNullOrEmpty(tempPath))
                                        {
                                            JsonNode? propNode = null;
                                            if (document.TryGetPropertyValue(prop.Name, out propNode))
                                            {
                                                JsonNode newNode = JsonValue.Create(tempPath);
                                                document[prop.Name] = newNode;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if(varProperties.Count > 0)
                    {
                        var options = new JsonSerializerOptions
                        {
                            TypeInfoResolver = new DefaultJsonTypeInfoResolver
                            {
                                Modifiers = { FilterAttributeProperties }
                            },
                            WriteIndented = true
                        };

                        foreach (var prop in varProperties)
                        {
                            JsonNode? defNode = null;
                            if(document.TryGetPropertyValue(prop.Name, out defNode))
                            {
                                document.Remove(prop.Name);
                            }   

                            JsonObject varNode = new JsonObject();
                            if (defNode != null)
                            {
                                varNode["_"] = defNode;
                            }
                            else
                            {
                                object? varVal = prop.GetValue(prototype);
                                varNode["_"] = JsonValue.Create(varVal);
                            }
                            foreach(var one in prop.GetCustomAttributes<RxPlatformVariableSourceAttribute>())
                            {
                                varNode[one.Element] = JsonSerializer.SerializeToNode(one, options);
                            }
                            foreach (var one in prop.GetCustomAttributes<RxPlatformVariableMapperAttribute>())
                            {
                                varNode[one.Element] = JsonSerializer.SerializeToNode(one, options);
                            }
                            foreach (var one in prop.GetCustomAttributes<RxPlatformFilterAttribute>())
                            {
                                varNode[one.Element] = JsonSerializer.SerializeToNode(one, options);
                            }

                            document[prop.Name] = varNode;
                        }
                    }
                    MemoryStream memstm = new MemoryStream();
                    Utf8JsonWriter writer = new Utf8JsonWriter(memstm);
                    document.WriteTo(writer);
                    writer.Flush();
                    jsonString = Encoding.UTF8.GetString(memstm.ToArray());
                }

            }

            return jsonString;
        }
        private void FillRuntimeDefinitions<T>(Dictionary<RxNodeId, PlatformTypeBuildMeta<T>> data) where T : RxPlatformTypeAttribute
        {
//            foreach (var kvp in data)
//            {
//                if (!kvp.Value.valid)
//                    continue;

//                var objType = kvp.Value;
//                if(objType.defaultConstructor == null)
//                {
//                    objType.valid = false;
//                    continue;
//                }
//                StringBuilder stream = new StringBuilder();
//                stream.AppendLine("{");
//                stream.AppendLine("\"def\":{");
//                stream.Append(@$"""programs"": [],
//""access"": {{
//{tabs}""roles"": []
//}},
//""overrides"": 
//");
//                data[kvp.Key] = objType;
//            }
        }
        public void FillTypes(PlatformTypeBuildData data)
        {
            FillRuntimeDefinitions(data.ObjectTypes);
            FillRuntimeDefinitions(data.PortTypes);
            FillRuntimeDefinitions(data.DomainTypes);
            FillRuntimeDefinitions(data.ApplicationTypes);

        }
    }
}