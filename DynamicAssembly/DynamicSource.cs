using ENSACO.RxPlatform.Attributes;
using ENSACO.RxPlatform.Runtime;

namespace DynamicAssembly
{
    [RxPlatformRuntime()]
    [RxPlatformSourceType(nodeId: "d290f1ee-6c54-4b01-90e6-d701748f0851")]
    public class DynamicSource : RxPlatformSourceRuntime
    {
        public uint? PeriodMsZrna { get; set; } = 1000;
        public virtual string? PeriodString { get; set; } = "zikica";
        public bool? ZeljkoProp44 { get; set; } = true;



        public virtual event Action<string?>? OnPeriodStringChange;

        public byte? Borisa { get; set; } = 55;

        public void Started()
        {
            OnPeriodStringChange += DynamicSource_OnPeriodStringChange;
            Console.WriteLine($"DynamicSource: Started method called.SubPeriodString = {SubStruct.SubPeriodString}");
            SourceChanged("perica");
        }

        public void SourceWrite(double newValue, Action<Exception?> callback)
        {
            SourceChanged(newValue);
            callback(null);
        }

        private void DynamicSource_OnPeriodStringChange(string? obj)
        {
            if (obj == null)
                SourceChangedBad();
            else
                SourceChanged(obj + SubStruct.SubPeriodString);
        }



        public virtual DynamicSubStruct SubStruct { get; } = new DynamicSubStruct
        {
            SubBorisa = 42,
            SubPeriodString = "SubZika"
        };

        public void Stopping()
        {
            Console.WriteLine("DynamicSource: Stopping method called.");
        }

    }
}
