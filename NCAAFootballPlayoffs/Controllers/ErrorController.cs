using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NCAAFootballPlayoffs.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Error
        public ActionResult Index()
        {
            return View();
        }

        //For when a user is unauthorized.
        public ActionResult Unauthorized()
        {
            return View();
        }

        //todo: add other error pages and set up default error handling
    }
}