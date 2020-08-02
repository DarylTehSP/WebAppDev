#pragma checksum "D:\SP stuff\Web Development\kachoweded - Copy\Learner\TMS\Views\AppNotes\Index.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "7179821a2e05c00df52e482c2c16d3aebfafe03d"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_AppNotes_Index), @"mvc.1.0.view", @"/Views/AppNotes/Index.cshtml")]
[assembly:global::Microsoft.AspNetCore.Mvc.Razor.Compilation.RazorViewAttribute(@"/Views/AppNotes/Index.cshtml", typeof(AspNetCore.Views_AppNotes_Index))]
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#line 1 "D:\SP stuff\Web Development\kachoweded - Copy\Learner\TMS\Views\_ViewImports.cshtml"
using TMS;

#line default
#line hidden
#line 2 "D:\SP stuff\Web Development\kachoweded - Copy\Learner\TMS\Views\_ViewImports.cshtml"
using TMS.Models;

#line default
#line hidden
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"7179821a2e05c00df52e482c2c16d3aebfafe03d", @"/Views/AppNotes/Index.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"4b920ee378507dd0789e3303ed664c79361ef06d", @"/Views/_ViewImports.cshtml")]
    public class Views_AppNotes_Index : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            BeginContext(0, 2125, true);
            WriteLiteral(@"<div class=""row"">
    <div class=""card col-12 w-100"">
        <div class=""card-header elegant-color white-text"">
            <div class=""container-fluid"">
                <div class=""row vertical-align"">
                    <h3 class=""one"" style=""padding-top:8px;"">View My Reminder Notes</h3>
                </div>
            </div>
        </div>
    </div>
    <div class=""card-body"">
        <div class=""row"">
            <div class=""col-md-12"">
                <div class=""float-right"">
                    <a id=""createOneNoteButton1""
                       href=""/AppNotes/CreateOneNote""
                       class=""btn btn-primary"">
                        Create a note
                    </a>
                </div>
            </div>
        </div>
        <div class=""row"">
            <div class=""table-responsive-md w-100"">
                <div id=""topNavigationInterfaceContainer"" class=""btn-toolbar float-right""
                     role=""toolbar""></div>
                <table ");
            WriteLiteral(@"class=""table table-bordered"">
                    <thead>
                        <tr class=""row m-0"" style=""height:50px;vertical-align:central"">
                            <th class=""col-md-1""><h5><b>#</b></h5></th>
                            <th class=""col-md-2""><h5><b>DEADLINE</b></h5></th>
                            <th class=""col-md-9""><h5><b>DONE</b></h5></th>
                        </tr>
                    </thead>
                    <tbody id=""dataTableBody""></tbody>
                </table>
                <div id=""bottomNavigationInterfaceContainer""
                     class=""btn-toolbar float-right"" role=""toolbar""></div>
            </div>
        </div>
        <div class=""row"">
            <div class=""col-md-12"">
                <div class=""float-right"">
                    <a id=""createOneNoteButton1""
                       href=""/AppNotes/CreateOneNote""
                       class=""btn btn-primary"">
                        Create a note
                    </a>
   ");
            WriteLiteral("             </div>\r\n            </div>\r\n        </div>\r\n    </div>\r\n</div>\r\n");
            EndContext();
            DefineSection("scripts", async() => {
                BeginContext(2142, 12688, true);
                WriteLiteral(@"
    <style>
        body {
            margin-top: 30px;
            padding-right: 0px;
        }
        /* <DIV> elements used in the view interface to act as a ""parent container"".
        for JavaScript logic to append <a> elements which are formatted as ""page number""
        buttons. By default <DIV> elements are not visible by default. Therefore,
        the following CSS code are used for testing/learning purpose to let viewer notice
        that there are 2 <div> elements in the view interface.
        */
        #dataBottomNavigationContainer, #dataTopNavigationContainer {
            min-height: 70px;
            max-height: 90px;
            border: 2px solid black;
            width: 300px;
        }

        td {
            font-size: 20px;
            line-height: 28px;
            max-height: 76px;
            min-height: 50px;
        }
    </style>
    <script>
        //Interested to learn IIFE Immediately-Invoked JavaScript Expression? Check out
        //6-min");
                WriteLiteral(@"ute video (very simple and good by a nice developer): https://youtu.be/FZAMeIQCSpQ
        (function ($, moment, window, document) {
            let pageSize = 5;//Fixed the page size to 5 per page
            //Call the loadData to begin fetching 1st set of 5 record data which fits into page 1
            loadData();
            function loadData() {
                /****************************************************/
                //function loadData is usually called inside the body of the
                //""callback function"" of the given to the done() method.
                /****************************************************/
                //Note 1:
                //The URL is built by using string interpolation technique.
                //https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Template_literals
                //'/API/AppNotes/GetNotesPageByPage?page_number=1&per_page=5'
                //The technique used is called ""interpolation"".
                /");
                WriteLiteral(@"/Notice that I did not use + string concatenation.
                //Note 2:
                //The ajax method uses a URL which has two query string parameter:per_page.
                //and page_number.
                //The query string parameter - value pairs, per_page = 5 and page_number = 1,
                //will be picked up by the web api.
                //The backend will identify these information as an instruction to fetch
                //the first 5 records which can fit into the page number 1.
                //You don't need to pass the user id to the server-side (you should not have such
                //information at the client-side anyway).
                //The server - side web api method's C# logic can
                //internally take care the user id(using C# code) because the
                //C# command can obtain the user id.
                $.ajax({
                    method: 'GET',
                    url: `/API/AppNotes/GetNotesPageByPage?page_number=1&per_page");
                WriteLiteral(@"=${pageSize}`,
                    dataType: 'json',
                    async: true,
                    cache: false,
                }).done(function (data) {
                    //Obtain the maximum number of page information from the JSON data
                    let maximumNumberOfPages = data.last_page;
                    let numberOfRecords = data.total;
                    if (numberOfRecords > 0) {
                        //If there are any record data found, start
                        //calling the renderData function and pass in an array of record data objects
                        //into the function.
                        renderData(data.records);
                        //Call two more functions to generate ""page number buttons""
                        //inside the respective <div> parent container elements.
                        createNavigationButtonsAtTop(maximumNumberOfPages);
                        createNavigationButtonsAtBottom(maximumNumberOfPages);
         ");
                WriteLiteral(@"           } else {
                        $('#dataTableBody').append($(`<tr class=""row m-0"">
                                <td class=""col-md-11 text-center text-muted"">
                                <h4>No records found.</h4></td></tr>`));
                    }//End of if else to either display rows of data or display 'none found' message
                });//End of ajax().done()
            }//loadData
            // This function accepts a data (array of records)
            // and prints rows of data to the <tbody> element.
            function renderData(records) {
                $('#dataTableBody').html('');//Clear the <tbody> element first
                for (var index = 0; index < records.length; index++) {
                    //The for-loop code here begins building and inserting the cells<td> and rows<tr>
                    //into the <tbody> element. Note: Two rows are created during each pass of the loop.
                    let $cellElement = null;
                    let r");
                WriteLiteral(@"ecord = records[index];
                    //Start - Defining the first row element to hold the row number, deadline and done at information
                    let $rowElement = $('<tr class=""row m-0""></tr>');
                    $cellElement = $(`<td rowspan=""2"" class=""col-md-1"">${record.rowNumber}</td>`);
                    $rowElement.append($cellElement);
                    $cellElement = $(`<td class=""col-md-2"">
${moment(record.deadLine).format('DD/MM/YYYY')}
</td>`);//end of append()
                    $rowElement.append($cellElement);
                    //Check if there is a date time value in the doneDate.
                    //If yes, use moment to format the date time display
                    if (record.doneAt !== null) {
                        $cellElement = $(`<td class=""col-md-9""><i class=""fas fa-check"">
</i>&nbsp;${moment(record.doneAt).format('DD/MM/YYYY hh:mm A')}
</td>`);
                    } else {
                        $cellElement = $(`<td class=""col-md-9"">&nbs");
                WriteLiteral(@"p;</td>`);
                    }
                    $rowElement.append($cellElement);
                    $('#dataTableBody').append($rowElement);//inserting a new row
                    //End - Defining the first row element to hold the row number, deadline and done at information
                    //Start- Defining the second row element to hold note information
                    $rowElement = $('<tr class=""row m-0""></tr>');
                    $rowElement.append(
                        $(`<td class=""col-md-1""></td><td class=""col-md-11"">${record.note}</td>`)
                    );//end of inserting a cell into a row
                    $('#dataTableBody').append($rowElement);
                    //End-- Defining the second row element to hold note information
                }//end of for loop
            }//end of renderData function
            function createNavigationButtonsAtTop(inMaximumNumOfPages) {
                $('#topNavigationInterfaceContainer').text('');
               ");
                WriteLiteral(@" for (let count = 1; count <= inMaximumNumOfPages; count++) {
                    //Create an <a> element which has two HTML data attributes.
                    //data-page-number and data-page-size. Associate the count value
                    //and the maximum number of record data per page information
                    //to the respective attributes.
                    let $pageNumberButtonLinkElement =
                        $(`<a href=\""#\"" data-page-number=""${count}""data-page-size=""${pageSize}"" class=""btn btn-secondary"" >${count}</a>&nbsp;`);
                    //Register an anonymous function to handle the element's //click event
                    $pageNumberButtonLinkElement.on('click', function (event) {
                        event.preventDefault();
                        //The event object describes the event which occurred.
                        //The target property represents the element which was clicked.
                        //The $(event.target) expression creates ");
                WriteLiteral(@"a jQuery object which take control the
                        //<a> which was clicked. For example, if page number button 3 is clicked,
                        //the event.target represents that <a> element which is clicked.
                        //$(event.target) takes control the <a> element which is clicked.
                        //The logic needs the page number value and the page size value from the
                        //respective <a> element which is clicked.
                        //Therefore, you can use the $(event.target)data(....) to extract out the
                        //data - page - number and data-page-size attribute value.
                        //$(event.target).data('page-number') extracts the HTML data attribute information
                        let pageSize = $(event.target).data('page-size');
                        //if button 3 is clicked, the .data('page-number') will give 3
                        let pageNumber = $(event.target).data('page-number');
     ");
                WriteLiteral(@"                   //Make a HTTP GET request using the URL which has query string data.
                        $.ajax({
                            method: ""GET"",
                            url:
                                `/API/AppNotes/GetNotesPageByPage?page_number=${pageNumber}&per_page=${pageSize}`,
                            dataType: ""json"",
                            async: true,
                            cache: false
                        }).done(function (data) {
                            //Call the renderData and pass in the array of record data
                            renderData(data.records);
                        })//End of ajax().done()
                    });//End of .on('click', function{....})
                    //Insert the created <a> element which has a function registered to its click event
                    //into the parent <div> element, topNavigationInterfaceContainer
                    $('#topNavigationInterfaceContainer').append($pageNumberBut");
                WriteLiteral(@"tonLinkElement);
                }//End of for loop
            }//End of createNavigationButtonsAtTop function
            function createNavigationButtonsAtBottom(inMaximumNumOfPages) {
                $('#bottomNavigationInterfaceContainer').text('');
                for (let count = 1; count <= inMaximumNumOfPages; count++) {
                    //Create a styled <a> element which appears as page number button interface
                    let $pageNumberButtonLinkElement =
                        $(`<a href=\""#\"" data-page-number=""${count}""data-page-size=""${pageSize}"" class=""btn btn-secondary"" >${count}</a>&nbsp;`);
                    //Register an anonymous function to handle the <a> element's
                    //click event.
                    $pageNumberButtonLinkElement.on('click', function (event) {
                        event.preventDefault();
                        let pageSize = $(event.target).data('page-size');//no need .data('data-page-size')
                        let pa");
                WriteLiteral(@"geNumber = $(event.target).data('page-number');
                        $.ajax({
                            method: ""GET"",
                            url:
                                `/API/AppNotes/GetNotesPageByPage?page_number=${pageNumber}&per_page=${pageSize}`,
                            dataType: ""json"",
                            async: true,
                            cache: false
                        }).done(function (data) {
                            //Call the renderData and pass in the array of record data
                            renderData(data.records);
                            //Call the createNavigationButtonsAtTopFunction
                            //and pass the max num of pages value into the function
                            createNavigationButtonsAtTop(maximumNumberOfPages);
                        })//End of ajax().done()
                    });//End of .on('click', function{....})
                    //Insert the created <a> element which has a fu");
                WriteLiteral(@"nction registered to its click event
                    //into the parent <div> container element, bottomNavigationInterfaceContainer
                    $('#bottomNavigationInterfaceContainer').append($pageNumberButtonLinkElement);
                }//End of for loop
            }//End of createNavigationButtonsAtBottom function
        })($, moment, window, window.document)
    </script>
");
                EndContext();
            }
            );
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<dynamic> Html { get; private set; }
    }
}
#pragma warning restore 1591
