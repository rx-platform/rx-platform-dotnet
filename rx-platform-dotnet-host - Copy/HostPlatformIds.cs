using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ENSACO.RxPlatform.Hosting.Internal
{
    internal static class HostPlatformIds
    {
        // source types
        public static readonly uint RX_VARIABLE_SOURCE_TYPE_ID = 0xc0000d9;
        public static readonly uint RX_PARENT_SOURCE_TYPE_ID = 0xc000047;

        // data type
        public static readonly uint RX_CLASS_DATA_BASE_ID = 0x00000015;

        public static readonly uint RX_CLASS_EVENT_BASE_NAME = 0x00000007;
        public static readonly uint RX_CLASS_FILTER_BASE_NAME = 0x0000000a;
        public static readonly uint RX_CLASS_VARIABLE_BASE_ID = 0x00000006;

        // object type
        public static readonly uint RX_CLASS_OBJECT_BASE_ID = 0x00000001;
        public static readonly uint RX_CLASS_DOMAIN_BASE_ID = 0x00000002;
        public static readonly uint RX_CLASS_APPLICATION_BASE_ID = 0x00000003;
        public static readonly uint RX_CLASS_PORT_BASE_ID = 0x00000004;

        // struct type
        public static readonly uint RX_CLASS_STRUCT_BASE_ID = 0x00000005;

        // mapper types
        public static readonly uint RX_VARIABLE_MAPPER_TYPE_ID = 0xc0000d3;
        public static readonly uint RX_PARENT_MAPPER_TYPE_ID = 0xc000045;
        public static readonly uint RX_EVENT_MAPPER_TYPE_ID = 0xc0000da;
        public static readonly uint RX_METHOD_MAPPER_TYPE_ID = 0xc0000d4;

        public static readonly uint RX_DOTNET_METHOD_TYPE_ID = 0xc000112;

        public static readonly uint RX_DOTNET_EVENT_TYPE_ID = 0xc000113;
        public static readonly uint RX_DOTNET_SOURCE_TYPE_ID = 0xc000114;
        public static readonly uint RX_DOTNET_MAPPER_TYPE_ID = 0xc000115;

        public static readonly uint RX_NS_SYSTEM_APP_ID = 0x0000000b;
        public static readonly uint RX_NS_SYSTEM_DOM_ID = 0x0000000c;
    }
}
