using ENSACO.RxPlatform.Attributes;
using ENSACO.RxPlatform.Hosting.Internal;
using ENSACO.RxPlatform.Hosting.Model.Code;
using ENSACO.RxPlatform.Hosting.Reflection;
using ENSACO.RxPlatform.Model;
using ENSACO.RxPlatform.Model.System;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace ENSACO.RxPlatform.Hosting.Model
{

    internal class RxRuntimeDefinitionCreater
    {
        static RxNodeId ExtractDomain(object prototype)
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
                        if(RxMetaData.Instance.RegisteredObjects.TryGetValue(prototype, out var instanceData))
                        {
                            return instanceData.id;
                        }
                    }
                }
            }
            return RxNodeId.NullId;
        }
        static RxNodeId ExtractApp(object prototype)
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
            return RxNodeId.NullId;
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
                if (prototype != null)
                {
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
                }
                stream.AppendLine($"{tabs}\"app\":{{");
                stream.AppendLine($"{tabs}{tabs}\"path\":\"{appPath}\"");
                stream.AppendLine($"{tabs}}},");
                stream.AppendLine($"{tabs}\"sim\":true,");
                stream.AppendLine($"{tabs}\"proc\":true");
                stream.AppendLine("},");
                stream.AppendLine("\"overrides\":");
                return true;
            }
            else if (attr is RxPlatformDomainType)
            {
                stream.AppendLine($"{tabs}\"processor\":\"{processor}\",");
                stream.AppendLine($"{tabs}\"priority\":\"{priority}\",");

                if (prototype != null)
                {
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
                }
                stream.AppendLine($"{tabs}\"app\":{{");
                stream.AppendLine($"{tabs}{tabs}\"path\":\"{appPath}\"");
                stream.AppendLine($"{tabs}}}");
                stream.AppendLine("},");
                stream.AppendLine("\"overrides\":");
                return true;
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
        internal static string CreateOverrides(object prototype, HostedPlatformLibrary hostLib)
        {
            string jsonString = JsonSerializer.Serialize(prototype);

            var relationProperties = ReflectionHelpers.GetRelationsPropertyInfos(prototype.GetType());
            if (relationProperties != null && relationProperties.Length > 0)
            {

                var document = JsonNode.Parse(jsonString) as JsonObject;

                if (document != null)
                {
                    foreach (var prop in relationProperties)
                    {
                        object? relInstance = prop.GetValue(prototype);
                        if (relInstance == null)
                        {
                            continue;
                        }
                        PlatformInstanceData instanceData = new PlatformInstanceData();
                        lock(RxMetaData.Instance.TypesLock)
                        {
                            RxMetaData.Instance.RegisteredObjects.TryGetValue(relInstance, out instanceData);
                        }
                        if(!instanceData.id.IsNull())
                        {
                            JsonNode? propNode = null;
                            if (document.TryGetPropertyValue(prop.Name, out propNode))
                            {
                                JsonNode newNode = JsonValue.Create(instanceData.path);
                                document[prop.Name] = newNode;
                            }
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