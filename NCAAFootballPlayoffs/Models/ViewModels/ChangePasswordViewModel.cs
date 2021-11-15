using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NCAAFootballPlayoffs.Models.ViewModels
{
    public class ChangePasswordViewModelMetaData
    {
        [Required(ErrorMessage = "Your old password is required.")]
        [Display(Name = "Old Password")]
        public string OldPassword
        {
            get; set;
        }
        [Required(ErrorMessage = "A new password is required.")]
        public string Password
        {
            get; set;
        }
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword
        {
            get; set;
        }
    }

    [MetadataType(typeof(ChangePasswordViewModelMetaData))]
    public class ChangePasswordViewModel
    {
        public int UserID
        {
            get; set;
        }
        public string EmailAddress
        {
            get; set;
        }
        public string OldPassword
        {
            get; set;
        }
        public string Password
        {
            get; set;
        }
        public string ConfirmPassword
        {
            get; set;
        }
    }
}