using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TMS.Models
{
    public class AccountTimeTable
    {
        public int AccountTimeTableId { get; set; }
        //Note that, 1 is Sunday, 2 is Monday, 3 is Tuesday and 7 is Saturday.
        public int DayOfWeekNumber { get; set; }
        public int AccountRateId { get; set; }
        public AccountRate AccountRate { get; set; }
        //public int StartTimeInMinutes { get; set; }
        //public int EndTimeInMinutes { get; set; }//http://stackoverflow.com/questions/538739/best-way-to-store-time-hhmm-in-a-database
        //When the instructor needs to create his timesheet for a particular month, the system
        //need to fetch to correct AccountTimeTable entity object which is 
        //applicable (due to AccountTimeTable and AccountRate's effective start and end date) a particular month
        //for the user to "select" to generate the "actual schedule" for that respective month.
        //To create the timesheet data. The server side logic will heavily rely on EffectiveStartDate
        //and EffectiveEndDate
        public DateTime EffectiveStartDateTime { get; set; }
        public DateTime EffectiveEndDateTime { get; set; }

        //There may be special situations that the admin need to set an AccountTimeTable record as "not visible" (false)
        //. By setting IsVisible property to false, the record will not be used by the Instructor to see any dates
        //which can be generated from the respective AccountTimeTable record.
        public bool IsVisible { get; set; } 

        public DateTime CreatedAt { get; set; }
        public int CreatedById { get; set; }
        public AppUser CreatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int UpdatedById { get; set; }
        public AppUser UpdatedBy { get; set; }

    }
}
