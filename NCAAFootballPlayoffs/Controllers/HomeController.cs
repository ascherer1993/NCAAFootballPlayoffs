using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NCAAFootballPlayoffs.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        //https://github.com/ascherer1993/NCAAFootballPlayoffs.git
        public ActionResult Index()
        {
            return View();
        }
    }
}