using ENSACO.RxPlatform.Host;
using ENSACO.RxPlatform.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicAssembly
{
    [RxPlatformDataType(nodeId: "A1B2C3D4-E5F6-4789-ABCD-1234567890AB")]
    [RxPlatformDeclare()]
    public class MyStruct
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public NestedStruct Nested { get; set; } = new NestedStruct 
        {
            Measurement = 77.7
        };
    }
    [RxPlatformDataType(nodeId: "B1C2D3E4-F5A6-4789-ABCD-0987654321BA")]
    [RxPlatformDeclare()]
    public struct NestedStruct
    {
        public bool IsActive { get; set; }
        public double Measurement { get; set; }
    }

    [RxPlatformDeclare()]
    [RxPlatformDataType(nodeId: "E3FCD26B-E741-44C3-B30F-ADB568DD322F")]
    public class DynamicSubDataType
    {
        public DynamicSubDataType()
        {
        }

        public uint? SubItem { get; set; } = 2000;
        public string? SubStringString { get; set; } = "zikica";

    }



    [RxPlatformDeclare()]
    [RxPlatformDataType(nodeId: "974B7853-7C8F-4D2C-A135-222C2717E325")]
    public class DynamicBaseDataType
    {
        public uint? BaseItem { get; set; } = 2000;
        public string? BaseStringString { get; set; } = "zikicabase";

    }


    [RxPlatformDeclare()]
    [RxPlatformDataType(nodeId: "ECD374A6-CE5A-4DD7-BD9D-DE7A3D6A81B1")]
    public class DynamicDataType : DynamicBaseDataType
    {
        public uint? PeriodMsCvejo { get; set; } = 1000;
        public string? PeriodString { get; set; } = "uros je ovde";
        public bool? ZeljkoProp44 { get; set; } = true;

        public DynamicSubDataType SubData { get; set; } = new DynamicSubDataType
        {
            SubItem = 5000,
            SubStringString = "subzikica"
        };

        public DynamicSubDataType[] SubDataArray { get; set; } = new DynamicSubDataType[]
        {
            new DynamicSubDataType
            {
                SubItem = 6000,
                SubStringString = "arrayzikica1"
            },
            new DynamicSubDataType
            {
                SubItem = 7000,
                SubStringString = "arrayzikica2"
            }
        };
        public List<DynamicSubDataType> SubDataList { get; set; } = new List<DynamicSubDataType>
        {
            new DynamicSubDataType
            {
                SubItem = 8000,
                SubStringString = "listzikica1"
            },
            new DynamicSubDataType
            {
                SubItem = 9000,
                SubStringString = "listzikica2"
            }
        };

    }
}
