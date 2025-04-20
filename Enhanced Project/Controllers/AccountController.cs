using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using SNHU_Capstone_Project.Models;
using SNHU_Capstone_Project.Models.Entities;
using SNHU_Capstone_Project.Models.Handlers;
using System.Security.Claims;
using static SNHU_Capstone_Project.Models.Entities.User;

namespace SNHU_Capstone_Project.Controllers
{
    public class AccountController : Controller
    {

        private readonly ILogger<AccountController> _logger;
        private readonly IConfiguration _configuration;
        private readonly MSSQLDatabaseContext _MSSQLDatabaseContext;

        public AccountController(ILogger<AccountController> logger, IConfiguration configuration, MSSQLDatabaseContext MSSQLDatabaseContext)
        {
            _logger = logger;
            _configuration = configuration;
            _MSSQLDatabaseContext = MSSQLDatabaseContext;
        }

        [Authorize(Policy = "UserIsAdministrator")]
        public IActionResult Administration()
        {
            return View(new User());
        }

        [HttpGet, Authorize(Policy = "UserIsAdministrator")]
        public IActionResult Search()
        {

            //  Initialize an empty holding object for the results of the search query
            var users = new List<User>();


            //  Fetch the search parameters from the request
            string? searchStr = Request.Query["searchQuery"];
            string? meta = Request.Query["meta"];

            //  Handle null query parameters
            if (searchStr == null)
            {
                searchStr = string.Empty;
            }

            if (meta == null)
            {
                meta = string.Empty;
            }


            try
            {

                //  adjust query for supported sort options
                switch (meta)
                {
                    default:
                        //  Lookup the query in the database
                        users = _MSSQLDatabaseContext.Users.Where(x => x.Username.Contains(searchStr) || x.Email.Contains(searchStr)).OrderBy(x => x.Id).ToList();
                        break;
                }


                //  Return
                return View(users);
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "An error occured attempting to fetch users from the database");

            }

            //  Return a 0 value as a failure has occured
            return Content("0");

        }

        [HttpPost, Authorize(Policy = "UserIsAdministrator")]
        public IActionResult Add(User user)
        {

            //  Return error messages to the user is there is issues with the form
            if (!ModelState.IsValid) return View("Administration", user);

            try
            {

                //  Attempt to add the user
                var j = new MSSQLAuthenticationHandler(_MSSQLDatabaseContext);
                j.AddUser(user);

            }
            catch (Exception ex)
            {

                //  Log the error and redirect the user to the Administration page
                _logger.LogError(ex, "An error occured attempting to add a user to the database");

                //  Package the message for the user
                ModelState.AddModelError("", ex.Message.Substring(ex.Message.IndexOf(": ") + 2));

                //  Return the view with errors
                return View("Administration", user);

            }

            //  Return the user to the user administration page
            return RedirectToAction("Administration");

        }

        [HttpGet, Authorize(Policy = "UserIsAdministrator")]
        public IActionResult Edit()
        {

            //  Initialize objects for the User lookup query
            var user = new User();
            Guid userID;


            //  Fetch the search parameter from the request
            string? userId = Request.Query["Id"];


            //  Handle an invalid result
            if (userId == null || !Guid.TryParse(userId, out userID))
            {

                //  Redirect the user to the Administration page
                return RedirectToAction("Administration");

            }


            try
            {

                //  Lookup the user ID in the database
                user = _MSSQLDatabaseContext.Users.First(x => x.Id.Equals(userID));

            }
            catch (Exception ex)
            {

                //  Log the error and redirect the user to the Administration page
                _logger.LogError(ex, "An error occured attempting to fetch a user from the database");
                return RedirectToAction("Administration");

            }

            //  Return
            return View(user);

        }

        [HttpPost, Authorize(Policy = "UserIsAdministrator")]
        public IActionResult Edit(User user)
        {

            //  Handle account status input
            var userStatus = Request.Form["ActiveStatus"];

            if (!userStatus.IsNullOrEmpty() && !userStatus.Equals("false"))
            {

                user.Status = UserStatus.Active;

            }
            else
            {
                user.Status = UserStatus.Inactive;
            }

            //  Initialize variables
            var j = new MSSQLAuthenticationHandler(_MSSQLDatabaseContext);

            try
            {

                //  Update the user
                j.UpdateUser(user);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }


            //  Return
            return RedirectToAction("Administration");

        }

        [HttpPost, Authorize(Policy = "UserIsAdministrator")]
        public IActionResult Delete(User user)
        {

            //  Delete the user
            try
            {

                _MSSQLDatabaseContext.Users.Remove(user);
                _MSSQLDatabaseContext.SaveChanges();

            }
            catch (Exception ex)
            {

                //  Log the error and redirect the user to the index page
                _logger.LogError(ex, "An error occured attempting to delete a user from the database");

            }

            //  Return
            return RedirectToAction("Administration");

        }

        public new IActionResult Unauthorized()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(User User)
        {

            //  Return error messages to the user is there is issues with the form
            if (!ModelState.IsValid) return View(User);


            //  Initialize the user authentication handler
            MSSQLAuthenticationHandler userAuthenticationHandler = new MSSQLAuthenticationHandler(_MSSQLDatabaseContext);

            //  Attempt MSSQL authentication
            try
            {
                User = userAuthenticationHandler.MSSQLLogin(User);

                if (User.Status.Equals(User.UserStatus.Active))
                {

                    //  Create base security context
                    var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, User.Username),
                            new Claim(ClaimTypes.Sid, User.Id.ToString())
                        };

                    
                    //  Attach user-specific permissions to the cookie package
                    if (User.isAdmin)
                    {
                        claims.Add(new Claim("Administrator", "True"));
                    }
                    if (User.canRead)
                    {
                        claims.Add(new Claim("PageCustomerAdministration", "True"));
                    }
                    if (User.canUpdate)
                    {
                        claims.Add(new Claim("PermissionUpdate", "True"));
                    }
                    if (User.canCreate)
                    {
                        claims.Add(new Claim("PermissionCreate", "True"));
                    }
                    if (User.canDelete)
                    {
                        claims.Add(new Claim("PermissionDelete", "True"));
                    }


                    var identity = new ClaimsIdentity(claims, "LoginCookieAuth");
                    ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(identity);

                    await HttpContext.SignInAsync("LoginCookieAuth", claimsPrincipal);

                    //  Send user to the home page
                    return RedirectToAction("Index", "Home");

                }
                else
                {

                    // Throw some error about an inactive account

                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unable to authenticate user: '{User.Username}' using the MSSQL database. Error: {ex}");
            }
            

            //  Attempt to authenticate with the default user account
            if (userAuthenticationHandler.DefaultAdminLogin(_configuration, User))
            {
                //  Create security context
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, User.Username),
                    new Claim("Administrator", "True")
                };

                var identity = new ClaimsIdentity(claims, "LoginCookieAuth");
                ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync("LoginCookieAuth", claimsPrincipal);

                //  Send user to the home page
                return RedirectToAction("Index", "Home");
            }

            return View(User);
        }

        public async Task<IActionResult> Logout ()
        {
            await HttpContext.SignOutAsync("LoginCookieAuth");
            //  Send user to the home page
            return RedirectToAction("Index", "Home");
        }

        [HttpGet, Authorize(Policy = "UserIsAdministrator")]
        public IActionResult ChangeUserPassword ()
        {
            //  Initialize objects for the User lookup query
            var user = new User();
            Guid userID;


            //  Fetch the search parameter from the request
            string? userId = Request.Query["Id"];


            //  Handle an invalid result
            if (userId == null || !Guid.TryParse(userId, out userID))
            {

                //  Redirect the user to the Administration page
                return RedirectToAction("Administration");

            }


            try
            {

                //  Lookup the user ID in the database
                user = _MSSQLDatabaseContext.Users.First(x => x.Id.Equals(userID));

            }
            catch (Exception ex)
            {

                //  Log the error and redirect the user to the Administration page
                _logger.LogError(ex, "An error occured attempting to fetch a user from the database");
                return RedirectToAction("Administration");

            }

            //  Return
            return View(user);
        }

        [HttpGet, Authorize(Policy = "UserIsAdministrator")]
        public IActionResult Add()
        {

            //  Create an empty user object for the new user
            var user = new User();

            //  Render the view
            return View(user);

        }

        [HttpPost, Authorize(Policy = "UserIsAdministrator")]
        public IActionResult ChangeUserPassword(User user)
        {

            //  Load variables
            var authHandler = new MSSQLAuthenticationHandler(_MSSQLDatabaseContext);
            
            //  Update the user
            try
            {

                authHandler.UpdateUserPassword(user);

            }
            catch (Exception ex)
            {

                //  Log the error and redirect the User to the Administration page
                _logger.LogError(ex, "An error occured attempting to update a user password in the database");
                return RedirectToAction("Administration");

            }

            //  Return
            return RedirectToAction("Administration");
        }
    }
}
