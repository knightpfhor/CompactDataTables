using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace KnightPfhor.Json
{
    /// <summary>
    /// Converts a <see cref="DataSet"/> to and from JSON.
    /// </summary>
    public class CompactDataSetConverter : JsonConverter
    {
        /// <summary>
        /// Writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer">The <see cref="JsonWriter"/> to write to.</param>
        /// <param name="value">The value.</param>
        /// <param name="serializer">The calling serializer.</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var dataSet = (DataSet)value;
            var resolver = serializer.ContractResolver as DefaultContractResolver;

            var converter = new CompactDataTableConverter();

            writer.WriteStartObject();

            foreach (DataTable table in dataSet.Tables)
            {
                writer.WritePropertyName((resolver != null) ? resolver.GetResolvedPropertyName(table.TableName) : table.TableName);

                converter.WriteJson(writer, table, serializer);
            }

            writer.WriteEndObject();
        }

        /// <summary>
        /// Reads the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="JsonReader"/> to read from.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="existingValue">The existing value of object being read.</param>
        /// <param name="serializer">The calling serializer.</param>
        /// <returns>The object value.</returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            if (reader.TokenType != JsonToken.StartObject)
            {
                throw new JsonException(string.Format(CultureInfo.InvariantCulture, "DataSet must start with StartObject token not {0}", reader.TokenType));
            }

            // handle typed datasets
            DataSet ds = (objectType == typeof(DataSet))
              ? new DataSet()
              : (DataSet)Activator.CreateInstance(objectType);

            var converter = new CompactDataTableConverter();

            reader.Read();

            if (reader.TokenType != JsonToken.PropertyName)
            {
                throw new JsonException(string.Format(CultureInfo.InvariantCulture, "Expected PropertyName token not {0}", reader.TokenType));
            }

            while (reader.TokenType == JsonToken.PropertyName)
            {
                var tableName = (string)reader.Value;

                DataTable dt = ds.Tables[tableName];
                bool exists = (dt != null);

                reader.Read();

                dt = (DataTable)converter.ReadJson(reader, typeof(DataTable), dt, serializer);

                dt.TableName = tableName;

                if (!exists)
                    ds.Tables.Add(dt);

                reader.Read();
            }

            if (reader.TokenType != JsonToken.EndObject)
            {
                throw new JsonException(string.Format(CultureInfo.InvariantCulture, "Expected EndObject token not {0}", reader.TokenType));
            }

            return ds;
        }

        /// <summary>
        /// Determines whether this instance can convert the specified value type.
        /// </summary>
        /// <param name="valueType">Type of the value.</param>
        /// <returns>
        /// 	<c>true</c> if this instance can convert the specified value type; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanConvert(Type valueType)
        {
            return typeof(DataSet).IsAssignableFrom(valueType);
        }
    }
}
