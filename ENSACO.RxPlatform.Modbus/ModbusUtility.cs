using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ENSACO.RxPlatform.Model;
using ENSACO.RxPlatform.Model.Modbus;
using ENSACO.RxPlatform.Model.System;
using ENSACO.RxPlatform.Runtime;

namespace ENSACO.RxPlatform.Modbus
{
    public struct ModbusTcpSlaveStack
    {
        public TCPServerPort TcpPort;
        public ModbusTCPSlave ModbusTcp;
        public ByteRouterPort RouterPort;
        public List<ModbusSlaveConnection> Slaves;
    }
    public struct ModbusTcpMasterStack
    {
        public TCPClientPort TcpPort;
        public ModbusTCPMaster ModbusTcp;
        public ByteRouterPort RouterPort;
        public List<ModbusMasterConnection> Slaves;
    }
    public static class ModbusUtility
    {
        public static ModbusTcpSlaveStack CreateModbusTcpSlaves(int tcpPortNumber, byte[] slaveAddresses)
        {
            var tcpPort = new TCPServerPort
            {
                Bind =
                {
                    IPPort = (ushort)tcpPortNumber
                },
                Timeouts =
                {
                    ReceiveTimeout = 5000,
                    SendTimeout = 5000
                }
            };
            var modbusTcpSlave = new ModbusTCPSlave
            {
                StackTop = tcpPort
            };
            var routerPort = new ByteRouterPort
            {
                StackTop = modbusTcpSlave,
                Options =
                {
                    Listener = true
                }
            };
            var slaves = new List<ModbusSlaveConnection>();
            foreach (var address in slaveAddresses)
            {
                var slaveConnection = new ModbusSlaveConnection
                {
                    Bind =
                    {
                        Address = address
                    },
                    StackTop = routerPort,
                };
                slaves.Add(slaveConnection);
            }
            return new ModbusTcpSlaveStack
            {
                TcpPort = tcpPort,
                ModbusTcp = modbusTcpSlave,
                RouterPort = routerPort,
                Slaves = slaves
            };
        }
        public async static Task DownloadStack(ModbusTcpSlaveStack slave, string prefix, string path, Assembly assembly)
        {
            if (slave.TcpPort != null)
            {
                await RxPlatformObjectRuntime.CreateObject(slave.TcpPort, prefix + "TcpServerPort"
                    ,path, RxNodeId.NullId, assembly);
            }
            if (slave.ModbusTcp != null)
            {
                await RxPlatformObjectRuntime.CreateObject(slave.ModbusTcp, prefix + "ModbusTcpPort"
                    , path, RxNodeId.NullId, assembly);
            }
            if (slave.RouterPort != null)
            {
                await RxPlatformObjectRuntime.CreateObject(slave.RouterPort, prefix + "RouterPort"
                    , path, RxNodeId.NullId, assembly);
            }
            if (slave.Slaves != null)
            {
                foreach (var modbusSlave in slave.Slaves)
                {
                    await RxPlatformObjectRuntime.CreateObject(modbusSlave, prefix + $"ModbusSlave_{modbusSlave.Bind.Address}"
                    , path, RxNodeId.NullId, assembly);
                }
            }
        }
        public static ModbusTcpMasterStack CreateModbusTcpMasters(string addr, int tcpPortNumber, byte[] slaveAddresses)
        {
            var tcpPort = new TCPClientPort
            {
                Connect =
                {
                    IPAddress = addr,
                    IPPort = (ushort)tcpPortNumber
                },
                Timeouts =
                {
                    ReceiveTimeout = 5000,
                    SendTimeout = 5000
                }
            };
            var modbusTcpSlave = new ModbusTCPMaster
            {
                StackTop = tcpPort
            };
            var routerPort = new ByteRouterPort
            {
                StackTop = modbusTcpSlave,
                Options =
                {
                    Initiator = true
                }
            };
            var slaves = new List<ModbusMasterConnection>();
            foreach (var address in slaveAddresses)
            {
                var slaveConnection = new ModbusMasterConnection
                {
                    Connect =
                    {
                        Address = address
                    },
                    StackTop = routerPort,
                };
                slaves.Add(slaveConnection);
            }
            return new ModbusTcpMasterStack
            {
                TcpPort = tcpPort,
                ModbusTcp = modbusTcpSlave,
                RouterPort = routerPort,
                Slaves = slaves
            };
        }
        public async static Task DownloadStack(ModbusTcpMasterStack master, string prefix, string path, Assembly assembly)
        {
            if (master.TcpPort != null)
            {
                await RxPlatformObjectRuntime.CreateObject(master.TcpPort, prefix + "TcpClientPort"
                    , path, RxNodeId.NullId, assembly);
            }
            if (master.ModbusTcp != null)
            {
                await RxPlatformObjectRuntime.CreateObject(master.ModbusTcp, prefix + "ModbusTcpPort"
                    , path, RxNodeId.NullId, assembly);
            }
            if (master.RouterPort != null)
            {
                await RxPlatformObjectRuntime.CreateObject(master.RouterPort, prefix + "RouterPort"
                    , path, RxNodeId.NullId, assembly);
            }
            if (master.Slaves != null)
            {
                foreach (var modbusSlave in master.Slaves)
                {
                    await RxPlatformObjectRuntime.CreateObject(modbusSlave, prefix + $"ModbusSlave_{modbusSlave.Connect.Address}"
                    , path, RxNodeId.NullId, assembly);
                }
            }
        }
    }
}
