using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NCAAFootballPlayoffs.Models.ViewModels
{
    public class SignInViewModelMetaData
    {
        [Required(ErrorMessage = "A username or email address is required.")]
        [Display(Name = "Username/Email Address")]
        public string UsernameOrEmail { get; set; }

        [Required(ErrorMessage = "A password is required.")]
        public string Password { get; set; }
    }

    [MetadataType(typeof(SignInViewModelMetaData))]
    public class SignInViewModel
    {
        public string UsernameOrEmail { get; set; }
        public string Password { get; set; }
    }
}