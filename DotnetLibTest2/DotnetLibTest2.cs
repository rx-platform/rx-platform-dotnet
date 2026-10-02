using ENSACO.RxPlatform;
using ENSACO.RxPlatform.Attributes;
using ENSACO.RxPlatform.Host;
using ENSACO.RxPlatform.Modbus;
using ENSACO.RxPlatform.Runtime;
using System.Reflection;

namespace DotnetLibTest2
{
    [RxPlatformLibrary()]
    public class DotnetLibTest2
    {
        internal static OtherObjectType?[] otherObjects = [];
        public static PlatformLibraryInfo Initialize(string path)
        {
            RxPlatformLog.WriteLogTrace("test", $"DynamicPlugin: Initialize called with path: {path}");
            return new PlatformLibraryInfo
            {
                Name = "test2",
            };
        }

        public static async void Start()
        {
            var temp1 = await RxPlatformObjectRuntime.CreateInstance<OtherObjectType>(new OtherObjectType
            {
                MojProp = "Zdravo Darko"
            }, "other1");
            var temp2 = await RxPlatformObjectRuntime.CreateInstance<OtherObjectType>(new OtherObjectType
            {
                MojProp = "Zdravo Darko2"
            }, "other2");
            otherObjects = new OtherObjectType?[]
            {
                temp1, temp2
            };

            var slaveStack = ModbusUtility.CreateModbusTcpSlaves(502, new byte[] { 1 });
            await ModbusUtility.DownloadStack(slaveStack, "ImeSlejva", Assembly.GetExecutingAssembly());

            var masterStack = ModbusUtility.CreateModbusTcpMasters("127.0.0.1", 502, new byte[] { 1 });
            foreach(var master in masterStack.Slaves)
            {
                master.Options.ScanRate = 1000;
            }
            await ModbusUtility.DownloadStack(masterStack, "ImeMastera", Assembly.GetExecutingAssembly());


            TestClassSlave? testClass3 = await RxPlatformObjectRuntime.CreateInstance<TestClassSlave>(new TestClassSlave
            {
                IntProperty = 888,
                Modbus = slaveStack.Slaves[0]
            }, "Slave");

            TestClassMaster? testClass2 = await RxPlatformObjectRuntime.CreateInstance<TestClassMaster>(new TestClassMaster
            {
                IntProperty = 888,
                Modbus = masterStack.Slaves[0]
            }, "Master");

            var testClass1 = await RxPlatformObjectRuntime.CreateInstance<TestClass1>(new TestClass1
            {
                IntProperty = 888
            }, "ime1");

            testClass1.IntProperty = 999;

            if (testClass3 != null)
            {
                await Task.Delay(10000);

                await testClass3.DisposeAsync();

                testClass3 = await RxPlatformObjectRuntime.CreateInstance<TestClassSlave>(new TestClassSlave
                {
                    IntProperty = 999,
                    Modbus = slaveStack.Slaves[0]
                }, "Slave");
            }

        }
        public static void Deinitialize()
        {
        }
    }
}
