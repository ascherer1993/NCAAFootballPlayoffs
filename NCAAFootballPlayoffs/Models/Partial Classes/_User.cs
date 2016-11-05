using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NCAAFootballPlayoffs.Models.MetaData
{
    public class UserMetadata
    {
        public int UserID { get; set; }
        public string Username { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] Salt { get; set; }
        public string EmailAddress { get; set; }
        public int PermissionID { get; set; }
        public bool Archived { get; set; }
        public string DisplayName { get; set; }
    }

    [MetadataType(typeof(UserMetadata))]
    public partial class User
    {
        
    }
}