using ENSACO.RxPlatform.Attributes;
using ENSACO.RxPlatform.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DynamicAssembly.SubNamespace
{
    [RxPlatformRuntime()]
    [RxPlatformObjectType(nodeId: "DFE14F0C-D551-472C-B15A-EA79DFC419FE")]
    public class SomeOtherDynamicObject : RxPlatformObjectRuntime
    {
        public virtual string? OtherProp1 { get; set; } = "other prop value";
        public virtual uint? OtherProp2 { get; set; } = 33;
    }
}
namespace DynamicAssembly
{
    public delegate void ChangedObjProp4(byte? newValue);
    [RxPlatformRuntime()]
    [RxPlatformObjectType(nodeId: "60C7D12C-69AA-4F40-8BA0-AAD471C6BD7D")]
    public class DynamicObject : RxPlatformObjectRuntime
    {
        public DynamicObject()
        {
        }
        public uint ObjectProp1111 { get; init; } = 1000;
        public virtual string? ObjectProp2 { get; set; } = "zikica";

        public virtual event Action<string?>? OnObjectProp2Change;
        public virtual bool? ObjectProp3 { get; protected set; } = true;

        public virtual Task<bool> WriteObjectProp4(byte newValue)
        {
            return Task.FromResult(false);
        }

        public virtual byte? ObjectProp4 { get; set;  } = 55;

        public virtual event ChangedObjProp4? OnObjectProp4Change;

        public virtual DynamicStruct Struktura1 { get; } = new DynamicStruct
        {
            Borisa = 123,
            PeriodString = "neki struct string"
        };

        public virtual Task<bool> WriteSubData(DynamicSubDataType newValue)
        {
            return Task.FromResult(false);
        }
        public virtual DynamicSubDataType? SubData { get; set; } = new DynamicSubDataType
        {
            SubItem = 5000,
            SubStringString = "subzikica"
        };

        public void Started()
        {
            Console.WriteLine("DynamicObject: Started method called.");

            OnObjectProp2Change += (newValue) =>
            {
                Task task = new Task(async () =>
                {
                    {
                        var options = new JsonSerializerOptions
                        {
                            WriteIndented = true
                        };

                        for (int i = 0; i < 3; i++)
                        {
                            Console.WriteLine($"DynamicPlugin: Setting ObjectProp2 to 'Value {i}'");
                            //ObjectProp2 = $"Value {i}";
                            var temp = new DynamicSubDataType
                            {
                                SubItem = ObjectProp4 != null ? (uint)(i + ObjectProp4) : (uint)i,
                                SubStringString = $"Neki string{i} *** {newValue}"
                            };
                            //SubData = temp;
                            if (!await WriteSubData(temp))
                                Console.WriteLine("DynamicObject: WriteSubData failed.");
                            //else
                            //    Console.WriteLine(JsonSerializer.Serialize(this, options));

                            if(!await WriteObjectProp4((byte)(i + 65)))
                                Console.WriteLine("DynamicObject: WriteObjectProp4 failed.");
                            await Task.Delay(1000);
                        }

                    }
                    GC.Collect();
                });
                task.Start();
            };

        }

        public void FunkcijaNeka22()
        {
            Console.WriteLine("DynamicObject: FunkcijaNeka method called.");

            ObjectProp3 = !ObjectProp3;

            var obj = OtherDynamicObj;
            if(obj != null)
            {
                Console.WriteLine($"DynamicObject: OtherDynamicObj.OtherProp1 = {obj.OtherProp1}");
            }
            else
            {
                Console.WriteLine("DynamicObject: OtherDynamicObj is null.");
            }
            if (obj == DynamicPlugin.other1)
                OtherDynamicObj = DynamicPlugin.other2;
            else
                OtherDynamicObj = DynamicPlugin.other1;
        }


        public void Stopping()
        {
            Console.WriteLine("DynamicObject: Stopping method called.");
        }

        public virtual SubNamespace.SomeOtherDynamicObject? OtherDynamicObj { get; set; } = null;
    }
}
