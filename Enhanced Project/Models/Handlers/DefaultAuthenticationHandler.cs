using Microsoft.AspNetCore.Identity;
using SNHU_Capstone_Project.Models.Entities;

namespace SNHU_Capstone_Project.Models.Handlers
{
    public class DefaultAuthenticationHandler
    {

        //  Login method to use default account stored in config file
        public bool DefaultAdminLogin(IConfiguration configuration, User user)
        {

            //  Initialize variables
            var passwordHasher = new PasswordHasher<User>();
            var configFilePath = "appSettings.json";

            try
            {

                //  Get the values from the configuration file
                var ConfigUsername = configuration.GetValue<string>("DefaultUserName");
                var ConfigPassword = configuration.GetValue<string>("DefaultUserPassword");
                bool ConfigEncrypted;
                if (!bool.TryParse(configuration.GetValue<string>("DefaultUserPasswordEncrypted"), out ConfigEncrypted))
                {
                    //  Default assumption
                    ConfigEncrypted = false;
                }


                //  Check if using encryption
                if (!ConfigEncrypted)
                {

                    //  Encrypt the password in the config file
                    //  Get the contents of the config file
                    string json = File.ReadAllText(configFilePath);

                    //  Load the contents into a dynamic json object
                    dynamic? jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
                    if (jsonObj == null)
                    {
                        throw new Exception("Initialization error: Could not initialize the JSON object when parsing the configuration file");
                    }

                    //  Encrypt the password
                    jsonObj["DefaultUserPassword"] = ConfigPassword = passwordHasher.HashPassword(user, ConfigPassword);
                    jsonObj["DefaultUserPasswordEncrypted"] = "True";

                    //  Save to file
                    string output = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);
                    File.WriteAllText(configFilePath, output);

                }

                //  Validate that the supplied username matches
                if (user.Username.Equals(ConfigUsername))
                {

                    //  Check the supplied password against the stored password
                    var validationResult = passwordHasher.VerifyHashedPassword(user, ConfigPassword, user.Password);

                    //  Validation sucess!
                    if (validationResult == PasswordVerificationResult.Success)
                    {
                        user.Password = ConfigPassword;
                        return true;
                    }

                    //  Rehash the password because the current hash is out of date
                    else if (validationResult == PasswordVerificationResult.SuccessRehashNeeded)
                    {
                        //  Get the contents of the config file
                        string json = File.ReadAllText(configFilePath);

                        //  Load the contents into a dynamic json object
                        dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(json);

                        //  Encrypt the password
                        jsonObj["DefaultUserPassword"] = ConfigPassword = passwordHasher.HashPassword(user, user.Password);
                        jsonObj["DefaultUserPasswordEncrypted"] = "True";
                        user.Password = ConfigPassword;

                        //  Save to file
                        string output = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);
                        File.WriteAllText(configFilePath, output);

                        return true;
                    }

                }

            }
            catch (Exception) { throw; }

            //  Default return nonsucessful authentication attempt
            return false;

        }

        //  Check a provided username to and determines of it is valid
        public void isValidUsername(string username)
        {

            //  Ensure that the username is not empty
            if (username == null)
            {
                throw new Exception("Invalid Username Exception: Username cannot be null");
            }

            //  Ensure that the username is valid with a length less than 21
            if (username.Length > 20)
            {
                throw new Exception("Invalid Username Exception: Username cannot be more than 20 characters long");
            }

            //  Ensure that the username is valid and does not contain whitespace
            if (username.Contains(" "))
            {
                throw new Exception("Invalid Username Exception: Username cannot contain whitespace");
            }

            //  Ensure that the username is valid and only contains alphanumeric, hyphen or period characters
            foreach (char c in username)
            {
                if (!(char.IsAsciiLetterOrDigit(c) || c.Equals('.') || c.Equals('-') || c.Equals('_')))
                {
                    throw new Exception("Invalid Username Exception: Username must only contain alphanumeric characters or the following aproved special characters: - . _");
                }
            }

        }

    }
}
