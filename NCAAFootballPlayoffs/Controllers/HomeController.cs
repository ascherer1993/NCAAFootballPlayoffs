using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NCAAFootballPlayoffs.Utilities;
using NCAAFootballPlayoffs.Models;
using Newtonsoft.Json;

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
        
        //public void doWhatIWant()
        //{
        //    List<string> msgs = new List<string>();
        //    UserAccountController uac = new UserAccountController();
        //    //uac.CreateAccount();
        //    ExcelManager em = new ExcelManager();
        //    em.testExcel();

        //    msgs.Add("Done.");
        //    var responseObject = new
        //    {
        //        msgs = msgs,
        //        success = true
        //    };

        //    var returnString = JsonConvert.SerializeObject(responseObject);
        //    //    return returnString;
        //    //}
        //}

        public FileStreamResult doWhatIWant()
        {
            ExcelManager em = new ExcelManager();
            var memStream = em.Download();
            return File(memStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "NCAAFootballPicks.xlsx");

        }

    }
}