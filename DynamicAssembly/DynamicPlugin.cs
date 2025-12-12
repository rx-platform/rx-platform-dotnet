using ENSACO.RxPlatform.Attributes;
using ENSACO.RxPlatform.Host;
using ENSACO.RxPlatform.Modbus;
using ENSACO.RxPlatform.Model.Modbus;
using ENSACO.RxPlatform.Runtime;
using System.Reflection;

namespace DynamicAssembly
{
    [RxPlatformLibrary()]
    public class DynamicPlugin
    {
        public static PlatformLibraryInfo Initialize()
        {
            // This method is just to force the compiler to include this class in the assembly
            return new PlatformLibraryInfo
            {
                Name = "test1"
            };
        }

        static DynamicObject? extended = null;
        internal static SubNamespace.SomeOtherDynamicObject? other1 = null;
        internal static SubNamespace.SomeOtherDynamicObject? other2 = null;


        public static async void Start()
        {
            var temp = new DynamicStruct();

            // /* just temporary excluded for testing
            var stack = ModbusUtility.CreateModbusTcpSlaves(502, new byte[] { 1, 2, 3 });
            stack.TcpPort.Timeouts.ReceiveTimeout = 60000;
            await ModbusUtility.DownloadStack(stack, "Test1", "ports", Assembly.GetExecutingAssembly());


            var masterStack = ModbusUtility.CreateModbusTcpMasters("127.0.0.1", 502, new byte[] { 2 });
            masterStack.TcpPort.Timeouts.ReceiveTimeout = 60000;
            await ModbusUtility.DownloadStack(masterStack, "Test1Master", "ports", Assembly.GetExecutingAssembly());

            System.Diagnostics.Debugger.Launch();


            other1 = await RxPlatformObjectRuntime.CreateInstance<SubNamespace.SomeOtherDynamicObject>(
                new SubNamespace.SomeOtherDynamicObject
                {
                    OtherProp2 = 55
                }, "OtherDynObj1");

            var other2Temp = new SubNamespace.SomeOtherDynamicObject
            {
                OtherProp1 = "Value from DynamicPlugin"
            };

            other2 = await RxPlatformObjectRuntime.CreateInstance<SubNamespace.SomeOtherDynamicObject>(
                other2Temp, "OtherDynObj2");

            extended = await RxPlatformObjectRuntime.CreateInstance<DynamicObject>(
                new ExtendedDynamicObject
                {
                    SubData = new DynamicSubDataType
                    {
                        SubItem = 9999,
                        SubStringString = "subdata value",
                    },
                    OtherDynamicObj = other2,
                    ModbusSlave = stack.Slaves[1],
                    ModbusMaster = masterStack.Slaves[0]
                }, "TestObj2");

            

            

            // */

            Task task = new Task(async () =>
            {
                {
                    // /* just temporary excluded for testing
                    for(int i = 2; i<3; i++)
                    {
                        await RxPlatformObjectRuntime.CreateObject(new DynamicObject
                        {
                            ObjectProp1111 = 5670 + (uint)i

                        }, i==2 ? "TestObj" : $"TestObj{i}");
                    }
                    DynamicObject ? src = await RxPlatformObjectRuntime.CreateInstance(new DynamicObject
                    {
                        ObjectProp1111 = 5678

                    },"DynObj");

                    if (src == null)
                    {
                        Console.WriteLine("DynamicPlugin: Failed to create DynamicObject instance.");
                        return;
                    }
                    else
                    {
                        src.ObjectProp2 = "Initial Value from DynamicPlugin";
                        src.OnObjectProp2Change += (newValue) =>
                        {
                            Console.WriteLine($"DynamicPlugin: OnObjectProp2Change event fired. New Value: {newValue}");
                        };
                        Console.WriteLine("DynamicPlugin: Created DynamicObject instance.");
                    }
                    // */
                    await Task.Delay(10000);
                    
                    return;
                                       
                }
            });
            task.Start();
        }

        public static void Deinitialize()
        {
            // This method is just to force the compiler to include this class in the assembly
        }
    }
}
