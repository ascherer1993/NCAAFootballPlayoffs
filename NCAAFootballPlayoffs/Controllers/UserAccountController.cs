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

        public ActionResult Index()
        {
            return View();
        }
        
        [AuthorizeUser]
        public ActionResult MyAccount()
        {
            return View();
        }

        [AuthorizeUser("Admin")]
        public ActionResult Admin()
        {
            return View();
        }

        [HttpGet]
        public ActionResult SignIn()
        {
            //If already signed in, redirect to home page
            if (Utilities.Authentication.IsSignedIn())
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        //Postback for signing in, redirects 
        [HttpPost]
        public ActionResult SignIn(SignInViewModel signInVM)
        {
            List<string> errors = new List<string>();
            if (ModelState.IsValid)
            {
                //Calles my authentication utility and adds any returned errors to my error list to be displayed
                errors.AddRange(Utilities.Authentication.SignIn(signInVM.EmailAddress, signInVM.Password));
                if (errors.Count == 0 && ModelState.IsValid)
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            foreach (string error in errors)
            {
                ModelState.AddModelError(error, error);
            }
            return View(signInVM);
        }

        //Signs t he user out and redirects them to the home page
        public ActionResult SignOut()
        {
            if (Utilities.Authentication.IsSignedIn())
            {
                Utilities.Authentication.SignOut();
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult CreateAccount()
        {
            //If already signed in, redirect to home page
            if (Utilities.Authentication.IsSignedIn())
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        //Postback for signing in, redirects 
        [HttpPost]
        public ActionResult CreateAccount(SignInViewModel signInVM)
        {
            List<string> errors = new List<string>();

            if (signInVM.Password != signInVM.ConfirmPassword)
            {
                ModelState.AddModelError("PasswordMismatch", "Your passwords did not match");
            }

            if (ModelState.IsValid)
            {
                //Calles my authentication utility and adds any returned errors to my error list to be displayed
                errors.AddRange(Utilities.Authentication.CreateAccount(signInVM));
                if (errors.Count == 0 && ModelState.IsValid)
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            foreach (string error in errors)
            {
                ModelState.AddModelError(error, error);
            }
            return View(signInVM);
        }

        /// <summary>
        /// This was a temporary method used to create my account. This will later be modified to the postback of creating an account
        /// </summary>
        /// <returns></returns>
        //public bool CreateAccount()
        //{
        //    string username = "ams0068";
        //    string password = "1234";
        //    string email = "ams0068@auburn.edu";

        //    byte[] newSalt = Authentication.GetSalt(32);
        //    byte[] passwordBytes = Encoding.UTF8.GetBytes(newSalt + password);

        //    var SHA512 = new SHA512Managed();

        //    MemoryStream stream = new MemoryStream(passwordBytes);

        //    var md5Password = SHA512.ComputeHash(stream);


        //    User newUser = new User();
        //    newUser.Salt = newSalt;
        //    newUser.PasswordHash = md5Password;
        //    newUser.EmailAddress = email;
        //    newUser.Archived = false;
        //    newUser.DisplayName = "Aaron Scherer";
        //    newUser.PermissionID = 2;



        //    db.Users.Add(newUser);

        //    UserName newUsername = new UserName();
        //    newUsername.UserNameText = username;
        //    newUsername.UserID = newUser.UserID;
        //    newUsername.Approved = false;
        //    newUsername.Archived = false;

        //    db.UserNames.Add(newUsername);
        //    db.SaveChanges();

        //    return true;
        //}
    }
}