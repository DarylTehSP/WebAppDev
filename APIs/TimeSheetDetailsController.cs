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

namespace TMS.APIs
{

		public class TimeSheetDetailQueryModelByInstructor
		{
				public int InstructorId { get; set; }
				public int Month { get; set; }
				public int Year { get; set; }
		}
		[Route("api/[controller]")]
    public class TimeSheetDetailsController : Controller
    {
        public IAppDateTimeService _appDateTimeService;
        public ApplicationDbContext Database { get; }

        public TimeSheetDetailsController(IAppDateTimeService appDateTimeService,
            ApplicationDbContext database)
        {

            Database = database;
            _appDateTimeService = appDateTimeService;
        }

        // GET: api/GetTimeSheetAndTimeSheetDetails/
        [HttpGet("GetTimeSheetAndTimeSheetDetails")]
        public IActionResult GetTimeSheetAndTimeSheetDetails(TimeSheetDetailQueryModelByInstructor query)
        {
            List<object> timeSheetDetailList = new List<object>();
            object oneTimeSheetData = null;
            object response;
            var oneTimeSheetQueryResult = Database.TimeSheets
                     .Include(input=>input.Instructor)
                     .Where(input => (input.InstructorId == query.InstructorId) &&
                     (input.YearAndMonth.Month == query.Month) &&
                     (input.YearAndMonth.Year == query.Year)).AsNoTracking().FirstOrDefault();
            if (oneTimeSheetQueryResult == null)
            {
                response = new
                {
                    timeSheet = oneTimeSheetData,
                    timeSheetDetails = timeSheetDetailList
                };

                return new JsonResult(response);
            }
            oneTimeSheetData = new
            {
                timeSheetId = oneTimeSheetQueryResult.TimeSheetId,
                instructorName = oneTimeSheetQueryResult.Instructor.FullName,
                year = oneTimeSheetQueryResult.YearAndMonth.Year,
                month = oneTimeSheetQueryResult.YearAndMonth.Month,
                instructorId = oneTimeSheetQueryResult.InstructorId,
                createdAt = oneTimeSheetQueryResult.CreatedAt,
                updatedAt = oneTimeSheetQueryResult.UpdatedAt,
                approvedAt = oneTimeSheetQueryResult.ApprovedAt
            };
            List<TimeSheetSchedule> timeSheetDetailsQueryResult = new List<TimeSheetSchedule>();
            if (oneTimeSheetQueryResult != null)
            {
                timeSheetDetailsQueryResult = Database.TimeSheetSchedules
                         .Where(input => input.TimeSheetId ==
                                   oneTimeSheetQueryResult.TimeSheetId)
                         .AsNoTracking().ToList<TimeSheetSchedule>();
            }
						//The following block of LINQ code is used for testing purpose to sort the 
						//timesheetdetail information by lesson dates.
						var sortedTimeSheetDetailList = from e in timeSheetDetailsQueryResult
												select new
												{
														timeSheetScheduleId = e.TimeSheetScheduleId,
														dateOfLesson = e.DateOfLesson,
														officialStartTimeInHHMM = e.OfficialStartTimeInHHMM,
                                                    officialEndTimeInMinutes = e.OfficialEndTimeInMinutes,
                                                    actualStartTimeInMinutes = e.ActualStartTimeInMinutes,
                                                    actualEndTimeInMinutes = e.ActualEndTimeInMinutes,
														wageRatePerHour = e.WageRatePerHour,
													    ratePerHour = e.RatePerHour,
                                                        customerAccountName = e.CustomerAccountName,
														sessionSynopsisNames = e.LessonTypeNames
												}
				               into temp
											 orderby temp.dateOfLesson ascending
											 select temp;




						foreach (var oneTimeSheetDetail in timeSheetDetailsQueryResult)
            {
                timeSheetDetailList.Add(new
                {
                    TimeSheetScheduleId = oneTimeSheetDetail.TimeSheetScheduleId,
                    dateOfLesson = oneTimeSheetDetail.DateOfLesson,
                    officialTimeIn = oneTimeSheetDetail.OfficialStartTimeInMinutes,
                    officialTimeOut = oneTimeSheetDetail.OfficialEndTimeInMinutes,
                    actualTimeIn = oneTimeSheetDetail.ActualStartTimeInMinutes,
                    actualTimeOut = oneTimeSheetDetail.ActualEndTimeInMinutes,
                    wageRatePerHour = oneTimeSheetDetail.WageRatePerHour,
                    ratePerHour = oneTimeSheetDetail.RatePerHour,
                    customerAccountName = oneTimeSheetDetail.CustomerAccountName,
                    sessionSynopsisNames = oneTimeSheetDetail.LessonTypeNames
                });
            }//end of foreach loop which builds the timeSheetDetailList List container .
            response = new
            {
                timeSheet = oneTimeSheetData,
                timeSheetDetails = sortedTimeSheetDetailList

						};

            return new JsonResult(response);

        }//End of GetTimeSheetAndTimeSheetDetails


        //PUT api/UpdateSessionSynopsis
        [HttpPut("ApproveTimeSheet/{id}")]
				[ValidateAntiForgeryToken]
				public IActionResult ApproveTimeSheet(int id, IFormCollection inFormData)
				{
						string customMessage = "";
            object oneTimeSheetData = null;
		
						int userInfoId = int.Parse(User.FindFirst("userid").Value);

            TimeSheet oneTimeSheet = Database.TimeSheets.Include(input=>input.Instructor)
								.Where(ts => ts.TimeSheetId == id).Single();
						bool isApproveStatus = bool.Parse(inFormData["isApprovedStatus"]);
						string messagePart = "";
						if (isApproveStatus == true)
						{
								//Update the TimeSheet data's ApprovedAt and ApprovedById information
								oneTimeSheet.ApprovedAt = _appDateTimeService.GetCurrentDateTime();
                oneTimeSheet.ApprovedById = int.Parse(User.FindFirst("userid").Value);
                messagePart = "approved";
                //The client-side logic will need a newly updated Timesheet parent data
                //to manage the interaction at the client side.
                oneTimeSheetData = new
                {
                    timeSheetId = oneTimeSheet.TimeSheetId,
                    instructorName = oneTimeSheet.Instructor.FullName,
                    year = oneTimeSheet.YearAndMonth.Year,
                    month = oneTimeSheet.YearAndMonth.Month,
                    instructorId = oneTimeSheet.InstructorId,
                    createdAt = oneTimeSheet.CreatedAt,
                    updatedAt = oneTimeSheet.UpdatedAt,
                    approvedAt = oneTimeSheet.ApprovedAt
                };
            }
            else{
								//Update the TimeSheet data's ApprovedAt and ApprovedById information
								//to null
								oneTimeSheet.ApprovedAt = null;
								oneTimeSheet.ApprovedById = null;
								messagePart = "pending";
                oneTimeSheetData = new
                {
                    timeSheetId = oneTimeSheet.TimeSheetId,
                    instructorName = oneTimeSheet.Instructor.FullName,
                    year = oneTimeSheet.YearAndMonth.Year,
                    month = oneTimeSheet.YearAndMonth.Month,
                    instructorId = oneTimeSheet.InstructorId,
                    createdAt = oneTimeSheet.CreatedAt,
                    updatedAt = oneTimeSheet.UpdatedAt,
                    approvedAt = oneTimeSheet.ApprovedAt
                };
            }
						try
						{
								Database.TimeSheets.Update(oneTimeSheet);
								Database.SaveChanges();
						}
						catch (Exception ex)
						{
									object httpFailRequestResultMessage = new { message = ex.InnerException.Message };
										//Return a bad http request message to the client
										return BadRequest(httpFailRequestResultMessage);
								
						}//End of try .. catch block on saving data
						 //Construct a custom message for the client
						 //Create a success message anonymous object which has a 
						 //Message member variable (property)
						var successRequestResultMessage = new
						{
								message = String.Concat("Updated the timesheet approved status to ", messagePart),
                                timeSheet = oneTimeSheetData
						};

						//Create a OkObjectResult type instance, httpOkResult.
						//When creating the object, provide the previous message object
						//successRequestResultMessage into it.
						OkObjectResult httpOkResult =
												new OkObjectResult(successRequestResultMessage);
						//Send the OkObjectResult class object back to the client.
						return httpOkResult;
				}//End of ApproveTimeSheet method

				// PUT api/values/5
				[HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
        //ConvertFromMinutesToHHMM
        string ConvertFromMinutesToHHMM(int inMinutes)
        {  //http://stackoverflow.com/questions/13044603/convert-time-span-value-to-format-hhmm-am-pm-using-c-sharp
            TimeSpan timespan = new TimeSpan(00, inMinutes, 00);
            DateTime time = DateTime.Today.Add(timespan);
            string formattedTime = time.ToString("hh:mm tt");
            return formattedTime;
        }//end of ConvertFromMinutesToHHMM


    }//end of Web API controller class
}
