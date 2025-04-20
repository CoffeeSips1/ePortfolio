using Microsoft.EntityFrameworkCore;
using SNHU_Capstone_Project.Models.Entities;
using System.Security.Permissions;

namespace SNHU_Capstone_Project.Models.Handlers
{
    public class MSSQLDatabaseInitializer
    {
        //  Class variables
        private static readonly string seedCustomersFilePath = "./Data/SeedCustomers.json";

        public static void Initialize(MSSQLDatabaseContext databaseContext)
        {
            //  Check to see if the database has been created
            databaseContext.Database.Migrate();

            //  Seed tables..
            try
            {
                InitializeCustomers(databaseContext, seedCustomersFilePath);
            }
            catch (Exception)
            {
                throw;
            }
            
        }

        /// <summary>
        /// Populates an empty customers table with seed data stored in a JSON file.
        /// </summary>
        /// <param name="databaseContext">MSSQLDatabaseContext object instance</param>
        public static void InitializeCustomers(MSSQLDatabaseContext databaseContext, string seedFile)
        {
            //  Check if there is any sample data in the database
            if (databaseContext.Customers.Any())
            {
                //  There is already data populated in the database. Example data is not needed.
                return;
            }

            //  Load a sample set of data
            string serializedCustomerList = File.ReadAllText(seedFile);
            var customerList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Customer>>(serializedCustomerList);

            //  Populate the database with seed data
            if (customerList != null)
            {
                foreach (var i in customerList)
                {
                    databaseContext.Add(i);
                }
                databaseContext.SaveChanges();
            }
            else
            {
                throw new Exception("Initialization error: Could not seed the database from the JSON object: " + seedFile + ".");
            }

        }

    }
}
