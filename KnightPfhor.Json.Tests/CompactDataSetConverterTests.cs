using System;
using System.Data;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace KnightPfhor.Json.Tests
{
    [TestClass]
    public class CompactDataSetConverterTests
    {
        [TestMethod]
        public void CanSerializeAndDeserializeWithSingleTable()
        {
            var converter = new CompactDataSetConverter();

            var initialSet = new DataSet();

            var initialTable = new DataTable { TableName = "Table1" };

            initialTable.Columns.Add("Int64 Data", typeof(long));
            initialTable.Columns.Add("Int32 Data", typeof(int));
            initialTable.Columns.Add("String Data", typeof(string));
            initialTable.Columns.Add("Byte Array Data", typeof(byte[]));

            initialTable.Rows.Add(long.MaxValue, int.MaxValue, "This is a string", new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });
            initialTable.Rows.Add(long.MinValue, int.MinValue, "This is another string", new byte[0]);
            initialTable.Rows.Add(DBNull.Value, DBNull.Value, DBNull.Value);

            initialSet.Tables.Add(initialTable);

            string serializedTable = JsonConvert.SerializeObject(initialSet, converter);

            Trace.WriteLine(serializedTable);

            var deserializedSet = (DataSet)JsonConvert.DeserializeObject(serializedTable, typeof(DataSet), converter);

            AssertDataSetsAreTheSame(initialSet, deserializedSet);
        }

        [TestMethod]
        public void CanSerializeAndDeserializeWithTwoTables()
        {
            var converter = new CompactDataSetConverter();

            var initialSet = new DataSet();

            var table1 = new DataTable { TableName = "Table1" };

            table1.Columns.Add("Int64 Data", typeof(long));
            table1.Columns.Add("Int32 Data", typeof(int));
            table1.Columns.Add("String Data", typeof(string));
            table1.Columns.Add("Byte Array Data", typeof(byte[]));

            table1.Rows.Add(long.MaxValue, int.MaxValue, "This is a string", new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });
            table1.Rows.Add(long.MinValue, int.MinValue, "This is another string", new byte[0]);
            table1.Rows.Add(DBNull.Value, DBNull.Value, DBNull.Value);

            initialSet.Tables.Add(table1);

            var table2 = new DataTable { TableName = "Table2" };

            table2.Columns.Add("DateTime Data", typeof(DateTime));
            table2.Columns.Add("Byte Data", typeof(byte));
            table2.Columns.Add("Boolean Data", typeof(bool));
            table2.Columns.Add("Float Data", typeof(float));
            table2.Columns.Add("Double Data", typeof(double));

            table2.Rows.Add(DateTime.MinValue, byte.MinValue, false, float.MinValue, double.MinValue);
            table2.Rows.Add(DateTime.MaxValue, byte.MaxValue, true, float.MaxValue, double.MaxValue);

            string serializedTable = JsonConvert.SerializeObject(initialSet, converter);

            Trace.WriteLine(serializedTable);

            var deserializedSet = (DataSet)JsonConvert.DeserializeObject(serializedTable, typeof(DataSet), converter);

            AssertDataSetsAreTheSame(initialSet, deserializedSet);
        }

        [TestMethod]
        public void CanSerializeAndDeserializeWithEmptyFirstTableAndPopulatedSecondTable()
        {
            var converter = new CompactDataSetConverter();

            var initialSet = new DataSet();

            var emptyTable = new DataTable { TableName = "Table1" };

            emptyTable.Columns.Add("Int32 Data", typeof(int));
            emptyTable.Columns.Add("String Data", typeof(string));

            initialSet.Tables.Add(emptyTable);

            var populatedTable = new DataTable { TableName = "Table2" };

            populatedTable.Columns.Add("String Data 2", typeof(string));
            populatedTable.Columns.Add("String Data 3", typeof(string));

            populatedTable.Rows.Add("This is not a number", "This is a string");
            populatedTable.Rows.Add("This isn't a number either", "But this is still a string");

            initialSet.Tables.Add(populatedTable);

            string serializedTable = JsonConvert.SerializeObject(initialSet, converter);

            Trace.WriteLine(serializedTable);

            var deserializedSet = (DataSet)JsonConvert.DeserializeObject(serializedTable, typeof(DataSet), converter);

            AssertDataSetsAreTheSame(initialSet, deserializedSet);
        }

        [TestMethod]
        public void NullCanBeDeserialized()
        {
            var converter = new CompactDataSetConverter();

            var deserializedSet = (DataSet)JsonConvert.DeserializeObject("null", typeof(DataSet), converter);

            Assert.IsNull(deserializedSet);
        }

        private void AssertDataSetsAreTheSame(DataSet initialSet, DataSet deserializedSet)
        {
            Assert.AreEqual(initialSet.Tables.Count, deserializedSet.Tables.Count);

            for (int i = 0; i < initialSet.Tables.Count; i++)
            {
                Assert.AreEqual(initialSet.Tables[i].TableName, deserializedSet.Tables[i].TableName);

                AssertColumnsHaveSameDefinition(initialSet.Tables[i], deserializedSet.Tables[i]);

                AssertRowsHaveSameData(initialSet.Tables[i], deserializedSet.Tables[i]);
            }
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
