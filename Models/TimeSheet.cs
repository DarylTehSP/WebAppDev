using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TMS.Models
{
    public class TimeSheet
    {
        public int TimeSheetId { get; set; }
        public DateTime YearAndMonth { get; set; } //Stores  yyyy-mm-01 (1st of any month or year)
       
        public List<TimeSheetSchedule> TimeSheetSchedules { get; set; }

        //Having InstructorId and Instructor navigation property
        //here is due to the need of many to one relationship between
        //TimeSheet entity type and the Instructor entity type.

        public int InstructorId { get; set; }
        public AppUser Instructor { get; set; }


        public int CreatedById { get; set; }
        public AppUser CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }

        //I felt like removing these 3 properties. After some thought that the admin
        //might step in to make changes on what the instructor has entered. I think I better
        //keep it.
        public int UpdatedById { get; set; }
        public AppUser UpdatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }

        public DateTime? VerifiedAndSubmittedAt { get; set; }
        public int CheckedById { get; set; }

        public AppUser ApprovedBy { get; set; }
        public int? ApprovedById { get; set; }
        public DateTime? ApprovedAt { get; set; }

    }
}
