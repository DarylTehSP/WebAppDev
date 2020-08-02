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

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TMS.APIs
{
    [Route("api/[controller]")]
    public class AccountRatesController : Controller
    {
        //The following one member variable and one readonly property
        //are required for every web api controller class.
        private IAppDateTimeService _appDateTimeService;
        public ApplicationDbContext Database { get; }
        //The following constructor code pattern is required for every Web API
        //controller class.
        public AccountRatesController(IAppDateTimeService appDateTimeService,
        ApplicationDbContext database)
        {
            Database = database;
            _appDateTimeService = appDateTimeService;
        }
        //Create a new customer account rate with its customer id
        [Authorize("ADMIN")]
        [HttpPost("CreateNewAccountRate/{id}")]
        public IActionResult CreateNewAccountRate(int id, [FromForm]IFormCollection webFormData)
        {

            int userId = int.Parse(User.FindFirst("userid").Value);
            AccountRate accountRate = new AccountRate();

            try
            {
                accountRate.CreatedById = userId;
                accountRate.RatePerHour = decimal.Parse(webFormData["accountRate"]);
                accountRate.EffectiveStartDate = DateTime.ParseExact(webFormData["startDate"], "dd/MM/yyyy", CultureInfo.InvariantCulture);
                accountRate.EffectiveEndDate = DateTime.ParseExact(webFormData["endDate"], "dd/MM/yyyy", CultureInfo.InvariantCulture);
                accountRate.CustomerAccountId = id;
                accountRate.CreatedAt = _appDateTimeService.GetCurrentDateTime();
                accountRate.UpdatedAt = _appDateTimeService.GetCurrentDateTime();
                accountRate.UpdatedById = userId;
                //To validate the account rate and date input based on the scenario
                var checkDateOverlapping = Database.AccountRates.FromSql($"uspCheckOverlapBeforeCreate { accountRate.CustomerAccountId},{ accountRate.EffectiveStartDate} , { accountRate.EffectiveEndDate}").ToList();
                var checkContinuous = Database.AccountRates.FromSql($"uspCheckAccountRateBeforeCreate {accountRate.RatePerHour}, {accountRate.CustomerAccountId},{ accountRate.EffectiveStartDate}").ToList();

                if (checkDateOverlapping.Count > 0)
                {
                    return BadRequest(new { message = "Overlaps with existing rate dates!" });
                }
                if(checkContinuous.Count > 0)
                {
                    return BadRequest(new { message = "You can update the effective end date in the existing account rate!" });
                }


                Database.AccountRates.Add(accountRate);
                Database.SaveChanges();

            }
            catch (Exception exceptionObject)
            {
                return BadRequest(exceptionObject.InnerException.Message);
            }
            return Ok(new
            {
                message = "Saved account rate."
            });


        }

        //Extract customer account rate based on its id
        [Authorize("ADMIN")]
        [HttpGet("GetCustomerAccountRate/{id}")]
        public IActionResult GetCustomerAccountRate(int id)
        {
            var oneCustomerAccountRate = Database.AccountRates.SingleOrDefault(input => input.AccountRateId == id);
            if (oneCustomerAccountRate == null)
            {
                return BadRequest(new { message = "Unable to retrieve customer account rate" });
            }
            else
            {
                var response = new
                {
                    accountRate = oneCustomerAccountRate.RatePerHour,
                    effectiveStartDate = oneCustomerAccountRate.EffectiveStartDate,
                    effectiveEndDate = oneCustomerAccountRate.EffectiveEndDate

                };
                return Ok(response);
            }
        }

        //Extract time table from database
        [Authorize("ADMIN")]
        [HttpGet("GetCustomerTimeTable/{id}")]
        public IActionResult GetCustomerTimeTable(int id)
        {
            var oneCustomerTimeTable = Database.AccountTimeTable.SingleOrDefault(input => input.AccountRateId == id);
            if (oneCustomerTimeTable == null)
            {
                return BadRequest(new { message = "Unable to retrieve customer account time table" });
            }
            else
            {
                var response = new
                {
                    day = oneCustomerTimeTable.DayOfWeekNumber,
                    effectveStartDate = oneCustomerTimeTable.EffectiveStartDateTime,
                    effectveEndDate = oneCustomerTimeTable.EffectiveEndDateTime,
                    visibility = oneCustomerTimeTable.IsVisible

                };
                return Ok(response);
            }
        }

        //To update account rate based on its id
        [Authorize("ADMIN")]
        [HttpPut("UpdateAccountRates/{id}")]
        public IActionResult UpdateAccountRate(int id, [FromForm]IFormCollection webFormData)
        {
            AccountRate foundOneAccountRate = Database.AccountRates.Single(input => input.AccountRateId == id);
            int userId = int.Parse(User.FindFirst("userid").Value);
            foundOneAccountRate.UpdatedById = userId;
            foundOneAccountRate.UpdatedAt = _appDateTimeService.GetCurrentDateTime();
            foundOneAccountRate.RatePerHour = decimal.Parse(webFormData["accountRate"]);
            foundOneAccountRate.EffectiveStartDate = DateTime.ParseExact(webFormData["startDate"], "dd/MM/yyyy", CultureInfo.InvariantCulture);
            foundOneAccountRate.EffectiveEndDate = DateTime.ParseExact(webFormData["endDate"], "dd/MM/yyyy", CultureInfo.InvariantCulture);
            //To validate the account rate and date input based on the scenario
            var checkDateOverlapping = Database.AccountRates.FromSql($"uspCheckOverlapBeforeUpdate {foundOneAccountRate.AccountRateId}, {foundOneAccountRate.CustomerAccountId},{foundOneAccountRate.EffectiveStartDate} , {foundOneAccountRate.EffectiveEndDate}").ToList();
            var checkNonContinuous = Database.AccountRates.FromSql($"uspCheckAccountRateBeforeUpdate {foundOneAccountRate.AccountRateId}, {foundOneAccountRate.CustomerAccountId},{foundOneAccountRate.EffectiveStartDate},{foundOneAccountRate.EffectiveEndDate}, {foundOneAccountRate.RatePerHour}").ToList();

            try
            {
                if (checkDateOverlapping.Count > 0)
                {
                    return BadRequest(new { message = "Overlaps with existing rate dates!" });
                }
                if (checkNonContinuous.Count > 0)
                {
                    return BadRequest(new { message = "You should create a new account rate instead of updating!" });
                }
                Database.AccountRates.Update(foundOneAccountRate);
                Database.SaveChanges();
            }
            catch (Exception exceptionObject)
            {
                return BadRequest(new { message = "Unable to update customer account rate"});
            }
            return Ok(new
            {
                message = "Updated customer account rate"
            });
        }

        //To delete the account rate
        [Authorize("ADMIN")]
        [HttpDelete("DeleteAccountRate/{id}")]
        public IActionResult DeleteAccountRate(int id, [FromForm]IFormCollection webFormData)
        {
            try
            {
                AccountRate foundOneAccountRate = Database.AccountRates.Single(input => input.AccountRateId == id);

                Database.AccountRates.Remove(foundOneAccountRate);
                Database.SaveChanges();

            }
            catch (Exception exceptionObject)
            {
                return BadRequest(exceptionObject.InnerException.Message);
            }
            return Ok(new
            {
                message = "Deleted customer account rate"
            });
        }

        //To retrieve all the account rate based on the id given
        [Authorize("ADMIN")]
        [HttpGet("GetCustomerAccountRatesPageByPage/{id}")]
        public JsonResult GetCustomerAccountRatesPageByPage(int id,
         [FromQuery]QueryPagingParametersForAccountRates inParameters)
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
            cmd.CommandText = "dbo.uspSelectCustomerAccountRates";
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
                    if (id == int.Parse(dr["CustomerAccountId"].ToString()))
                    {
                        //Get each column values
                        int rowNumber = int.Parse(dr["ROWNUM"].ToString());
                        int customerAccountId = int.Parse(dr["CustomerAccountId"].ToString());
                        int accountRateId = int.Parse(dr["AccountRateId"].ToString());
                        string customerAccountName = Database.CustomerAccounts.Single(input => input.CustomerAccountId == customerAccountId).AccountName;
                        string accountRate = dr["RatePerHour"].ToString();
                        totalRecords = int.Parse(dr["TotalCount"].ToString());
                        DateTime EffectiveStartDate = Convert.ToDateTime(dr["EffectiveStartDate"].ToString());
                        DateTime EffectiveEndDate = Convert.ToDateTime(dr["EffectiveEndDate"].ToString());

                        //Create an anonymous object and at the same time
                        //add it into the recordList collection
                        recordList.Add(new
                        {
                            rowNumber = rowNumber,
                            accountRateId = accountRateId,
                            customerAccountId = customerAccountId,
                            customerAccountName = customerAccountName,
                            accountRate = accountRate,
                            EffectiveStartDate = EffectiveStartDate,
                            EffectiveEndDate = EffectiveEndDate
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
                nextPageUrl = "/API/AccountRates/GetCustomerAccountRatesPageByPage/"+ id + "?per_page=" +
                pageSize.ToString() + "&page_number=" + (currentPage + 1);
            }
            else
            {
                prevPageUrl = "/API/AccountRates/GetCustomerAccountRatesPageByPage/" + id + "?per_page=" +
                pageSize.ToString() + "&page_number=" + (currentPage - 1);
                if (currentPage == totalPage)
                {
                    nextPageUrl = null;
                }
                else
                {
                    nextPageUrl = "/API/AccountRates/GetCustomerAccountRatesPageByPage/" + id + "per_page=" +
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

        public class QueryPagingParametersForAccountRates
        {
            [BindRequired]
            public int page_number { get; set; }
            public int per_page { get; set; }
        }
    }
}

