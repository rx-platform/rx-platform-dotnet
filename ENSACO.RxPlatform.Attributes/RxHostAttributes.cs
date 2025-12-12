using ENSACO.RxPlatform.Model;

namespace ENSACO.RxPlatform.Attributes
{
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
        public RxPlatformObjectType(string nodeId, string directory = "", string name = "")
            : base(nodeId, directory, name)
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
        public RxPlatformStructType(string nodeId, string directory = "", string name = "")
            : base(nodeId, directory, name)
        {
        }
    }


    [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class RxPlatformMapperType : RxPlatformTypeAttribute
    {
        public RxPlatformMapperType(string nodeId, string directory = "", string name = "")
            : base(nodeId, directory, name)
        {
        }
    }


    [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class RxPlatformMethodType : RxPlatformTypeAttribute
    {
        public RxPlatformMethodType(string nodeId, string directory = "", string name = "")
            : base(nodeId, directory, name)
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
        public RxPlatformEventType(string nodeId, string directory = "", string name = "")
            : base(nodeId, directory, name)
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
}
