using ENSACO.RxPlatform;
using ENSACO.RxPlatform.Attributes;
using ENSACO.RxPlatform.Model.System;
using ENSACO.RxPlatform.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DynamicAssembly.SubNamespace
{
    [RxPlatformDeclare()]
    [RxPlatformDataType(nodeId: "CDCD58DA-B41E-43D4-9638-8C9BC6A179CB")]
    public struct OutArgs
    {
        public int OutArg1 { get; set; }
        public int OutArg2 { get; set; }
    }
    [RxPlatformDeclare()]
    [RxPlatformDataType(nodeId: "878841F3-C143-42BE-9D62-F0A9F53AC1AB")]
    public class InArgs
    {
        public uint transId { get; set; } = 0;
        public double quantity { get; set; } = 0;
        public int recepie { get; set; } = 0;
        public double baseDensity { get; set; } = 0;
        public uint chamber { get; set; } = 0;
        public string regPlate { get; set; } = "";
        public string fuelDesc { get; set; } = "";
        public int tehRecepie { get; set; } = 0;
        public double ratio1 { get; set; } = 100;
        public double ratio2 { get; set; } = 0;
        public double add1ratio { get; set; } = 0;
        public double add2ratio { get; set; } = 0;
        public double add3ratio { get; set; } = 0;
        public double add4ratio { get; set; } = 0;
        //public int InArg1 { get; set; }
        //public int InArg2 { get; set; }
        //public string InArg3 { get; set; }
    }

    [RxPlatformRuntime()]
    [RxPlatformObjectType(nodeId: "DFE14F0C-D551-472C-B15A-EA79DFC419FE")]
    public class SomeOtherDynamicObject : RxPlatformObjectRuntime
    {
        public void Started()
        {
            
        }

        [RxPlatformAbstractMethod]
        public virtual Task SomeAbstract()
        {
            throw new NotImplementedException("SomeAbstract method not implemented.");
        }

        [RxPlatformAbstractMethod]
        public virtual Task<OutArgs> SomeWithRes()
        {
            throw new NotImplementedException("SomeWithRes method not implemented.");
        }

        [RxPlatformAbstractMethod]
        public virtual Task<OutArgs> SomeWithArgRes(InArgs args)
        {
            throw new NotImplementedException("SomeWithArgRes method not implemented.");
        }

        public Task BorisaPretrcavanje()
        {
            RxPlatformLog.WriteLogTrace("test", "SomeOtherDynamicObject: BorisaPretrcavanje method called.");
            return Task.CompletedTask;
        }
        public virtual string? OtherProp1 { get; set; } = "other prop value";
        public virtual uint? OtherProp2 { get; set; } = 33;
    }

    [RxPlatformRuntime()]
    [RxPlatformObjectType(nodeId: "3349C3BA-0373-497F-A39F-8F30061975A1")]
    public class ExtendedSomeOtherDynamicObject : SomeOtherDynamicObject
    {

        public new void Started()
        {
            RxPlatformLog.WriteLogTrace("test", "***************SomeOtherDynamicObject: Started method called.");
            OnSomePropChange += SomeOtherDynamicObject_OnSomePropChange;
        }

        private async void SomeOtherDynamicObject_OnSomePropChange(int? obj)
        {
            if (obj == 1)
            {
                RxPlatformLog.WriteLogTrace("test", $"Waiting for condition...");
                bool result = await WaitCondition(() =>
                  {
                      if ((OtherProp2 != null && OtherProp2 > 64))
                          return true;
                      else
                          return false;
                  }, 10000);

                if (!result)
                {
                    RxPlatformLog.WriteLogTrace("test", $"Condition not met exiting...");
                    return;
                }
                RxPlatformLog.WriteLogTrace("test", $"Calling TestVoidVoid WF");
                try
                {
                    RxPlatformLog.WriteLogTrace("test", $"Calling TestVoidVoid WF");
                    await SomeAbstract();
                    RxPlatformLog.WriteLogTrace("test", "Call done");
                    RxPlatformLog.WriteLogTrace("test", "calling TestArgVoid WF");
                    var res = await SomeWithRes();
                    RxPlatformLog.WriteLogTrace("test", $"Call done results are: {res.OutArg1},{res.OutArg2}");
                    InArgs args = new InArgs
                    {
                        transId = 10,
                        quantity = 20,
                        recepie = 30
                    };
                    RxPlatformLog.WriteLogTrace("test", $"calling TestVoidArg WF with args");
                    RxPlatformLog.WriteLogTrace("test", "Call done");
                    args = new InArgs
                    {
                        transId = 3,
                        quantity = 4,
                        recepie = 5
                    };
                    RxPlatformLog.WriteLogTrace("test", $"calling TestArgArg WF with args");
                    res = await SomeWithArgRes(args);
                    RxPlatformLog.WriteLogTrace("test", $"Call done all done results are: {res.OutArg1},{res.OutArg2}.");
                }
                catch (Exception ex)
                {
                    RxPlatformLog.WriteLogTrace("test", $"SomeOtherDynamicObject: Exception in Execution: {ex.Message}");
                }
            }
            SomeProp = 0;
        }
        public virtual int? SomeProp { get; set; } = 0;

        public virtual event Action<int?>? OnSomePropChange;
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

        public DateTime TimeProp { get; init; } = DateTime.Now;

        public virtual double? ObjectPropDare { get; set; } = 77.0;

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
            Struktura1.OnStringArrayChange += Struktura1_OnStringArrayChange;
            Struktura1.PeriodString = "zikicaDodatak";
            RxPlatformLog.WriteLogTrace("test", "DynamicObject: Started method called.");

            OnOtherDynamicObjChange += DynamicObject_OnOtherDynamicObjChange;
            OnOtherDynamicObjConnected += DynamicObject_OnOtherDynamicObjConnected;
            OnOtherDynamicObjDisconnected += DynamicObject_OnOtherDynamicObjDisconnected;
            OnSomeOtherDynamicObjectConnected += DynamicObject_OnSomeOtherDynamicObjectConnected;
            OnSomeOtherDynamicObjectDisconnected += DynamicObject_OnSomeOtherDynamicObjectDisconnected;

            Struktura1.OnBorisaChange += (newValue) =>
            {
                RxPlatformLog.WriteLogTrace("test", $"DynamicObject: Struktura1.OnBorisaChange event fired. New Value: {newValue}");
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
                        RxPlatformLog.WriteLogTrace("test", $"DynamicPlugin: Setting ObjectProp2 to 'Value {i}'");
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
                        //    RxPlatformLog.WriteLogTrace("test", "DynamicObject: WriteSubData failed.");
                        //else
                        //    RxPlatformLog.WriteLogTrace("test", JsonSerializer.Serialize(this, options));

                        if(!await WriteObjectProp4((byte)(i + 65)))
                            RxPlatformLog.WriteLogTrace("test", "DynamicObject: WriteObjectProp4 failed.");
                        await Task.Delay(2000);
                    }
                    GC.Collect();
                });
                task.Start();
            };

        }

        private void DynamicObject_OnSomeOtherDynamicObjectDisconnected(string arg, SubNamespace.SomeOtherDynamicObject? obj)
        {
            RxPlatformLog.WriteLogTrace("test", $"DynamicObject: DynamicObject_OnSomeOtherDynamicObjectDisconnected event fired. {arg}: {obj}");

        }

        private void DynamicObject_OnSomeOtherDynamicObjectConnected(string arg, SubNamespace.SomeOtherDynamicObject? obj)
        {
            RxPlatformLog.WriteLogTrace("test", $"DynamicObject: DynamicObject_OnSomeOtherDynamicObjectConnected event fired. {arg}: {obj}");

        }

        private void DynamicObject_OnOtherDynamicObjDisconnected(SubNamespace.SomeOtherDynamicObject? obj)
        {
            RxPlatformLog.WriteLogTrace("test", $"DynamicObject: DynamicObject_OnOtherDynamicObjDisconnected event fired. New Value: {obj}");

        }

        private void DynamicObject_OnOtherDynamicObjConnected(SubNamespace.SomeOtherDynamicObject? obj)
        {
            RxPlatformLog.WriteLogTrace("test", $"DynamicObject: DynamicObject_OnOtherDynamicObjConnected event fired. New Value: {obj}");

        }

        private void Struktura1_OnStringArrayChange(string[]? obj)
        {

        }

        private void DynamicObject_OnOtherDynamicObjChange(SubNamespace.SomeOtherDynamicObject? obj)
        {
            RxPlatformLog.WriteLogTrace("test", $"DynamicObject: OnOtherDynamicObjChange event fired. New Value: {obj}");
        }

        private void MyEvent_OnObjectProp2Change(string? obj)
        {
            RxPlatformLog.WriteLogTrace("test", $"DynamicObject: MyEvent_OnObjectProp2Change event fired. New Value: {obj}");
        }
        public void FunkcijaNeka22()
        {
            RxPlatformLog.WriteLogTrace("test", "DynamicObject: FunkcijaNeka method called.");

            var now = DateTime.Now;

            DerivedDynamicSubDataType[] arrasub = [];
            if(SubData?.SubDataArray?.Length  == 0)
            {
                arrasub = new DerivedDynamicSubDataType[]
                {
                    new DerivedDynamicSubDataType
                    {
                        SubItem = 999,
                        SubStringString = "novi sub data string + " + now.ToString()
                    },
                    new DerivedDynamicSubDataType
                    {
                        SubItem = 888,
                        SubStringString = "još jedan novi sub data string + " + now.ToString()
                    }
                };
            }
            DynamicDataType data = new DynamicDataType
            {
                TimeProp = DateTime.Now,
                SubData = new DynamicSubDataType
                {
                    SubItem = 1234,
                    SubStringString = "novi sub data string"
                },
                SubDataArray = arrasub
            };
            SubData = data;


            string[] newVal = [
                ];

            Struktura1.StringArray = newVal;

            ObjectProp3 = !ObjectProp3;

            var obj = OtherDynamicObj;
            if(obj != null)
            {
                RxPlatformLog.WriteLogTrace("test", $"DynamicObject: OtherDynamicObj.OtherProp1 = {obj.OtherProp1}");
            }
            else
            {
                RxPlatformLog.WriteLogTrace("test", "DynamicObject: OtherDynamicObj is null.");
            }
            if (obj == DynamicPlugin.other1)
                OtherDynamicObj = DynamicPlugin.other2;
            else
                OtherDynamicObj = DynamicPlugin.other1;
        }

        public async Task FunTaskVoid()
        {
            RxPlatformLog.WriteLogTrace("test", "DynamicObject: FunTaskVoid method called.");

            ObjectProp3 = !ObjectProp3;
            await WriteObjectProp4((byte)(ObjectProp4 != null ? ObjectProp4 + 1 : 1));

            await Task.Delay(1000);

            var obj = OtherDynamicObj;
            if (obj != null)
            {
                RxPlatformLog.WriteLogTrace("test", $"DynamicObject: OtherDynamicObj.OtherProp1 = {obj.OtherProp1}");
            }
            else
            {
                RxPlatformLog.WriteLogTrace("test", "DynamicObject: OtherDynamicObj is null.");
            }
            //if (obj == DynamicPlugin.other1)
            //    OtherDynamicObj = DynamicPlugin.other2;
            //else
            //    OtherDynamicObj = DynamicPlugin.other1;
        }

        public async Task FunTaskArg(DynamicSubDataType args)
        {
            RxPlatformLog.WriteLogTrace("test", "DynamicObject: FunTaskArg method called.");

            RxPlatformLog.WriteLogTrace("test", $"DynamicObject: Received args - SubItem: {args.SubItem}, SubStringString: {args.SubStringString}");

            ObjectProp3 = !ObjectProp3;
            await WriteObjectProp4((byte)(args.SubItem != null ? args.SubItem : 1));



            await Task.Delay(1000);

            RxPlatformLog.WriteLogTrace("test", "DynamicObject: FunTaskArg method exiting.");

        }


        public DynamicSubDataType FunRetArg(DynamicSubDataType args)
        {
            RxPlatformLog.WriteLogTrace("test", "DynamicObject: FunRetArg method called.");

            RxPlatformLog.WriteLogTrace("test", $"DynamicObject: Received args - SubItem: {args.SubItem}, SubStringString: {args.SubStringString}");

            ObjectProp3 = !ObjectProp3;

            return new DynamicSubDataType
            {
                SubItem = args.SubItem != null ? args.SubItem + 100 : 100,
                SubStringString = args.SubStringString != null ? args.SubStringString + " - modified" : "default string"
            };
            //await Task.Delay(1000);
            //RxPlatformLog.WriteLogTrace("test", "DynamicObject: FunTaskArg method exiting.");

        }


        public void Stopping()
        {
            RxPlatformLog.WriteLogTrace("test", "DynamicObject: Stopping method called.");
        }

        public virtual event Action<SubNamespace.SomeOtherDynamicObject?>? OnOtherDynamicObjChange;

        public virtual event Action<SubNamespace.SomeOtherDynamicObject?>? OnOtherDynamicObjConnected;
        public virtual event Action<SubNamespace.SomeOtherDynamicObject?>? OnOtherDynamicObjDisconnected;
        public virtual event Action<string, SubNamespace.SomeOtherDynamicObject?>? OnSomeOtherDynamicObjectConnected;
        public virtual event Action<string, SubNamespace.SomeOtherDynamicObject?>? OnSomeOtherDynamicObjectDisconnected;

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
            RxPlatformLog.WriteLogTrace("test", "DynamicObject: FunRetArg method called.");

            RxPlatformLog.WriteLogTrace("test", $"DynamicObject: Received args - SubItem: {args.SubItem}, SubStringString: {args.SubStringString}");


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
