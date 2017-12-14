using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NCAAFootballPlayoffs.Models.ViewModels
{
    public class ResetPasswordViewModelMetaData
    {
        public string Key { get; set; }

        public string GeneratedKey { get; set; }
        [Display(Name = "Email Address")]
        public string EmailAddress { get; set; }
        [Display(Name = "Password")]
        public string Password { get; set; }
        [Display(Name = "Confirm Password")]
        public string PasswordConfirm { get; set; }
    }

    [MetadataType(typeof(ResetPasswordViewModelMetaData))]
    public class ResetPasswordViewModel
    {
        public string Key { get; set; }

        public string GeneratedKey { get; set; }

        public string EmailAddress { get; set; }

        public string Password { get; set; }

        public string PasswordConfirm { get; set; }
    }
}