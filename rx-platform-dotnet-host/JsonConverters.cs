using ENSACO.RxPlatform.Attributes;
using ENSACO.RxPlatform.Hosting.Common;
using ENSACO.RxPlatform.Hosting.Model.Items;
using ENSACO.RxPlatform.Runtime;
using System.Collections;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace ENSACO.RxPlatform.Hosting.Reflection
{

    internal class IgnoreObjectForBoolConverter : JsonConverter<bool>
    {
        public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // Check if the current JSON token is the start of an object/struct
            if (reader.TokenType == JsonTokenType.StartObject)
            {
                // Skip the entire JSON object hierarchy
                reader.Skip();

                // Return the default value for the expected simple type (e.g., null for string, 0 for int)
                return default;
            }
            // Otherwise, deserialize the simple type normally using standard options
            return reader.GetBoolean();
        }

        public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value, options);
        }
    }

    internal class IgnoreObjectForStringConverter : JsonConverter<string>
    {
        public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // Check if the current JSON token is the start of an object/struct
            if (reader.TokenType == JsonTokenType.StartObject)
            {
                // Skip the entire JSON object hierarchy
                reader.Skip();

                // Return the default value for the expected simple type (e.g., null for string, 0 for int)
                return default;
            }
            // Otherwise, deserialize the simple type normally using standard options
            return reader.GetString() is string strValue ? (string)(object)strValue : default;
        }

        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value, options);
        }
    }

    internal class IgnoreObjectForByteConverter : JsonConverter<byte>
    {
        public override byte Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // Check if the current JSON token is the start of an object/struct
            if (reader.TokenType == JsonTokenType.StartObject)
            {
                // Skip the entire JSON object hierarchy
                reader.Skip();

                // Return the default value for the expected simple type (e.g., null for string, 0 for int)
                return default;
            }
            // Otherwise, deserialize the simple type normally using standard options
            return reader.GetByte();
        }

        public override void Write(Utf8JsonWriter writer, byte value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value, options);
        }
    }

    internal class IgnoreObjectForSByteConverter : JsonConverter<sbyte>
    {
        public override sbyte Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // Check if the current JSON token is the start of an object/struct
            if (reader.TokenType == JsonTokenType.StartObject)
            {
                // Skip the entire JSON object hierarchy
                reader.Skip();

                // Return the default value for the expected simple type (e.g., null for string, 0 for int)
                return default;
            }
            // Otherwise, deserialize the simple type normally using standard options
            return reader.GetSByte();
        }

        public override void Write(Utf8JsonWriter writer, sbyte value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value, options);
        }
    }

    internal class IgnoreObjectForShortConverter : JsonConverter<short>
    {
        public override short Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // Check if the current JSON token is the start of an object/struct
            if (reader.TokenType == JsonTokenType.StartObject)
            {
                // Skip the entire JSON object hierarchy
                reader.Skip();

                // Return the default value for the expected simple type (e.g., null for string, 0 for int)
                return default;
            }
            // Otherwise, deserialize the simple type normally using standard options
            return reader.GetInt16();
        }

        public override void Write(Utf8JsonWriter writer, short value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value, options);
        }
    }

    internal class IgnoreObjectForUShortConverter : JsonConverter<ushort>
    {
        public override ushort Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // Check if the current JSON token is the start of an object/struct
            if (reader.TokenType == JsonTokenType.StartObject)
            {
                // Skip the entire JSON object hierarchy
                reader.Skip();

                // Return the default value for the expected simple type (e.g., null for string, 0 for int)
                return default;
            }
            // Otherwise, deserialize the simple type normally using standard options
            return reader.GetUInt16();
        }

        public override void Write(Utf8JsonWriter writer, ushort value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value, options);
        }
    }

    internal class IgnoreObjectForIntConverter : JsonConverter<int>
    {
        public override int Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // Check if the current JSON token is the start of an object/struct
            if (reader.TokenType == JsonTokenType.StartObject)
            {
                // Skip the entire JSON object hierarchy
                reader.Skip();

                // Return the default value for the expected simple type (e.g., null for string, 0 for int)
                return default;
            }
            // Otherwise, deserialize the simple type normally using standard options
            return reader.GetInt32();
        }

        public override void Write(Utf8JsonWriter writer, int value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value, options);
        }
    }

    internal class IgnoreObjectForUIntConverter : JsonConverter<uint>
    {
        public override uint Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // Check if the current JSON token is the start of an object/struct
            if (reader.TokenType == JsonTokenType.StartObject)
            {
                // Skip the entire JSON object hierarchy
                reader.Skip();

                // Return the default value for the expected simple type (e.g., null for string, 0 for int)
                return default;
            }
            // Otherwise, deserialize the simple type normally using standard options
            return reader.GetUInt32();
        }

        public override void Write(Utf8JsonWriter writer, uint value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value, options);
        }
    }

    internal class IgnoreObjectForLongConverter : JsonConverter<long>
    {
        public override long Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // Check if the current JSON token is the start of an object/struct
            if (reader.TokenType == JsonTokenType.StartObject)
            {
                // Skip the entire JSON object hierarchy
                reader.Skip();

                // Return the default value for the expected simple type (e.g., null for string, 0 for int)
                return default;
            }
            // Otherwise, deserialize the simple type normally using standard options
            return reader.GetInt64();
        }

        public override void Write(Utf8JsonWriter writer, long value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value, options);
        }
    }

    internal class IgnoreObjectForULongConverter : JsonConverter<ulong>
    {
        public override ulong Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // Check if the current JSON token is the start of an object/struct
            if (reader.TokenType == JsonTokenType.StartObject)
            {
                // Skip the entire JSON object hierarchy
                reader.Skip();

                // Return the default value for the expected simple type (e.g., null for string, 0 for int)
                return default;
            }
            // Otherwise, deserialize the simple type normally using standard options
            return reader.GetUInt64();
        }

        public override void Write(Utf8JsonWriter writer, ulong value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value, options);
        }
    }

    internal class IgnoreObjectForDoubleConverter : JsonConverter<double>
    {
        public override double Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // Check if the current JSON token is the start of an object/struct
            if (reader.TokenType == JsonTokenType.StartObject)
            {
                // Skip the entire JSON object hierarchy
                reader.Skip();

                // Return the default value for the expected simple type (e.g., null for string, 0 for int)
                return default;
            }
            // Otherwise, deserialize the simple type normally using standard options
            return reader.GetDouble();
        }

        public override void Write(Utf8JsonWriter writer, double value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value, options);
        }
    }

    internal class IgnoreObjectForFloatConverter : JsonConverter<float>
    {
        public override float Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // Check if the current JSON token is the start of an object/struct
            if (reader.TokenType == JsonTokenType.StartObject)
            {
                // Skip the entire JSON object hierarchy
                reader.Skip();

                // Return the default value for the expected simple type (e.g., null for string, 0 for int)
                return default;
            }
            // Otherwise, deserialize the simple type normally using standard options
            return reader.GetSingle();
        }

        public override void Write(Utf8JsonWriter writer, float value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value, options);
        }
    }

    internal class IgnoreObjectForGuidConverter : JsonConverter<Guid>
    {
        public override Guid Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // Check if the current JSON token is the start of an object/struct
            if (reader.TokenType == JsonTokenType.StartObject)
            {
                // Skip the entire JSON object hierarchy
                reader.Skip();

                // Return the default value for the expected simple type (e.g., null for string, 0 for int)
                return default;
            }
            // Otherwise, deserialize the simple type normally using standard options
            return reader.GetGuid();
        }

        public override void Write(Utf8JsonWriter writer, Guid value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value, options);
        }
    }

    internal class IgnoreObjectForDecimalConverter : JsonConverter<decimal>
    {
        public override decimal Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // Check if the current JSON token is the start of an object/struct
            if (reader.TokenType == JsonTokenType.StartObject)
            {
                // Skip the entire JSON object hierarchy
                reader.Skip();

                // Return the default value for the expected simple type (e.g., null for string, 0 for int)
                return default;
            }
            // Otherwise, deserialize the simple type normally using standard options
            return reader.GetDecimal();
        }

        public override void Write(Utf8JsonWriter writer, decimal value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value, options);
        }
    }

    internal class IgnoreObjectForDateTimeConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // Check if the current JSON token is the start of an object/struct
            if (reader.TokenType == JsonTokenType.StartObject)
            {
                // Skip the entire JSON object hierarchy
                reader.Skip();

                // Return the default value for the expected simple type (e.g., null for string, 0 for int)
                return default;
            }
            // Otherwise, deserialize the simple type normally using standard options
            return reader.GetDateTime();
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value, options);
        }
    }

    internal class IgnoreObjectForBoolArrayConverter : JsonConverter<bool[]>
    {
        public override bool[]? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // Check if the current JSON token is the start of an object/struct
            if (reader.TokenType == JsonTokenType.StartObject)
            {
                // Skip the entire JSON object hierarchy
                reader.Skip();

                // Return the default value for the expected simple type (e.g., null for string, 0 for int)
                return default;
            }
            // Otherwise, deserialize the array normally using standard options
            var list = new List<bool>();
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndArray)
                    break;
                if (reader.TokenType == JsonTokenType.StartObject)
                {
                    reader.Skip();
                    list.Add(default);
                }
                else
                {
                    list.Add(reader.GetBoolean());
                }
            }
            return list.ToArray();
        }

        public override void Write(Utf8JsonWriter writer, bool[] value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            foreach (var item in value)
                writer.WriteBooleanValue(item);
            writer.WriteEndArray();
        }
    }

    internal class IgnoreObjectForStringArrayConverter : JsonConverter<string[]>
    {
        public override string[]? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // Check if the current JSON token is the start of an object/struct
            if (reader.TokenType == JsonTokenType.StartObject)
            {
                // Skip the entire JSON object hierarchy
                reader.Skip();

                // Return the default value for the expected simple type (e.g., null for string, 0 for int)
                return default;
            }
            // Otherwise, deserialize the array normally using standard options
            var list = new List<string>();
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndArray)
                    break;
                if (reader.TokenType == JsonTokenType.StartObject)
                {
                    reader.Skip();
                    list.Add(string.Empty);
                }
                else
                {
                    list.Add(reader.GetString() ?? string.Empty);
                }
            }
            return list.ToArray();
        }

        public override void Write(Utf8JsonWriter writer, string[] value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            foreach (var item in value)
                writer.WriteStringValue(item);
            writer.WriteEndArray();
        }
    }

    internal class IgnoreObjectForByteArrayConverter : JsonConverter<byte[]>
    {
        public override byte[]? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // Check if the current JSON token is the start of an object/struct
            if (reader.TokenType == JsonTokenType.StartObject)
            {
                // Skip the entire JSON object hierarchy
                reader.Skip();

                // Return the default value for the expected simple type (e.g., null for string, 0 for int)
                return default;
            }
            // Otherwise, deserialize the array normally using standard options
            var list = new List<byte>();
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndArray)
                    break;
                if (reader.TokenType == JsonTokenType.StartObject)
                {
                    reader.Skip();
                    list.Add(default);
                }
                else
                {
                    list.Add(reader.GetByte());
                }
            }
            return list.ToArray();
        }

        public override void Write(Utf8JsonWriter writer, byte[] value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            foreach (var item in value)
                writer.WriteNumberValue(item);
            writer.WriteEndArray();
        }
    }

    internal class IgnoreObjectForSByteArrayConverter : JsonConverter<sbyte[]>
    {
        public override sbyte[]? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // Check if the current JSON token is the start of an object/struct
            if (reader.TokenType == JsonTokenType.StartObject)
            {
                // Skip the entire JSON object hierarchy
                reader.Skip();

                // Return the default value for the expected simple type (e.g., null for string, 0 for int)
                return default;
            }
            // Otherwise, deserialize the array normally using standard options
            var list = new List<sbyte>();
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndArray)
                    break;
                if (reader.TokenType == JsonTokenType.StartObject)
                {
                    reader.Skip();
                    list.Add(default);
                }
                else
                {
                    list.Add(reader.GetSByte());
                }
            }
            return list.ToArray();
        }

        public override void Write(Utf8JsonWriter writer, sbyte[] value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            foreach (var item in value)
                writer.WriteNumberValue(item);
            writer.WriteEndArray();
        }
    }

    internal class IgnoreObjectForShortArrayConverter : JsonConverter<short[]>
    {
        public override short[]? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // Check if the current JSON token is the start of an object/struct
            if (reader.TokenType == JsonTokenType.StartObject)
            {
                // Skip the entire JSON object hierarchy
                reader.Skip();

                // Return the default value for the expected simple type (e.g., null for string, 0 for int)
                return default;
            }
            // Otherwise, deserialize the array normally using standard options
            var list = new List<short>();
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndArray)
                    break;
                if (reader.TokenType == JsonTokenType.StartObject)
                {
                    reader.Skip();
                    list.Add(default);
                }
                else
                {
                    list.Add(reader.GetInt16());
                }
            }
            return list.ToArray();
        }

        public override void Write(Utf8JsonWriter writer, short[] value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            foreach (var item in value)
                writer.WriteNumberValue(item);
            writer.WriteEndArray();
        }
    }

    internal class IgnoreObjectForUShortArrayConverter : JsonConverter<ushort[]>
    {
        public override ushort[]? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // Check if the current JSON token is the start of an object/struct
            if (reader.TokenType == JsonTokenType.StartObject)
            {
                // Skip the entire JSON object hierarchy
                reader.Skip();

                // Return the default value for the expected simple type (e.g., null for string, 0 for int)
                return default;
            }
            // Otherwise, deserialize the array normally using standard options
            var list = new List<ushort>();
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndArray)
                    break;
                if (reader.TokenType == JsonTokenType.StartObject)
                {
                    reader.Skip();
                    list.Add(default);
                }
                else
                {
                    list.Add(reader.GetUInt16());
                }
            }
            return list.ToArray();
        }

        public override void Write(Utf8JsonWriter writer, ushort[] value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            foreach (var item in value)
                writer.WriteNumberValue(item);
            writer.WriteEndArray();
        }
    }

    internal class IgnoreObjectForIntArrayConverter : JsonConverter<int[]>
    {
        public override int[]? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // Check if the current JSON token is the start of an object/struct
            if (reader.TokenType == JsonTokenType.StartObject)
            {
                // Skip the entire JSON object hierarchy
                reader.Skip();

                // Return the default value for the expected simple type (e.g., null for string, 0 for int)
                return default;
            }
            // Otherwise, deserialize the array normally using standard options
            var list = new List<int>();
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndArray)
                    break;
                if (reader.TokenType == JsonTokenType.StartObject)
                {
                    reader.Skip();
                    list.Add(default);
                }
                else
                {
                    list.Add(reader.GetInt32());
                }
            }
            return list.ToArray();
        }

        public override void Write(Utf8JsonWriter writer, int[] value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            foreach (var item in value)
                writer.WriteNumberValue(item);
            writer.WriteEndArray();
        }
    }


    internal class IgnoreObjectForUIntArrayConverter : JsonConverter<uint[]>
    {
        public override uint[]? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // Check if the current JSON token is the start of an object/struct
            if (reader.TokenType == JsonTokenType.StartObject)
            {
                // Skip the entire JSON object hierarchy
                reader.Skip();

                // Return the default value for the expected simple type (e.g., null for string, 0 for int)
                return default;
            }
            // Otherwise, deserialize the array normally using standard options
            var list = new List<uint>();
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndArray)
                    break;
                if (reader.TokenType == JsonTokenType.StartObject)
                {
                    reader.Skip();
                    list.Add(default);
                }
                else
                {
                    list.Add(reader.GetUInt32());
                }
            }
            return list.ToArray();
        }

        public override void Write(Utf8JsonWriter writer, uint[] value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            foreach (var item in value)
                writer.WriteNumberValue(item);
            writer.WriteEndArray();
        }
    }

    internal class IgnoreObjectForLongArrayConverter : JsonConverter<long[]>
    {
        public override long[]? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // Check if the current JSON token is the start of an object/struct
            if (reader.TokenType == JsonTokenType.StartObject)
            {
                // Skip the entire JSON object hierarchy
                reader.Skip();

                // Return the default value for the expected simple type (e.g., null for string, 0 for int)
                return default;
            }
            // Otherwise, deserialize the array normally using standard options
            var list = new List<long>();
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndArray)
                    break;
                if (reader.TokenType == JsonTokenType.StartObject)
                {
                    reader.Skip();
                    list.Add(default);
                }
                else
                {
                    list.Add(reader.GetInt64());
                }
            }
            return list.ToArray();
        }

        public override void Write(Utf8JsonWriter writer, long[] value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            foreach (var item in value)
                writer.WriteNumberValue(item);
            writer.WriteEndArray();
        }
    }

    internal class IgnoreObjectForULongArrayConverter : JsonConverter<ulong[]>
    {
        public override ulong[]? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // Check if the current JSON token is the start of an object/struct
            if (reader.TokenType == JsonTokenType.StartObject)
            {
                // Skip the entire JSON object hierarchy
                reader.Skip();

                // Return the default value for the expected simple type (e.g., null for string, 0 for int)
                return default;
            }
            // Otherwise, deserialize the array normally using standard options
            var list = new List<ulong>();
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndArray)
                    break;
                if (reader.TokenType == JsonTokenType.StartObject)
                {
                    reader.Skip();
                    list.Add(default);
                }
                else
                {
                    list.Add(reader.GetUInt64());
                }
            }
            return list.ToArray();
        }

        public override void Write(Utf8JsonWriter writer, ulong[] value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            foreach (var item in value)
                writer.WriteNumberValue(item);
            writer.WriteEndArray();
        }
    }

    internal class IgnoreObjectForDoubleArrayConverter : JsonConverter<double[]>
    {
        public override double[]? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // Check if the current JSON token is the start of an object/struct
            if (reader.TokenType == JsonTokenType.StartObject)
            {
                // Skip the entire JSON object hierarchy
                reader.Skip();

                // Return the default value for the expected simple type (e.g., null for string, 0 for int)
                return default;
            }
            // Otherwise, deserialize the array normally using standard options
            var list = new List<double>();
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndArray)
                    break;
                if (reader.TokenType == JsonTokenType.StartObject)
                {
                    reader.Skip();
                    list.Add(default);
                }
                else
                {
                    list.Add(reader.GetDouble());
                }
            }
            return list.ToArray();
        }

        public override void Write(Utf8JsonWriter writer, double[] value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            foreach (var item in value)
                writer.WriteNumberValue(item);
            writer.WriteEndArray();
        }
    }

    internal class IgnoreObjectForFloatArrayConverter : JsonConverter<float[]>
    {
        public override float[]? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // Check if the current JSON token is the start of an object/struct
            if (reader.TokenType == JsonTokenType.StartObject)
            {
                // Skip the entire JSON object hierarchy
                reader.Skip();

                // Return the default value for the expected simple type (e.g., null for string, 0 for int)
                return default;
            }
            // Otherwise, deserialize the array normally using standard options
            var list = new List<float>();
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndArray)
                    break;
                if (reader.TokenType == JsonTokenType.StartObject)
                {
                    reader.Skip();
                    list.Add(default);
                }
                else
                {
                    list.Add(reader.GetSingle());
                }
            }
            return list.ToArray();
        }

        public override void Write(Utf8JsonWriter writer, float[] value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            foreach (var item in value)
                writer.WriteNumberValue(item);
            writer.WriteEndArray();
        }
    }

    internal class IgnoreObjectForGuidArrayConverter : JsonConverter<Guid[]>
    {
        public override Guid[]? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // Check if the current JSON token is the start of an object/struct
            if (reader.TokenType == JsonTokenType.StartObject)
            {
                // Skip the entire JSON object hierarchy
                reader.Skip();

                // Return the default value for the expected simple type (e.g., null for string, 0 for int)
                return default;
            }
            // Otherwise, deserialize the array normally using standard options
            var list = new List<Guid>();
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndArray)
                    break;
                if (reader.TokenType == JsonTokenType.StartObject)
                {
                    reader.Skip();
                    list.Add(default);
                }
                else
                {
                    list.Add(reader.GetGuid());
                }
            }
            return list.ToArray();
        }

        public override void Write(Utf8JsonWriter writer, Guid[] value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            foreach (var item in value)
                writer.WriteStringValue(item);
            writer.WriteEndArray();
        }
    }

    internal class IgnoreObjectForDecimalArrayConverter : JsonConverter<decimal[]>
    {
        public override decimal[]? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // Check if the current JSON token is the start of an object/struct
            if (reader.TokenType == JsonTokenType.StartObject)
            {
                // Skip the entire JSON object hierarchy
                reader.Skip();

                // Return the default value for the expected simple type (e.g., null for string, 0 for int)
                return default;
            }
            // Otherwise, deserialize the array normally using standard options
            var list = new List<decimal>();
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndArray)
                    break;
                if (reader.TokenType == JsonTokenType.StartObject)
                {
                    reader.Skip();
                    list.Add(default);
                }
                else
                {
                    list.Add(reader.GetDecimal());
                }
            }
            return list.ToArray();
        }

        public override void Write(Utf8JsonWriter writer, decimal[] value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            foreach (var item in value)
                writer.WriteNumberValue(item);
            writer.WriteEndArray();
        }
    }

    internal class IgnoreObjectForDateTimeArrayConverter : JsonConverter<DateTime[]>
    {
        public override DateTime[]? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // Check if the current JSON token is the start of an object/struct
            if (reader.TokenType == JsonTokenType.StartObject)
            {
                // Skip the entire JSON object hierarchy
                reader.Skip();

                // Return the default value for the expected simple type (e.g., null for string, 0 for int)
                return default;
            }
            // Otherwise, deserialize the array normally using standard options
            var list = new List<DateTime>();
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndArray)
                    break;
                if (reader.TokenType == JsonTokenType.StartObject)
                {
                    reader.Skip();
                    list.Add(default);
                }
                else
                {
                    list.Add(reader.GetDateTime());
                }
            }
            return list.ToArray();
        }

        public override void Write(Utf8JsonWriter writer, DateTime[] value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            foreach (var item in value)
                writer.WriteStringValue(item);
            writer.WriteEndArray();
        }
    }
}