using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NCAAFootballPlayoffs.Utilities;
using NCAAFootballPlayoffs.Models;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace NCAAFootballPlayoffs.Controllers
{
    public class HomeController : Controller
    {
        NCAAFootballPlayoffsEntities db = new NCAAFootballPlayoffsEntities();

        // GET: Home
        //https://github.com/ascherer1993/NCAAFootballPlayoffs.git
        [AuthorizeUser]
        public ActionResult Index()
        {
            User user = null;
            string email = Authentication.GetLoginInfo();
            if (email != null)
            {
                user = db.Users.FirstOrDefault(f => f.EmailAddress == email);
            }

            return View(user);
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
            var memStream = em.DownloadPicks();
            return File(memStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "NCAAFootballPicks.xlsx");
        }

        public ActionResult doWhatIWantTwo()
        {
            Utilities.Authentication.SignIn("ams0068@auburn.edu", "1234");
            return RedirectToAction("Index", "Home");
        }

        public ActionResult doWhatIWantThree()
        {
            General.SendEmail("ascherer1993@gmail.com", "This is the message", "Test");
            return RedirectToAction("Index", "Home");
        }
    }
}