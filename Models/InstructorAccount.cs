using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TMS.Models
{
    public class InstructorAccount
    {
        public int InstructorAccountId { get; set; }
        public int InstructorId { get; set; }
        public AppUser Instructor { get; set; }
        public int CustomerAccountId { get; set; }
        public CustomerAccount CustomerAccount { get; set; }

        public decimal WageRate { get; set; }

        /* Note: The customer shared that very often instructor will argue with office admin  on issues      
        such as "I cannot see my account to create my timesheet. You did not assign me to the account." 
        The intention for having CreatedAt, CreatedBy and CreatedById is to capture such data inside the 
        system database to assist in dispute investigation*/
        public DateTime CreatedAt { get; set; }
        public int CreatedById { get; set; }
        public AppUser CreatedBy { get; set; }
        
    }
}
