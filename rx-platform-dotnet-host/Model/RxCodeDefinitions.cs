namespace ENSACO.RxPlatform.Hosting.Model.Code
{



    class RxStructCodeData
    {
        public string name { get; set; } = "";
        public bool isNullAble { get; set; } = false;
        public string? codeType { get; set; } = null;
        public string itemId { get; set; } = "";
        public int array { get; set; } = -1;
    }
    class RxPropertyCodeData
    {
        public string name { get; set; } = "";
        public bool isNullAble { get; set; } = false;
        public bool canWrite { get; set; } = false;
        public string? setModifier { get; set; } = null;
        public string? codeType { get; set; } = null;
        public string? defaultValue { get; set; } = null;

        public string? eventName { get; set; } = null;
        public bool writeMethod { get; set; } = false;
        public bool jsonValue { get; set; } = false;
        public string itemId  { get; set; } = "";
    }
    class RxOwnMethodCodeData
    {
        public string name { get; set; } = "";
        public bool isAsync { get; set; } = false;
        public string? argumentType { get; set; } = null;
        public bool isNullAbleArgument { get; set; } = false;
        public string? defaultValue { get; set; } = null;
        public bool jsonArgument { get; set; } = false;
        public string? resultType { get; set; } = null;
        public bool isNullAbleResult { get; set; } = false;
    };



    class RxCallableMethodCodeData
    {
        public string name { get; set; } = "";
        public bool isAsync { get; set; } = false;
        public string? argumentType { get; set; } = null;
        public bool isNullAbleArgument { get; set; } = false;
        public string? defaultValue { get; set; } = null;
        public bool jsonArgument { get; set; } = false;
        public string? resultType { get; set; } = null;
        public string itemId { get; set; } = "";
        public bool isNullAbleResult { get; set; } = false;
    };

    class RxOwnRelationCodeData
    {
        public string name { get; set; } = "";
        public bool isNullAble { get; set; } = false;
        public bool canWrite { get; set; } = false;
        public string? setModifier { get; set; } = null;
        public string? codeType { get; set; } = null;
        public string? defaultValue { get; set; } = null;
        public string? eventName { get; set; } = null;
        public string itemId { get; set; } = "";
    };

}