using NCAAFootballPlayoffs.Models;
using NCAAFootballPlayoffs.Models.ViewModels;
using NCAAFootballPlayoffs.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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

        [AuthorizeUser("Admin")]
        public ActionResult Index()
        {
            IEnumerable<Username> usernames = db.Usernames.Where(f => !f.Archived).OrderBy(g => g.User.EmailAddress);
            return View(usernames);
        }
        
        [HttpGet]
        [AuthorizeUser("Admin")]
        public ActionResult Edit(int usernameID)
        {
            Username username = db.Usernames.Find(usernameID);
            return View(username);
        }
        [HttpPost]
        [AuthorizeUser("Admin")]
        public ActionResult Edit(Username username)
        {
            try
            {
                db.Entry(username).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            catch(Exception e)
            {
                return View(username);
            }
            return RedirectToAction("Index");
        }

        [AuthorizeUser("Admin")]
        [HttpGet]
        public ActionResult Impersonate()
        {
            IEnumerable<User> users = db.Users.Where(f => !f.Archived).OrderBy(g => g.EmailAddress);
            return View(users);
        }

        [AuthorizeUser("Admin")]
        [HttpPost]
        public ActionResult Impersonate(int userID)
        {
            User user = db.Users.Find(userID);
            if (user != null)
            {
                Authentication.Impersonate(user.EmailAddress);
                return RedirectToAction("Index", "Home");
            }

            IEnumerable<User> users = db.Users.Where(f => !f.Archived).OrderBy(g => g.EmailAddress);
            return View(users);
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
            if (signInVM.EmailAddress != signInVM.ConfirmEmailAddress)
            {
                ModelState.AddModelError("EmailMismatch", "Your emails did not match");
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

        [HttpGet]
        [AuthorizeUser]
        public ActionResult CreateBracket()
        {
            //If already signed in, redirect to home page
            if (!Utilities.Authentication.IsSignedIn())
            {
                return RedirectToAction("SignIn", "UserAccount");
            }
            Username username = new Username();
            string email = Authentication.GetLoginInfo();
            username.User = db.Users.FirstOrDefault(f => f.EmailAddress == email);
            username.UserID = username.User.UserID;
            return View(username);
        }

        //Postback for signing in, redirects 
        [HttpPost]
        [AuthorizeUser]
        public ActionResult CreateBracket(Username username)
        {
            List<string> errors = new List<string>();

            if (ModelState.IsValid)
            {
                //Calles my authentication utility and adds any returned errors to my error list to be displayed
                db.Usernames.Add(username);
                db.SaveChanges();

                if (errors.Count == 0 && ModelState.IsValid)
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            foreach (string error in errors)
            {
                ModelState.AddModelError(error, error);
            }
            return View(username);
        }

        [AuthorizeUser]
        public ActionResult EmailList(int? seasonID)
        {
            if (seasonID == null)
            {
                seasonID = General.GetActiveSeasonID();
            }

            string emailListString = "";

            IEnumerable<User> users = db.Users.Where(f => !f.Archived && f.Usernames.Any(g => g.Approved && !g.Archived)).OrderBy(h => h.EmailAddress);

            foreach (var user in users)
            {
                emailListString += user.EmailAddress + "; ";
            }


            return View(emailListString);
        }

        [HttpGet]
        public ActionResult ResetPassword()
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            string key = new string(Enumerable.Repeat(chars, 6).Select(s => s[random.Next(s.Length)]).ToArray());

            ResetPasswordViewModel rpvm = new ResetPasswordViewModel();
            rpvm.GeneratedKey = key;

            return View(rpvm);
        }

        [HttpPost]
        public ActionResult ResetPassword(ResetPasswordViewModel rpvm)
        {
            List<string> errors = new List<string>();

            if (rpvm.GeneratedKey.ToLower() != rpvm.Key.ToLower())
            {
                errors.Add("Your key did not match the key sent to " + rpvm.EmailAddress + ".");
            }

            if (rpvm.Password.ToLower() != rpvm.PasswordConfirm.ToLower())
            {
                errors.Add("Your passwords do not match.");
            }

            User user = db.Users.FirstOrDefault(f => f.EmailAddress == rpvm.EmailAddress.ToLower());
            if (user != null && errors.Count == 0)
            {
                user.Salt = Authentication.GetSalt(32);
                user.PasswordHash = Authentication.getPasswordHash(user.Salt, rpvm.Password.ToLower());
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("SignIn", "UserAccount");
            }
            else if (user == null)
            {
                errors.Add("This email could not be found in the database.");
            }

            foreach (string error in errors)
            {
                ModelState.AddModelError(error, error);
            }
            return View(rpvm);
        }

        public void SendKeyToEmail(string email, string generatedKey)
        {
            General.SendEmail(email, "Key: " + generatedKey, "Password Reset");
        }


        [AjaxOnly]
        public bool IsAdmin()
        {
            if (Authentication.IsMemberOf("Admin"))
            {
                return true;
            }
            return false;
        }
    }
}