using TMS.Models;
//Need InternshipManagementSystem_V1.Data so that the .NET can find
//the ApplicationDbContext class.
using TMS.Data;
using TMS.Services;
using TMS.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System.Globalization;
using Microsoft.Extensions.Configuration;
using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.Http;
using System.Data.Common;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TMS.APIs
{
    [Authorize(Roles ="ADMIN,INSTRUCTOR")]
    [Route("api/[controller]")]
    public class TimeSheetsController : Controller
    {
        public IAppDateTimeService _appDateTimeService;
        public ApplicationDbContext Database { get; }

        //Create a Constructor, so that the .NET engine can pass in the ApplicationDbContext object
        //which represents the database session.
        public TimeSheetsController(IAppDateTimeService appDateTimeService,
            ApplicationDbContext database)
        {

            Database = database;
            _appDateTimeService = appDateTimeService;
        }
		[HttpGet("GetCustomerAndAccountTimeTableDataByInstructorId")]
		public JsonResult GetCustomerAndAccountTimeTableDataByInstructorId()
		{
			int instructorId = int.Parse(User.FindFirst("userid").Value);

			//Create an List of objects first, records.
			List<object> records = new List<object>();
			//https://stackoverflow.com/questions/2191847/sql-cte-counting-childs-recursion
			//Requires the namespace System.Data.Common
			var now = _appDateTimeService.GetCurrentDateTime();
			string startOfMonthDMY = new DateTime(now.Year, now.Month, 1).ToString("d/MM/yyyy");
			DbCommand cmd = Database.Database.GetDbConnection().CreateCommand();
			var param1 = cmd.CreateParameter();
			param1.ParameterName = "currentMonthInDMY";
			param1.Value = startOfMonthDMY;
			cmd.Parameters.Add(param1);
			var param2 = cmd.CreateParameter();
			param2.ParameterName = "instructorId";
			param2.Value = instructorId;
			cmd.Parameters.Add(param2);
			cmd.Connection.Open();
			cmd.CommandText = "uspGetApplicableCustomerAccountTimeTableDetailsByInstructorId";
			cmd.CommandType = System.Data.CommandType.StoredProcedure;

			DbDataReader dr = cmd.ExecuteReader();
			if (dr.HasRows)
			{
				while (dr.Read())
				{
					records.Add(new
					{
						customerAccountId = dr.GetInt32(12),
						accountName = dr.GetString(13),
						accountVisibility = dr.GetBoolean(14),
						accountRatePerHour = dr.GetDecimal(15),
						accountRateEffectiveStartDate = dr.GetDateTime(0),
						accountRateEffectiveEndDate = dr.GetDateTime(1),
						accountRateId = dr.GetInt32(8),
						accountTimeTableId = dr.GetInt32(9),
						timeTableEffectiveStartDate = dr.GetDateTime(3),
						timeTableEffectiveEndDate = dr.GetDateTime(4),
						timeTableVisibility = dr.GetBoolean(7),
						wageRate = dr.GetDecimal(11),
						startTimeInMinutes = dr[5],
						endTimeInMinutes = dr[6],
						dayOfWeekNumber = dr[2]

					});
				}
			}
			cmd.Connection.Close();
			return new JsonResult(new { currentDateTime = _appDateTimeService.GetCurrentDateTime(), records });
		}

		[HttpPost("CreateNewTimeSheet")]
        public IActionResult CreateNewTimeSheet([FromBody] List<TimeSheetDataEntryDTO> scheduleList)
        {
            TimeSheetSchedule schedule = new TimeSheetSchedule();
            int userId = int.Parse(User.FindFirst("userid").Value); //Get current user id value
            DateTime currentDateTime = _appDateTimeService.GetCurrentDateTime();
            var newTimeSheet = new TimeSheet();
            newTimeSheet.CreatedAt = currentDateTime;
            newTimeSheet.UpdatedAt = currentDateTime;
            newTimeSheet.InstructorId = userId;
            newTimeSheet.YearAndMonth = new DateTime(currentDateTime.Year, currentDateTime.Month, 1);
            newTimeSheet.UpdatedById = userId;
            newTimeSheet.CreatedById = userId;
            newTimeSheet.TimeSheetSchedules = new List<TimeSheetSchedule>();
            foreach (var oneData in scheduleList)
            {

                schedule = new TimeSheetSchedule();
                schedule.CustomerAccountName = oneData.customerAccountName;
                schedule.DateOfLesson = DateTime.ParseExact(oneData.dateOfLesson, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                schedule.OfficialStartTimeInMinutes = oneData.officialStartTimeInMinutes;
                schedule.OfficialEndTimeInMinutes = oneData.officialEndTimeInMinutes;
                schedule.OfficialStartTimeInHHMM = oneData.officialStartTimeInHHMM;
                schedule.OfficialEndTimeInHHMM = oneData.officialEndTimeInHHMM;
                schedule.RatePerHour = oneData.ratePerHour;
                schedule.WageRatePerHour = oneData.wageRatePerHour;
                schedule.IsReplacementInstructor = false;
                schedule.IsSystemCreated = true;
                schedule.LessonTypeNames = "";
                schedule.CreatedAt = currentDateTime;
                schedule.UpdatedAt = currentDateTime;
                schedule.CreatedById = userId;
                schedule.UpdatedById = userId;
                schedule.Status = "NOT UPDATED"; //NOT UPDATED, UPDATED, COMPLETED
                newTimeSheet.TimeSheetSchedules.Add(schedule);
            }//for

            try
            {
                Database.TimeSheets.Add(newTimeSheet);
                Database.SaveChanges();
            }
            catch (Exception ex)
            {
                string customMessage = "";
                if (ex.InnerException.Message.Contains("TimeSheet_UniqueConstraint") == true)
                {
                    customMessage = "Timesheet for " + newTimeSheet.YearAndMonth.Month + "/" + newTimeSheet.YearAndMonth.Year + " already exists.";

                    object httpFailRequestResultMessage = new { message = customMessage };
                    //Return a bad http request message to the client
                    return BadRequest(httpFailRequestResultMessage);
                }
                else
                {
                    return BadRequest(ex.InnerException.Message);
                }
            }
            return Ok(new { message = "Created time sheet" });
        }//CreateNewTimeSheet
        [HttpGet("GetOneTimeSheetSchedule/{id}")]
        public JsonResult GetOneTimeSheetSchedule(int id){
            
            object result = null;
            TimeSheetSchedule oneTimeSheetSchedule = null;
            //The method logic uses the current login user id to obtain the time sheet (based on the given year and month)
            int userId = int.Parse(User.FindFirst("userid").Value); //Get current user id value
           oneTimeSheetSchedule = Database.TimeSheetSchedules.Where(timeSheetSchedule => timeSheetSchedule.TimeSheetScheduleId==id)
                    .Include(timeSheetSchedule => timeSheetSchedule.TimeSheet).SingleOrDefault();
            TimeSheetScheduleSignature oneTimeSheetScheduleSignature = Database.TimeSheetScheduleSignature.Where(signature => signature.TimeSheetScheduleId == id).SingleOrDefault();

            if (oneTimeSheetSchedule != null)
            {
                    TimeSpan span = TimeSpan.FromMinutes(oneTimeSheetSchedule.OfficialStartTimeInMinutes);
                    string startTimeInHHMM = span.ToString(@"hh\:mm");
                    span = TimeSpan.FromMinutes(oneTimeSheetSchedule.OfficialEndTimeInMinutes);
                    string endTimeInHHMM = span.ToString(@"hh\:mm");
                    byte[] signatureImage=new byte[] { };
                    if (oneTimeSheetScheduleSignature!=null){
                    signatureImage = oneTimeSheetScheduleSignature.Signature;
                    }
                //The properties applied in this anonymous object matches the needs of the front-end's update timesheet schedule logic.
                result = (new
                {
                    id = oneTimeSheetSchedule.TimeSheetScheduleId,
                    title = oneTimeSheetSchedule.OfficialStartTimeInHHMM + '~' + oneTimeSheetSchedule.OfficialEndTimeInHHMM + ' ' + oneTimeSheetSchedule.CustomerAccountName,
                    customerAccountName = oneTimeSheetSchedule.CustomerAccountName,
                    officialStartTimeHHMM = startTimeInHHMM,
                    officialEndTimeHHMM = endTimeInHHMM,
                    officialStartTimeInMinutes = oneTimeSheetSchedule.OfficialStartTimeInMinutes,
                    officialEndTimeInMinutes = oneTimeSheetSchedule.OfficialEndTimeInMinutes,
                    actualStartTimeInMinutes = oneTimeSheetSchedule.ActualStartTimeInMinutes,
                    actualEndTimeInMinutes = oneTimeSheetSchedule.ActualEndTimeInMinutes,
                    lessonTypeNames = oneTimeSheetSchedule.LessonTypeNames,
                    status = oneTimeSheetSchedule.Status,
                    isSystemCreated = oneTimeSheetSchedule.IsSystemCreated,
                    isReplacementInstructor = oneTimeSheetSchedule.IsReplacementInstructor,
                    dateOfLesson = oneTimeSheetSchedule.DateOfLesson,
                    ratePerHour = oneTimeSheetSchedule.RatePerHour,
                    signature = signatureImage
                    });
             
            }

            return new JsonResult(result);


        }//GetOneTimeSheetSchedule
        //Calling URL   /API/TimeSheets/GetTimeSheetAndSchedules?year=2019&month=9
        [HttpGet("GetTimeSheetAndSchedules")]
        public JsonResult GetTimeSheetAndSchedules([FromQuery] string year = "", [FromQuery] string month = "")
        {
            DateTime timeSheetDateForSearching;
            object result = null;
            List<object> scheduleList = new List<object>();
            //The method logic uses the current login user id to obtain the time sheet (based on the given year and month)
            int userId = int.Parse(User.FindFirst("userid").Value); //Get current user id value
            if ((year == "") || (month == ""))
            {
                timeSheetDateForSearching = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            }
            else
            {
                timeSheetDateForSearching = new DateTime(int.Parse(year), int.Parse(month), 1);
            }
            TimeSheet oneTimeSheet = Database.TimeSheets.Where(timeSheet => timeSheet.YearAndMonth == timeSheetDateForSearching)
                    .Include(timeSheet => timeSheet.TimeSheetSchedules).Include(timeSheet => timeSheet.Instructor).SingleOrDefault();

            if (oneTimeSheet != null) {
                foreach (var schedule in oneTimeSheet.TimeSheetSchedules)
                {
                    TimeSpan span = TimeSpan.FromMinutes(schedule.OfficialStartTimeInMinutes);
                    string startTimeInHHMM = span.ToString(@"hh\:mm");
                    span = TimeSpan.FromMinutes(schedule.OfficialEndTimeInMinutes);
                    string endTimeInHHMM = span.ToString(@"hh\:mm");
                    //The properties applied in this anonymous object matches the needs of the front-end's tui.calender Schedule class object.
                    scheduleList.Add(new
                    {
                        id = schedule.TimeSheetScheduleId,
                        calendarId = "0",
                        title = schedule.OfficialStartTimeInHHMM + '~' + schedule.OfficialEndTimeInHHMM + ' ' + schedule.CustomerAccountName,
                        category = "time",
                        start = schedule.DateOfLesson.ToString("yyyy-MM-dd") + "T" + startTimeInHHMM,
                        end = schedule.DateOfLesson.ToString("yyyy-MM-dd") + "T" + endTimeInHHMM,
                        isReadOnly = true,
                        isAllDay = false
                    });
                }//foreach loop block
                 //After building up the scheduleList, build the final result object's content
                 //Assign the scheduleList property with the populated scheduleList object 
                result = new
                {
                    timeSheetId = oneTimeSheet.TimeSheetId,
                    instructorFullName = oneTimeSheet.Instructor.FullName,
                    createdAt = oneTimeSheet.CreatedAt,
                    year = oneTimeSheet.YearAndMonth.Year,
                    month = oneTimeSheet.YearAndMonth.Month,
                    scheduleList = scheduleList,
                    numOfTimeSheetDetails = oneTimeSheet.TimeSheetSchedules.Count
                };
            }

            return new JsonResult(result);
        }//end of GetTimeSheetAndSchedules()

        //PUT api/UpdateTimeSheetSchedule
        [HttpPut("UpdateTimeSheetSchedule")]
        public IActionResult UpdateTimeSheetSchedule(IFormCollection inFormData)
        {
            //Obtain the id value to search for the correct TimeSheetSchedule entity
            long timeSheetScheduleId = Int64.Parse(inFormData["timeSheetScheduleId"]);

            //Obtain the user id of the user who has logon
            int userId = int.Parse(User.FindFirst("userid").Value); //Get current user id value
          

            var oneSchedule = Database.TimeSheetSchedules
      .Where(input => input.TimeSheetScheduleId == timeSheetScheduleId)
      .Single();

            oneSchedule.UpdatedAt = _appDateTimeService.GetCurrentDateTime();
            oneSchedule.UpdatedById = userId;
            oneSchedule.Status = "COMPLETED";
            oneSchedule.LessonTypeNames = inFormData["lessonTypeNames"];
            oneSchedule.TimeSheetScheduleSignature = new TimeSheetScheduleSignature();
            oneSchedule.TimeSheetScheduleSignature.TimeSheetScheduleId = oneSchedule.TimeSheetId;
            oneSchedule.TimeSheetScheduleSignature.Signature = Convert.FromBase64String(inFormData["signatureData"]);
            oneSchedule.ActualStartTimeInMinutes = int.Parse(inFormData["actualStartTimeInMinutes"]);
            oneSchedule.ActualEndTimeInMinutes = int.Parse(inFormData["actualEndTimeInMinutes"]);
            try
            {
                Database.TimeSheetSchedules.Update(oneSchedule);

                Database.SaveChanges();
            }
            catch (Exception ex)
            {
                object httpFailRequestResultMessage = new { message = ex.InnerException.Message };
                //Return a bad http request message to the client
                return BadRequest(httpFailRequestResultMessage);

            }//End of try .. catch block on update data
             //Construct a custom message for the client
             //Create a success message anonymous object which has a 
             //Message member variable (property)
            var successRequestResultMessage = new
            {
                message = "Updated timesheet data."
            };

            //Create a OkObjectResult class instance, httpOkResult.
            //When creating the object, provide the previous message object into it.
            OkObjectResult httpOkResult =
                        new OkObjectResult(successRequestResultMessage);
            //Send the OkObjectResult class object back to the client.
            return httpOkResult;
        }//End of CreateTimeInTimeOutDataSignature() method

        [Serializable]
        public class TimeSheetDataEntryDTO
        {
            public int id { get; set; }
            public string officialStartTimeInHHMM { get; set; }
            public string officialEndTimeInHHMM { get; set; }
            public string customerAccountName { get; set; }
            public int officialStartTimeInMinutes { get; set; }
            public int officialEndTimeInMinutes { get; set; }
            public decimal ratePerHour { get; set; }
            public decimal wageRatePerHour { get; set; }
            public string dateOfLesson { get; set; }
        }
    }
}
