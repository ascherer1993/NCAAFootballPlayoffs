using NCAAFootballPlayoffs.Models;
using NCAAFootballPlayoffs.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace NCAAFootballPlayoffs.Utilities
{
    public class Authentication
    {
        public static bool IsSignedIn()
        {
            //if (HttpContext.Current.Session["username"] != null)
            //{
            //    return true;
            //}
            if (HttpContext.Current.Session["loginEmail"] != null)
            {
                return true;
            }
            return false;
        }

        public static string GetLoginInfo()
        {
            if (IsSignedIn())
            {
                //string username = (string) HttpContext.Current.Session["username"];
                string username = (string)HttpContext.Current.Session["loginEmail"];
                return username;
            }
            return null;
        }


        //Checks to see if the current user is a member of one of the roles received as a parameter.
        public static bool IsMemberOf(params string[] roles)
        {
            bool isMember = false;

            using (NCAAFootballPlayoffsEntities db = new NCAAFootballPlayoffsEntities())
            {
                string emailAddress = GetLoginInfo();
                User user = db.Users.FirstOrDefault(f => f.EmailAddress == emailAddress);
                if (user == null)
                {
                    //If the user can not be found, return false, logging in has failed
                    return false;
                }

                //Checks each role to make sure the user is one of the required roles
                foreach (string role in roles)
                {
                    switch (role)
                    {
                        case "Admin":
                            if (user.Permission.PermissionName == "Admin")
                            {
                                isMember = true;
                            }
                            break;
                        case "User":
                            if (user.Permission.PermissionName == "User")
                            {
                                isMember = true;
                            }
                            break;
                        default:
                            break;
                    }
                }
            }

            return isMember;
        }

        //signs the user in if their log in credentials are correct
        public static List<string> SignIn(string email, string password)
        {
            email = email.ToLower();
            password = password.ToLower();

            List<string> errors = new List<string>();
            using (var db = new NCAAFootballPlayoffsEntities())
            {
                //Fetch a user from the db with the username typed in
                User user = db.Users.FirstOrDefault(f => f.EmailAddress.ToLower() == email);
                if (user == null)
                {
                    //If the user can not be found, a error is added to the list of errors
                    errors.Add("The username or email address you have entered is invalid.");
                    
                }
                if (user != null)
                {
                    //Hash info found at http://stackoverflow.com/questions/4181198/how-to-hash-a-password
                    //And http://stackoverflow.com/questions/11367727/how-can-i-sha512-a-string-in-c
                    var SHA512 = new SHA512Managed();

                    byte[] passwordBytes = Encoding.UTF8.GetBytes(user.Salt + password);
                    MemoryStream stream = new MemoryStream(passwordBytes);
                    var md5Password = SHA512.ComputeHash(stream);

                    if (md5Password.SequenceEqual(user.PasswordHash))
                    {
                        HttpContext.Current.Session["loginEmail"] = user.EmailAddress;
                    }
                    else
                    {
                        errors.Add("The password you have entered is invalid.");
                    }
                }
            }

            return errors;
        }
        //Signs the user out
        public static bool SignOut()
        {
            HttpContext.Current.Session["loginEmail"] = null;
            return true;
        }


        public static List<string> CreateAccount(SignInViewModel signInVM)
        {
            signInVM.EmailAddress = signInVM.EmailAddress.ToLower();
            signInVM.Password = signInVM.Password.ToLower();

            List<string> errors = new List<string>();
            using (var db = new NCAAFootballPlayoffsEntities())
            {
                //Fetch a user from the db with the username typed in
                User user = db.Users.FirstOrDefault(f => f.EmailAddress.ToLower() == signInVM.EmailAddress);
                if (user != null)
                {
                    //If the user can not be found, a error is added to the list of errors
                    errors.Add("An account with this email address alread exists.");

                }
                else
                {
                    //Hash info found at http://stackoverflow.com/questions/4181198/how-to-hash-a-password
                    //And http://stackoverflow.com/questions/11367727/how-can-i-sha512-a-string-in-c

                    byte[] newSalt = Authentication.GetSalt(32);
                    byte[] passwordBytes = Encoding.UTF8.GetBytes(newSalt + signInVM.Password);

                    var SHA512 = new SHA512Managed();

                    MemoryStream stream = new MemoryStream(passwordBytes);

                    var md5Password = SHA512.ComputeHash(stream);


                    User newUser = new User();
                    newUser.Salt = newSalt;
                    newUser.PasswordHash = md5Password;
                    newUser.EmailAddress = signInVM.EmailAddress;
                    newUser.Archived = false;
                    newUser.DisplayName = signInVM.DisplayName;
                    newUser.PermissionID = 2;
                    db.Users.Add(newUser);

                    Username newUsername = new Username();
                    newUsername.UsernameText = signInVM.BracketName;
                    newUsername.UserID = newUser.UserID;
                    newUsername.Approved = false;
                    newUsername.Archived = false;

                    db.Usernames.Add(newUsername);
                    db.SaveChanges();

                    SignIn(signInVM.EmailAddress, signInVM.Password);
                }
            }

            return errors;
        }




        //Got from http://codereview.stackexchange.com/questions/93614/salt-generation-in-c
        public static byte[] GetSalt(int maximumSaltLength)
        {
            var salt = new byte[maximumSaltLength];
            using (var random = new RNGCryptoServiceProvider())
            {
                random.GetNonZeroBytes(salt);
            }

            return salt;
        }
    }

}