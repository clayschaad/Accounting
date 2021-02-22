using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Schaad.Accounting.Converter
{
    public class IntegerConverter : JsonConverter<int>
    {
        public override int Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();
            if (string.IsNullOrEmpty(value))
            {
                return 0;
            }
            return int.Parse(value);
        }
        public override void Write(Utf8JsonWriter writer, int value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue(value);
        }
    }
}
