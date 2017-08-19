using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NCAAFootballPlayoffs.Models.ViewModels
{
    public class HomeViewModel
    {
        public User User { get; set; }

        public IEnumerable<UserName> Usernames { get; set; }
    }
}