using System;
using System.Data;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace KnightPfhor.Json.Tests
{
    [TestClass]
    public class CompactDataTableConverterTests
    {
        [TestMethod]
        public void AllDataTypesAtOnce()
        {
            var converter = new CompactDataTableConverter();

            var baseTable = new DataTable();

            baseTable.Columns.Add("String Data", typeof(string));
            baseTable.Columns.Add("Int Data", typeof(int));
            baseTable.Columns.Add("Byte Array Data", typeof(byte[]));
            baseTable.Columns.Add("Date Time", typeof(byte[]));

            baseTable.Rows.Add("Code 1", 1, new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });
            baseTable.Rows.Add("Code 2", 2, new byte[] { 100, 100, 60, 70, 90 });
            baseTable.Rows.Add("Code 3", 3, new byte[] { 55, 42, 12, 89, 77, 66, 127 });
            baseTable.Rows.Add("Code 4", 4, new byte[] { 22, 21, 23, 20, 19, 18, 17, 16 });

            string serializedTable = JsonConvert.SerializeObject(baseTable, converter);

            Trace.WriteLine(serializedTable);

            var deserializedTable = (DataTable)JsonConvert.DeserializeObject(serializedTable, typeof(DataTable), converter);

            AssertColumnsHaveSameDefinition(baseTable, deserializedTable);

            AssertRowsHaveSameData(baseTable, deserializedTable);
        }

        [TestMethod]
        public void ByteArraysCanBeDeserialized()
        {
            var converter = new CompactDataTableConverter();

            var baseTable = new DataTable();

            baseTable.Columns.Add("Byte Array Data", typeof(byte[]));

            baseTable.Rows.Add(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });
            baseTable.Rows.Add(new byte[] { byte.MaxValue, byte.MinValue });
            baseTable.Rows.Add(new byte[] { 55, 42, 12, 89, 77, 66, 127 });
            baseTable.Rows.Add(new byte[] { 22, 21, 23, 20, 19, 18, 17, 16 });
            baseTable.Rows.Add(new byte[0]);
            baseTable.Rows.Add(new byte[6]);
            baseTable.Rows.Add(DBNull.Value);

            string serializedTable = JsonConvert.SerializeObject(baseTable, converter);

            Trace.WriteLine(serializedTable);

            var deserializedTable = (DataTable)JsonConvert.DeserializeObject(serializedTable, typeof(DataTable), converter);

            AssertColumnsHaveSameDefinition(baseTable, deserializedTable);

            AssertRowsHaveSameData(baseTable, deserializedTable);
        }

        [TestMethod]
        public void BytesCanBeDeserialized()
        {
            var converter = new CompactDataTableConverter();

            var baseTable = new DataTable();

            baseTable.Columns.Add("Byte Data", typeof(byte));

            baseTable.Rows.Add(22);
            baseTable.Rows.Add(byte.MinValue);
            baseTable.Rows.Add(byte.MaxValue);
            baseTable.Rows.Add(DBNull.Value);

            string serializedTable = JsonConvert.SerializeObject(baseTable, converter);

            Trace.WriteLine(serializedTable);

            var deserializedTable = (DataTable)JsonConvert.DeserializeObject(serializedTable, typeof(DataTable), converter);

            AssertColumnsHaveSameDefinition(baseTable, deserializedTable);

            AssertRowsHaveSameData(baseTable, deserializedTable);
        }

        [TestMethod]
        public void DateTimeCanBeDeserialized()
        {
            var converter = new CompactDataTableConverter();

            var baseTable = new DataTable();

            baseTable.Columns.Add("Date Time Data", typeof(DateTime));

            baseTable.Rows.Add(new DateTime(2013, 10, 15, 10, 30, 16, 133));
            baseTable.Rows.Add(DateTime.MinValue);
            baseTable.Rows.Add(DateTime.MaxValue);
            baseTable.Rows.Add(DBNull.Value);

            string serializedTable = JsonConvert.SerializeObject(baseTable, converter);

            Trace.WriteLine(serializedTable);

            var deserializedTable = (DataTable)JsonConvert.DeserializeObject(serializedTable, typeof(DataTable), converter);

            AssertColumnsHaveSameDefinition(baseTable, deserializedTable);

            AssertRowsHaveSameData(baseTable, deserializedTable);
        }

        [TestMethod]
        public void DecimalCanBeDeserialized()
        {
            var converter = new CompactDataTableConverter();

            var baseTable = new DataTable();

            baseTable.Columns.Add("Decimal Data", typeof(Decimal));

            baseTable.Rows.Add(133.899999m);
            baseTable.Rows.Add(0m);
            baseTable.Rows.Add(Decimal.MinValue);
            baseTable.Rows.Add(Decimal.MaxValue);
            baseTable.Rows.Add(Decimal.MinusOne);
            baseTable.Rows.Add(DBNull.Value);

            string serializedTable = JsonConvert.SerializeObject(baseTable, converter);

            Trace.WriteLine(serializedTable);

            var deserializedTable = (DataTable)JsonConvert.DeserializeObject(serializedTable, typeof(DataTable), converter);

            AssertColumnsHaveSameDefinition(baseTable, deserializedTable);

            AssertRowsHaveSameData(baseTable, deserializedTable);
        }

        [TestMethod]
        public void Int32CanBeDeserialized()
        {
            var converter = new CompactDataTableConverter();

            var baseTable = new DataTable();

            baseTable.Columns.Add("Int32 Data", typeof(int));

            baseTable.Rows.Add(256);
            baseTable.Rows.Add(-128);
            baseTable.Rows.Add(0);
            baseTable.Rows.Add(int.MinValue);
            baseTable.Rows.Add(int.MaxValue);
            baseTable.Rows.Add(DBNull.Value);

            string serializedTable = JsonConvert.SerializeObject(baseTable, converter);

            Trace.WriteLine(serializedTable);

            var deserializedTable = (DataTable)JsonConvert.DeserializeObject(serializedTable, typeof(DataTable), converter);

            AssertColumnsHaveSameDefinition(baseTable, deserializedTable);

            AssertRowsHaveSameData(baseTable, deserializedTable);
        }

        [TestMethod]
        public void StringCanBeDeserialized()
        {
            var converter = new CompactDataTableConverter();

            var baseTable = new DataTable();

            baseTable.Columns.Add("String Data", typeof(string));

            baseTable.Rows.Add("This is a string");
            baseTable.Rows.Add(String.Empty);
            baseTable.Rows.Add(DBNull.Value);

            string serializedTable = JsonConvert.SerializeObject(baseTable, converter);

            Trace.WriteLine(serializedTable);

            var deserializedTable = (DataTable)JsonConvert.DeserializeObject(serializedTable, typeof(DataTable), converter);

            AssertColumnsHaveSameDefinition(baseTable, deserializedTable);

            AssertRowsHaveSameData(baseTable, deserializedTable);
        }

        [TestMethod]
        public void BoolCanBeDeserialized()
        {
            var converter = new CompactDataTableConverter();

            var baseTable = new DataTable();

            baseTable.Columns.Add("Boolean Data", typeof(bool));

            baseTable.Rows.Add(true);
            baseTable.Rows.Add(false);
            baseTable.Rows.Add(DBNull.Value);

            string serializedTable = JsonConvert.SerializeObject(baseTable, converter);

            Trace.WriteLine(serializedTable);

            var deserializedTable = (DataTable)JsonConvert.DeserializeObject(serializedTable, typeof(DataTable), converter);

            AssertColumnsHaveSameDefinition(baseTable, deserializedTable);

            AssertRowsHaveSameData(baseTable, deserializedTable);
        }

        [TestMethod]
        public void Int64CanBeDeserialized()
        {
            var converter = new CompactDataTableConverter();

            var baseTable = new DataTable();

            baseTable.Columns.Add("Int64 Data", typeof(long));

            baseTable.Rows.Add(100001);
            baseTable.Rows.Add(0);
            baseTable.Rows.Add(long.MaxValue);
            baseTable.Rows.Add(long.MinValue);
            baseTable.Rows.Add(DBNull.Value);

            string serializedTable = JsonConvert.SerializeObject(baseTable, converter);

            Trace.WriteLine(serializedTable);

            var deserializedTable = (DataTable)JsonConvert.DeserializeObject(serializedTable, typeof(DataTable), converter);

            AssertColumnsHaveSameDefinition(baseTable, deserializedTable);

            AssertRowsHaveSameData(baseTable, deserializedTable);
        }

        [TestMethod]
        public void FloatCanBeDeserialized()
        {
            var converter = new CompactDataTableConverter();

            var baseTable = new DataTable();

            baseTable.Columns.Add("Float Data", typeof(float));

            baseTable.Rows.Add(100001.333333);
            baseTable.Rows.Add(0);
            baseTable.Rows.Add(float.MinValue);
            baseTable.Rows.Add(float.MaxValue);
            baseTable.Rows.Add(float.Epsilon);
            baseTable.Rows.Add(float.NegativeInfinity);
            baseTable.Rows.Add(float.PositiveInfinity);
            baseTable.Rows.Add(float.NaN);
            baseTable.Rows.Add(DBNull.Value);

            string serializedTable = JsonConvert.SerializeObject(baseTable, converter);

            Trace.WriteLine(serializedTable);

            var deserializedTable = (DataTable)JsonConvert.DeserializeObject(serializedTable, typeof(DataTable), converter);

            AssertColumnsHaveSameDefinition(baseTable, deserializedTable);

            AssertRowsHaveSameData(baseTable, deserializedTable);
        }

        [TestMethod]
        public void DoubleCanBeDeserialized()
        {
            var converter = new CompactDataTableConverter();

            var baseTable = new DataTable();

            baseTable.Columns.Add("Double Data", typeof(double));

            baseTable.Rows.Add(100001.333333);
            baseTable.Rows.Add(0);
            baseTable.Rows.Add(double.MinValue);
            baseTable.Rows.Add(double.MaxValue);
            baseTable.Rows.Add(double.Epsilon);
            baseTable.Rows.Add(double.NegativeInfinity);
            baseTable.Rows.Add(double.PositiveInfinity);
            baseTable.Rows.Add(double.NaN);
            baseTable.Rows.Add(DBNull.Value);

            string serializedTable = JsonConvert.SerializeObject(baseTable, converter);

            Trace.WriteLine(serializedTable);

            var deserializedTable = (DataTable)JsonConvert.DeserializeObject(serializedTable, typeof(DataTable), converter);

            AssertColumnsHaveSameDefinition(baseTable, deserializedTable);

            AssertRowsHaveSameData(baseTable, deserializedTable);
        }

        [TestMethod]
        public void EmptyTableCanBeDeserialized()
        {
            var converter = new CompactDataTableConverter();

            var baseTable = new DataTable();

            baseTable.Columns.Add("String Data", typeof(string));
            baseTable.Columns.Add("Int Data", typeof(int));
            baseTable.Columns.Add("Byte Array Data", typeof(byte[]));
            baseTable.Columns.Add("Date Time", typeof(byte[]));

            string serializedTable = JsonConvert.SerializeObject(baseTable, converter);

            Trace.WriteLine(serializedTable);

            var deserializedTable = (DataTable)JsonConvert.DeserializeObject(serializedTable, typeof(DataTable), converter);

            AssertColumnsHaveSameDefinition(baseTable, deserializedTable);

            AssertRowsHaveSameData(baseTable, deserializedTable);
        }

        [TestMethod]
        public void NullCanBeDeserialized()
        {
            var converter = new CompactDataTableConverter();

            var deserializedTable = (DataTable)JsonConvert.DeserializeObject("null", typeof(DataTable), converter);

            Assert.IsNull(deserializedTable);
        }

        private void AssertColumnsHaveSameDefinition(DataTable baseTable, DataTable deserializedTable)
        {
            Assert.AreEqual(baseTable.Columns.Count,
                deserializedTable.Columns.Count,
                "Number of columns is different, base {0} deserialized: {1}",
                baseTable.Columns.Count,
                deserializedTable.Columns.Count);

            for (int i = 0; i < baseTable.Columns.Count; i++)
            {
                Assert.AreEqual(baseTable.Columns[i].ColumnName,
                    deserializedTable.Columns[i].ColumnName,
                    "Column names don't match at position {0}.  Expected {1}, actual {2}",
                    i,
                    baseTable.Columns[i].ColumnName,
                    deserializedTable.Columns[i].ColumnName);

                Assert.AreEqual(baseTable.Columns[i].DataType,
                    deserializedTable.Columns[i].DataType,
                    "Data types don't match at position {0}.  Expected {1}, actual {2}",
                    i,
                    baseTable.Columns[i].DataType,
                    deserializedTable.Columns[i].DataType);
            }
        }

        private void AssertRowsHaveSameData(DataTable baseTable, DataTable deserializedTable)
        {
            Assert.AreEqual(baseTable.Rows.Count,
                deserializedTable.Rows.Count,
                "Number of Rows is different, base {0} deserialized: {1}",
                baseTable.Rows.Count,
                deserializedTable.Rows.Count);

            for (int i = 0; i < baseTable.Rows.Count; i++)
            {
                for (int j = 0; j < baseTable.Columns.Count; j++)
                {
                    if (baseTable.Rows[i][j] == null)
                    {
                        Assert.IsNull(deserializedTable.Rows[i][j]);

                    } if (baseTable.Rows[i][j] == DBNull.Value)
                    {
                        Assert.AreEqual(DBNull.Value, deserializedTable.Rows[i][j]);

                    }
                    else if (baseTable.Columns[j].DataType == typeof(byte[]))
                    {
                        var baseArray = (byte[])baseTable.Rows[i][j];
                        var deserializedArray = (byte[])deserializedTable.Rows[i][j];

                        Assert.AreEqual(baseArray.Length,
                            deserializedArray.Length,
                            "Byte array at row {0}, column {1} ({2}) doesn't have the correct length.  Expected {3}, actual {4}",
                            i,
                            j,
                            baseTable.Columns[j].ColumnName,
                            baseArray.Length,
                            deserializedArray.Length);

                        for (int k = 0; k < baseArray.Length; k++)
                        {
                            Assert.AreEqual(baseArray[k],
                                deserializedArray[k],
                                "Byte array at row {0}, column {1} ({2}) has incorrect byte at position {3}.  Expected {4}, actual {5}",
                                i,
                                j,
                                baseTable.Columns[j].ColumnName,
                                k,
                                baseArray[k],
                                deserializedArray[k]);
                        }
                    }
                    else
                    {
                        Assert.AreEqual(baseTable.Rows[i][j],
                            deserializedTable.Rows[i][j],
                            "Data doesn't match at row {0}, column {1} ({2}).  Expected {3}, actual {4}",
                            i,
                            j,
                            baseTable.Columns[j].ColumnName,
                            baseTable.Rows[i][j],
                            deserializedTable.Rows[i][j]);
                    }
                }
            }
        }
    }
}
