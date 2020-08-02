using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;


using Microsoft.Extensions.Options;
using TMS.Data;
using TMS.Helpers;
using TMS.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using TMS.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;
using System.Diagnostics;
using System.Text;

namespace TMS.APIs
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AccountManagerController : Controller
    {
        private IUserService _userService;
        private ApplicationDbContext _database;
        private AppSettings _appSettings;
        //Constructor 
        public AccountManagerController(
            IUserService userService,
            ApplicationDbContext database,
            IOptions<AppSettings> appSettings)
        {
            _userService = userService;
            _database = database;
            _appSettings = appSettings.Value;
        }

        // Post api/AccountManager/CreateUser
        [HttpPost("CreateUser")]
        public async Task<IActionResult> CreateUserAsync([FromForm] IFormCollection data)
        {
            //Create an object AppUser type object, user
            AppUser user = new AppUser();

            //Start passing the collected data into the new AppUser object.
            user.FullName = data["fullName"];
            user.UserName = data["email"];
            user.RoleId = int.Parse(data["roleId"]);
            try
            {
                await _userService.CreateAsync(user, "p@ssw0rd");
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
            //Send back an OK with 200 status code
            return Ok(new
            {
                message = "Saved user record"
            });
        }//End of CreateUserAsync web api

        // Put api/AccountManager/UpdateUser
        [HttpPut("UpdateUser/{id}")]
        public async Task<IActionResult> UpdateUser(int id,[FromForm] IFormCollection data)
        {
            //Create an object AppUser type object, user
            AppUser user = new AppUser();

            //Start passing the collected data into the new AppUser object.
            user.Id = id;
            user.FullName = data["fullName"];
            user.UserName = data["email"];
            user.RoleId = int.Parse(data["roleId"]);
            try
            {
               await _userService.UpdateAsync(user, null);
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
            //Send back an OK with 200 status code
            return Ok(new
                {
                    message = "Saved user record"
                });
            

        }//End of UpdateUser web api

        // Put api/AccountManager/DeleteUser
        [HttpDelete("DeleteUser/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
    
            try
            {
                _userService.DeleteAsync(id);

            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
            //Send back an OK with 200 status code
            return Ok(new
            {
                message = "Deleted user record"
            });


        }//End of DeleteUser web api

        //GET api/AccountManager/GetUsers
        [HttpGet("GetUsers")]
        public async Task<IActionResult> GetUsers()
        {
            List<AppUser> users = new List<AppUser>();
            try
            {
                users = await _userService.GetAllUsersAsync();

            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
            //Send back an OK with 200 status code
            return Ok(new
            {
                records = users
            });


        }//End of GetUsers web api
        public async Task<IActionResult> GetAdminRoleUsers()
        {
            List<AppUser> users = new List<AppUser>();
            try
            {
                users = await _userService.GetAllUsersByRoleIdAsync(1);

            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
            //Send back an OK with 200 status code
            return Ok(new
            {
                records = users
            });


        }//End of GetAdminRoleUsers web api
        // GET api/AccountManager/GetOneUser
        [HttpGet("GetOneUser/{id}")]
        public async Task<IActionResult> GetOneUser(int id)
        {
            AppUser user = new AppUser();
            try
            {
                user = await _userService.GetUserByIdAsync(id);
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
            //Send back an OK with 200 status code
            return Ok(new
            {
                record = user
            });
        }//End of GetOneUser web api

         //GET api/AccountManager/GetRolesForInputControls
        [HttpGet("GetRolesForInputControls")]
        public IActionResult GetRolesForInputControls()
        {
            List<AppRole> users = new List<AppRole>();
            try
            {
                users = _database.AppRoles.ToList();
            }
            catch (Exception ex)
            {
                //Return error message if there was an exception
                return BadRequest(new { message = "Unable to retrieve role records." });
            }
            //Send back an OK with 200 status code
            return Ok(new
            {
                records = users
            });
        }//End of GetRolesForInputControls web api

        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [HttpPost("signup")]
        public async Task<IActionResult> SignUp([FromForm] IFormCollection inFormData)
        {
            AppUser newUser = new AppUser()
            {
                FullName = inFormData["fullName"],
                UserName = inFormData["email"],
                RoleId = 3 /* Sign Up functionality can only create PENDINGUSER role user */
            };
            try
            {
                // save 
                await _userService.CreateAsync(newUser, inFormData["password"]);
                return Ok(new
                {
                    signUpStatus = true,
                    message = "Completed instructor registration."
                });
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }


    }//end of AccountManagerController
}//end of namespace
