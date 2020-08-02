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
    public class CustomerAccountsController : Controller
    {
        //The following one member variable and one readonly property
        //are required for every web api controller class.
        private IAppDateTimeService _appDateTimeService;
        public ApplicationDbContext Database { get; }
        //The following constructor code pattern is required for every Web API
        //controller class.
        public CustomerAccountsController(IAppDateTimeService appDateTimeService,
        ApplicationDbContext database)
        {
            Database = database;
            _appDateTimeService = appDateTimeService;
        }
        //Create a new customer accounts with its corresponding comments and account rates
        [Authorize("ADMIN")]
        [HttpPost("CreateCustomerAccounts")]
        public IActionResult CreateNewCustomer([FromForm]IFormCollection webFormData)
        {

            int userId = int.Parse(User.FindFirst("userid").Value);
            CustomerAccount customerAccount = new CustomerAccount();
            CustomerAccountComment customerAccountComment;
            AccountRate accountRate;
            //try
            //{
                customerAccount.AccountName = webFormData["accountName"];
                customerAccount.IsVisible = bool.Parse(webFormData["visible"]);
                customerAccount.CreatedById = userId;
                customerAccount.CreatedAt = _appDateTimeService.GetCurrentDateTime();
                customerAccount.UpdatedAt = _appDateTimeService.GetCurrentDateTime();
                customerAccount.UpdatedById = userId;

                if (webFormData["comments"] != "")
                {
                    foreach (string comment in webFormData["comments"])
                    {
                        customerAccountComment = new CustomerAccountComment();
                        customerAccountComment.Comment = comment;
                        customerAccountComment.CreatedAt = _appDateTimeService.GetCurrentDateTime();
                        customerAccountComment.CreatedById = userId;
                        customerAccountComment.UpdatedAt = _appDateTimeService.GetCurrentDateTime();
                        customerAccount.Comments.Add(customerAccountComment);
                    }
                }
               
                foreach (string rate in webFormData["rates"])
                {
                    accountRate = new AccountRate();
                    accountRate.RatePerHour = int.Parse(rate);
                    accountRate.EffectiveStartDate = DateTime.ParseExact(webFormData["startDate"], "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    accountRate.EffectiveEndDate = DateTime.ParseExact(webFormData["endDate"], "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    accountRate.CreatedAt = _appDateTimeService.GetCurrentDateTime();
                    accountRate.UpdatedAt = _appDateTimeService.GetCurrentDateTime();
                    accountRate.CreatedById = userId;
                    accountRate.UpdatedById = userId;
                    customerAccount.AccountRates.Add(accountRate);
                }

                Database.CustomerAccounts.Add(customerAccount);
                Database.SaveChanges();

            //} catch (Exception exceptionObject)
            //{
                //if (exceptionObject.Message.Contains("CustomerAccount_AccountName_UniqueConstraint") == false)
                //{
                    //return BadRequest(new { message = "Unable to create customer account due to another record having the same account name " + webFormData["accountName"] });
                //}

            //}
            return Ok(new
            {
                message = "Saved customer account."
            });
        }

        //To retrieve customer account based on its id number
        [Authorize("ADMIN")]
        [HttpGet("GetCustomerAccount/{id}")]
        public IActionResult GetCustomerAccount(int id)
        {
            var oneCustomerAccount = Database.CustomerAccounts.SingleOrDefault(input => input.CustomerAccountId == id);
            if (oneCustomerAccount == null)
            {
                return BadRequest(new { message = "Unable to retrieve customer account" });
            }
            else
            {
                var response = new
                {
                    accountId = oneCustomerAccount.CustomerAccountId,
                    accountName = oneCustomerAccount.AccountName,
                    visibility = oneCustomerAccount.IsVisible,
                    createdAt = oneCustomerAccount.CreatedAt,
                    createdBy = Database.AppUsers.Single(input => input.Id == oneCustomerAccount.CreatedById).FullName,
                    updatedAt = oneCustomerAccount.UpdatedAt,
                    updatedBy = Database.AppUsers.Single(input => input.Id == oneCustomerAccount.UpdatedById).FullName

                };
                return Ok(response);
            }

        }

        //To update the customer account based on its id value
        [Authorize("ADMIN")]
        [HttpPut("UpdateCustomerAccount/{id}")]
        public IActionResult UpdateCustomerAccount(int id, [FromForm]IFormCollection webFormData)
        {
            string customMessage = "";
            CustomerAccount foundOneCustomer = Database.CustomerAccounts.Single(input => input.CustomerAccountId == id);
            int userId = int.Parse(User.FindFirst("userid").Value);
            foundOneCustomer.UpdatedById = userId;
            foundOneCustomer.UpdatedAt = _appDateTimeService.GetCurrentDateTime();
            foundOneCustomer.AccountName = webFormData["accountName"];
            foundOneCustomer.IsVisible = bool.Parse(webFormData["visible"].ToString());
            try
            {
                Database.CustomerAccounts.Update(foundOneCustomer);
                Database.SaveChanges();
            }
            catch (Exception ex)
            {    
                    customMessage = "Unable to update customer account due to another record having the same account name" + webFormData["accountName"];
                    return BadRequest(new { message = customMessage});
            }
            return Ok(new
            {
                message = "Updated customer account"
            });
        } 

        //Create customer account summary
        [Authorize("ADMIN")]
        [HttpGet("GetCustomerAccountSummary/{id}")]
        public IActionResult GetCustomerAccountSummary(int id)
        {
            var accountName = Database.CustomerAccounts.Single(input => input.CustomerAccountId == id).AccountName;
            var foundAccountRate = Database.AccountRates.Where(input => input.CustomerAccountId == id).Count();
            var foundComments = Database.CustomerAccountComments.Where(input => input.CustomerAccountId == id).Count();
            var foundInstructor = Database.InstructorAccounts.Where(input => input.CustomerAccountId == id).Count();
            var response = new
            {
                accountName = accountName,
                accountRateCount = foundAccountRate,
                commentCount = foundComments,
                instructorCount = foundInstructor
                
            };
            return Ok(response);
          
        }
        //To delete customer account
        [Authorize("ADMIN")]
        [HttpDelete("DeleteCustomerAccount/{id}")]
        public IActionResult DeleteCustomerAccount(int id, [FromForm]IFormCollection webFormData)
        {
           
            try
            {
                CustomerAccount foundOneCustomer = Database.CustomerAccounts.Single(input => input.CustomerAccountId == id);

                Database.CustomerAccounts.Remove(foundOneCustomer);
                Database.SaveChanges();
                
            }
            catch (Exception exceptionObject)
            {
                return BadRequest(exceptionObject.InnerException.Message);
            }
            return Ok(new
            {
                message = "Deleted customer account"
            });
        }

        //To retrieve customer account comment based on the id
        [Authorize("ADMIN")]
        [HttpGet("GetCustomerAccountCommentsPageByPage/{id}")]
        public JsonResult GetCustomerAccountRatesPageByPage(int id,
        [FromQuery]QueryPagingParametersForAccountComments inParameters)
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
            cmd.CommandText = "dbo.uspSelectCustomerComments";
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
                        int createdById = int.Parse(dr["CreatedById"].ToString());
                        int userId = int.Parse(User.FindFirst("userid").Value);
                        string createdBy = Database.AppUsers.Single(input => input.Id == createdById).FullName;
                        int customerCommentId = int.Parse(dr["CustomerAccountCommentId"].ToString());
                        DateTime CreatedAt = Convert.ToDateTime(dr["CreatedAt"].ToString());
                        string customerAccountName = Database.CustomerAccounts.Single(input => input.CustomerAccountId == customerAccountId).AccountName;
                        string customerComment = dr["Comment"].ToString();
                        int? parentId = null;

                        if (dr.IsDBNull(dr.GetOrdinal("ParentId")) == false)
                        {
                            parentId = int.Parse(dr["ParentId"].ToString());
                        }

                        //Create an anonymous object and at the same time
                        //add it into the recordList collection
                        recordList.Add(new
                        {
                            rowNumber = rowNumber,
                            customerAccountId = customerAccountId,
                            customerAccountName = customerAccountName,
                            createdById = createdById,
                            userId = userId,
                            createdBy = createdBy,
                            createdAt = CreatedAt,
                            customerCommentId = customerCommentId,
                            customerComment = customerComment,
                            parentId = parentId
                        }) ;
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
                nextPageUrl = "/API/CustomeAccounts/GetCustomerAccountCommentsPageByPage/" + id + "?per_page=" +
                pageSize.ToString() + "&page_number=" + (currentPage + 1);
            }
            else
            {
                prevPageUrl = "/API/CustomeAccounts/GetCustomerAccountCommentsPageByPage/" + id + "?per_page=" +
                pageSize.ToString() + "&page_number=" + (currentPage - 1);
                if (currentPage == totalPage)
                {
                    nextPageUrl = null;
                }
                else
                {
                    nextPageUrl = "/API/CustomeAccounts/GetCustomerAccountCommentsPageByPage/" + id + "per_page=" +
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

        //To create a new customer comment
        [Authorize("ADMIN")]
        [HttpPost("CreateCustomerComment/{id}")]
        public IActionResult CreateCustomerComment(int id, [FromForm]IFormCollection webFormData)
        {

            int userId = int.Parse(User.FindFirst("userid").Value);
            CustomerAccountComment customerAccountComment = new CustomerAccountComment();
            try
            {
                customerAccountComment.Comment = webFormData["customerComment"];
                customerAccountComment.CreatedAt = _appDateTimeService.GetCurrentDateTime();
                customerAccountComment.CustomerAccountId = id;
                customerAccountComment.ParentId = null;
                customerAccountComment.CreatedById = userId;
                customerAccountComment.UpdatedAt = _appDateTimeService.GetCurrentDateTime();
             
                Database.CustomerAccountComments.Add(customerAccountComment);
                Database.SaveChanges();
            }
            catch (Exception exceptionObject)
            {
                
                 return BadRequest(exceptionObject.InnerException.Message);
                

            }
            return Ok(new
            {
                message = "Saved customer account comment."
            });
        }

        //To reply a customer comment
        [Authorize("ADMIN")]
        [HttpPost("ReplyCustomerComment/{id}")]
        public IActionResult ReplyCustomerComment(int id, [FromForm]IFormCollection webFormData)
        {

            int userId = int.Parse(User.FindFirst("userid").Value);
            CustomerAccountComment customerAccountComment = new CustomerAccountComment();
            try
            {
                customerAccountComment.Comment = webFormData["customerComment"];
                customerAccountComment.CreatedAt = _appDateTimeService.GetCurrentDateTime();
                customerAccountComment.CustomerAccountId = id;
                customerAccountComment.ParentId = int.Parse(webFormData["parentId"]);
                customerAccountComment.CreatedById = userId;
                customerAccountComment.UpdatedAt = _appDateTimeService.GetCurrentDateTime();

                Database.CustomerAccountComments.Add(customerAccountComment);
                Database.SaveChanges();
            }
            catch (Exception exceptionObject)
            {

                return BadRequest(exceptionObject.InnerException.Message);


            }
            return Ok(new
            {
                message = "Replied customer comment."
            });
        }

        //To retrieve all customer account record in the database
        [Authorize("ADMIN")]
        [HttpGet("GetCustomerAccountsPageByPage")]
        public JsonResult GetCustomerAccountsPageByPage(
         [FromQuery]QueryPagingParametersForAccounts inParameters)
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
            cmd.CommandText = "dbo.uspSelectCustomerAccounts";
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
                    //Get each column values
                    int rowNumber = int.Parse(dr["ROWNUM"].ToString());
                    int customerAccountId = int.Parse(dr["CustomerAccountId"].ToString());
                    string accountName = dr["AccountName"].ToString();
                    totalRecords = int.Parse(dr["TotalCount"].ToString());
                    bool isVisible = bool.Parse(dr["IsVisible"].ToString());
                    int createdById = int.Parse(dr["CreatedById"].ToString());
                    string createdBy = Database.AppUsers.Single(input => input.Id == createdById).FullName;
                    int updatedById = int.Parse(dr["UpdatedById"].ToString());
                    string updatedBy = Database.AppUsers.Single(input => input.Id == updatedById).FullName;
                    DateTime UpdatedAt = Convert.ToDateTime(dr["UpdatedAt"].ToString());
                   
                    //Create an anonymous object and at the same time
                    //add it into the recordList collection
                    recordList.Add(new
                    {
                        rowNumber = rowNumber,
                        customerAccountId = customerAccountId,
                        accountName = accountName,
                        isVisible = isVisible,
                        createdBy = createdBy,
                        updatedBy = updatedBy,
                        UpdatedAt = UpdatedAt,
                    });
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
                nextPageUrl = "/API/AppNotes/GetCustomerAccountsPageByPage?per_page=" +
                pageSize.ToString() + "&page_number=" + (currentPage + 1);
            }
            else
            {
                prevPageUrl = "/API/AppNotes/GetCustomerAccountsPageByPage?per_page=" +
                pageSize.ToString() + "&page_number=" + (currentPage - 1);
                if (currentPage == totalPage)
                {
                    nextPageUrl = null;
                }
                else
                {
                    nextPageUrl = "/API/AppNotes/GetCustomerAccountsPageByPage?per_page=" +
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
    }//end of GetNotesPageByPage()
    

    public class QueryPagingParametersForAccounts
        {
            [BindRequired]
            public int page_number { get; set; }
            public int per_page { get; set; }
        }
    }

    public class QueryPagingParametersForAccountComments
    {
        [BindRequired]
        public int page_number { get; set; }
        public int per_page { get; set; }
    }
}

