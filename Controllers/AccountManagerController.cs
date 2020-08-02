using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TMS.Controllers
{
    public class AccountManagerController : Controller
    {
        [Authorize(Roles = "ADMIN")]
        public IActionResult ManageUsers()
        {
            return View();
        }
		[Authorize(Roles = "ADMIN")]
		public IActionResult UpdateUser()
        {
            return View();
        }
		[Authorize(Roles = "ADMIN")]
		public IActionResult AddUser()
        {
            return View();
        }
    }
}
