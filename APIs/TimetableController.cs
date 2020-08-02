using System;
using System.Collections;
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

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TMS.APIs
{
    [Route("api/[controller]")]
    public class TimetableController : Controller
    {
        //The following one member variable and one readonly property
        //are required for every web api controller class.
        private IAppDateTimeService _appDateTimeService;
        public ApplicationDbContext Database { get; }
        //The following constructor code pattern is required for every Web API
        //controller class.
        public TimetableController(IAppDateTimeService appDateTimeService,
        ApplicationDbContext database)
        {
            Database = database;
            _appDateTimeService = appDateTimeService;
        }

        //To retrieve all the account rate based on the id given
        [Authorize("ADMIN")]
        [HttpGet("GetCustomerAccountTimetablePageByPage/{id}")]
        public JsonResult GetCustomerAccountTimetablePageByPage(int id,
         [FromQuery]QueryPagingParametersForAccountTimetable inParameters)
        {
            int pageSize = 10;
            int totalPage = 0;
            int startRecord = 0;
            int endRecord = 0;
            int currentPage = 0;

            List<object> recordList = new List<object>();
            int totalRecords = 0;
            if (ModelState.IsValid)
            {
                currentPage = Int32.Parse(inParameters.page_number.ToString());
                pageSize = Int32.Parse(inParameters.per_page.ToString());
            }
            else
            {
                currentPage = 1;
                pageSize = 10;
            }
            if (currentPage == 1)
            {
                startRecord = 1;
            }
            else
            {
                startRecord = (currentPage * pageSize) + 1;
            }
            endRecord = pageSize * currentPage;
            //To use DbCommand class, you need the namespace System.Data.Common
            //Create a new DbCommand type object
            DbCommand cmd = Database.Database.GetDbConnection().CreateCommand();
            //Tell the DbCommand object, cmd to open a connection to db
            cmd.Connection.Open();
            //Pass the SQL to the DbCommand type object, cmd.
            //Let the DbCommand type object cmd know that this is a stored procedure.
            cmd.CommandText = "dbo.uspSelectCustomerAccountTimetable";
            //Tell the DbCommand object, cmd that this is a stored procedure.
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            //Pass the page number value to the stored procedure's @pageNo parameter
            DbParameter parameter = cmd.CreateParameter();
            parameter.DbType = System.Data.DbType.Int32;
            parameter.ParameterName = "pageNo";
            parameter.Value = currentPage;
            cmd.Parameters.Add(parameter);
            //Pass the page size value to the stored procedure's @pageSize parameter
            parameter = cmd.CreateParameter();
            parameter.DbType = System.Data.DbType.Int32;
            parameter.ParameterName = "pageSize";
            parameter.Value = pageSize;
            cmd.Parameters.Add(parameter);

            // The above commands will "build-up" one SQL command such as:
            // EXEC dbo.uspSelectNotes @userId=1,@pageNo =1,
            // @sortColumn='DONEAT , @sortOrder='ASC',
            // @pageSize = 5
            //Hard code it here because not getting it from the client-side
            DbDataReader dr = cmd.ExecuteReader();//This is the part where SQL is sent to DB
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    if (id == int.Parse(dr["AccountRateId"].ToString()))
                    {
                        //Get each column values
                        int rowNumber = int.Parse(dr["ROWNUM"].ToString());
                        DateTime startDate = Convert.ToDateTime(dr["EffectiveStartDateTime"].ToString());
                        DateTime endDate = Convert.ToDateTime(dr["EffectiveEndDateTime"].ToString());
                        int accountRateId = int.Parse(dr["AccountRateId"].ToString());
                        decimal accountRate = Database.AccountRates.Single(input => input.AccountRateId == accountRateId).RatePerHour;
                        int customerAccountId = Database.AccountRates.Single(input => input.AccountRateId == accountRateId).CustomerAccountId;
                        string customerName = Database.CustomerAccounts.Single(input => input.CustomerAccountId == customerAccountId).AccountName;
                        bool isVisible = bool.Parse(dr["IsVisible"].ToString());
                        int accountTimeTableId = int.Parse(dr["AccountTimetableId"].ToString());
                        string day = "";
                        int dayNum = int.Parse(dr["DayOfWeekNumber"].ToString());
                        if (dayNum == 1)
                        {
                            day = "Monday";
                        }
                        else if (dayNum == 2)
                        {
                            day = "Tuesday";
                        }
                        else if (dayNum == 3)
                        {
                            day = "Wednesday";
                        }
                        else if (dayNum == 4)
                        {
                            day = "Thursday";
                        }
                        else if (dayNum == 5)
                        {
                            day = "Friday";
                        }
                        else if (dayNum == 6)
                        {
                            day = "Saturday";
                        }
                        else
                        {
                            day = "Sunday";
                        }
                        //Create an anonymous object and at the same time
                        //add it into the recordList collection
                        recordList.Add(new
                        {
                            rowNumber = rowNumber,
                            accountTimetableId = accountTimeTableId,
                            accountRateId = accountRateId,
                            customerAccountId = customerAccountId,
                            accountRate = accountRate,
                            customerName = customerName,
                            isVisible = isVisible,
                            startDate = startDate,
                            endDate = endDate,
                            day = day
                        });
                    }

                }
            }

            cmd.Connection.Close();
            totalPage = (int)Math.Ceiling((double)totalRecords / pageSize);
            object finalResult = new object();
            string nextPageUrl = "";
            string prevPageUrl = "";
            if (currentPage == 1)
            {
                prevPageUrl = null;
                nextPageUrl = "/API/Timetable/GetCustomerAccountTimetablePageByPage/" + id + "?per_page=" +
                pageSize.ToString() + "&page_number=" + (currentPage + 1);
            }
            else
            {
                prevPageUrl = "/API/Timetable/GetCustomerAccountTimetablePageByPage/" + id + "?per_page=" +
                pageSize.ToString() + "&page_number=" + (currentPage - 1);
                if (currentPage == totalPage)
                {
                    nextPageUrl = null;
                }
                else
                {
                    nextPageUrl = "/API/Timetable/GetCustomerAccountTimetablePageByPage/" + id + "per_page=" +
                    pageSize.ToString() + "&page=" + (currentPage + 1);
                }
            }
            finalResult = new
            {
                total = recordList.Count,
                current_page = currentPage,
                per_page = pageSize,
                last_page = totalPage, /* Used by the client-side to generate page no buttons */
                next_page_url = nextPageUrl,
                prev_page_url = prevPageUrl,
                records = recordList,
                from = startRecord,
                to = endRecord
            };
            return new JsonResult(finalResult);
        }

        //Create a new customer account timetable with its customer id
        [Authorize("ADMIN")]
        [HttpPost("CreateNewAccountTimetable/{id}")]
        public IActionResult CreateNewAccountTimetable(int id, [FromForm]IFormCollection webFormData)
        {

            int userId = int.Parse(User.FindFirst("userid").Value);
            AccountTimeTable accountTimeTable = new AccountTimeTable();

            try
            {

                accountTimeTable.DayOfWeekNumber = int.Parse(webFormData["dayNum"]);
                accountTimeTable.EffectiveStartDateTime = DateTime.ParseExact(webFormData["startDateAndTime"], "d/M/yyyy HH:mm", CultureInfo.InvariantCulture);
                accountTimeTable.EffectiveEndDateTime = DateTime.ParseExact(webFormData["endDateAndTime"], "d/M/yyyy HH:mm", CultureInfo.InvariantCulture);
                accountTimeTable.IsVisible = bool.Parse(webFormData["isVisible"]);
                accountTimeTable.CreatedById = userId;
                accountTimeTable.UpdatedById = userId;
                accountTimeTable.CreatedAt = _appDateTimeService.GetCurrentDateTime();
                accountTimeTable.UpdatedAt = _appDateTimeService.GetCurrentDateTime();
                accountTimeTable.AccountRateId = id;

                var checkIdentical = Database.AccountTimeTable.FromSql($"uspAccountTimeTableCheckIdenticalBeforeAdd { accountTimeTable.AccountRateId},{accountTimeTable.DayOfWeekNumber},{ accountTimeTable.EffectiveStartDateTime} , { accountTimeTable.EffectiveEndDateTime}").ToList();
                var checkOverlap = Database.AccountTimeTable.FromSql($"uspAccountTimeTableOverlapBeforeAdd { accountTimeTable.EffectiveStartDateTime},{accountTimeTable.EffectiveEndDateTime},{ accountTimeTable.AccountRateId} , { accountTimeTable.DayOfWeekNumber}").ToList();
                if (checkIdentical.Count > 0)
                {
                    AccountTimeTable idd = new AccountTimeTable();

                    foreach (var a in checkIdentical)
                    {
                        var response = new
                        {
                            day = a.DayOfWeekNumber,
                            startDateTime = a.EffectiveStartDateTime,
                            endDateTime = a.EffectiveEndDateTime,
                            visibility = a.IsVisible

                        };
                        if (response != null)
                        {
                            return BadRequest(new { record = response, message = "Identical" });
                        }
                    }
                }
                if (checkOverlap.Count > 0)
                {
                    AccountTimeTable idd = new AccountTimeTable();

                    foreach (var a in checkOverlap)
                    {
                        var response = new
                        {
                            day = a.DayOfWeekNumber,
                            startDateTime = a.EffectiveStartDateTime,
                            endDateTime = a.EffectiveEndDateTime,
                            visibility = a.IsVisible

                        };
                        if (response != null)
                        {
                            return BadRequest(new { record = response, message = "Overlap" });
                        }
                    }

                }
                else
                {
                    Database.AccountTimeTable.Add(accountTimeTable);
                    Database.SaveChanges();
                }

            }
            catch (Exception exceptionObject)
            {
                return BadRequest(exceptionObject.InnerException.Message);
            }

            return Ok(new
            {
                message = "Saved account timetable."
            });


        }

        //Extract time table from database
        [Authorize("ADMIN")]
        [HttpGet("GetAccountTimetable/{id}")]
        public IActionResult GetCustomerTimeTable(int id)
        {
            var oneCustomerTimeTable = Database.AccountTimeTable.SingleOrDefault(input => input.AccountTimeTableId == id);
            DateTime effectveStartDateTime = oneCustomerTimeTable.EffectiveStartDateTime;
            DateTime effectveEndDateTime = oneCustomerTimeTable.EffectiveEndDateTime;
            var startDate = effectveStartDateTime.ToString("dd/MM/yyyy");
            var endDate = effectveEndDateTime.ToString("dd/MM/yyyy");
            var startTime = effectveStartDateTime.ToString("HH:mm");
            var endTime = effectveEndDateTime.ToString("HH:mm");
            if (oneCustomerTimeTable == null)
            {
                return BadRequest(new { message = "Unable to retrieve customer account time table" });
            }
            else
            {
                var response = new
                {
                    day = oneCustomerTimeTable.DayOfWeekNumber,
                    startDate = startDate,
                    startTime = startTime,
                    endDate = endDate,
                    endTime = endTime,
                    visibility = oneCustomerTimeTable.IsVisible

                };
                return Ok(response);
            }
        }
        //To update account rate based on its id
        [Authorize("ADMIN")]
        [HttpPut("UpdateAccountTimetable/{id}")]
        public IActionResult UpdateAccountTimetable(int id, [FromForm]IFormCollection webFormData)
        {
            AccountTimeTable foundOneAccountTimetable = Database.AccountTimeTable.Single(input => input.AccountTimeTableId == id);
            int userId = int.Parse(User.FindFirst("userid").Value);
            foundOneAccountTimetable.UpdatedById = userId;
            foundOneAccountTimetable.UpdatedAt = _appDateTimeService.GetCurrentDateTime();
            foundOneAccountTimetable.DayOfWeekNumber = int.Parse(webFormData["dayOfNum"]);
            foundOneAccountTimetable.EffectiveStartDateTime = DateTime.ParseExact(webFormData["startDateAndTime"], "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
            foundOneAccountTimetable.EffectiveEndDateTime = DateTime.ParseExact(webFormData["endDateAndTime"], "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
            foundOneAccountTimetable.IsVisible = bool.Parse(webFormData["isVisible"]);

            var checkIdentical = Database.AccountTimeTable.FromSql($"uspAccountTimeTableCheckIdenticalBeforeUpdate { foundOneAccountTimetable.AccountRateId},{foundOneAccountTimetable.DayOfWeekNumber},{ foundOneAccountTimetable.EffectiveStartDateTime} , { foundOneAccountTimetable.EffectiveEndDateTime}").ToList();
            var checkOverlap = Database.AccountTimeTable.FromSql($"uspAccountTimeTableOverlapBeforeUpdate  { foundOneAccountTimetable.EffectiveStartDateTime},{foundOneAccountTimetable.EffectiveEndDateTime},{ foundOneAccountTimetable.AccountRateId} , { foundOneAccountTimetable.DayOfWeekNumber}").ToList();

            try
            {
                if (checkIdentical.Count > 0)
                {
                    AccountTimeTable idd = new AccountTimeTable();

                    foreach (var a in checkIdentical)
                    {
                        var response = new
                        {
                            day = a.DayOfWeekNumber,
                            startDateTime = a.EffectiveStartDateTime,
                            endDateTime = a.EffectiveEndDateTime,
                            visibility = a.IsVisible

                        };

                        return BadRequest(new { record = response, message = "Identical" });

                    }
                }
                else if (checkOverlap.Count > 0)
                {
                    AccountTimeTable idd = new AccountTimeTable();

                    foreach (var a in checkOverlap)
                    {
                        var response = new
                        {
                            day = a.DayOfWeekNumber,
                            startDateTime = a.EffectiveStartDateTime,
                            endDateTime = a.EffectiveEndDateTime,
                            visibility = a.IsVisible

                        };

                        return BadRequest(new { record = response, message = "Overlap" });

                    }
                }
                else
                {
                    Database.AccountTimeTable.Update(foundOneAccountTimetable);
                    Database.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Unable to update account timetable" });
            }
            return Ok(new
            {
                message = "Updated customer account timetable"
            });
        }

       

        //Create timetable summary

        [Authorize("ADMIN")]
        [HttpGet("GetCustomerTimetableSummary/{id}")]
        public IActionResult GetCustomerAccountSummary(int id)
        {
            var dayNum = Database.AccountTimeTable.Single(input => input.AccountTimeTableId == id).DayOfWeekNumber;
            DateTime startDateTime = Database.AccountTimeTable.Single(input => input.AccountTimeTableId == id).EffectiveStartDateTime;
            DateTime endDateTime = Database.AccountTimeTable.Single(input => input.AccountTimeTableId == id).EffectiveEndDateTime;
            var startTime = startDateTime.ToString("HH:mm");
            var endTime = endDateTime.ToString("HH:mm");
            var startDate = startDateTime.ToString("dd/MM/yyyy");
            var endDate = endDateTime.ToString("dd/MM/yyyy");
            string day = "";
            if (dayNum == 1)
            {
                day = "Monday";
            }
            else if (dayNum == 2)
            {
                day = "Tuesday";
            }
            else if (dayNum == 3)
            {
                day = "Wednesday";
            }
            else if (dayNum == 4)
            {
                day = "Thursday";
            }
            else if (dayNum == 5)
            {
                day = "Friday";
            }
            else if (dayNum == 6)
            {
                day = "Saturday";
            }
            else
            {
                day = "Sunday";
            }

            var response = new
            {
                day = day,
                startTime = startTime,
                endTime = endTime,
                startDate = startDate,
                endDate = endDate
            };
            return Ok(response);

        }


        //Delete timetable
        [Authorize("ADMIN")]
        [HttpDelete("DeleteTimetable/{id}")]
        public IActionResult DeleteTimetable(int id, [FromForm]IFormCollection webFormData)
        {
            try
            {
                AccountTimeTable foundOneAccountTimetable = Database.AccountTimeTable.Single(input => input.AccountTimeTableId == id);

                Database.AccountTimeTable.Remove(foundOneAccountTimetable);
                Database.SaveChanges();

            }
            catch (Exception exceptionObject)
            {
                return BadRequest(exceptionObject.InnerException.Message);
            }
            return Ok(new
            {
                message = "Deleted customer account timetable"
            });
        }
        public class QueryPagingParametersForAccountTimetable
        {
            [BindRequired]
            public int page_number { get; set; }
            public int per_page { get; set; }
        }
    }
}
