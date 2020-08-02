using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TMS.Models
{
    public class TimeSheetSchedule
    {
        public int TimeSheetScheduleId { get; set; }

        public TimeSheetScheduleSignature TimeSheetScheduleSignature { get; set; }
        public int TimeSheetId { get; set; }
        public TimeSheet TimeSheet { get; set; }

        //I have removed the foreign key and navigation property relationship
        //between the TimeSheetDetail and the LessonType.
        public string LessonTypeNames { get; set; }  //This property stores string data such as "Rehearsal, Choreography" etc.
        
        public string CustomerAccountName { get; set; }
        public decimal RatePerHour { get; set; }
        public string OfficialStartTimeInHHMM { get; set; }
        public string OfficialEndTimeInHHMM { get; set; }

        public DateTime DateOfLesson { get; set; }

        public bool IsReplacementInstructor { get; set; }

        //Note, most of the use case scenarios will yield true for IsSystemCreated because the instructor
        //uses the system features to "mass create" most of the timesheet schedule records.
        public bool IsSystemCreated { get; set; }//If the instructor manually creates a schedule record, the value is false.

        //The following properties are mapped to 3 columns to facilitate cleaner SQL queries.
        //The values should be copied from other tables by using code. No foreign key referencing is going to be used.
        public decimal WageRatePerHour { get; set; }  //The value is copied from InstructorAccount when calculations are done.
        public int OfficialStartTimeInMinutes { get; set; } //This value is copied from AccountTimeTable
        public int OfficialEndTimeInMinutes { get; set; } //This value is copied from AccountTimeTable

        //The following two properties allow NULL values. When the record is created, you will notice that the 
        //field values in the database table will be NULL.
        public int? ActualStartTimeInMinutes { get; set; } //To be updated by the instructor after providing the service
        public int? ActualEndTimeInMinutes { get; set; } //To be updated by the instructor after providing the service

        //The following 6 properties are needed because instructor role user can manually create a TimeSheetSchedule record
        public int CreatedById { get; set; }
        public AppUser CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }

        public int UpdatedById { get; set; }
        public AppUser UpdatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }

        public string Status { get; set; }

    }
}
