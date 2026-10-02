using ENSACO.RxPlatform.Attributes;
using ENSACO.RxPlatform.Model.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ENSACO.RxPlatform.Model.Modbus;

namespace DynamicAssembly
{

    [RxPlatformDeclare()]
    [RxPlatformVariableType(nodeId: "383148A9-A85C-46AA-AC6F-FE3E9BFC715A")]
    public class DynamicSlaveVariable<T> : SimpleVariable<T>
    {
    //    public OpcSimpleMapper OPC { get; set; } = new OpcSimpleMapper
    //    {
    //        Port = "OPCServer2"
    //    };
    //    public ModbusHoldingRegister HR { get; set; } = new ModbusHoldingRegister
    //    {
    //        Port = "ModbusSlave"
    //    };

    //    public RegisterSource Reg { get; set; } = new RegisterSource { };

    }
    [RxPlatformDeclare()]
    [RxPlatformVariableType(nodeId: "C863B95F-FECB-4D81-A632-D6E7BF87BE79")]
    public class DynamicMasterVariable<T> : SimpleVariable<T>
    {
        //public OpcSimpleMapper OPC { get; set; } = new OpcSimpleMapper
        //{
        //    Port = "OPCServer"
        //};
        //public ModbusHoldingRegisterSource HR { get; set; } = new ModbusHoldingRegisterSource
        //{
        //    Address = 0,
        //    Port = "ModbusMaster"
        //};
        //public LinearScaling Scaling { get; set; } = new LinearScaling
        //{
        //    HiEU = 2,
        //    LowEU = 0,
        //    HiRaw = 1,
        //    LowRaw = 0
        //};
        //public uint ObjectProp1111 { get; init; } = 1000;
        //public virtual string? ObjectProp2 { get; set; } = "zikica";
    }

    [RxPlatformDeclare()]
    [RxPlatformVariableType(nodeId: "BB63E70F-A998-45F2-8A11-B1231AE8F4C9")]
    public class DynamicOPCServerVariable<T> : SimpleVariable<T>
    {
        //public OpcSimpleMapper OPC { get; set; } = new OpcSimpleMapper
        //{
        //    Port = "OPCServer"
        //};
        //public RegisterSource Reg { get; set; } = new RegisterSource { };
    }


    [RxPlatformRuntime()]
    [RxPlatformObjectType(nodeId: "DCDAFF71-68BE-4F2F-B3C7-F7500C960E64")]
    public class ExtendedDynamicObject : DynamicObject
    {
        [RegisterSource(Output: true)]
        [ModbusHoldingRegister(Output: true, Port = "ModbusSlave", Address = 1)]
        [OpcSimpleMapper(Output: true, Port = "OPCServer")]
        public override string? ObjectProp2 { get; set; } = "jbg from variable";


        [ModbusHoldingRegisterSource(Output: true, Port = "ModbusMaster", Address = 1)]
        [OpcSimpleMapper(Output: true, Port = "OPCServer")]
        [LinearScaling(Output: true, HiEU = 10, LowEU = 0, HiRaw = 100, LowRaw = 0)]
        public override byte? ObjectProp4 { get; set; } = 55;

        [RegisterSource(Output:true)]
        [OpcSimpleMapper(Output: true, Port = "OPCServer")]
        public override bool? ObjectProp3 { get; set; } = false;

        [PortReference()]
        public ModbusSlaveConnection? ModbusSlave { get; set; }

        [PortReference()]
        public ModbusMasterConnection? ModbusMaster { get; set; }

        [PortReference()]
        public OpcServerBase? OPCServer { get; set; }
        [PortReference()]
        public OpcServerBase? OPCServer2 { get; set; }

        [OpcSimpleMethodMapper(Port = "OPCServer")]
        public async Task TestnaFunckija()
        {
            ObjectProp3 = !ObjectProp3;
            await Task.Delay(2000);
            ObjectProp3 = !ObjectProp3;
        }

        [OpcSimpleMapper(Port = "OPCServer")]
        [PlatformSource(Path: ".OPCServer.Status.Endpoints")]
        [CalcFilter(InPath: "x + 100")]
        public virtual int? IsOPCOnline { get; } = 0;

        //public ModbusStructSource SrcMap { get; } = new ModbusStructSource
        //{
        //    HoldingRegAddress = 1
        //};
    }
}
