using ENSACO.RxPlatform.Model;
using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ENSACO.RxPlatform.Json
{
    public class RxNodeIdJsonConverter : JsonConverter<RxNodeId>
    {
        public override RxNodeId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string? s = reader.GetString();
            if(s == null)
                return RxNodeId.NullId;
            else
                return RxNodeId.FromString(s);
        }
        public override void Write(Utf8JsonWriter writer, RxNodeId value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}
