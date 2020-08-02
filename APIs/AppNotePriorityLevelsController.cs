using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TMS.Data;
using TMS.Services;
namespace TMS.APIs
{
    [Route("api/[controller]")]
    public class AppNotePriorityLevelsController : Controller
    {
        //The following one member variable and one readonly property
        //are required for every web api controller
        //class.
        private IAppDateTimeService _appDateTimeService;
        public ApplicationDbContext Database { get; }
        //The following constructor code pattern is required for every Web API
        //controller class.
        public AppNotePriorityLevelsController(IAppDateTimeService appDateTimeService,
        ApplicationDbContext database)
        {
            Database = database;
            _appDateTimeService = appDateTimeService;
        }
        //****************************************************************
        // GET: /api/appnoteprioritylevels/getallprioritylevels
        //****************************************************************
        [HttpGet("GetAllPriorityLevels")]
        public IActionResult GetAllPriorityLevels()
        {
            List<object> dataList = new List<object>();
            var priorityLevels = Database.AppNotePriorityLevels.ToList();
            foreach (var priorityLevel in priorityLevels)
       
        {
                dataList.Add(new
                {
                    id = priorityLevel.AppNotePriorityLevelId,
                    name = priorityLevel.PriorityLevelName
                });//end of dataList.Add(...)
            }//end of foreach
            return new JsonResult(dataList);
        }//end of GetAllPriorityLevels web api method
    }//end of API controller class section
}//end of namespace