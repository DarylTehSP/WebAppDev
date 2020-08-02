using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TMS.Controllers
{
//Authorization at controller level
    [Authorize(Roles = "ADMIN,INSTRUCTOR")]
    public class ExperimentsController : Controller
    {
        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult ExperimentBootStrapSwitch()
        {
            return View();
        }
        public IActionResult ExperimentCheckBoxBehavior()
        {
            return View();
        }
        public IActionResult ExperimentClientSidePaging()
        {
            return View();
        }
		public IActionResult ExperimentStoragePage1()
		{
			return View();
		}
		public IActionResult ExperimentStoragePage2()
		{
			return View();

		}
		public IActionResult ExperimentJQueryCommentsLibrary()
		{
			return View();

		}
	}//End of action controller class
}//End of namespace
