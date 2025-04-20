using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace SNHU_Capstone_Project.Models.Entities
{
    public class User
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required(ErrorMessage = "Invalid Username")]
        public string Username { get; set; } = "";
        [Required(ErrorMessage = "Invalid Password")]
        public string Password { get; set; } = "";
        public string? Email { get; set; } = string.Empty;
        public UserStatus Status { get; set; } = UserStatus.NotAuthenticated;
        public DateTime LastLogin { get; set; }
        public bool isAdmin { get; set; } = false;
        public bool canCreate { get; set; } = false;
        public bool canRead { get; set; } = false;
        public bool canUpdate { get; set; } = false;
        public bool canDelete { get; set; } = false;


        /// <summary>
        /// Accepts a string and checks to see if it matches the encrypted password.
        /// </summary>
        /// <returns>True / False determination of a password match</returns>
        public bool ValidatePassword(string UnvalidatedPassword)
        {
            PasswordHasher<User> passwordHasher = new PasswordHasher<User>();

            if (Password == null)
            {
                return true;
            }

            var result = passwordHasher.VerifyHashedPassword(this, Password, UnvalidatedPassword);

            if (result == PasswordVerificationResult.Success) return true;

            if (result == PasswordVerificationResult.SuccessRehashNeeded)
            {
                Password = passwordHasher.HashPassword(this, UnvalidatedPassword);
                return true;
            }

            return false;
        }

        public enum UserStatus
        {
            Inactive = 0,
            Active = 1,
            NotAuthenticated = 2
        }
    }

    public sealed class UserPasswordRequirements
    {

        public int MinLength { get; set; } = 8;
        public int MaxLength { get; set; } = 255;
        public int MinSpecialCharacters { get; set; } = 1;
        public int MinCapitalLetters { get; set; } = 1;
        public int MinNumericCharacters { get; set; } = 1;
        public int HistoryRemembered { get; set; } = 1;
        public List<char> BlacklistedCharacters { get; set; } = new List<char>() { '(', ')', '{', '}', '[', ']', '`', '"', ';', };


        //  Function checks the supplied string to determine if it meets the password requirements
        public void ValidatePassword(string password)
        {

            //  Validate length
            if (password.Length < MinLength || password.Length > MaxLength)
            {
                throw new Exception(string.Format("Invalid Password Exception: Password is more than {0} characters or less than {1} characters long", MaxLength, MinLength));
            }


            //  Count the special, upper and numeric characters in the string
            int s = 0;
            int u = 0;
            int n = 0;

            foreach (char c in password)
            {

                //  Special character counter
                if (!char.IsAsciiLetterOrDigit(c))
                {
                    s++;
                }

                //  Upper character counter
                if (char.IsAsciiLetterUpper(c))
                {
                    u++;
                }

                //  Numeric character counter
                if (char.IsAsciiDigit(c))
                {
                    n++;
                }

            }

            //  Validate special characters
            if (s < MinSpecialCharacters)
            {
                throw new Exception(string.Format("Invalid Password Exception: Password does not contain at least {0} special characters", MinSpecialCharacters));
            }

            //  Valdiate upper requirement
            if (u < MinCapitalLetters)
            {
                throw new Exception(string.Format("Invalid Password Exception: Password does not contain at least {0} upper case characters", MinCapitalLetters));
            }

            //  Validate numeric requirement
            if (n < MinNumericCharacters)
            {
                throw new Exception(string.Format("Invalid Password Exception: Password does not contain at least {0} numeric characters", MinNumericCharacters));
            }

        }

    }

}
