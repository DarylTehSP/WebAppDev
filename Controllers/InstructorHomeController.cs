using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TMS.Controllers
{
    public class InstructorHomeController : Controller
    {
        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult CreateMyTimesheet()
        {
            return View();
        }
        public IActionResult UpdateTimeSheetSchedule(){
            return View();
        }
        public IActionResult ManageMyTimesheet()
        {
            return View();
        }
        public IActionResult PrepareMyTimesheet()
        {
            return View();
        }
    }
}
