using ENSACO.RxPlatform.Model;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization.Metadata;

namespace ENSACO.RxPlatform.Attributes
{
    [System.AttributeUsage(System.AttributeTargets.All, AllowMultiple = false, Inherited = false)]
    public class RxPlatformIgnoreAttribute : Attribute
    {
        public RxPlatformIgnoreAttribute()
        {
        }
    }
    [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class RxPlatformLibrary : Attribute
    {
        public RxPlatformLibrary()
        {
        }
    }
    //
    [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class RxPlatformLibraryVersion : Attribute
    {
        uint version;
        public RxPlatformLibraryVersion(ushort major = 0, ushort minor = 0)
        {
            version = ((uint)major << 16) | minor;
        }
        public uint Version { get { return version; } }
    }
    [System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
    public class RxPlatformDeclareAttribute : Attribute
    {
        public RxPlatformDeclareAttribute()
        {
        }
    }
    [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class RxPlatformRuntimeAttribute : Attribute
    {
        public RxPlatformRuntimeAttribute()
        {
        }
    }
    [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class RxPlatformObjectMonitorAttribute : Attribute
    {
        public RxPlatformObjectMonitorAttribute()
        {
        }
    }
    [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class RxPlatformStructMonitorAttribute : Attribute
    {
        public RxPlatformStructMonitorAttribute()
        {
        }
    }
    public class RxPlatformTypeAttribute : Attribute
    {
        public RxNodeId ParentId { get; }
        public RxNodeId NodeId { get; }
        public string Directory { get; }
        public string Name { get; }

        public RxPlatformTypeAttribute(string nodeId, string directory = "", string name = "", string parentId = "")
        {
            NodeId = RxNodeId.FromString(nodeId);
            ParentId = RxNodeId.FromString(parentId);
            Directory = directory;
            Name = name;
        }
    }
    [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class RxPlatformSourceType : RxPlatformTypeAttribute
    {
        public RxPlatformSourceType(string nodeId, string directory = "", string name = "")
            : base(nodeId, directory, name)
        {
        }
    }
    [System.AttributeUsage(System.AttributeTargets.Class|System.AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
    public class RxPlatformDataType : RxPlatformTypeAttribute
    {
        public RxPlatformDataType(string nodeId, string directory = "", string name = "")
            : base(nodeId, directory, name)
        {
        }
    }
    [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class RxPlatformObjectType : RxPlatformTypeAttribute
    {
        public RxPlatformObjectType(string nodeId, string directory = "", string name = "", string parentId = "")
            : base(nodeId, directory, name, parentId)
        {
        }
    }
    [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class RxPlatformPortType : RxPlatformTypeAttribute
    {
        public RxPlatformPortType(string nodeId, string directory = "", string name = "")
            : base(nodeId, directory, name)
        {
        }
    }
    [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class RxPlatformDomainType : RxPlatformTypeAttribute
    {
        public RxPlatformDomainType(string nodeId, string directory = "", string name = "")
            : base(nodeId, directory, name)
        {
        }
    }
    [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class RxPlatformApplicationType : RxPlatformTypeAttribute
    {
        public RxPlatformApplicationType(string nodeId, string directory = "", string name = "")
            : base(nodeId, directory, name)
        {
        }
    }

    [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class RxPlatformStructType : RxPlatformTypeAttribute
    {
        public RxPlatformStructType(string nodeId, string directory = "", string name = "", string parentId = "")
            : base(nodeId, directory, name, parentId)
        {
        }
    }


    [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class RxPlatformMapperType : RxPlatformTypeAttribute
    {
        public RxPlatformMapperType(string nodeId, string directory = "", string name = "", string parentId = "")
            : base(nodeId, directory, name, parentId)
        {
        }
    }

    public class RxMethodConstructionData
    {
        public RxNodeId methodId = RxNodeId.NullId;
        
        public string GetMethodName()
        {
            StringBuilder stream = new StringBuilder();
            stream.Append("Method");
            foreach (var attr in mapperAttributes)
            {
                stream.Append("_");
                stream.Append(attr.Name);
            }
            return stream.ToString();
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

        public JsonObject GetInitData()
        {
            JsonObject initData = new JsonObject();
            var options = new JsonSerializerOptions
            {
                TypeInfoResolver = new DefaultJsonTypeInfoResolver
                {
                    Modifiers = { FilterAttributeProperties }
                },
                WriteIndented = true
            };
            foreach (var attr in mapperAttributes)
            {
                initData[attr.Element] = JsonSerializer.SerializeToNode(attr, attr.GetType(), options);
                string temp = JsonSerializer.Serialize(attr, attr.GetType(), options);
            }
            return initData;
        }
        public List<RxPlatformMethodMapperAttribute> mapperAttributes = new List<RxPlatformMethodMapperAttribute>();
     
    }


    [System.AttributeUsage(System.AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class RxPlatformMethodType : RxPlatformTypeAttribute
    {
        public RxPlatformMethodType(string nodeId, string directory = "", string name = "", string parentId = "")
            : base(nodeId, directory, name, parentId)
        {
        }
    }
    [System.AttributeUsage(System.AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class RxPlatformAbstractMethod : RxPlatformMethodType
    {
        public RxPlatformAbstractMethod(string nodeId = "", string directory = "", string name = "", string parentId = "")
            : base(nodeId == "" ? "1:i:19" : nodeId, directory, name, parentId)
            
        {
        }
    }
    [System.AttributeUsage(System.AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class RxPlatformRelationAttribute : RxPlatformTypeAttribute
    {
        protected RxPlatformRelationAttribute(string nodeId, string directory = "", string name = "")
            : base(nodeId, directory, name)
        {
        }
    }

    [System.AttributeUsage(System.AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
    public class RxPlatformVariableSourceAttribute : RxPlatformTypeAttribute
    {
        protected RxPlatformVariableSourceAttribute(string nodeId, string directory = "", string name = ""  , string Element = "", bool Input = false, bool Output = false, bool Sim = false, bool Proc = false)
            : base(nodeId, directory, name)
        {
            this.Element = Element;
            this.Input = Input;
            this.Output = Output;
            this.Sim = Sim;
            this.Proc = Proc;
        }
        public string Element { get; }
        public bool Input { get; }
        public bool Output { get; }
        public bool Sim { get; }
        public bool Proc { get; }
    }

    [System.AttributeUsage(System.AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
    public class RxPlatformFilterAttribute : RxPlatformTypeAttribute
    {
        protected RxPlatformFilterAttribute(string nodeId, string directory = "", string name = "", string Element = "", bool Input = false, bool Output = false, bool Sim = false, bool Proc = false)
            : base(nodeId, directory, name)
        {
            this.Element = Element;
            this.Input = Input;
            this.Output = Output;
            this.Sim = Sim;
            this.Proc = Proc;
        }
        public string Element { get; }
        public bool Input { get; }
        public bool Output { get; }
        public bool Sim { get; }
        public bool Proc { get; }
        
    }
    [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class RxPlatformStructuralSourceAttribute : RxPlatformTypeAttribute
    {
        protected RxPlatformStructuralSourceAttribute(string nodeId, string directory = "", string name = "", string Element = "", bool Input = false, bool Output = false, bool Sim = false, bool Proc = false)
            : base(nodeId, directory, name)
        {

            this.Element = Element;
            this.Input = Input;
            this.Output = Output;
            this.Sim = Sim;
            this.Proc = Proc;
        }

        public string Element { get; }
        public bool Input { get; }
        public bool Output { get; }
        public bool Sim { get; }
        public bool Proc { get; }
        
    }


    [System.AttributeUsage(System.AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
    public class RxPlatformVariableMapperAttribute : RxPlatformTypeAttribute
    {
        protected RxPlatformVariableMapperAttribute(string nodeId, string directory = "", string name = "", string Element = "", bool Write = false, bool Read = true, bool Sim = false, bool Proc = false)
            : base(nodeId, directory, name)
        {

            this.Element = Element;
            this.Write = Write;
            this.Read = Read;
            this.Sim = Sim;
            this.Proc = Proc;
        }

        public string Element { get; }
        public bool Write { get; }
        public bool Read { get; }
        public bool Sim { get; }
        public bool Proc { get; }
        
    }
    [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class RxPlatformStructuralMapperAttribute : RxPlatformTypeAttribute
    {
        protected RxPlatformStructuralMapperAttribute(string nodeId, string directory = "", string name = "", string Element = "", bool Write = false, bool Read = true, bool Sim = false, bool Proc = false)
            : base(nodeId, directory, name)
        {

            this.Element = Element;
            this.Read = Read;
            this.Write = Write;
            this.Sim = Sim;
            this.Proc = Proc;
        }

        public string Element { get; }
        public bool Write { get; }
        public bool Read { get; }
        public bool Sim { get; }
        public bool Proc { get; }
        
    }

    [System.AttributeUsage(System.AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class RxPlatformMethodMapperAttribute : RxPlatformTypeAttribute
    {
        protected RxPlatformMethodMapperAttribute(string nodeId, string directory = "", string name = "", string Element = "", bool Write = false, bool Read = true, bool Sim = false, bool Proc = false)
            : base(nodeId, directory, name)
        {

            this.Element = Element;
            this.Write = Write;
            this.Read = Read;
            this.Sim = Sim;
            this.Proc = Proc;
        }

        public string Element { get; }
        public bool Write { get; }
        public bool Read { get; }
        public bool Sim { get; }
        public bool Proc { get; }
        
    }



    [System.AttributeUsage(System.AttributeTargets.Event, AllowMultiple = false, Inherited = false)]
    public class RxPlatformEventMapperAttribute : RxPlatformTypeAttribute
    {
        protected RxPlatformEventMapperAttribute(string nodeId, string directory = "", string name = "", string Element = "", bool Write = false, bool Read = true, bool Sim = false, bool Proc = false)
            : base(nodeId, directory, name)
        {

            this.Element = Element;
            this.Write = Write;
            this.Read = Read;
            this.Sim = Sim;
            this.Proc = Proc;
        }

        public string Element { get; }
        public bool Write { get; }
        public bool Read { get; }
        public bool Sim { get; }
        public bool Proc { get; }
        
    }

    public class RxVariableConstructionData
    {
        public RxNodeId variableId = RxNodeId.NullId;
        public string GetVariableName()
        {
            StringBuilder stream = new StringBuilder();
            stream.Append("Variable");
            foreach (var attr in sourceAttributes)
            {
                stream.Append("_");
                stream.Append(attr.Name);
                stream.Append("_");
                if (attr.Input && attr.Output)
                    stream.Append("IO");
                else if(attr.Input)
                    stream.Append("I");
                else if(attr.Output)
                    stream.Append("O");
            }
            foreach (var attr in mapperAttributes)
            {
                stream.Append("_");
                stream.Append(attr.Name);
                stream.Append("_");
                if (attr.Read && attr.Write)
                    stream.Append("RW");
                else if (attr.Read)
                    stream.Append("R");
                else if (attr.Write)
                    stream.Append("W");
            }
            foreach (var attr in filterAttributes)
            {
                stream.Append("_");
                stream.Append(attr.Name);
                stream.Append("_");
                if (attr.Input && attr.Output)
                    stream.Append("IO");
                else if (attr.Input)
                    stream.Append("I");
                else if (attr.Output)
                    stream.Append("O");
            }
            return stream.ToString();
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

        public JsonObject GetInitData()
        {
            JsonObject initData = new JsonObject();
            var options = new JsonSerializerOptions
            {
                TypeInfoResolver = new DefaultJsonTypeInfoResolver
                {
                    Modifiers = { FilterAttributeProperties }
                },
                WriteIndented = true
            };
            foreach (var attr in sourceAttributes)
            {
                initData[attr.Element] = JsonSerializer.SerializeToNode(attr, attr.GetType(), options);
            }
            foreach (var attr in mapperAttributes)
            {
                initData[attr.Element] = JsonSerializer.SerializeToNode(attr, attr.GetType(), options);
                string temp = JsonSerializer.Serialize(attr, attr.GetType(), options);
            }
            foreach (var attr in filterAttributes)
            {
                initData[attr.Element] = JsonSerializer.SerializeToNode(attr, attr.GetType(), options);
            }
            return initData;
        }
        public List<RxPlatformVariableSourceAttribute> sourceAttributes = new List<RxPlatformVariableSourceAttribute>();
        public List<RxPlatformVariableMapperAttribute> mapperAttributes = new List<RxPlatformVariableMapperAttribute>();
        public List<RxPlatformFilterAttribute> filterAttributes = new List<RxPlatformFilterAttribute>();
    }

    [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class RxPlatformVariableType : RxPlatformTypeAttribute
    {
        public RxPlatformVariableType(string nodeId, string directory = "", string name = "")
            : base(nodeId, directory, name)
        {
        }
    }

    [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class RxPlatformEventType : RxPlatformTypeAttribute
    {
        public Type? Arguments { get; }
        public RxPlatformEventType(string nodeId, string directory = "", string name = "", Type? argType = null, string parentId = "")
            : base(nodeId, directory, name, parentId)
        {
            Arguments = argType;
        }
    }
    [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class RxPlatformEventType<T> : RxPlatformEventType
    {
        public RxPlatformEventType(string nodeId, string directory = "", string name = "", string parentId = "")
            : base(nodeId, directory, name, typeof(T), parentId)
        {
        }
    }

    [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class RxPlatformFilterType : RxPlatformTypeAttribute
    {
        public RxPlatformFilterType(string nodeId, string directory = "", string name = "")
            : base(nodeId, directory, name)
        {
        }
    }

    [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class RxPlatformDisplayType : RxPlatformTypeAttribute
    {
        public RxPlatformDisplayType(string nodeId, string directory = "", string name = "", string parentId = "")
            : base(nodeId, directory, name, parentId)
        {
        }
    }
}
