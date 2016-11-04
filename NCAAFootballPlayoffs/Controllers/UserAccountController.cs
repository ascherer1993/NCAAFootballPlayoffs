using NCAAFootballPlayoffs.Models;
using NCAAFootballPlayoffs.Models.ViewModels;
using NCAAFootballPlayoffs.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace NCAAFootballPlayoffs.Controllers
{
    public class UserAccountController : Controller
    {
        NCAAFootballPlayoffsEntities db = new NCAAFootballPlayoffsEntities();

        // GET: UserAccount
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult SignIn()
        {
            if (Utilities.Authentication.IsSignedIn())
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public ActionResult SignIn(SignInViewModel signInVM)
        {
            bool successfulSignIn = Utilities.Authentication.SignIn(signInVM.UsernameOrEmail, signInVM.Password);
            if (successfulSignIn)
            {
                return RedirectToAction("Index", "Home");
            }
            return View(signInVM);
        }

        public bool CreateAccount()
        {
            string username = "ascherer1993";
            string password = "ams1990";
            string email = "ascherer1993@gmail.com";

            byte[] newSalt = Authentication.GetSalt(32);

            byte[] passwordBytes = Encoding.UTF8.GetBytes(newSalt + password);

            //Hash info found at http://stackoverflow.com/questions/4181198/how-to-hash-a-password
            var SHA512 = new SHA512Managed();


            MemoryStream stream = new MemoryStream(passwordBytes);

            var md5Password = SHA512.ComputeHash(stream);


            User newUser = new User();
            newUser.Username = username;
            newUser.Salt = newSalt;
            newUser.PasswordHash = md5Password;
            newUser.EmailAddress = email;
            newUser.Archived = false;
            newUser.DisplayName = "Aaron Scherer";
            newUser.PermissionID = 1;

            db.Users.Add(newUser);
            db.SaveChanges();

            return true;
        }
    }
}