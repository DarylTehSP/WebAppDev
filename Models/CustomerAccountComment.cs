using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TMS.Models
{
    public class CustomerAccountComment
    {
        public int CustomerAccountCommentId { get; set; }
        public string Comment { get; set; }
        public CustomerAccount CustomerAccount {get;set;}
        public int CustomerAccountId { get; set; }
        public DateTime CreatedAt { get; set; }
        public int CreatedById { get; set; }
        public AppUser CreatedBy { get; set;}
        public int? ParentId { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
