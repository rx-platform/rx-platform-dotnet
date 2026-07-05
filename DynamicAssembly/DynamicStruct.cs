using ENSACO.RxPlatform;
using ENSACO.RxPlatform.Attributes;
using ENSACO.RxPlatform.Runtime;

namespace DynamicAssembly
{
    [RxPlatformRuntime()]
    [RxPlatformStructType(nodeId: "54EDB8C9-81B4-4E9D-A0CB-826C5407DAC4")]
    public class DynamicSubStruct : RxPlatformStructRuntime
    {
        public uint? SubPeriodMsZrna { get; init; } = 1000;
        public virtual string? SubPeriodString { get; set; } = "zikica";
        public bool SubZeljkoProp44 { get; } = true;



        public byte SubBorisa { get; init; } = 55;

        public void Started()
        {
            RxPlatformLog.WriteLogTrace("test", $"DynamicSubStruct: Started method called.SubPeriodString = {SubPeriodString}");
        }
        public void Stopping()
        {
            RxPlatformLog.WriteLogTrace("test", "DynamicSubStruct: Stopping method called.");
        }

    }

    [RxPlatformRuntime()]
    [RxPlatformStructType(nodeId: "C1AF1414-F289-4299-B958-E022E400389C")]
    public class DynamicStruct : RxPlatformStructRuntime
    {
        public DynamicStruct()
        {
        }
        public virtual string[]? StringArray { get; set; } = new string[] { "one", "two", "three" };

        public virtual event Action<string[]?>? OnStringArrayChange;
        public virtual DateTime? TimeProp { get; set; } = DateTime.Now;
        public uint? PeriodMsZrna { get; init; } = 1000;
        public virtual string? PeriodString { get; set; } = "zikica";
        public bool? ZeljkoProp44 { get; init; } = true;



        public virtual event Action<string?>? OnPeriodStringChange;

        public virtual byte? Borisa { get; set; } = 55;

        public virtual event ChangedObjProp4? OnBorisaChange;

        public virtual DynamicSubStruct SubStruct { get; } = new DynamicSubStruct
        {
            SubBorisa = 33,
            SubPeriodString = "SubZika222"
        };

        public void Started()
        {
            RxPlatformLog.WriteLogTrace("test", $"DynamicStruct: Started method called.SubPeriodString = {SubStruct.SubPeriodString}");
            OnBorisaChange += (newValue) =>
            {
                RxPlatformLog.WriteLogTrace("test", $"DynamicStruct: OnBorisaChange event fired. New Value: {newValue}");
            };
        }
        public void Stopping()
        {
            RxPlatformLog.WriteLogTrace("test", "DynamicStruct: Stopping method called.");
        }

    }
}
