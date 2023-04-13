using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Npgsql;

namespace ConsoleAppLogAnalizer
{
    public class PrimaryLogExporter
    {
        public PrimaryLogExporter(string filePath)
        {
            FilePath = filePath;
        }
        public PrimaryLogExporter(NpgsqlConnection connection)
        {
            Connection = connection;
        }
        private string FilePath { get; set; }
        public NpgsqlConnection Connection { get; set; }

        public List<PetexPrimaryLog> GetPetexPrimaryLogs(long start, long end)
        {
            if (FilePath == null)
                throw new Exception("File path is not defined");
            var primaryLogs= new List<PetexPrimaryLog>();

            File
                .ReadLines(FilePath)
                .Take((Index)start..(Index)(end + 1))
                .ToList()
                .ForEach(x => primaryLogs.Add(new PetexPrimaryLog(x)));
            return primaryLogs;
        }
        public List<PetexPrimaryLog> GetPetexPrimaryLogs(long start)
        {
            int end = File.ReadLines(FilePath).Count();
            return GetPetexPrimaryLogs(start, end);
        }

        public DataTable GetDataTable(string tableName, string[] columnNames, long start, long end)
        {
            var dt = new DataTable();
            var logList = GetPetexPrimaryLogs(start, end);
            dt.ConstructTable(tableName, columnNames, new Type[] { typeof(string) });

            foreach (var item in logList)
                dt.Rows.Add(item);
            return dt;
        }

        public DataTable GetDataTable(string tableName, string[] columnNames, long start)
        {
            int end = File.ReadLines(FilePath).Count();
            return GetDataTable(tableName, columnNames, start, end);
        }

    }
}
