using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NCAAFootballPlayoffs.Models
{
    public class UsernameMetadata
    {
        [Required(ErrorMessage = "A bracket name is required.")]
        [Display(Name = "Bracket Name")]
        public string UserNameText { get; set; }
    }

    [MetadataType(typeof(UsernameMetadata))]
    public partial class UserName
    {
    }
}