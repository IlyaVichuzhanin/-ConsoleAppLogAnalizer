using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ConsoleAppLogAnalizer
{
    public static class DataTableExtension
    {
        public static DataTable ConstructTable(this DataTable table, string tableName, string[] columnNames, Type[] columnTypes)
        {
            table.TableName = tableName;

            for (var i = 0; i < columnNames.Length; i++)
            {
                DataColumn column = new DataColumn(columnNames[i], columnTypes[i]);
                table.Columns.Add(column);
            }
            return table;
        }

        public static DataTable ConstructTable(this DataTable table, string tableName, string primaryKeyColumnName, Type primaryKeyColumnType, string[] columnNames, Type[] columnTypes)
        {
            table.TableName = tableName;
            table.AddColumn(primaryKeyColumnName, primaryKeyColumnType);

            table.PrimaryKey = new DataColumn[] { table.Columns[primaryKeyColumnName] };
            for (var i = 0; i < columnNames.Length; i++)
            {
                DataColumn column = new DataColumn(columnNames[i], columnTypes[i]);
                table.Columns.Add(column);
            }
            return table;
        }

        public static DataTable AddColumn(this DataTable table, string columnName, Type columnType)
        {
            table.Columns.Add(new DataColumn(columnName, columnType));
            return table;
        }
    }
}
