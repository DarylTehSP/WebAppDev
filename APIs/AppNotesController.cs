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
    [Route("api/[controller]")]
    public class AppNotesController : Controller
    {
        //The following one member variable and one readonly property
        //are required for every web api controller class.
        private IAppDateTimeService _appDateTimeService;
        public ApplicationDbContext Database { get; }
        //The following constructor code pattern is required for every Web API
        //controller class.
        public AppNotesController(IAppDateTimeService appDateTimeService,
        ApplicationDbContext database)
        {
            Database = database;
            _appDateTimeService = appDateTimeService;
        }

        //*****************************************
        //POST api/AppNotes/CreateOneNote
        //*****************************************
        [HttpPost("CreateOneNote")]
        public IActionResult CreateOneNote([FromForm]IFormCollection webFormData)
        {

            int userId = int.Parse(User.FindFirst("userid").Value);

            AppNote newNote = new AppNote();

            try
            {
                newNote.Note = webFormData["note"];
                newNote.DeadLine = DateTime.ParseExact(webFormData["deadline"], "dd/MM/yyyy",
                CultureInfo.InvariantCulture);
                newNote.CreatedById = userId;
                newNote.CreatedAt = _appDateTimeService.GetCurrentDateTime();
                newNote.AppNotePriorityLevelId = int.Parse(webFormData["priorityLevelId"].ToString());
                newNote.DoneAt = null;
                Database.Add(newNote);
                Database.SaveChanges();
            }
            catch (Exception exceptionObject)
            {
                return BadRequest(exceptionObject.InnerException.Message);
            }//End of Try..Catch block
            return Ok(new
            {
                message = "Saved note record."
            });
        }
        [Authorize("ADMIN")]
        [HttpGet("GetNotesPageByPage")]
        public JsonResult GetNotesPageByPage(
         [FromQuery]QueryPagingParametersForNotes inParameters)
        {
            int pageSize = 10;
            int totalPage = 0;
            int startRecord = 0;
            int endRecord = 0;
            int currentPage = 0;
            //This is a technique to obtain the user id value which is
            //associated to the user who is currently logon
            int userId = int.Parse(User.FindFirst("userid").Value);
            
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
            cmd.CommandText = "dbo.uspSelectNotes";
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
            //Pass the user id to the stored procedure's @userId parameter
            parameter = cmd.CreateParameter();
            parameter.DbType = System.Data.DbType.Int32;
            parameter.ParameterName = "userId";
            parameter.Value = userId;
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
                    int recordId = int.Parse(dr["AppNoteId"].ToString());
                    int rowNumber = int.Parse(dr["ROWNUM"].ToString());
                    string note = dr["Note"].ToString();
                    DateTime? doneAt = null;
                    totalRecords = int.Parse(dr["TotalCount"].ToString());
                    DateTime deadLine = Convert.ToDateTime(dr["DeadLine"].ToString());
                    //I had a tough time trying to read out the datetime data and
                    //assign it to the respective DateTime datatype variable
                    //because the datetime for DoneAt can be NULL
                    if (dr.IsDBNull(dr.GetOrdinal("DoneAt")) == false)
                    {
                        doneAt = Convert.ToDateTime(dr["DoneAt"].ToString());
                    }
                    //Create an anonymous object and at the same time
                    //add it into the recordList collection
                    recordList.Add(new
                    {
                        id = recordId,
                        rowNumber = rowNumber,
                        note = note,
                        doneAt = doneAt,
                        deadLine = deadLine
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
                nextPageUrl = "/API/AppNotes/GetNotesPageByPage?per_page=" +
                pageSize.ToString() + "&page_number=" + (currentPage + 1);
            }
            else
            {
                prevPageUrl = "/API/AppNotes/GetNotesPageByPage?per_page=" +
                pageSize.ToString() + "&page_number=" + (currentPage - 1);
                if (currentPage == totalPage)
                {
                    nextPageUrl = null;
                }
                else
                {
                    nextPageUrl = "/API/AppNotes/GetNotesPageByPage?per_page=" +
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
    }
        public class QueryPagingParametersForNotes
        {
            [BindRequired]
            public int page_number { get; set; }
            public int per_page { get; set; }
        }


    
}//End of namespace
