using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TMS.Models
{
    public class TimeSheetScheduleSignature
    {
        public int TimeSheetScheduleSignatureId { get; set; }//primary key, auto-number field property
        public byte[] Signature { get; set; }//property for binary signature image
        public int TimeSheetScheduleId { get; set; }//foreign key property
        public TimeSheetSchedule TimeSheetSchedule { get; set; }//navigation property
    }
}
