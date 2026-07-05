using DynamicAssembly.SubNamespace;
using ENSACO.RxPlatform;
using ENSACO.RxPlatform.Attributes;
using ENSACO.RxPlatform.Runtime;

namespace DynamicAssembly
{
    [RxPlatformObjectMonitor()]
    public class SomeMonitored : RxPlatformObjectRuntime
    {
        public void Started()
        {

        }

        [RxPlatformAbstractMethod]
        public virtual Task SomeAbstract()
        {
            throw new NotImplementedException("SomeAbstract method not implemented.");
        }

        [RxPlatformAbstractMethod]
        public virtual Task<OutArgs> SomeWithRes()
        {
            throw new NotImplementedException("SomeWithRes method not implemented.");
        }

        [RxPlatformAbstractMethod]
        public virtual Task<OutArgs> SomeWithArgRes(InArgs args)
        {
            throw new NotImplementedException("SomeWithArgRes method not implemented.");
        }

        public Task BorisaPretrcavanje()
        {
            RxPlatformLog.WriteLogTrace("test", "SomeOtherDynamicObject: BorisaPretrcavanje method called.");
            return Task.CompletedTask;
        }
        public virtual string? OtherProp1 { get; set; } = "other prop value";
        public virtual uint? OtherProp2 { get; set; } = 33;
    }
}
