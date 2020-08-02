using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TMS.Data;
using TMS.Models;
using TMS.Services;
using System.Diagnostics;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TMS.APIs
{
    [Route("api/[controller]")]
    public class InstructorController : Controller
    {
        private IAppDateTimeService _appDateTimeService;
        private ApplicationDbContext Database { get; }
        public InstructorController(IAppDateTimeService appDateTimeService,
        ApplicationDbContext database)
        {
            Database = database;
            _appDateTimeService = appDateTimeService;
        }
        // GET: api/<controller>
        [HttpGet("GetAllInstructor/{custId}")]
        public IActionResult GetAllInstructor(int custId, [FromQuery] string pgNo, [FromQuery] string pgSize)
        {
            string customMessage = "";
            Debug.WriteLine(pgNo + ">>>" + pgSize);
            if ((pgNo == null) || (pgSize == null))
            {
                return BadRequest(new { message = "Query Parameter Missing!" });
            }

            try
            {
                // Retrieve the instructor records based on the parameters
                var curPageNo = int.Parse(pgNo);
                var reqPageSize = int.Parse(pgSize);
                List<object> appUserList = new List<object>();

                var allQuery = Database.AppUsers
                        .Where(x => x.RoleId == 2)
                        .Include(x => x.InstructorAccounts).ToList();

                foreach (var eachAppUser in allQuery)
                {
                    var isAssign = false;
                    var customerName = "";
                    List<string> customerAssign = new List<string>();
                    decimal wageRate = 0;
                    for (int i = 0; i < eachAppUser.InstructorAccounts.ToList().Count(); i++)
                    {
                        var oneCustomer = Database.CustomerAccounts
                            .SingleOrDefault(customer => customer.CustomerAccountId == eachAppUser.InstructorAccounts.ToList()[i].CustomerAccountId);

                        if (eachAppUser.InstructorAccounts.ToList()[i].CustomerAccountId == custId)
                        {
                            wageRate = eachAppUser.InstructorAccounts.ToList()[i].WageRate;
                            isAssign = true;
                        }
                        customerName = oneCustomer.AccountName;
                        customerAssign.Add(customerName);
                    }
                    appUserList.Add(new
                    {
                        custAss = customerAssign,
                        id = eachAppUser.Id,
                        rate = wageRate,
                        fullName = eachAppUser.FullName,
                        email = eachAppUser.UserName,
                        assignToCustomer = isAssign
                    });
                }//end of the foreach block\

                int totalNoPage = (int)Math.Ceiling((double)appUserList.Count() / reqPageSize);
                var queryString = appUserList.Skip((curPageNo - 1) * reqPageSize).Take(reqPageSize);

                return Ok(new
                {
                    totalPageCnt = totalNoPage,
                    records = queryString
                });

            }
            catch (Exception ex)
            {
                customMessage = "The request could not be processed " +
               "due to internal errors. Please, try again later";
                return BadRequest(new { message = customMessage });
            }
        }

        [HttpGet("GetAssignInstructor/{custId}")]
        public IActionResult Get(int custId)
        {
            List<object> assignedInstructorList = new List<object>();
            object dataSummary = new object();

            var foundOneCustomer = Database.InstructorAccounts
               .Where(x => x.CustomerAccount.CustomerAccountId == custId).AsNoTracking().FirstOrDefault();

            if (foundOneCustomer == null)
            {
                return NotFound(new { message = "Assign instructor record is not found." });
            }

            var assignInstructorQueryResult = Database.InstructorAccounts
             .Where(x => x.CustomerAccount.CustomerAccountId == custId)
             .Include(y => y.CustomerAccount).Include(z => z.Instructor).Include(a => a.CreatedBy);

            //The following foreach loop aims to create a
            //into a single List of anonymous objects.  
            foreach (var eachInstructor in assignInstructorQueryResult)
            {
                assignedInstructorList.Add(new
                {
                    instructorAccountId = eachInstructor.InstructorAccountId,
                    accountName = eachInstructor.CustomerAccount.AccountName,
                    fullName = eachInstructor.Instructor.FullName,
                    email = eachInstructor.Instructor.UserName,
                    wageRate = eachInstructor.WageRate,
                    createdAt = eachInstructor.CreatedAt,
                    createdBy = eachInstructor.CreatedBy.FullName
                });
            }//end of the foreach block\
            return Ok(assignedInstructorList);
        }

        [HttpGet("gcats")]                                                           //project is customer account;employee is instructor
        public IActionResult GetCustomerAssignedToSelf()
        {
            int userId = int.Parse(User.FindFirst("userid").Value); //Retrieve the user id of the current logged in user
            List<InstructorAccount> cusIAList = Database.InstructorAccounts.Where(i => i.InstructorId == userId).Include(c => c.CustomerAccount).ToList();
            List<object> customerList = new List<object>();
            foreach (var record in cusIAList)
            {
                customerList.Add(new
                {
                    customerId = record.CustomerAccount.CustomerAccountId,
                    customerName = record.CustomerAccount.AccountName
                });
            }//end of the foreach block\
            return Ok(customerList);
        }

        [HttpGet("populategcats/{monthyear}")] //check correct date and projectId then fill in hours worked
        public IActionResult GetCustomerAssignedHoursToSelf(String monthyear)
        {
            int userId = int.Parse(User.FindFirst("userid").Value); //Retrieve the user id of the current logged in user
            List<InstructorAccount> cusIAList = Database.InstructorAccounts.Where(i => i.InstructorId == userId).Include(c => c.CustomerAccount).ToList();
            List<int> customerIdList = new List<int>();
            foreach (var record in cusIAList)
            {
                customerIdList.Add(record.CustomerAccount.CustomerAccountId);
            }//end of the foreach block\
            DateTime SoMDate = DateTime.ParseExact("01-" + monthyear, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            DateTime EoMDate = SoMDate.AddMonths(1);
            List<TimeLog> timelogRecord = Database.TimeLogs.Where(i => i.EmployeeId == userId).Where(p => customerIdList.Contains(p.ProjectId)).Where(d => d.Date.CompareTo(SoMDate) >= 0).Where(d => d.Date.CompareTo(EoMDate) < 0).ToList();
            List<object> tlRecList = new List<object>();
            foreach (var record in timelogRecord)
            {
                tlRecList.Add(new
                {
                    cellId = record.Date.Day + "-" + record.ProjectId,
                    hoursWorked = record.HoursWorked
                });
            }//end of the foreach block\
            return Ok(tlRecList);
        }

        //POST data from user input
        [HttpPost("CreateTimeLog")]
        public IActionResult CreateTimeLog([FromForm] IFormCollection userFormData)
        {
            string customMessage = "";
            try
            {

                customMessage = userFormData["date"];

                //Get user id
                int userId = int.Parse(User.FindFirst("userid").Value);
                DateTime dateChosen = DateTime.ParseExact(userFormData["date"], "dd-MM-yyyy", CultureInfo.InvariantCulture);
                //String dateChosen = userFormData["day"] +"/"+ userFormData ["month"] + "/" + userFormData["year"];
                //int year=int.Parse(userFormData["year"]), month= int.Parse(userFormData["month"]), day= int.Parse(userFormData["day"]);
                // int timeLogId = 100000000+dateChosen.Day*1000000+dateChosen.Month*10000+dateChosen.Year;

                TimeLog newTimeLog = new TimeLog();

                // newTimeLog.Id = timeLogId;
                newTimeLog.Date = dateChosen;
                newTimeLog.ProjectId = int.Parse(userFormData["project"]);
                newTimeLog.HoursWorked = int.Parse(userFormData["hoursWorked"]);
                newTimeLog.EmployeeId = userId;

                Database.Add(newTimeLog);
                Database.SaveChanges();

            }
            catch (Exception ex)
            {
                //customMessage = ex.ToString();
                return BadRequest(new { message = ex.ToString() });
            }//End of try .. catch block on saving data

            //Send back an OK with 200 status code
            return Ok(new
            {
                message = customMessage
            });
        }


        //PUT data from user input
        [HttpPut("UpdateTimeLog")]
        public IActionResult UpdateTimeLog([FromForm] IFormCollection userFormData)
        {
            string customMessage = "";
            try
            {

                customMessage = userFormData["date"];

                //Get user id
                int userId = int.Parse(User.FindFirst("userid").Value);
                DateTime dateChosen = DateTime.ParseExact(userFormData["date"], "dd-MM-yyyy", CultureInfo.InvariantCulture);
                TimeLog foundOneTimeLog = Database.TimeLogs.Where(x => x.EmployeeId == userId).Where(d => d.Date == dateChosen).Where(p => p.ProjectId == int.Parse(userFormData["project"])).FirstOrDefault();
                //Database.TimeLogs.Remove(foundOneTimeLog);
                //String dateChosen = userFormData["day"] +"/"+ userFormData ["month"] + "/" + userFormData["year"];
                //int year=int.Parse(userFormData["year"]), month= int.Parse(userFormData["month"]), day= int.Parse(userFormData["day"]);
                // int timeLogId = 100000000+dateChosen.Day*1000000+dateChosen.Month*10000+dateChosen.Year;

                // foundOneTimeLog.Id = timeLogId;
                foundOneTimeLog.Date = dateChosen;
                foundOneTimeLog.ProjectId = int.Parse(userFormData["project"]);
                foundOneTimeLog.HoursWorked = int.Parse(userFormData["hoursWorked"]);
                foundOneTimeLog.EmployeeId = userId;

                Database.TimeLogs.Update(foundOneTimeLog);
                Database.SaveChanges();

            }
            catch (Exception ex)
            {
                //customMessage = ex.ToString();
                return BadRequest(new { message = ex.ToString() });
            }//End of try .. catch block on saving data

            //Send back an OK with 200 status code
            return Ok(new
            {
                message = customMessage
            });
        }



        // POST api/<controller>
        [HttpPost]
        public IActionResult Post([FromForm] IFormCollection data)
        {
            string customMessage = "";

            try
            {
                //Get user id
                int userId = int.Parse(User.FindFirst("userid").Value);

                InstructorAccount newInstructorAccount = new InstructorAccount();   

                newInstructorAccount.CustomerAccountId = int.Parse(data["custId"]);
                newInstructorAccount.InstructorId = int.Parse(data["instructorId"]);
                newInstructorAccount.WageRate = decimal.Parse(data["wageRate"]);
                newInstructorAccount.CreatedAt = _appDateTimeService.GetCurrentDateTime();
                newInstructorAccount.CreatedById = userId;

                Database.Add(newInstructorAccount);
                Database.SaveChanges();
            }
            catch (Exception ex)
            {
                customMessage = "The request could not be processed " +
                               "due to internal errors. Please, try again later";
                return BadRequest(new { message = customMessage });
            }//End of try .. catch block on saving data

            //Send back an OK with 200 status code
            return Ok(new
            {
                message = "Saved new Instructor Account record"
            });
        }



        // PUT api/<controller>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        [HttpDelete("UnAssignInstructorFromCustomerAccount/")]
        public IActionResult UnAssignInstructorFromCustomerAccount([FromQuery] string value)
        {
            string customMessage = "";
            var listOfId = value.Split(',').ToList();
            List<InstructorAccount> instructorCustomerAccountList = new List<InstructorAccount>();

            try
            {
                instructorCustomerAccountList = Database.InstructorAccounts
                    .Where(record => listOfId.Contains(record.InstructorAccountId.ToString())).ToList();
                if (instructorCustomerAccountList != null)
                {
                    Database.InstructorAccounts.RemoveRange(instructorCustomerAccountList);
                }
                Database.SaveChanges();
            }
            catch (Exception ex)
            {
                customMessage = "Unable to delete assigned instructor record.";
                return BadRequest(new { message = customMessage });
            }

            return Ok(new { message = "Remove assignment information successfully" });
        }
    }
}
