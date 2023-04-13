using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppLogAnalizer
{
    public class PostgresConnection 

    {
        private string Host { get; set; }
        private string Port { get; set; }
        private string User { get; set; }
        private string Password { get; set; }
        private string DatabaseName { get; set; }
        public NpgsqlConnection Connection { get; set; }
        public PostgresConnection()
        {
            Host= "localhost";
            Port = "5432";
            User= "postgres";
            Password= "123";
            DatabaseName= "SoftwareLogData";

            var connectionString = String.Format($"Server={Host};Port={Port};" +
                $"User ID={User};Password={Password};Database={DatabaseName}");

            Connection = new NpgsqlConnection( connectionString);
        }
    }
}
