using ENSACO.RxPlatform.Attributes;
using ENSACO.RxPlatform.Runtime;

namespace DynamicAssembly
{
    [RxPlatformRuntime()]
    [RxPlatformStructType(nodeId: "54EDB8C9-81B4-4E9D-A0CB-826C5407DAC4")]
    public class DynamicSubStruct : RxPlatformStructRuntime
    {
        public uint? SubPeriodMsZrna { get; set; } = 1000;
        public virtual string? SubPeriodString { get; set; } = "zikica";
        public bool? SubZeljkoProp44 { get; set; } = true;



        public byte? SubBorisa { get; set; } = 55;

        public void Started()
        {
            Console.WriteLine($"DynamicSubStruct: Started method called.SubPeriodString = {SubPeriodString}");
        }
        public void Stopping()
        {
            Console.WriteLine("DynamicSubStruct: Stopping method called.");
        }

    }

    [RxPlatformRuntime()]
    [RxPlatformStructType(nodeId: "C1AF1414-F289-4299-B958-E022E400389C")]
    public class DynamicStruct : RxPlatformStructRuntime
    {
        public uint? PeriodMsZrna { get; set; } = 1000;
        public virtual string? PeriodString { get; set; } = "zikica";
        public bool? ZeljkoProp44 { get; set; } = true;



        public virtual event Action<string?>? OnPeriodStringChange;

        public byte? Borisa { get; set; } = 55;

        public virtual DynamicSubStruct? SubStruct { get; } = new DynamicSubStruct
        {
            SubBorisa = 33,
            SubPeriodString = "SubZika222"
        };

        public void Started()
        {
            Console.WriteLine($"DynamicStruct: Started method called.SubPeriodString = {SubStruct.SubPeriodString}");
        }
        public void Stopping()
        {
            Console.WriteLine("DynamicStruct: Stopping method called.");
        }

    }
}
