using NCAAFootballPlayoffs.Models;
using NCAAFootballPlayoffs.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NCAAFootballPlayoffs.Controllers
{
    public class SeasonController : Controller
    {
        NCAAFootballPlayoffsEntities db = new NCAAFootballPlayoffsEntities();

        [AuthorizeUser("Admin")]
        [HttpGet]
        public ActionResult Index()
        {
            IEnumerable<Season> seasons = db.Seasons.Where(f => !f.Archived).AsEnumerable();
            return View(seasons);
        }

        [AuthorizeUser("Admin")]
        [HttpGet]
        public ActionResult CreateSeason()
        {
            return View();
        }

        [AuthorizeUser("Admin")]
        [HttpPost]
        public ActionResult CreateSeason(Season season)
        {
            try
            {
                db.Seasons.Add(season);
                db.SaveChanges();
            }
            catch
            {
                return View(season);
            }


            List<string> errors = new List<string>();
            if (ModelState.IsValid)
            {
                //Calles my authentication utility and adds any returned errors to my error list to be displayed
                //errors.AddRange(Utilities.Authentication.SignIn(signInVM.EmailAddress, signInVM.Password));
                if (errors.Count == 0)
                {
                    return RedirectToAction("Admin", "UserAccount");
                }
            }
            foreach (string error in errors)
            {
                ModelState.AddModelError(error, error);
            }
            
            return View();
        }

        [AuthorizeUser("Admin")]
        [HttpGet]
        public ActionResult Edit(int id)
        {
            Season season = db.Seasons.Find(id);
            return View(season);
        }

        [AuthorizeUser("Admin")]
        [HttpPost]
        public ActionResult Edit(Season season)
        {
            try
            {
                db.Entry(season).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            catch
            {
                return View(season);
            }


            List<string> errors = new List<string>();
            if (ModelState.IsValid)
            {
                //Calles my authentication utility and adds any returned errors to my error list to be displayed
                //errors.AddRange(Utilities.Authentication.SignIn(signInVM.EmailAddress, signInVM.Password));
                if (errors.Count == 0)
                {
                    return RedirectToAction("Admin", "UserAccount");
                }
            }
            foreach (string error in errors)
            {
                ModelState.AddModelError(error, error);
            }

            return View();
        }

        [AuthorizeUser("Admin")]
        [HttpGet]
        public ActionResult SetSeason()
        {
            IEnumerable<Season> seasons = db.Seasons.Where(f => !f.Archived).AsEnumerable();
            return View(seasons);
        }

        [AuthorizeUser("Admin")]
        [HttpPost]
        public ActionResult SetSeason(int seasonID)
        {
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    Season currentActiveSeason = db.Seasons.FirstOrDefault(f => f.ActiveSeason == true);
                    currentActiveSeason.ActiveSeason = false;
                    db.Entry(currentActiveSeason).State = System.Data.Entity.EntityState.Modified;

                    Season newActiveSeason = db.Seasons.Find(seasonID);
                    newActiveSeason.ActiveSeason = true;
                    db.Entry(newActiveSeason).State = System.Data.Entity.EntityState.Modified;

                    db.SaveChanges();
                    dbContextTransaction.Commit();
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("There was an issue changing the active season", "Error");
                    dbContextTransaction.Rollback();
                }
            }


            List<string> errors = new List<string>();
            if (ModelState.IsValid)
            {
                if (errors.Count == 0)
                {
                    return RedirectToAction("Admin", "UserAccount");
                }
            }
            foreach (string error in errors)
            {
                ModelState.AddModelError(error, error);
            }

            IEnumerable<Season> seasons = db.Seasons.Where(f => !f.Archived).AsEnumerable();
            return View(seasons);
        }

        public FileStreamResult SeasonPicksToExcel(int? seasonID)
        {
            ExcelManager em = new ExcelManager();
            var memStream = em.DownloadPicks(seasonID);
            return File(memStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "NCAAFootballPicks.xlsx");
        }
    }
}