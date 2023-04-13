using ConsoleAppLogAnalizer.Sessions;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static Npgsql.Replication.PgOutput.Messages.RelationMessage;


namespace ConsoleAppLogAnalizer
{
    public class DataExporter
    {
        public PostgresConnection ConnectionSettings { get; set; }
        public DataExporter(PostgresConnection connectionSettings)
        {
            ConnectionSettings = connectionSettings;
        }

        private bool CheckTableMatching(string tableNameForImport, string[] columnsForImport, DataTable dataTableForImport)
        {
            if (dataTableForImport.TableName != tableNameForImport)
                return false;
            foreach (var column in columnsForImport)
            {
                if (!dataTableForImport.Columns.Contains(column))
                {
                    return false;
                }
                else
                {
                    var cmdString = String.Format(
                        $"SELECT {column} " +
                        $"FROM {tableNameForImport} " +
                        $"LIMIT 1");

                    NpgsqlCommand cmd = new NpgsqlCommand(cmdString, ConnectionSettings.Connection);
                    cmd.Prepare();
                    var reader = cmd.ExecuteReader();
                    var columnType = reader.GetFieldType(column);
                    reader.Close();
                    if (columnType != dataTableForImport.Columns[column].DataType)
                        return false;
                }
            }
            return true;
        }

        public void ExportDataTable(string tableNameForImport, string[] columnsForImport, DataTable dataTableForImport)
        {
            ConnectionSettings.Connection.Open();

            if (!CheckTableMatching(tableNameForImport, columnsForImport, dataTableForImport))
                throw new Exception("Данные не соответствуют таблице импорта");

            var importColumnNames1 = columnsForImport.Aggregate((x, y) => $"{x}" + $", {y}");
            var importColumnNames2 = "@" + columnsForImport.Aggregate((x, y) => $"{x}" + $", @{y}");
            var dataSet = new DataSet();
            var dataAdapter = new NpgsqlDataAdapter($"select * from {tableNameForImport}", ConnectionSettings.Connection);
            var command = new NpgsqlCommand($"INSERT INTO {tableNameForImport}({importColumnNames1})" +
                $"VALUES({importColumnNames2})", ConnectionSettings.Connection);

            dataAdapter.InsertCommand = command;

            for (var i = 0; i < columnsForImport.Length; i++)
            {
                dataAdapter.InsertCommand.Parameters.Add(new NpgsqlParameter(columnsForImport[i], dataTableForImport.Columns[i].DataType.FullName));
                dataAdapter.InsertCommand.Parameters[i].Direction = ParameterDirection.Input;
                dataAdapter.InsertCommand.Parameters[i].SourceColumn = columnsForImport[i];
            }

            dataAdapter.Fill(dataSet);
            DataTable newData = dataSet.Tables[0];
            for (var i = 0; i < dataTableForImport.Rows.Count; i++)
            {
                var newRow = newData.NewRow();
                for (var j = 0; j < dataTableForImport.Columns.Count; j++)
                {
                    newRow[columnsForImport[j]] = dataTableForImport.Rows[i][j];
                }
                newData.Rows.Add(newRow);
            }
            var ds = dataSet.GetChanges();
            if (ds != null)
            {
                dataAdapter.Update(ds);
                dataSet.Merge(ds);
                dataSet.AcceptChanges();
            }
            else
            {
                Console.WriteLine("Данные для добавления отсутствуют");
            }

            ConnectionSettings.Connection.Close();
        }

        

        public void ExportList(string tableNameForImport, string columnForImport, List<string> dataSetForImport)
        {
            NpgsqlCommand command = new NpgsqlCommand($"INSERT INTO {tableNameForImport}({columnForImport})" +
                $"VALUES(:{columnForImport})", ConnectionSettings.Connection);
            
            foreach (var data in dataSetForImport)
            { 
                    command.Parameters.AddWithValue(columnForImport, data);
                    command.ExecuteNonQuery();
            }
        }

        

        public static void UpdateDataTable(DataTable tableToUpdate, string primaryKeyColumn)
        {
            var connSettings = new PostgresConnection();
            connSettings.Connection.Open();
            var columnsToUpdate = "";

            for(var i=0;i< tableToUpdate.Columns.Count;i++)
            {
                columnsToUpdate = columnsToUpdate+tableToUpdate.Columns[i].ColumnName + " = @" + tableToUpdate.Columns[i].ColumnName+", ";
            }
            columnsToUpdate = columnsToUpdate[..^2];
            //var columnsToUpdate= tableToUpdate.Columns.Aggregate((x, y) => $"{x}" + $", {y}");
            //var primaryKeyColumnName = tableToUpdate.PrimaryKey[0].ColumnName;

            var commandText = $"UPDATE {tableToUpdate.TableName} " +
                                    $"SET {columnsToUpdate} " +
                                    $"WHERE {primaryKeyColumn}=@{primaryKeyColumn}";
            
            foreach (DataRow dr in tableToUpdate.Rows)
            {
                var cmd = new NpgsqlCommand(commandText, connSettings.Connection);
                for (var i = 0; i < tableToUpdate.Columns.Count; i++)
                {
                    
                    cmd.Parameters.AddWithValue(tableToUpdate.Columns[i].ColumnName, dr[tableToUpdate.Columns[i].ColumnName]);
                    
                }
                cmd.ExecuteNonQuery();
            }
            connSettings.Connection.Close();
        }





    }
}

