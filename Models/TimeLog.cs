using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace TMS.Models
{
    public class TimeLog
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int ProjectId { get; set; }
        public int HoursWorked { get; set; }
        public int EmployeeId { get; set; }
    }
}