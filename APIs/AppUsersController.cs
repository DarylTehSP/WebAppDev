using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using TMS.Data;
using TMS.Models;
using TMS.Services;
namespace TMS.APIs
{

    // For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

    namespace TMS.APIs
    {
        [Route("api/[controller]")]
        public class AppUsersController : Controller
        {

            //The following one member variable and one readonly property
            //are required for every web api controller
            //class.
            private IAppDateTimeService _appDateTimeService;
            public ApplicationDbContext Database { get; }
            //The following constructor code pattern is required for every Web API
            //controller class.
            public AppUsersController(IAppDateTimeService appDateTimeService,
            ApplicationDbContext database)
            {
                Database = database;
                _appDateTimeService = appDateTimeService;
            }

            [HttpGet("GetAllUserNames")]
            public IActionResult GetAllUserNames()
            {
                List<object> dataList = new List<object>();
                var userNames = Database.AppUsers.ToList();
                foreach (var username in userNames)

                {
                    dataList.Add(new
                    {
                        id = username.Id,
                        name = username.UserName
                    });//end of dataList.Add(...)
                }//end of foreach
                return new JsonResult(dataList);
            }//end of GetAllPriorityLevels web api method
        }//end of API controller class section
    }
 }

