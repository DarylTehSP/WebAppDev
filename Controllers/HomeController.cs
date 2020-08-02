using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TMS.Models;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using TMS.Services;
using TMS.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace TMS.Controllers
{
    public class HomeController : Controller
    {
        private IUserService _userService;
        private readonly AppSettings _appSettings;


        public HomeController(
                  IUserService userService,
                  IOptions<AppSettings> appSettings)
        {
            _userService = userService;
            _appSettings = appSettings.Value;
        }
        public IActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        [ActionName("Login")]
        public IActionResult Login()
        {
            return View();
        }
		//The online article shares basic concept of antiforgerytoken and CSRF
		/*http://prideparrot.com/blog/archive/2012/7/securing_all_forms_using_antiforgerytoken*/

		[AllowAnonymous]
        [ValidateAntiForgeryToken]
		[HttpPost("Login")]
        public async Task<IActionResult> Login([FromForm] IFormCollection data)
        {//Referred to the code at this GitHub repo
         //https://github.com/minato128/aspnet-core20-auth-sample/blob/master/WebApplication6/Controllers/HomeController.cs
             //I am avoiding the usage of the ViewModel. You can go through the online
             //tutorials to replace the logic here by using ViewModel.
            if( (data["passwordInput"].ToString().Trim()=="")||(data["usernameInput"].ToString().Trim()==""))
            {
                //Make a ViewBag
                //ViewBag lifecycle only last for this request cycle.
                ViewBag.Message = "User name or password is missing";
                return View();
            }
			//Try remove await keyword from the expression below.
			//You will see error highlights at the var claimsIdentity section....
            var user = await _userService.AuthenticateAsync(data["usernameInput"], data["passwordInput"]);

            if (user == null)
            {
                //Make a ViewBag
                ViewBag.Message = "User name or password is wrong";
                return View();
            }

            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);

            /********************************************************************/
            //The following block of code has been added to support token storage in Browser cookie
            var claimsIdentity = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, user.Id.ToString() ),
                new Claim("username", user.UserName.ToString()),
                new Claim("fullName", user.FullName.ToString()),
                new Claim("userid", user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role.RoleName)
             }, "CookieAuthenticationScheme");

            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            await Request.HttpContext.SignInAsync("CookieAuthenticationScheme", claimsPrincipal);
            /********************************************************************/
            
            if (user.Role.RoleName.Contains("ADMIN")) {
                return Redirect("/CustomerAccounts/Index");
            }
            if (user.Role.RoleName.Contains("INSTRUCTOR"))
            {
                return Redirect("/InstructorHome/Index");
            }
            if (user.Role.RoleName.Contains("PENDINGUSER"))
            {
                return Redirect("/PendingUserHome/Index");
            }
            return Redirect("/");
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("CookieAuthenticationScheme");
            return Redirect("/");
        }
        public IActionResult Forbidden()
        {
            return View();
        }
        public IActionResult Register()
        {
            return View();
        }

    }
}
