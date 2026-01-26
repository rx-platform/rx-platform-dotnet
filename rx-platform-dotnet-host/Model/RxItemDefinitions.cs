using ENSACO.RxPlatform.Hosting.Common;
using System.Text.Json.Serialization;

namespace ENSACO.RxPlatform.Hosting.Model.Items
{

    class RxHostValueType
    {
        public rx_value_t type { get; set; } = rx_value_t.Null;

        public object? val { get; set; } = null;
    }
    class RXHostReferenceId
    {
        public string id { get; set; } = "";
    };
    class RXHostReferencePath
    {
        public string id { get; set; } = "";
    };
    class RoleData
    {
        public string role { get; set; } = "";
    };
    class RxAccessInfo
    {
        public RoleData[] roles { get; set; } = Array.Empty<RoleData>();
    };

    class RxMetaItem
    {
        public string name { get; set; } = "";
        public virtual string type { get; set; } = "meta";
        public string description { get; set; } = "";
    };

    class RxHostConstItem : RxMetaItem
    {
        public override string type { get; set; } = "const_value";
        public string? datatype { get; } = null;
        public int array { get; set; } = -1;
        public bool config { get; set; } = false;
        public bool ro { get; set; } = false;
        public bool persist { get; set; } = false;
        public RxHostValueType value { get; set; } = new RxHostValueType();
        public RxAccessInfo access { get; set; } = new RxAccessInfo();
    };

    class RxHostPropItem : RxMetaItem
    {
        public override string type { get; set; } = "value";
        public string? datatype { get; } = null;
        public int array { get; set; } = -1;
        public bool ro { get; set; } = false;
        public bool persist { get; set; } = false;
        public RxHostValueType value { get; set; } = new RxHostValueType();
        public RxAccessInfo access { get; set; } = new RxAccessInfo();
    };
    class RxHostConstBlockItem : RxMetaItem
    {
        public override string type { get; set; } = "const_value";
        public RXHostReferenceId datatype { get; set; } = new RXHostReferenceId();
        public int array { get; set; } = -1;
        public bool config { get; set; } = false;
        public bool ro { get; set; } = false;
        public bool persist { get; set; } = false;
        public RxHostValueType value { get; } = new RxHostValueType();
    };
    class RxHostPropBlockItem : RxMetaItem
    {
        public override string type { get; set; } = "value";
        public RXHostReferenceId datatype { get; set; } = new RXHostReferenceId();
        public int array { get; set; } = -1;
        public bool ro { get; set; } = false;
        public bool persist { get; set; } = false;
        public RxHostValueType value { get; set; } = new RxHostValueType();
        public RxAccessInfo access { get; set; } = new RxAccessInfo();
    };


    class RxHostStructItem : RxMetaItem
    {
        public override string type { get; set; } = "struct";
        public RXHostReferenceId target { get; set; } = new RXHostReferenceId();
        public int array { get; set; } = -1;
        public RxAccessInfo access { get; set; } = new RxAccessInfo();
    };

    class RxHostVariableItem : RxMetaItem
    {
        public override string type { get; set; } = "variable";
        public RXHostReferenceId target { get; set; } = new RXHostReferenceId();
        public int array { get; set; } = -1;
        public string? datatype { get; } = null;
        public bool ro { get; set; } = false;
        public bool persist { get; set; } = false;
        public RxHostValueType value { get; set; } = new RxHostValueType();
        public RxAccessInfo access { get; set; } = new RxAccessInfo();
    };


    class RxHostBlockVariableItem : RxMetaItem
    {
        public override string type { get; set; } = "variable";
        public RXHostReferenceId target { get; set; } = new RXHostReferenceId();
        public int array { get; set; } = -1;
        public RXHostReferenceId datatype { get; set; } = new RXHostReferenceId();
        public bool ro { get; set; } = false;
        public bool persist { get; set; } = false;
        public RxHostValueType value { get; set; } = new RxHostValueType();
        public RxAccessInfo access { get; set; } = new RxAccessInfo();
    };

    class RxDataItem
    {
        public string name { get; set; } = "";
        public virtual bool simple { get; set; } = true;
        public string description { get; set; } = "";
    };

    class RxHostSimpleDataItem : RxDataItem
    {
        public override bool simple { get; set; } = true;
        public int array { get; set; } = -1;
        public RxHostValueType value { get; set; } = new RxHostValueType();
    };

    class RxHostBlockDataItem : RxDataItem
    {
        public override bool simple { get; set; } = false;
        public int array { get; set; } = -1;
        public RXHostReferenceId target { get; set; } = new RXHostReferenceId();
    };

    class RxRelationDataItem
    {
        public string name { get; set; } = "";
        public RXHostReferenceId relation { get; set; } = new RXHostReferenceId();
        public RXHostReferenceId target { get; set; } = new RXHostReferenceId();
        public string description { get; set; } = "";
        public RxAccessInfo access { get; set; } = new RxAccessInfo();
    };


    class RxMethodDataItem
    {
        public string name { get; set; } = "";
        public RXHostReferenceId target { get; set; } = new RXHostReferenceId();
        public string description { get; set; } = "";
        public RxAccessInfo access { get; set; } = new RxAccessInfo();

        [JsonPropertyName("in")]
        public RXHostReferenceId inType { get; set; } = new RXHostReferenceId();

        [JsonPropertyName("out")]
        public RXHostReferenceId outType { get; set; } = new RXHostReferenceId();

    };


    class RxFilterDataItem
    {
        public string name { get; set; } = "";
        public RXHostReferenceId target { get; set; } = new RXHostReferenceId();
        public string description { get; set; } = "";
        public RxAccessInfo access { get; set; } = new RxAccessInfo();

        public bool input { get; set; } = true;
        public bool output { get; set; } = true;
        public bool sim { get; set; } = true;
        public bool proc { get; set; } = true;

    };


    class RxMapperDataItem
    {
        public string name { get; set; } = "";
        public RXHostReferenceId target { get; set; } = new RXHostReferenceId();
        public string description { get; set; } = "";
        public RxAccessInfo access { get; set; } = new RxAccessInfo();

        public bool read { get; set; } = true;
        public bool write { get; set; } = true;
        public bool sim { get; set; } = true;
        public bool proc { get; set; } = true;

    };


    class RxSourceDataItem
    {
        public string name { get; set; } = "";
        public RXHostReferenceId target { get; set; } = new RXHostReferenceId();
        public string description { get; set; } = "";
        public RxAccessInfo access { get; set; } = new RxAccessInfo();

        public bool input { get; set; } = true;
        public bool output { get; set; } = true;
        public bool sim { get; set; } = true;
        public bool proc { get; set; } = true;

    };


    class RxDisplayDataItem
    {
        public string name { get; set; } = "";
        public RXHostReferenceId target { get; set; } = new RXHostReferenceId();
        public string description { get; set; } = "";
        public RxAccessInfo access { get; set; } = new RxAccessInfo();

        public bool input { get; set; } = true;
        public bool output { get; set; } = true;
        public bool sim { get; set; } = true;
        public bool proc { get; set; } = true;

    };


}