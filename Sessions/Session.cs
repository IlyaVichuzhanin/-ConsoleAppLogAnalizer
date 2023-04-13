using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppLogAnalizer.Sessions
{
    public class Session                         //<T> where T : Log
    {
        //public Session() { }
        public Session(int? softwareUserID, int? employeeID, int? softwareId, int? softwareModuleId, DateTime logInDateTime, DateTime logOutDateTime, bool sessionIsFinished)
        {
            SoftwareUserID = softwareUserID;
            EmployeeID = employeeID;
            SoftwareId = softwareId;
            SoftwareModuleId = softwareModuleId;
            LogInDateTime = logInDateTime;
            LogOutDateTime = logOutDateTime;
            SessionIsFinished= sessionIsFinished;
        }
        public int? SoftwareUserID { get; set; }
        public int? EmployeeID{ get; set; }
        public int? SoftwareId { get; set; }
        public DateTime LogInDateTime { get; set; }
        public DateTime LogOutDateTime { get; set; }
        public int? SoftwareModuleId { get; set; }
        public bool SessionIsFinished { get; set; }
        public double sessionTime
        {
            get { return (LogOutDateTime - LogInDateTime).TotalSeconds; }
        }
    }
}
