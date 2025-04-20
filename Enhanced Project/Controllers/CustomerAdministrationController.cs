using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using SNHU_Capstone_Project.Models.Entities;
using SNHU_Capstone_Project.Models.Handlers;
using System;

namespace SNHU_Capstone_Project.Controllers
{
    [Authorize(Policy = "PageCustomerAdministration")]
    public class CustomerAdministrationController : Controller
    {
        //  Private class variables
        private readonly ILogger<CustomerAdministrationController> _logger;
        private readonly IConfiguration _configuration;
        private readonly MSSQLDatabaseContext _MSSQLDatabaseContext;

        //  Class constructor
        public CustomerAdministrationController(ILogger<CustomerAdministrationController> logger, IConfiguration configuration, MSSQLDatabaseContext MSSQLDatabaseContext)
        {
            _logger = logger;
            _configuration = configuration;
            _MSSQLDatabaseContext = MSSQLDatabaseContext;
        }

        /// <summary>
        /// Default view, displays a list of customers and their chosen plan type
        /// </summary>
        /// <returns>IActionResult of the rendered view</returns>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Utility IActionResult. Used by the default fiew to fetch all or a subset
        /// of custoemrs based on the search query string.
        /// </summary>
        /// <returns>IActionResult of raw table content</returns>
        [HttpGet]
        public IActionResult Search()
        {

            //  Initialize an empty holding object for the results of the search query
            var customers = new List<Customer>();

            
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
                    case "AddedASC":
                        customers = _MSSQLDatabaseContext.Customers.Where(x => x.Name.Contains(searchStr)).OrderBy(x => x.Added).ToList();
                        break;

                    case "AddedDEC":
                        customers = _MSSQLDatabaseContext.Customers.Where(x => x.Name.Contains(searchStr)).OrderByDescending(x => x.Added).ToList();
                        break;

                    case "NameASC":
                        customers = _MSSQLDatabaseContext.Customers.Where(x => x.Name.Contains(searchStr)).OrderBy(x => x.Name).ToList();
                        break;

                    case "NameDEC":
                        customers = _MSSQLDatabaseContext.Customers.Where(x => x.Name.Contains(searchStr)).OrderByDescending(x => x.Name).ToList();
                        break;

                    case "ServiceASC":
                        customers = _MSSQLDatabaseContext.Customers.Where(x => x.Name.Contains(searchStr)).OrderBy(x => x.ChosenService).ToList();
                        break;

                    case "ServiceDEC":
                        customers = _MSSQLDatabaseContext.Customers.Where(x => x.Name.Contains(searchStr)).OrderByDescending(x => x.ChosenService).ToList();
                        break;

                    case "DEC":
                        customers = _MSSQLDatabaseContext.Customers.Where(x => x.Name.Contains(searchStr)).OrderByDescending(x => x.Id).ToList();
                        break;

                    default:
                        //  Lookup the query in the database
                        customers = _MSSQLDatabaseContext.Customers.Where(x => x.Name.Contains(searchStr)).OrderBy(x => x.Id).ToList();
                        break;
                }
                    

                //  Return
                return View(customers);
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "An error occured attempting to fetch customers from the database");

            }

            //  Return a 0 value as a failure has occured
            return Content("0");

        }

        [HttpPost, Authorize(Policy = "CreateCustomers")]
        public IActionResult Add(Customer customer)
        {

            try
            {

                //  Attempt to update the customer
                customer.Added = DateTime.Now;
                _MSSQLDatabaseContext.Customers.Add(customer);
                _MSSQLDatabaseContext.SaveChanges();

            }
            catch (Exception ex)
            {

                //  Log the error and redirect the customer to the custoemr index page
                _logger.LogError(ex, "An error occured attempting to add a customer to the database");
                return RedirectToAction("Index");

            }

            //  Return the user to the customers page
            return RedirectToAction("Index");

        }

        [HttpGet]
        public IActionResult Edit()
        {
            
            //  Initialize objects for the customer lookup query
            var customer = new Customer();
            Guid customerID;

            
            //  Fetch the search parameter from the request
            string? customerId = Request.Query["Id"];
            

            //  Handle an invalid result
            if (customerId == null || !Guid.TryParse(customerId, out customerID))
            {

                //  Redirect the user to the customer index page
                return RedirectToAction("Index");

            } 


            try
            {

                //  Lookup the customer ID in the database
                customer = _MSSQLDatabaseContext.Customers.First(x => x.Id.Equals(customerID));

            }
            catch (Exception ex)
            {

                //  Log the error and redirect the customer to the custoemr index page
                _logger.LogError(ex, "An error occured attempting to fetch a customer from the database");
                return RedirectToAction("Index");

            }

            //  Return
            return View(customer);

        }

        [HttpPost, Authorize(Policy = "EditCustomers")]
        public IActionResult Edit(Customer customer)
        {

            try
            {

                //  Attempt to update the customer
                _MSSQLDatabaseContext.Customers.Update(customer);
                _MSSQLDatabaseContext.SaveChanges();

            }
            catch (Exception ex)
            {

                //  Log the error and redirect the customer to the custoemr index page
                _logger.LogError(ex, "An error occured attempting to update a customer in the database with ID " + customer.Id);
                return RedirectToAction("Index");

            }

            //  Return user to the customer administration page
            return RedirectToAction("Index");

        }

        [HttpPost, Authorize(Policy = "DeleteCustomers")]
        public IActionResult Delete(Customer customer)
        {

            //  Delete the customer
            try
            {

                _MSSQLDatabaseContext.Customers.Remove(customer);
                _MSSQLDatabaseContext.SaveChanges();

            }
            catch (Exception ex)
            {

                //  Log the error and redirect the customer to the custoemr index page
                _logger.LogError(ex, "An error occured attempting to delete a customer from the database");

            }

            //  Return
            return RedirectToAction("Index");

        }

    }
}
