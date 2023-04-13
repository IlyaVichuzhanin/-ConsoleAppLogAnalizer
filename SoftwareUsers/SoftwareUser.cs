using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppLogAnalizer.SoftwareUsers
{
    public class SoftwareUser
    {
        public SoftwareUser(int softwareUserID, string windowsUserName, string computerUserName, int employeeID)
        {
            SoftwareUserID=softwareUserID;
            WindowsUserName = windowsUserName;
            ComputerUserName = computerUserName;
            EmployeeID = employeeID;
        }
        public SoftwareUser(string windowsUserName)
        {
            
            WindowsUserName = windowsUserName;
            ComputerUserName = "undefined";
            EmployeeID = 1;
        }
        public int SoftwareUserID { get; set; }
        public int EmployeeID { get; set; }
        public string WindowsUserName { get; set; }
        public string ComputerUserName { get; set; }
        public static void AddUndefinedUserToDB(SoftwareUser user)
        {
            var dt = new DataTable().ConstructTable(
                "softwareusers",
                new string[] {"windowsusername", "computerusername" },
                new Type[] { typeof(string), typeof(string) });
            dt.Rows.Add();
            dt.Rows[0]["windowsusername"] = user.WindowsUserName;
            dt.Rows[0]["computerusername"] = user.ComputerUserName;

            var importer = new DataExporter(new PostgresConnection());
            importer.ExportDataTable(
                "softwareusers",
                new string[] { "windowsusername", "computerusername" },
                dt);
        }
        public static void UpdateUndefinedUsers()
        {
            var connSettings = new PostgresConnection();
            connSettings.Connection.Open();

            var softwareUserDictionary = new Dictionary<int, SoftwareUser>();
            NpgsqlCommand command = new NpgsqlCommand(
                "SELECT * FROM softwareusers", connSettings.Connection);
            command.Prepare();
            NpgsqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                var user = new SoftwareUser(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetInt32(3));
                softwareUserDictionary.Add(user.SoftwareUserID, user);
            }
            reader.Close();

            //update structurated logs

            var dataAdapter = new NpgsqlDataAdapter($"select * from structuredlogs", connSettings.Connection);
            DataTable undefinedLogs = new DataTable("structuredlogs");
            NpgsqlCommandBuilder commandBuilder = new NpgsqlCommandBuilder(dataAdapter);
            dataAdapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;
            dataAdapter.Fill(undefinedLogs);

            DataRow[] changedRows = undefinedLogs.Select("employeeid=1");

            foreach (var row in changedRows)
            {
                row["employeeid"] = softwareUserDictionary[(int)row["softwareuserid"]].EmployeeID;
            }

            dataAdapter.Update(undefinedLogs);
            undefinedLogs.Clear();
            dataAdapter.Fill(undefinedLogs);

            //update sessions

            dataAdapter = new NpgsqlDataAdapter($"select * from softwaresessions", connSettings.Connection);
            var undefinedSessions = new DataTable("softwaresessions");
            commandBuilder = new NpgsqlCommandBuilder(dataAdapter);
            dataAdapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;
            dataAdapter.Fill(undefinedSessions);

            changedRows = undefinedSessions.Select("employeeid=1");

            foreach (var row in changedRows)
            {
                row["employeeid"] = softwareUserDictionary[(int)row["softwareuserid"]].EmployeeID;
            }

            dataAdapter.Update(undefinedSessions);
            undefinedSessions.Clear();
            dataAdapter.Fill(undefinedSessions);

            connSettings.Connection.Close();
        }

    }
}
