using NCAAFootballPlayoffs.Models;
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
            if (HttpContext.Current.Session["username"] != null)
            {
                return true;
            }
            return false;
        }

        public static string GetUsername()
        {
            if (IsSignedIn())
            {
                string username = (string) HttpContext.Current.Session["username"];
                return username;
            }
            return null;
        }

        public static bool IsMemberOf(params string[] roles)
        {
            bool isMember = false;

            using (NCAAFootballPlayoffsEntities db = new NCAAFootballPlayoffsEntities())
            {
                string username = GetUsername();
                User user = db.Users.FirstOrDefault(f => f.Username == username);
                if (user == null)
                {
                    //If the user can not be found, return false, logging in has failed
                    return false;
                }

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

        public static bool SignIn(string username, string password)
        {
            using (var db = new NCAAFootballPlayoffsEntities())
            {
                //Fetch a user from the db with the username typed in
                User user = db.Users.FirstOrDefault(f => f.Username == username);
                if (user == null)
                {
                    //If the user can not be found, return false, logging in has failed
                    return false;
                }

                //Hash info found at http://stackoverflow.com/questions/4181198/how-to-hash-a-password
                //And http://stackoverflow.com/questions/11367727/how-can-i-sha512-a-string-in-c
                var SHA512 = new SHA512Managed();

                byte[] passwordBytes = Encoding.UTF8.GetBytes(user.Salt + password);
                MemoryStream stream = new MemoryStream(passwordBytes);
                var md5Password = SHA512.ComputeHash(stream);

                if (md5Password.SequenceEqual(user.PasswordHash))
                {
                    HttpContext.Current.Session["username"] = username;
                }
                else
                {
                    return false;
                }
            }

            return true;
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