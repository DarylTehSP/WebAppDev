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
    public class LessonTypesController : Controller
    {

        //The following one member variable and one readonly property
        //are required for every web api controller class.
        private IAppDateTimeService _appDateTimeService;
        public ApplicationDbContext Database { get; }
        //The following constructor code pattern is required for every Web API
        //controller class.
        public LessonTypesController(IAppDateTimeService appDateTimeService,
        ApplicationDbContext database)
        {
            Database = database;
            _appDateTimeService = appDateTimeService;
        }


        [Authorize("ADMIN")]
        [HttpGet("GetLessonTypesPageByPage")]
        public JsonResult GetLessonTypesPageByPage(
                                   [FromQuery]QueryPagingParametersForLessonTypes inParameters)
        {
            int pageSize = 10;
            int totalPage = 0;
            int startRecord = 0;
            int endRecord = 0;
            int currentPage = 0;

            string fieldToSort = "LessonTypeName";
            string sortDirection = "ASC";

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
            cmd.CommandText = "dbo.uspSelectLessonTypes";
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

            //Pass the field to sort information to the procedures @sortColumn parameter
            parameter = cmd.CreateParameter();
            parameter.DbType = System.Data.DbType.String;
            //Hard code this here because not getting it from the client side
            parameter.ParameterName = "sortColumn";
            parameter.Value = fieldToSort;
            cmd.Parameters.Add(parameter);
            //Pass the field to sort order to the procedures @sortOrder parameter
            parameter = cmd.CreateParameter();
            parameter.DbType = System.Data.DbType.String;
            parameter.DbType = System.Data.DbType.String;
            //Hard code this here because not getting it from the client side
            parameter.ParameterName = "sortOrder";
            parameter.Value = sortDirection;
            cmd.Parameters.Add(parameter);

            // The above commands will "build-up" one SQL command such as:
            // EXEC dbo.uspSelectNotes @userId=1,@pageNo =1,
            // @sortColumn='DONEAT , @sortOrder='ASC',
            // @pageSize = 5
            DbDataReader dr = cmd.ExecuteReader();//This is the part where SQL is sent to DB
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    //Get each column values
                    int recordId = int.Parse(dr["LessonTypeId"].ToString());
                    int rowNumber = int.Parse(dr["ROWNUM"].ToString());
                    string lessonTypeName = dr["LessonTypeName"].ToString();
                    bool isVisible = Convert.ToBoolean(dr["IsVisible"].ToString());
                    totalRecords = int.Parse(dr["TotalCount"].ToString());
                   
                    //Create an anonymous object and at the same time
                    //add it into the recordList collection
                    recordList.Add(new
                    {
                        id = recordId,
                        rowNumber = rowNumber,
                        lessonTypeName = lessonTypeName,
                        isVisible = isVisible
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
                nextPageUrl = "/API/LessonTypes/GetLessonTypesPageByPage?per_page=" +
                pageSize.ToString() + "&page_number=" + (currentPage + 1);
            }

            {
                prevPageUrl = "/API/LessonTypes/GetLessonTypesPageByPage?per_page=" +
                pageSize.ToString() + "&page_number=" + (currentPage - 1);
                if (currentPage == totalPage)
                {
                    nextPageUrl = null;
                }
                else
                {
                    nextPageUrl = "/API/LessonTypes/GetLessonTypesPageByPage?per_page=" +
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
    }//End of Web API controller class

        //Setup a class so that we can have cleaner code
        //to extract values from query string data passed from the client side 
        public class QueryPagingParametersForLessonTypes
        {
            [BindRequired]
            public int page_number { get; set; }
            public int per_page { get; set; }
        }
    
}
