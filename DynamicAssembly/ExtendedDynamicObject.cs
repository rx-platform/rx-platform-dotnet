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
        public ModbusHoldingRegister HR { get; } = new ModbusHoldingRegister
        {
            Port = "ModbusSlave"
        };

        public RegisterSource Reg { get; set; } = new RegisterSource { };

    }
    [RxPlatformDeclare()]
    [RxPlatformVariableType(nodeId: "C863B95F-FECB-4D81-A632-D6E7BF87BE79")]
    public class DynamicMasterVariable<T> : SimpleVariable<T>
    {
        public ModbusHoldingRegisterSource HR { get; set; } = new ModbusHoldingRegisterSource
        {
            Port = "ModbusMaster"
        };
    }


    [RxPlatformDeclare()]
    [RxPlatformObjectType(nodeId: "DCDAFF71-68BE-4F2F-B3C7-F7500C960E64")]
    public class ExtendedDynamicObject : DynamicObject
    {
        public new DynamicSlaveVariable<string> ObjectProp2 { get; set; } = new DynamicSlaveVariable<string>
        {
            _ = "jbg from variable"
        };
        public new DynamicMasterVariable<byte> ObjectProp4 { get; } = new DynamicMasterVariable<byte>();

        [PortReference()]
        public ModbusSlaveConnection? ModbusSlave { get; set; }

        [PortReference()]
        public ModbusMasterConnection? ModbusMaster { get; set; }
    }
}
