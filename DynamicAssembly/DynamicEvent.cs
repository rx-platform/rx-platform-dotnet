using ENSACO.RxPlatform.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicAssembly
{

    [RxPlatformRuntime()]
    [RxPlatformEventType<DynamicDataType>(nodeId: "1:g:3047908E-9012-4C99-BB7D-7DBE9EBAAC44", name: "DynamicEvent")]
    public class DynamicEvent : ENSACO.RxPlatform.Runtime.RxPlatformEventRuntime
    {
        public virtual event Action<DynamicDataType>? Fire;

        public uint ObjectProp1111 { get; init; } = 1000;
        public virtual string? ObjectProp2 { get; set; } = "zikica";

        public virtual event Action<string?>? OnObjectProp2Change;
        public virtual bool? ObjectProp3 { get; protected set; } = true;


        public void Started()
        {
            Console.WriteLine($"DynamicEvent: Started method called.ObjectProp2 = {ObjectProp2}");
            OnObjectProp2Change += DynamicEvent_OnObjectProp2Change;
        }

        private void DynamicEvent_OnObjectProp2Change(string? obj)
        {
            Console.WriteLine($"DynamicEvent: Changed method called.OnObjectProp2Change = {ObjectProp2}");

            if (Fire != null)
            {
                Fire(new DynamicDataType
                {
                    TimeProp = DateTime.Now,
                    SubData = new DynamicSubDataType
                    {
                        SubItem = 33,
                        SubStringString = $"Neki string *** {ObjectProp2}"
                    }
                });
            }
        }

        public void Stopping()
        {
            Console.WriteLine("DynamicEvent: Stopping method called.");
        }
    }
}
