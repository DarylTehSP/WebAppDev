using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TMS.Models
{
    public class AccountRate
    {
        public int AccountRateId { get; set; }
        public int CustomerAccountId { get; set; }
        public CustomerAccount CustomerAccount { get; set; }
        public decimal RatePerHour { get; set; }
        public DateTime EffectiveStartDate { get; set; }
        public DateTime EffectiveEndDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public int CreatedById { get; set; }
        public AppUser CreatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int UpdatedById { get; set; }
        public AppUser UpdatedBy { get; set; }
        public List<AccountTimeTable> AccountTimeTables{ get; set; }
    }
}
