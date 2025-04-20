using Azure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SNHU_Capstone_Project.Models.Entities;

namespace SNHU_Capstone_Project.Models.Handlers
{
    public sealed class MSSQLAuthenticationHandler : DefaultAuthenticationHandler
    {

        //  Class variables
        private readonly MSSQLDatabaseContext _dbContext;

        //  Default constructor
        public MSSQLAuthenticationHandler(MSSQLDatabaseContext databaseContext)
        {

            //  Save database context to this handler instance
            _dbContext = databaseContext;

        }

        //  Login method for MSSQL
        public User MSSQLLogin(User user)
        {
            try
            {
                //  Lookup user in the database
                var UnvalidatedUser = _dbContext.Users.First(u => u.Username == user.Username || u.Email == user.Username);

                //  Validate that the user was found
                if (UnvalidatedUser != null)
                {

                    //  Validate Password
                    if (UnvalidatedUser.ValidatePassword(user.Password))
                    {

                        //  Update last login
                        UnvalidatedUser.LastLogin = DateTime.Now;
                        _dbContext.Users.Update(UnvalidatedUser);
                        _dbContext.SaveChanges();

                        return UnvalidatedUser;

                    }

                }

            }
            catch (Exception)
            {
                throw;
            }

            //  Failed to authenticate. Return failure
            throw new AuthenticationFailedException("Could not authenticate this user. Invalid username, password or database error.");
        }


        //  Method for creating a new user
        public bool AddUser(User user)
        {

            try
            {

                //  Ensure that the username is valid
                isValidUsername(user.Username);

                //  Ensure that the username does not already exist
                if (_dbContext.Users.Any(u => u.Username.Equals(user.Username)))
                {
                    throw new Exception("Invalid Username Exception: The supplied username is already in use");
                }

                //  Ensure that the password meets the requirements
                var userPasswordRequirements = new UserPasswordRequirements();
                userPasswordRequirements.ValidatePassword(user.Password);

                //  Encrypt the password and save it to the user object
                PasswordHasher<User> passwordHasher = new PasswordHasher<User>();
                user.Password = passwordHasher.HashPassword(user, user.Password);

                //  Set the account to the default state (active)
                user.Status = User.UserStatus.Active;

            }
            catch (Exception)
            {
                throw;
            }


            //  The user passed the validation checks. Push to the database
            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();


            //  Assume sucessful completion
            return true;

        }


        //  Method for updating a user password
        public bool UpdateUserPassword(User user)
        {

            try
            {

                //  Ensure that the password meets the requirements
                var userPasswordRequirements = new UserPasswordRequirements();
                userPasswordRequirements.ValidatePassword(user.Password);

                //  Encrypt the password and save it to the user object
                PasswordHasher<User> passwordHasher = new PasswordHasher<User>();
                user.Password = passwordHasher.HashPassword(user, user.Password);

                var dbUser = _dbContext.Users.First(x => x.Id.Equals(user.Id));

                //  The password passed the validation checks. Push to the database
                dbUser.Password = user.Password;
                _dbContext.Users.Update(dbUser);
                _dbContext.SaveChanges();

                //  Assume sucessful completion
                return true;

            }
            catch (Exception)
            {
                throw;
            }

        }


        //  Method for updating an existing user
        public bool UpdateUser(User user)
        {

            //  Validate that the Id provided is valid
            if (!user.Id.Equals(null))
            {

                //  Lookup the user in the database
                var dbUser = _dbContext.Users.Find(user.Id);


                //  Check that a valid user was found
                if (dbUser != null)
                {

                    //  Signify to the context to track changes
                    _dbContext.Users.Attach(dbUser);


                    //  Make the possible changes to the user
                    dbUser.Email = user.Email;
                    
                    //  Apply a user status change. Preserves the current status of the user
                    if (dbUser.Status != user.Status)
                    {
                        
                        dbUser.Status = user.Status;
                        
                    }


                    //  Validate a valid username change
                    if (!dbUser.Username.Equals(user.Username))
                    {

                        //  Ensure that the username is valid
                        isValidUsername(user.Username);

                        //  Ensure that the username does not already exist
                        if (_dbContext.Users.Any(u => u.Username.Equals(user.Username)))
                        {
                            throw new Exception("Invalid Username Exception: The supplied username is already in use");
                        }

                    }


                    //  Update permissions
                    dbUser.canRead = user.canRead;
                    dbUser.canUpdate = user.canUpdate;
                    dbUser.canDelete = user.canDelete;
                    dbUser.canCreate = user.canCreate;
                    dbUser.isAdmin = user.isAdmin;


                    //  Push the changes to the database
                    _dbContext.SaveChanges();

                }
                else
                {
                    throw new Exception(string.Format("Invalid GUID Exception: A request to render the EditUser page failed because the supplied GUID {0} could not be found in the database", user.Id.ToString()));
                }

            }
            else
            {
                throw new Exception(string.Format("Invalid GUID Exception: A request to render the EditUser page failed because the supplied GUID {0} was invalid", user.Id.ToString()));
            }

            //  Assume sucessful completion
            return true;

        }


        //  Method for getting the user from the database from its ID
        public User GetUser(Guid guid)
        {

            //  Lookup the user in the database
            var user = _dbContext.Users.Find(guid);


            //  Check that a valid user was found
            if (user == null)
            {

                //  A user could not be found
                throw new Exception(string.Format("Invalid GUID Exception: A user with the GUID {0} could not be found", guid.ToString()));

            }
            else
            {
                return user;
            }

        }

    }
}
