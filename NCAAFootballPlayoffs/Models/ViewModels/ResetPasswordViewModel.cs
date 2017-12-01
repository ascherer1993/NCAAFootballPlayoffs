using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NCAAFootballPlayoffs.Models.ViewModels
{
    public class ResetPasswordViewModel
    {
        public string Key { get; set; }

        public string GeneratedKey { get; set; }

        public string EmailAddress { get; set; }

        public string Password { get; set; }

        public string PasswordConfirm { get; set; }
    }
}