using ENSACO.RxPlatform.Attributes;
using ENSACO.RxPlatform.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotnetLibTest2
{

    [RxPlatformRuntime()]
    [RxPlatformObjectType(nodeId: "E8807CAB-B202-49DB-8959-7FDAC7887D9C")]
    public class OtherObjectType : RxPlatformObjectRuntime
    {
        public virtual string? MojProp { get; set; } = "Zika";
    }
}
