using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TMS.Models
{
    public class CustomerAccount
    {
        public CustomerAccount()
        {  //Require a constructor to initialize this List property first.  
            this.AccountRates = new List<AccountRate>();
            this.Comments = new List<CustomerAccountComment>();
        }

        public int CustomerAccountId { get; set; }

        public string AccountName { get; set; }


        public List<CustomerAccountComment> Comments { get; set; }
        public bool IsVisible { get; set; }
        public DateTime CreatedAt { get; set; }
        public int CreatedById { get; set; }
        public AppUser CreatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int UpdatedById { get; set; }
        public AppUser UpdatedBy { get; set; }

        public List<InstructorAccount> InstructorAccounts { get; set; }
        public List<AccountRate> AccountRates { get; set; }

        // public List<TimeLog> TimeLogs { get; set; }
    }
}
