using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NCAAFootballPlayoffs.Models
{
    public partial class UserBonusQuestionPick
    {
        public string Text { get; set; }
        public bool DisplayAsMultChoice { get; set; }
        public int BonusQuestionID { get; set; }
    }
}