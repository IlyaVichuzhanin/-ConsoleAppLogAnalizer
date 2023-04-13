using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ConsoleAppLogAnalizer.Connections;
using ConsoleAppLogAnalizer.Logs;
using ConsoleAppLogAnalizer.Logs.LogCreator;
using ConsoleAppLogAnalizer.Logs.PrimaryLogs;
using ConsoleAppLogAnalizer.SoftwareUsers;
using Npgsql;
using NpgsqlTypes;


namespace ConsoleAppLogAnalizer
{
    class Programm
    {
        static void Main()
        {
            Stopwatch st = new Stopwatch();
            Stopwatch st1 = new Stopwatch();
            Stopwatch st2 = new Stopwatch();
            Stopwatch st3 = new Stopwatch();
            Stopwatch st4 = new Stopwatch();
            Stopwatch st5 = new Stopwatch();
            Stopwatch st6 = new Stopwatch();
            Stopwatch stTotal = new Stopwatch();

            st6.Start();
            SoftwareUser.UpdateUndefinedUsers();
            st6.Stop();

            stTotal.Start();
            st.Start();
            var petexPrimaryLogImporter = new PetexPrimaryLogImporter();
            var newPetexPrimaryLogs = petexPrimaryLogImporter.ImportNewPrimaryLogs();
            petexPrimaryLogImporter.ExportPrimaryLogsToDB(newPetexPrimaryLogs);

            var tNavPrimaryLogImporter = new tNavPrimaryLogImporter();
            var newtNavPrimaryLogs = tNavPrimaryLogImporter.ImportNewPrimaryLogs();
            tNavPrimaryLogImporter.ExportPrimaryLogsToDB(newtNavPrimaryLogs);

            st.Stop();

            st1.Start();
            var unstructuredPetexLogs = petexPrimaryLogImporter.ImportUnstructuredPrimaryLogs();
            var unstructuredtNavLogs = tNavPrimaryLogImporter.ImportUnstructuredPrimaryLogs();
            st1.Stop();

            st2.Start();
            var petexLogCreator = new PetexLogCreator();
            var tNavLogCreator = new TNavLogCreator();
            var newStructuredPetexLogs = petexLogCreator.GetStructuredLogs(unstructuredPetexLogs);
            var newStructuredtNavLogs = tNavLogCreator.GetStructuredLogs(unstructuredtNavLogs);
            st2.Stop();

            st3.Start();
            DataExporter.UpdateDataTable(unstructuredPetexLogs, "id");
            DataExporter.UpdateDataTable(unstructuredtNavLogs, "id");
            st3.Stop();

            st4.Start();
            PetexLogCreator.WriteStructuredLogsToDB(newStructuredPetexLogs);
            TNavLogCreator.WriteStructuredLogsToDB(newStructuredtNavLogs);
            st4.Stop();

            st5.Start();

            var newStructuredLogs = new List<StructuredLog>();
            newStructuredLogs.AddRange(newStructuredPetexLogs);
            newStructuredLogs.AddRange(newStructuredtNavLogs);
            var newSessions = SessionCreator.GetSessionsFromLogs(newStructuredLogs);
            SessionCreator.ExportSoftwareSessionsToDB(newSessions);
            st5.Stop();



            stTotal.Stop();

            Console.WriteLine(st.ElapsedMilliseconds / 60000.0);
            Console.WriteLine(st1.ElapsedMilliseconds / 60000.0);
            Console.WriteLine(st2.ElapsedMilliseconds / 60000.0);
            Console.WriteLine(st3.ElapsedMilliseconds / 60000.0);
            Console.WriteLine(st4.ElapsedMilliseconds / 60000.0);
            Console.WriteLine(st5.ElapsedMilliseconds / 60000.0);
            Console.WriteLine(st6.ElapsedMilliseconds / 60000.0);
            Console.WriteLine(stTotal.ElapsedMilliseconds / 60000.0);


        }
    }
}