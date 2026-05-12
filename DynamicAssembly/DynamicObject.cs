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
        public void Started()
        {
            Console.WriteLine("***************SomeOtherDynamicObject: Started method called.");
        }
        public virtual string? OtherProp1 { get; set; } = "other prop value";
        public virtual uint? OtherProp2 { get; set; } = 33;
    }
}
namespace DynamicAssembly
{
    public delegate void ChangedObjProp4(byte? newValue);
    [RxPlatformRuntime()]
    [RxPlatformObjectType(nodeId: "60C7D12C-69AA-4F40-8BA0-AAD471C6BD7D", directory: "data")]
    public class DynamicObject : RxPlatformObjectRuntime
    {
        public DynamicObject()
        {
        }


        public DynamicObject(DynamicDataType initData)
        {
            SubData = initData;
        }

        public virtual DynamicEvent MyEvent { get; } = new DynamicEvent
        {
        };
        public virtual Guid? ObjectPropGuid { get; protected set; } = Guid.NewGuid();

        public uint ObjectProp1111 { get; init; } = 1000;
        public virtual string? ObjectProp2 { get; set; } = "zikica";
        public virtual string[]? ObjectProp2Array { get; set; } = new string[] { "zikica" };

        public virtual event Action<string?>? OnObjectProp2Change;
        public virtual bool? ObjectProp3 { get; set; } = true;
        public virtual double? ObjectProp7 { get; set; } = 1.0;

        public virtual Task<bool> WriteObjectProp4(byte newValue)
        {
            return Task.FromResult(false);
        }

        public virtual byte? ObjectProp4 { get; set;  } = 55;

        public virtual event Action<byte?>? OnObjectProp4Change;

        public virtual DynamicStruct Struktura1 { get; } = new DynamicStruct
        {
            Borisa = 123,
            PeriodString = "neki struct string"
        };

        public virtual Task<bool> WriteSubData(DynamicDataType newValue)
        {
            return Task.FromResult(false);
        }
        public virtual DynamicDataType? SubData { get; protected set; } = new DynamicDataType
        {
            TimeProp = DateTime.Now,
            SubData = new DynamicSubDataType
            {
                SubItem = 5000,
                SubStringString = "subzikica"
            },
            SubDataArray = []
            
        };

        public void Started()
        {
            Struktura1.PeriodString = "zikicaDodatak";
            Console.WriteLine("DynamicObject: Started method called.");

            OnOtherDynamicObjChange += DynamicObject_OnOtherDynamicObjChange;

            Struktura1.OnBorisaChange += (newValue) =>
            {
                Console.WriteLine($"DynamicObject: Struktura1.OnBorisaChange event fired. New Value: {newValue}");
            };

            // MyEvent.OnObjectProp2Change += MyEvent_OnObjectProp2Change;

            OnObjectProp2Change += (newValue) =>
            {
                Task task = new Task(async () =>
                {
                    var options = new JsonSerializerOptions
                    {
                        WriteIndented = true
                    };

                    for (int i = 0; i < 10; i++)
                    {
                        Console.WriteLine($"DynamicPlugin: Setting ObjectProp2 to 'Value {i}'");
                        //ObjectProp2 = $"Value {i}";
                        var temp = new DynamicDataType
                        {
                            TimeProp = DateTime.Now,
                            SubData = new DynamicSubDataType
                            {
                                SubItem = ObjectProp4 != null ? (uint)(i + ObjectProp4) : (uint)i,
                                SubStringString = $"Neki string{i} *** {newValue}"
                            },
                            SubDataArray = new DerivedDynamicSubDataType[]
                            {
                                new DerivedDynamicSubDataType
                                {
                                    Novi = (uint)(i + 100),
                                    SubStringString = $"Array string {i}"
                                },
                                new DerivedDynamicSubDataType
                                {
                                    Novi = (uint)(i + 150),
                                    SubItem = (uint)(i + 200),
                                    SubStringString = $"Array string {i + 1}"
                                }
                            },
                        };
                        SubData = temp;
                        //SubData = temp;
                        //if (!await WriteSubData(temp))
                        //    Console.WriteLine("DynamicObject: WriteSubData failed.");
                        //else
                        //    Console.WriteLine(JsonSerializer.Serialize(this, options));

                        if(!await WriteObjectProp4((byte)(i + 65)))
                            Console.WriteLine("DynamicObject: WriteObjectProp4 failed.");
                        await Task.Delay(2000);
                    }
                    GC.Collect();
                });
                task.Start();
            };

        }

        private void DynamicObject_OnOtherDynamicObjChange(SubNamespace.SomeOtherDynamicObject? obj)
        {
            Console.WriteLine($"DynamicObject: OnOtherDynamicObjChange event fired. New Value: {obj}");
        }

        private void MyEvent_OnObjectProp2Change(string? obj)
        {
            Console.WriteLine($"DynamicObject: MyEvent_OnObjectProp2Change event fired. New Value: {obj}");
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

        public async Task FunTaskVoid()
        {
            Console.WriteLine("DynamicObject: FunTaskVoid method called.");

            ObjectProp3 = !ObjectProp3;
            await WriteObjectProp4((byte)(ObjectProp4 != null ? ObjectProp4 + 1 : 1));

            await Task.Delay(1000);

            var obj = OtherDynamicObj;
            if (obj != null)
            {
                Console.WriteLine($"DynamicObject: OtherDynamicObj.OtherProp1 = {obj.OtherProp1}");
            }
            else
            {
                Console.WriteLine("DynamicObject: OtherDynamicObj is null.");
            }
            //if (obj == DynamicPlugin.other1)
            //    OtherDynamicObj = DynamicPlugin.other2;
            //else
            //    OtherDynamicObj = DynamicPlugin.other1;
        }

        public async Task FunTaskArg(DynamicSubDataType args)
        {
            Console.WriteLine("DynamicObject: FunTaskArg method called.");

            Console.WriteLine($"DynamicObject: Received args - SubItem: {args.SubItem}, SubStringString: {args.SubStringString}");

            ObjectProp3 = !ObjectProp3;
            await WriteObjectProp4((byte)(args.SubItem != null ? args.SubItem : 1));



            await Task.Delay(1000);

            Console.WriteLine("DynamicObject: FunTaskArg method exiting.");

        }


        public DynamicSubDataType FunRetArg(DynamicSubDataType args)
        {
            Console.WriteLine("DynamicObject: FunRetArg method called.");

            Console.WriteLine($"DynamicObject: Received args - SubItem: {args.SubItem}, SubStringString: {args.SubStringString}");

            ObjectProp3 = !ObjectProp3;

            return new DynamicSubDataType
            {
                SubItem = args.SubItem != null ? args.SubItem + 100 : 100,
                SubStringString = args.SubStringString != null ? args.SubStringString + " - modified" : "default string"
            };
            //await Task.Delay(1000);
            //Console.WriteLine("DynamicObject: FunTaskArg method exiting.");

        }


        public void Stopping()
        {
            Console.WriteLine("DynamicObject: Stopping method called.");
        }

        public virtual event Action<SubNamespace.SomeOtherDynamicObject?> OnOtherDynamicObjChange;

        public virtual SubNamespace.SomeOtherDynamicObject? OtherDynamicObj { get; set; } = null;


        public virtual SubNamespace.SomeOtherDynamicObject? OtherDynamicObj2 { get; set; } = null;

    }

    [RxPlatformRuntime()]
    [RxPlatformObjectType(nodeId: "99D96D02-006E-4149-BAC0-051F3E9F01F9")]
    public class DynamicObjectDerived : DynamicObject
    {
        public DynamicObjectDerived() { }



        public DynamicObjectDerived(DynamicDataType initData)
        {
            SubData = initData;
        }

        public async Task<DynamicDataType> FunTaskRetArg(DynamicSubDataType args)
        {
            Console.WriteLine("DynamicObject: FunRetArg method called.");

            Console.WriteLine($"DynamicObject: Received args - SubItem: {args.SubItem}, SubStringString: {args.SubStringString}");


            ObjectProp3 = !ObjectProp3;

            await Task.Delay(1000);

            return new DynamicDataType
            {
                SubData = new DynamicSubDataType
                {
                    SubItem = args.SubItem != null ? args.SubItem + 100 : 100,
                    SubStringString = args.SubStringString != null ? args.SubStringString + " - modified" : "default string"
                }
            };

        }

    }
}
