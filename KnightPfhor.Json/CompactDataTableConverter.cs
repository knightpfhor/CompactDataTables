using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace KnightPfhor.Json
{
    public class CompactDataTableConverter : JsonConverter
    {
        /// <summary>
        /// Writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer">The <see cref="JsonWriter"/> to write to.</param>
        /// <param name="value">The value.</param>
        /// <param name="serializer">The calling serializer.</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (writer == null) throw new ArgumentNullException("writer");
            if (serializer == null) throw new ArgumentNullException("serializer");

            DataTable table = (DataTable)value;

            writer.WriteStartArray();

            writer.WriteStartObject();

            foreach (DataColumn column in table.Columns)
            {
                writer.WritePropertyName(column.ColumnName);

                if (column.DataType == typeof(bool))
                {
                    writer.WriteValue((int)DataType.Boolean);
                }
                else if (column.DataType == typeof(int))
                {
                    writer.WriteValue((int)DataType.Int32);
                }
                else if (column.DataType == typeof(long))
                {
                    writer.WriteValue((int)DataType.Int64);
                }
                else if (column.DataType == typeof(byte))
                {
                    writer.WriteValue((int)DataType.Byte);
                }
                else if (column.DataType == typeof(decimal))
                {
                    writer.WriteValue((int)DataType.Decimal);
                }
                else if (column.DataType == typeof(float))
                {
                    writer.WriteValue((int)DataType.Float);
                }
                else if (column.DataType == typeof(double))
                {
                    writer.WriteValue((int)DataType.Double);
                }
                else if (column.DataType == typeof(DateTime))
                {
                    writer.WriteValue((int)DataType.DateTime);
                }
                else if (column.DataType == typeof(byte[]))
                {
                    writer.WriteValue((int)DataType.ByteArray);
                }
                else
                {
                    writer.WriteValue((int)DataType.String);
                }
            }

            writer.WriteEndObject();

            foreach (DataRow row in table.Rows)
            {
                writer.WriteStartArray();

                foreach (DataColumn column in row.Table.Columns)
                {
                    serializer.Serialize(writer, row[column]);
                }

                writer.WriteEndArray();
            }

            writer.WriteEndArray();
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
            if (reader == null) throw new ArgumentNullException("reader");
            if (objectType == null) throw new ArgumentNullException("objectType");

            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            if (reader.TokenType != JsonToken.StartArray)
            {
                throw new JsonException(string.Format(CultureInfo.InvariantCulture, "Expected StartArray token not {0}", reader.TokenType));
            }

            var dt = existingValue as DataTable;

            if (dt == null)
            {
                // handle typed datasets
                dt = (objectType == typeof(DataTable))
                       ? new DataTable()
                       : (DataTable)Activator.CreateInstance(objectType);

                dt.Locale = CultureInfo.CurrentCulture;
            }

            var columnTypes = new List<DataType>();

            reader.Read(); // Move from StartArray to StartObject

            if (reader.TokenType != JsonToken.StartObject)
            {
                throw new JsonException(string.Format(CultureInfo.InvariantCulture, "Expected StartObject token not {0}", reader.TokenType));
            }

            reader.Read(); // move from StartObject to PropertyName

            if (reader.TokenType != JsonToken.PropertyName)
            {
                throw new JsonException(string.Format(CultureInfo.InvariantCulture, "Expected PropertyName token not {0}", reader.TokenType));
            }

            while (reader.TokenType == JsonToken.PropertyName)
            {
                var columnName = (string)reader.Value;

                reader.ReadAsInt32();

                var dataType = (int)reader.Value;

                dt.Columns.Add(new DataColumn(columnName, TypeFromDataType(dataType)));

                columnTypes.Add((DataType)dataType);

                reader.Read();
            }

            if (reader.TokenType != JsonToken.EndObject)
            {
                throw new JsonException(string.Format(CultureInfo.InvariantCulture, "Expected EndObject token not {0}", reader.TokenType));
            }

            reader.Read();

            // If there are no rows in this table, we might actually be at the end of our object
            // so just move on
            if (reader.TokenType == JsonToken.StartArray)
            {
                while (reader.TokenType == JsonToken.StartArray)
                {
                    var row = dt.NewRow();

                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        ReadByDataType(reader, columnTypes[i]);

                        if (reader.TokenType == JsonToken.Null)
                        {
                            row[i] = DBNull.Value;
                        }
                        else
                        {
                            row[i] = reader.Value;
                        }
                    }

                    row.EndEdit();

                    dt.Rows.Add(row);

                    // We've finished reading all of the columns, so the next this we read should
                    // be at the should be an EndArray token
                    reader.Read();

                    if (reader.TokenType != JsonToken.EndArray)
                    {
                        throw new JsonException(string.Format(CultureInfo.InvariantCulture,
                            "Expected EndArray token not {0}.  It is possible that there is a column count mismatch",
                            reader.TokenType));
                    }

                    reader.Read(); // This should be the start array element, but could be the end of the enclosing array
                }
            }

            if (reader.TokenType != JsonToken.EndArray)
            {
                throw new JsonException(string.Format(CultureInfo.InvariantCulture, "Expected EndArray token not {0}", reader.TokenType));
            }

            return dt;
        }

        private static void ReadByDataType(JsonReader reader, DataType dataType)
        {
            switch (dataType)
            {
                case DataType.String:
                    reader.ReadAsString();
                    break;

                case DataType.Int32:
                    reader.ReadAsInt32();
                    break;

                case DataType.ByteArray:
                    reader.ReadAsBytes();
                    break;

                case DataType.Decimal:
                    reader.ReadAsDecimal();
                    break;

                case DataType.DateTime:
                    reader.ReadAsDateTime();
                    break;

                    // These types are dealt with, but there's no explicit reader method for them
                    // so we just have to trust that it does the right thing.
                case DataType.Byte:
                case DataType.Int64:
                case DataType.Boolean:
                case DataType.Float:
                case DataType.Double:
                    reader.Read();
                    break;
            }
        }

        private static Type TypeFromDataType (int dataType)
        {
            Type columnType;

            switch (dataType)
            {
                case (int) DataType.String:
                    columnType = typeof (string);
                    break;

                case (int) DataType.Boolean:
                    columnType = typeof (bool);
                    break;

                case (int) DataType.Int32:
                    columnType = typeof (int);
                    break;

                case (int) DataType.Int64:
                    columnType = typeof (long);
                    break;

                case (int) DataType.Byte:
                    columnType = typeof (byte);
                    break;

                case (int) DataType.Decimal:
                    columnType = typeof (decimal);
                    break;

                case (int) DataType.DateTime:
                    columnType = typeof (DateTime);
                    break;

                case (int) DataType.Float:
                    columnType = typeof (float);
                    break;

                case (int) DataType.Double:
                    columnType = typeof (double);
                    break;

                case (int) DataType.ByteArray:
                    columnType = typeof (byte[]);
                    break;

                default:
                    throw new JsonException(string.Format(CultureInfo.InvariantCulture, "Invalid column type returned {0}", dataType));
            }
            
            return columnType;
        }

        /// <summary>
        /// Determines whether this instance can convert the specified value type.
        /// </summary>
        /// <param name="objectType">Type of the value.</param>
        /// <returns>
        /// 	<c>true</c> if this instance can convert the specified value type; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanConvert(Type objectType)
        {
            return typeof(DataTable).IsAssignableFrom(objectType);
        }
    }

}
