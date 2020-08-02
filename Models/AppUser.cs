using System;
using System.Collections.Generic;

namespace TMS.Models
{
    public class AppUser
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int RoleId { get; set; }
        public AppRole Role { get; set; }
        public List<InstructorAccount> InstructorAccounts { get; set; }
        public List<TimeSheet> TimeSheets { get; set; }
    }
}