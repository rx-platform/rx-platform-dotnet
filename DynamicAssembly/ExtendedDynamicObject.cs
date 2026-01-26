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
        public OpcSimpleMapper OPC { get; set; } = new OpcSimpleMapper
        {
            Port = "OPCServer2"
        };
        public ModbusHoldingRegister HR { get; set; } = new ModbusHoldingRegister
        {
            Port = "ModbusSlave"
        };

        public RegisterSource Reg { get; set; } = new RegisterSource { };

    }
    [RxPlatformDeclare()]
    [RxPlatformVariableType(nodeId: "C863B95F-FECB-4D81-A632-D6E7BF87BE79")]
    public class DynamicMasterVariable<T> : SimpleVariable<T>
    {
        public OpcSimpleMapper OPC { get; set; } = new OpcSimpleMapper
        {
            Port = "OPCServer"
        };
        public ModbusHoldingRegisterSource HR { get; set; } = new ModbusHoldingRegisterSource
        {
            Address = 0,
            Port = "ModbusMaster"
        };
        public LinearScaling Scaling { get; set; } = new LinearScaling
        {
            HiEU = 2,
            LowEU = 0,
            HiRaw = 1,
            LowRaw = 0
        };
    }

    [RxPlatformDeclare()]
    [RxPlatformVariableType(nodeId: "BB63E70F-A998-45F2-8A11-B1231AE8F4C9")]
    public class DynamicOPCServerVariable<T> : SimpleVariable<T>
    {
        public OpcSimpleMapper OPC { get; set; } = new OpcSimpleMapper
        {
            Port = "OPCServer"
        };
        public RegisterSource Reg { get; set; } = new RegisterSource { };
    }


    [RxPlatformDeclare()]
    [RxPlatformObjectType(nodeId: "DCDAFF71-68BE-4F2F-B3C7-F7500C960E64")]
    public class ExtendedDynamicObject : DynamicObject
    {
        public new DynamicSlaveVariable<string> ObjectProp2 { get; set; } = new DynamicSlaveVariable<string>
        {
            _ = "jbg from variable",
        };
        public new DynamicMasterVariable<byte> ObjectProp4 { get; } = new DynamicMasterVariable<byte>();

        public new DynamicOPCServerVariable<bool> ObjectProp3 { get; } = new DynamicOPCServerVariable<bool>();

        [PortReference()]
        public ModbusSlaveConnection? ModbusSlave { get; set; }

        [PortReference()]
        public ModbusMasterConnection? ModbusMaster { get; set; }

        [PortReference()]
        public OpcServerBase? OPCServer { get; set; }
        [PortReference()]
        public OpcServerBase? OPCServer2 { get; set; }

        public ModbusStructSource SrcMap { get; } = new ModbusStructSource
        {
            HoldingRegAddress = 1
        };
    }
}
