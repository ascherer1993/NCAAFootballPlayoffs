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
    
    public partial class QuestionAnswer
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public QuestionAnswer()
        {
            this.UserBonusQuestionPicks = new HashSet<UserBonusQuestionPick>();
        }
    
        public int QuestionAnswerID { get; set; }
        public string Text { get; set; }
        public int BonusQuestionID { get; set; }
        public bool IsCorrectAnswer { get; set; }
        public bool Archived { get; set; }
    
        public virtual BonusQuestion BonusQuestion { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserBonusQuestionPick> UserBonusQuestionPicks { get; set; }
    }
}
