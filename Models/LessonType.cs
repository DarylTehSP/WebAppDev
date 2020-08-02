using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TMS.Models
{
    public class LessonType
    {
        public int LessonTypeId { get; set; }
        public string LessonTypeName { get; set; }
        //The instructor view functionality logic will check this property to decide
        //whether to display the lesson type to the user (instructor role user) to choose.
        public bool IsVisible { get; set; }
        public DateTime CreatedAt { get; set; }
        public int CreatedById { get; set; }
        public AppUser CreatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int UpdatedById { get; set; }
        public AppUser UpdatedBy { get; set; }

    }
}
