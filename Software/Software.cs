using ConsoleAppLogAnalizer.Sessions;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppLogAnalizer.Software
{
    public class Software
    {
        public Software()
        {
            SoftwareDictionary=new Dictionary<int, string>();
            var connSettings = new PostgresConnection();
            connSettings.Connection.Open();

            NpgsqlCommand selectCommand = new NpgsqlCommand($"SELECT * FROM software ORDER BY software_id ASC", connSettings.Connection);
            selectCommand.Prepare();
            NpgsqlDataReader dr = selectCommand.ExecuteReader();

            while (dr.Read())
            {
                SoftwareDictionary.Add(
                   dr.GetFieldValue<int>("software_id"),
                   dr.GetFieldValue<string>("software_name")
                   );
            }
            dr.Close();
            connSettings.Connection.Close();
        }
        public Dictionary<int,string> SoftwareDictionary { get; set; }

    }
}
