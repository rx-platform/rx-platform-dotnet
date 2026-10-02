using ENSACO.RxPlatform.Attributes;
using ENSACO.RxPlatform.Runtime;

namespace TestingPlatform
{
    [RxPlatformRuntime()]
    [RxPlatformObjectType(nodeId: "D915E2C1-2EDB-4C80-AF9E-255A0646E7D2")]
    public class Class1 : RxPlatformObjectRuntime
    {
        public virtual string? MyProperty { get; set; } = "initial value";
        public virtual int? Counter { get; set; } = 0;		
        public void MyMethod()
        {
            Counter++;
            MyProperty = $"I've been called {Counter} times";
        }
    }
}

