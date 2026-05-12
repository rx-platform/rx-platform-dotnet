using ENSACO.RxPlatform.Model;
using ENSACO.RxPlatform.Model.System;
using ENSACO.RxPlatform.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ENSACO.RxPlatform.MQTT
{
    public struct MQTTClientStack
    {
        public TCPClientPort TcpPort;
        public MqttSimpleClient Client;
    }
    public static class MQTTUtility
    {
        public static MQTTClientStack CreateBinMQTTClient(string ipAddress, int port, string clientId, string topicBase = "")
        {
            var tcpPort = new TCPClientPort
            {
                Connect =
                {
                    IPAddress = ipAddress,
                    IPPort = (ushort)port
                },
                Timeouts =
                {
                    ReceiveTimeout = 50000,
                    SendTimeout = 5000
                }
            };

            var mttclient = new MqttSimpleClient
            {
                StackTop = tcpPort,
                Options = new MqttClientOptions
                {
                    ClientID = clientId,
                    KeepAlive = 60,
                    PublishTimeBuffer = 200,
                    TopicBase = topicBase
                },
                
            };
            return new MQTTClientStack
            {
                TcpPort = tcpPort,
                Client = mttclient
            };
        }
        public async static Task DownloadStack(MQTTClientStack stack, string prefix, string path, Assembly assembly)
        {
            if (stack.TcpPort != null)
            {
                await RxPlatformObjectRuntime.CreateObject(stack.TcpPort, prefix + "TcpServerPort"
                    , path, RxNodeId.NullId, assembly);
            }
            if (stack.Client != null)
            {
                await RxPlatformObjectRuntime.CreateObject(stack.Client, prefix + "Client"
                    , path, RxNodeId.NullId, assembly);
            }
        }
    }
}
