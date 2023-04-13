using ConsoleAppLogAnalizer.Sessions;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleAppLogAnalizer;


namespace ConsoleAppLogAnalizer
{
    public class SessionCreator //where Log : LogDataApplication.Log
    {
        public SessionCreator() { }
        public List<StructuredLog> GetUnmatchedLogsFromDB()
        {
            var unmatchedLogs = new List<StructuredLog>();
            var connSettings = new PostgresConnection();
            connSettings.Connection.Open();

            NpgsqlCommand selectCommand = new NpgsqlCommand($"SELECT * FROM structuredlogs WHERE logismatched=FALSE ORDER BY log_id ASC", connSettings.Connection);
            selectCommand.Prepare();
            NpgsqlDataReader dr = selectCommand.ExecuteReader();

            while (dr.Read())
            {
                unmatchedLogs.Add(new StructuredLog(
                   dr.GetFieldValue<string>("operation"),
                   dr.GetFieldValue<DateTime>("operationdatetime"),
                   dr.GetFieldValue<int>("software_id"),
                   dr.GetFieldValue<int>("softwaremodule_id"),
                   dr.GetFieldValue<int>("softwareuser_id"),
                   dr.GetFieldValue<int>("employee_id")
                   ));
            }
            dr.Close();

            NpgsqlCommand deleteCommand = new NpgsqlCommand($"DELETE FROM petexstructuredlogs WHERE logismatched=FALSE", connSettings.Connection);
            deleteCommand.Prepare();
            deleteCommand.ExecuteNonQuery();
            connSettings.Connection.Close();

            return unmatchedLogs;
        }

        public static List<Session> GetOpenSessionsFromDB()
        {
            var openSessions = new List<Session>();
            var connSettings = new PostgresConnection();
            connSettings.Connection.Open();

            NpgsqlCommand selectCommand = new NpgsqlCommand($"SELECT * FROM softwaresessions WHERE sessionisfinished=FALSE ORDER BY id ASC", connSettings.Connection);
            selectCommand.Prepare();
            NpgsqlDataReader dr = selectCommand.ExecuteReader();

            while (dr.Read())
            {
                openSessions.Add(new Session(
                   dr.GetFieldValue<int>("softwareuserid"),
                   dr.GetFieldValue<int>("employeeid"),
                   dr.GetFieldValue<int>("softwareid"),
                   dr.GetFieldValue<int>("softwaremoduleid"),
                   dr.GetFieldValue<DateTime>("logindatetime"),
                   DateTime.Now,
                   dr.GetFieldValue<bool>("sessionisfinished")
                   ));
            }
            dr.Close();
            NpgsqlCommand deleteCommand = new NpgsqlCommand($"DELETE FROM softwaresessions WHERE sessionisfinished=FALSE", connSettings.Connection);
            deleteCommand.Prepare();
            deleteCommand.ExecuteNonQuery();
            connSettings.Connection.Close();

            return openSessions;
        }

        public static Session? GetClosedSession(StructuredLog logout, List<Session> openSessions)
        {
            foreach (var session in openSessions)
            {
                if (session.SoftwareUserID == logout.SoftwareUserID 
                    && session.SoftwareId==logout.SoftwareID
                    && session.SoftwareModuleId == logout.SoftwareModuleID 
                    && session.SessionIsFinished==false)
                {
                    session.LogOutDateTime = logout.OperationDateTime;
                    session.SessionIsFinished = true;
                    openSessions.Remove(session);
                    return session;
                }
            }
            return null;
        }

        public static List<Session> GetSessionsFromLogs(List<StructuredLog> logList)
        {
            var sessionList = new List<Session>();
            var openSessions = GetOpenSessionsFromDB();
            foreach (var log in logList)
            {
                if (log.Operation == "login")
                {
                    var newSession = new Session(
                        log.SoftwareUserID,
                        log.EmployeeID,
                        log.SoftwareID,
                        log.SoftwareModuleID,
                        log.OperationDateTime,
                        DateTime.Now,
                        false);
                    openSessions.Add(newSession);
                }
                else
                {
                    var closedSession = GetClosedSession(log, openSessions);
                    if (closedSession == null)
                        throw new Exception("Не найдена открытая сессия.");
                    sessionList.Add(closedSession);
                }
            }
            sessionList.AddRange(openSessions);
            return sessionList;
        }

        public static void ExportSoftwareSessionsToDB(List<Session> sessions)
        {
            var dt = new DataTable();
            var tableName = "softwaresessions";
            var columnNames = new string[]
                {
                    "softwareuserid",
                    "employeeid",
                    "softwareid",
                    "softwaremoduleid",
                    "logindatetime",
                    "logoutdatetime",
                    "sessionisfinished",
                    "sessionisdevided",
                    "sessiontime"
                };

            var columnTypes = new Type[]
                {
                    typeof(int),
                    typeof(int),
                    typeof(int),
                    typeof(int),
                    typeof(DateTime),
                    typeof(DateTime),
                    typeof(bool),
                    typeof(bool),
                    typeof(double)
                };

            dt.ConstructTable(tableName, columnNames, columnTypes);

            sessions
                .ForEach(session =>
                {
                    var newRow = dt.NewRow();
                    newRow["softwareuserid"] = session.SoftwareUserID;
                    newRow["employeeid"] = session.EmployeeID;
                    newRow["softwareid"] = session.SoftwareId;
                    newRow["softwaremoduleid"] = session.SoftwareModuleId;
                    newRow["logindatetime"] = session.LogInDateTime;
                    newRow["logoutdatetime"] = session.LogOutDateTime;
                    newRow["sessionisfinished"] = session.SessionIsFinished;
                    newRow["sessionisdevided"] = false;
                    newRow["sessiontime"] = session.sessionTime;
                    dt.Rows.Add(newRow);
                });
            var dataExporter = new DataExporter(new PostgresConnection());
            dataExporter.ExportDataTable(tableName, columnNames, dt);
        }



    }
}
