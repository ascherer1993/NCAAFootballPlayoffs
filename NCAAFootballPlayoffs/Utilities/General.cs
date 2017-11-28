using NCAAFootballPlayoffs.Models;
using NCAAFootballPlayoffs.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace NCAAFootballPlayoffs.Utilities
{
    public class General
    {
        public static int GetActiveSeasonID()
        {
            using (NCAAFootballPlayoffsEntities db = new NCAAFootballPlayoffsEntities())
            {
                return db.Seasons.FirstOrDefault(f => !f.Archived && f.ActiveSeason).SeasonID;
            }
        }
    }
}