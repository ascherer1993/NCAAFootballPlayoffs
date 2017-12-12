using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NCAAFootballPlayoffs.Models.ViewModels
{
    public class SignInViewModelMetaData
    {
        [Required(ErrorMessage = "An email address is required.")]
        [Display(Name = "Email Address")]
        public string EmailAddress { get; set; }
        [Display(Name = "Confirm Email Address")]
        public string ConfirmEmailAddress { get; set; }
        [Required(ErrorMessage = "A password is required.")]
        public string Password { get; set; }
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }
        [Display(Name = "Bracket Name")]
        public string BracketName { get; set; }
        [Display(Name = "First and Last Name")]
        public string DisplayName { get; set; }
    }

    [MetadataType(typeof(SignInViewModelMetaData))]
    public class SignInViewModel
    {
        public string EmailAddress { get; set; }
        public string ConfirmEmailAddress { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string BracketName { get; set; }
        public string DisplayName { get; set; }
    }
}