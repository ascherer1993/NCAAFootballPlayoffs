using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NCAAFootballPlayoffs.Models.ViewModels
{
    public class SignInViewModel
    {
        public string UsernameOrEmail { get; set; }

        public string Password { get; set; }
    }
}