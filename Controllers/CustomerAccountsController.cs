using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TMS.Controllers
{
  public class CustomerAccountsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create()
        {
            return View();
        }

        public IActionResult ManageAccountRates()
        {
            return View();
        }

        public IActionResult ManageCustomerComments()
        {
            return View();

        }

        public IActionResult AssignInstructors()
        {
            return View();
        }

        public IActionResult ManageInstructorAssignment()
        {
            return View();
        }
    }
}
