using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TMS.Models
{
    public class AppNote
    {
        public int AppNoteId { get; set; }
        public string Note { get; set; }
        public DateTime DeadLine { get; set; }
        public DateTime? DoneAt { get; set; }//https://www.dotnetperls.com/nullable-datetime
        public int AppNotePriorityLevelId { get; set; }
        public AppNotePriorityLevel AppNotePriorityLevel { get; set; }

        public DateTime CreatedAt { get; set; }
        public int CreatedById { get; set; }
        public AppUser CreatedBy { get; set; }
    
    }
}
