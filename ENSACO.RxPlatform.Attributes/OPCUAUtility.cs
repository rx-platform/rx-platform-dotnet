using ENSACO.RxPlatform.Model;
using ENSACO.RxPlatform.Model.System;
using ENSACO.RxPlatform.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ENSACO.RxPlatform.OPCUA
{
    public struct OPCUAServerStack
    {
        public TCPServerPort TcpPort;
        public OpcBinTransport Transport;
        public OpcSecBase Security;
        public OpcServerBase[] Servers;
    }
    public struct OPCUAClientStack
    {
        public TCPClientPort TcpPort;
        public OpcClientBinTransport Transport;
        public OpcClientSecBase Security;
        public OpcClientBase Client;
    }
    public enum OPCUAType
    {
        SimpleBinary
    }
    public enum OPCUASecurityType
    {
        None,
        Basic128Rsa15,
        Basic256,
        Basic256Sha256,
        Aes128Sha256RsaOaep,
        Aes256Sha256RsaPss
    }
    public static class OPCUAUtility
    {
        public static OPCUAServerStack CreateOPCUAServer(int tcpPortNumber, string[] appNames
            , OPCUASecurityType securityType= OPCUASecurityType.None
            , OPCUAType serverType = OPCUAType.SimpleBinary)
        {
            var tcpPort = new TCPServerPort
            {
                Bind =
                {
                    IPPort = (ushort)tcpPortNumber
                },
                Timeouts =
                {
                    ReceiveTimeout = 50000,
                    SendTimeout = 5000
                }
            };


            OpcBinTransport? transport = null;
            OpcSecBase? security = null;
            List<OpcServerBase> servers = new List<OpcServerBase>();

            switch(serverType)
            {
                case OPCUAType.SimpleBinary:
                    transport = new OpcBinTransport
                    {
                        StackTop = tcpPort
                    };

                    switch (securityType)
                    {
                        case OPCUASecurityType.None:
                            security = new OpcBinSecNone
                            {
                                StackTop = transport
                            };
                            break;
                        default:
                            throw new Exception("Unsupported security type");
                    }
                    foreach (var appName in appNames)
                    {
                        if(string .IsNullOrEmpty(appName))
                        {
                            throw new Exception("Empty Application name");
                        }
                        var server = new OpcSimpleBinServer
                        {
                            StackTop = security,
                            Options = new OpcServerOptions
                            {
                                AppName = appName,
                                AppUri = "urn:rx-platform.org/" + appName
                            },
                            Bind =
                            {
                                Endpoint =  appName
                            }
                        };
                        servers.Add(server);
                    }
                    break;
                default:
                    throw new Exception("Unsupported server type");
            }
            if(transport == null || security == null)
            {
                throw new Exception("Shouldn't be null");
            }
            return new OPCUAServerStack
            {
                TcpPort = tcpPort,
                Transport = transport,
                Security = security,
                Servers = servers.ToArray()
            };
        }
        public async static Task DownloadStack(OPCUAServerStack stack, string prefix, string path, Assembly assembly)
        {
            if (stack.TcpPort != null)
            {
                await RxPlatformObjectRuntime.CreateObject(stack.TcpPort, prefix + "TcpServerPort"
                    , path, RxNodeId.NullId, assembly);
            }
            if (stack.Transport != null)
            {
                await RxPlatformObjectRuntime.CreateObject(stack.Transport, prefix + "Transport"
                    , path, RxNodeId.NullId, assembly);
            }
            if (stack.Security != null)
            {
                await RxPlatformObjectRuntime.CreateObject(stack.Security, prefix + "SecurityChannel"
                    , path, RxNodeId.NullId, assembly);
            }
            foreach(var server in stack.Servers)
            {
                await RxPlatformObjectRuntime.CreateObject(server, prefix + "Server" + server.Options.AppName
                    , path, RxNodeId.NullId, assembly);
            }
        }
    }
}
