using ENSACO.RxPlatform;
using ENSACO.RxPlatform.Attributes;
using ENSACO.RxPlatform.Model.Modbus;
using ENSACO.RxPlatform.Model.System;
using ENSACO.RxPlatform.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotnetLibTest2
{
    [RxPlatformRuntime()]
    [RxPlatformStructType(nodeId: "1D65561C-3BEF-41E9-8163-681B7240C746")]
    public class TestStruct : RxPlatformStructRuntime
    {
        public virtual int? IntProperty { get; set; } = 1000;

        public void TestStructStarted()
        {
            RxPlatformLog.WriteLogTrace("test", $"DynamicPlugin: TestStruct Started called with IntProperty: {IntProperty}");
        }


        public void TestStructStopping()
        {
            RxPlatformLog.WriteLogTrace("test", $"DynamicPlugin: TestStruct Stopped called with IntProperty: {IntProperty}");
        }
    }

    [RxPlatformRuntime()]
    [RxPlatformStructType(nodeId: "AFBE7085-72F5-456E-A441-F2D429A01989")]
    public class TestStruct2 : TestStruct
    {
        [ModbusHoldingRegisterSource(Output: true, Port = "Modbus", Address = 60)]
        public override int? IntProperty { get; set; } = 900;

        public void TestStruct2Started()
        {
            RxPlatformLog.WriteLogTrace("test", $"DynamicPlugin: TestStruct2 Started called with IntProperty: {IntProperty}");
        }
        public void TestStruct2Stopping()
        {
            RxPlatformLog.WriteLogTrace("test", $"DynamicPlugin: TestStruct2 Stopped called with IntProperty: {IntProperty}");
        }
    }

    [RxPlatformRuntime()]
    [RxPlatformStructType(nodeId: "A362E1E4-90E8-45C0-8534-D2AF9E8A5060")]
    public class TestStruct3 : TestStruct
    {
        [ModbusHoldingRegister(Write : true,Port = "Modbus", Address = 60)]
        public override int? IntProperty { get; set; } = 900;

        public void TestStruct3Started()
        {
            RxPlatformLog.WriteLogTrace("test", $"DynamicPlugin: TestStruct3 Started called with IntProperty: {IntProperty}");
        }

        public void TestStruct3Stopping()
        {
            RxPlatformLog.WriteLogTrace("test", $"DynamicPlugin: TestStruct3 Stopped called with IntProperty: {IntProperty}");
        }
    }
}
