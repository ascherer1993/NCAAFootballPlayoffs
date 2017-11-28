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
    
    public partial class BonusQuestion
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public BonusQuestion()
        {
            this.QuestionAnswers = new HashSet<QuestionAnswer>();
        }
    
        public int BonusQuestionID { get; set; }
        public string Text { get; set; }
        public bool DisplayAsMultChoice { get; set; }
        public int SeasonID { get; set; }
        public bool Archived { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<QuestionAnswer> QuestionAnswers { get; set; }
        public virtual Season Season { get; set; }
    }
}
