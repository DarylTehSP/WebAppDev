using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TMS.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TMS.APIs
{
    [Route("api/[controller]")]
    public class DateTimeSettingsController : Controller
    {
        public IAppDateTimeService _appDateTimeService;
        public DateTimeSettingsController(IAppDateTimeService appDateTimeService)
        {
            _appDateTimeService = appDateTimeService;
        }

        // POST api/<controller>/SetupActualDateTime
        [HttpPost("SetupActualDateTime")]
        public IActionResult SetupActualDateTime([FromForm]IFormCollection webFormData)
        {
            int month = int.Parse(webFormData["month"]);
            int year = int.Parse(webFormData["year"]);
            int day = int.Parse(webFormData["day"]);
            int hour = int.Parse(webFormData["hour"]);
            int minute = int.Parse(webFormData["minute"]);
            try
            {
                _appDateTimeService.SetCurrentDateTime("actual", DateTime.Now);
            }
            catch (Exception exceptionObject)
            {

                var customMessage = "Runtime error has occurred to set up a actual current date time";
                object httpFailRequestResultMessage = new { message = customMessage };
                //Return a bad http request message to the client
                return BadRequest(httpFailRequestResultMessage);


            }//End of Try..Catch block
            var successRequestResultMessage = new
            {
                message = $"Manage to set the current date time to {_appDateTimeService.GetCurrentDateTime()}"
            };
            return Ok(successRequestResultMessage);
        }//End of Post method (SetupActualDateTime)
        // POST api/<controller>/MockupDateTimeForTesting
        [HttpPost("MockupDateTimeForTesting")]
        public IActionResult MockupDateTimeForTesting([FromForm]IFormCollection webFormData)
        {
            int month = int.Parse(webFormData["month"]);
            int year = int.Parse(webFormData["year"]);
            int day = int.Parse(webFormData["day"]);
            int hour = int.Parse(webFormData["hour"]);
            int minute = int.Parse(webFormData["minute"]);
            try
            {
                _appDateTimeService.SetCurrentDateTime("mock", new DateTime(year, month, day, hour, minute, 0));
            }
            catch (Exception exceptionObject)
            {

                var customMessage = "Runtime error has occurred to set up a testing mockup current date time";
                object httpFailRequestResultMessage = new { message = customMessage };
                //Return a bad http request message to the client
                return BadRequest(httpFailRequestResultMessage);


            }//End of Try..Catch block
            var successRequestResultMessage = new
            {
                message = $"Manage to set the current date time to {_appDateTimeService.GetCurrentDateTime()}"
            };
            return Ok(successRequestResultMessage);
        }//End of Post method (MockupDateTimeForTesting)

        [HttpGet("GetSystemDateTime")]
        public JsonResult GetSystemDateTime()
        {
            return new JsonResult(new { currentDateTime = _appDateTimeService.GetCurrentDateTime() });
        }


    }
}
