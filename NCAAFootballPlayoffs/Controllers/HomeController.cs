using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NCAAFootballPlayoffs.Utilities;
using NCAAFootballPlayoffs.Models;

namespace NCAAFootballPlayoffs.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        //https://github.com/ascherer1993/NCAAFootballPlayoffs.git
        public ActionResult Index()
        {
            //UserAccountController uac = new UserAccountController();
            //uac.CreateAccount();
            //Authentication.LogIn("ascherer1993", "ams1990");
            return View();
        }



    }
}