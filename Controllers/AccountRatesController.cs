using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TMS.Controllers
{
    public class AccountRatesController : Controller
    {
        // GET: /<controller>/
        public IActionResult ManageAccountRates()
        {
            return View();
        }

        public IActionResult CreateAccountRate()
        {
            return View();
        }
    }
}
