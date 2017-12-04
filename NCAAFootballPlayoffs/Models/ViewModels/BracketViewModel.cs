using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NCAAFootballPlayoffs.Models.ViewModels
{
    public class BracketViewModel
    {
        public bool IsAdmin { get; set; }
        public bool CanEditPicks { get; set; }
        public int UsernameID { get; set; }
        public string Username { get; set; }
        public int SeasonID { get; set; }
        public int SeasonYear { get; set; }

    }
}