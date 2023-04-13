using ConsoleAppLogAnalizer.SoftwareUsers;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppLogAnalizer.Software
{
    public class SoftwareModule
    {
        public SoftwareModule(int softwareModuleID, string softwareModuleName, int resourceID, int softwareID)
        {
            SoftwareModuleID = softwareModuleID;
            SoftwareModuleName = softwareModuleName;
            ResourceID = resourceID;
            SoftwareID = softwareID;
        }
        public SoftwareModule(string softwareModuleName)
        {
            SoftwareModuleName = softwareModuleName;
            ResourceID = 1;
            SoftwareID = 1;
        }
        public int SoftwareModuleID { get; set; }
        public string SoftwareModuleName { get;set; }
        public int ResourceID { get; set; }
        public int SoftwareID { get; set; }

        public static void AddUndefinedModuleToDB(SoftwareModule module)
        {
            var dt = new DataTable().ConstructTable(
                "softwaremodules",
                new string[] 
                { 
                    "softwaremodulename",
                    "resourсeid",
                    "softwareid"
                },
                new Type[] 
                { 
                    typeof(string),
                    typeof(int),
                    typeof(int)
                });
            dt.Rows.Add();
            dt.Rows[0]["softwaremodulename"] = module.SoftwareModuleName;
            dt.Rows[0]["resourсeid"] = module.ResourceID;
            dt.Rows[0]["softwareid"] = module.SoftwareID;

            var importer = new DataExporter(new PostgresConnection());
            importer.ExportDataTable(
                "softwaremodules",
                new string[] 
                {
                    "softwaremodulename",
                    "resourсeid",
                    "softwareid"
                },
                dt);
        }

    }
}
