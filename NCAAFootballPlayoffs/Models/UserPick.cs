//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NCAAFootballPlayoffs.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class UserPick
    {
        public int UserPickID { get; set; }
        public int ChosenTeamID { get; set; }
        public int GameID { get; set; }
        public int UserNameID { get; set; }
        public bool IsSurePick { get; set; }
    
        public virtual Team Team { get; set; }
        public virtual UserName UserName { get; set; }
        public virtual Game Game { get; set; }
    }
}
