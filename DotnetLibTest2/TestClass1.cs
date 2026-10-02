using ENSACO.RxPlatform;
using ENSACO.RxPlatform.Attributes;
using ENSACO.RxPlatform.Model.Modbus;
using ENSACO.RxPlatform.Model.System;
using ENSACO.RxPlatform.Runtime;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotnetLibTest2
{
    [RxPlatformRuntime()]
    [RxPlatformObjectType(nodeId: "5F5199B1-B81D-4738-83D8-4753598AB4FF")]
    public class TestClass1 : RxPlatformObjectRuntime
    {

        public virtual int? IntProperty { get; set; } = 1000;

        public virtual event Action<int?>? OnIntPropertyChange;
        public virtual TestStruct Struktura { get; } = new TestStruct();

        public virtual OtherObjectType? OtherObject { get; set; } = null;

        public virtual event Action<OtherObjectType>? OnOtherObjectConnected;
        public virtual event Action<OtherObjectType>? OnOtherObjectDisconnected;

        public void TestClass1Started()
        {
            OnOtherObjectConnected += (obj) =>
            {
                IntProperty = obj.MojProp?.Length;
            };
        }

        public void TestClass1Stopping()
        {
            RxPlatformLog.WriteLogTrace("test", $"DynamicPlugin: TestClass1 Stopped called with IntProperty: {IntProperty}");
        }
        [OpcSimpleMethodMapper(Port = "OPC")]
        public async Task SomeFunction()
        {
            IntProperty = 99;
            RxPlatformLog.WriteLogTrace("test", $"DynamicPlugin: TestClass1 SomeFunction called with IntProperty: {IntProperty}");
            await Task.Delay(3000);
            RxPlatformLog.WriteLogTrace("test", $"DynamicPlugin: TestClass1 SomeFunction finished with IntProperty: {IntProperty}");


            if (OtherObject == DotnetLibTest2.otherObjects[0])
            {
                OtherObject = DotnetLibTest2.otherObjects[1];
            }
            else
            {
                OtherObject = DotnetLibTest2.otherObjects[0];
            }

        }

        [PortReference]
        public virtual string? OPC { get; init; } = "OpcServer";
    }

    [RxPlatformRuntime()]
    [RxPlatformObjectType(nodeId: "BE796A12-092E-4917-B437-12FFEDB5D3A8")]
    public class TestClassMaster : TestClass1
    {

        [OpcSimpleMapper(Write: true, Port = "OPCServer")]
        [ModbusHoldingRegisterSource(Output : true, Input: false, Port = "Modbus", Address = 55)]
        public override int? IntProperty { get; set; } = 5666;

        public virtual Task<bool> WriteIntProperty(int newValue)
        {
            return Task.FromResult(true);
        }


        public async Task ProbaWrite()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            IntProperty = 1234;
            sw.Stop();
            Console.WriteLine($"ProbaWrite took {sw.ElapsedMilliseconds} ms");

            sw.Start();
            await WriteIntProperty(5432);
            sw.Stop();
            Console.WriteLine($"ProbaWrite took {sw.ElapsedMilliseconds} ms");
        }
        public override TestStruct Struktura => new TestStruct2();

        public void TestClassMasterStarted()
        {
            RxPlatformLog.WriteLogTrace("test", $"DynamicPlugin: TestClassMaster Started called with IntProperty: {IntProperty}");
        }

        public void TestClassMasterStopping()
        {
            RxPlatformLog.WriteLogTrace("test", $"DynamicPlugin: TestClassMaster Stopped called with IntProperty: {IntProperty}");
        }

        [PortReference]
        public virtual ModbusMasterConnection? Modbus { get; init; } = null;

        //public virtual ModbusStructSource ModbusStruct { get; init; } = new ModbusStructSource();

    }

    [RxPlatformRuntime()]
    [RxPlatformObjectType(nodeId: "FC3E79A0-E01F-4489-9FF7-1C91B2E16092")]
    public class TestClassSlave : TestClass1
    {

        [OpcSimpleMapper(Write: true, Port = "OPCServer")]
        [ModbusHoldingRegister(Write: true, Port = "Modbus", Address = 55)]
        [LinearScaling(Input:true, Output : true, HiEU = 1000, LowEU = 0, HiRaw = 100, LowRaw = 0)]
        public override int? IntProperty { get; set; } = 7766;
        public override TestStruct Struktura => new TestStruct3();

        public void TestClassSlaveStarted()
        {
            RxPlatformLog.WriteLogTrace("test", $"DynamicPlugin: TestClassSlave Started called with IntProperty: {IntProperty}");
        }


        public void TestClassSlaveStopping()
        {
            RxPlatformLog.WriteLogTrace("test", $"DynamicPlugin: TestClassSlave Stopped called with IntProperty: {IntProperty}");
        }

        [PortReference]
        public virtual ModbusSlaveConnection? Modbus { get; init; } = null;

        //public virtual ModbusStructMapper ModbusStruct { get; init; } = new ModbusStructMapper();

    }

}
