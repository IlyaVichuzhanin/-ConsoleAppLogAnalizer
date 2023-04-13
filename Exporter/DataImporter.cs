using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Npgsql;

namespace ConsoleAppLogAnalizer
{
    public class DataImporter                           ///<T>
    {
        public DataImporter(PostgresConnection connectionSettings)
        {
            ConnectionSettings = connectionSettings;
        }
        public PostgresConnection ConnectionSettings { get; set; }

        public long GetNumberOfRecords(string tableName)
        {
            ConnectionSettings.Connection.Open();
            NpgsqlCommand command = new NpgsqlCommand($"SELECT COUNT(*) FROM {tableName}", ConnectionSettings.Connection);
            var recordsNumder = (long)command.ExecuteScalar();
            ConnectionSettings.Connection.Close();
            return recordsNumder;
        }

        //public List<T> GetUnstructuredLogList<PetexLog>()
        //{
        //    var primaryLogs = GetList("SELECT log_info " +
        //        "FROM petexprimarylogs WHERE logisstructured=false " +
        //        "ORDER BY auto_log_id ASC");
        //    return primaryLogs;
        //}

    }
}
