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
    
    public partial class UserBonusQuestionPick
    {
        public int UserBonusQuestionPickID { get; set; }
        public int SelectedAnswerID { get; set; }
        public int UsernameID { get; set; }
    
        public virtual QuestionAnswer QuestionAnswer { get; set; }
        public virtual Username Username { get; set; }
    }
}
