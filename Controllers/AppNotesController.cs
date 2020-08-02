using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TMS.Controllers
{
    public class AppNotesController : Controller
    {
        // GET: api/<controller>
        public IActionResult Index()
        {
            return View();
        }//end of Index action method

        public IActionResult CreateOneNote()
        {
            return View();
        }//end of CreateOneNote action method


    }//end of action controller class
}//end of namespace
