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
    
    public partial class Game
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Game()
        {
            this.UserPicks = new HashSet<UserPick>();
        }
    
        public int GameID { get; set; }
        public int SeasonID { get; set; }
        public string GameName { get; set; }
        public string TVStation { get; set; }
        public Nullable<System.DateTime> GameDatetime { get; set; }
        public Nullable<int> LocationID { get; set; }
        public int FavoriteID { get; set; }
        public int UnderdogID { get; set; }
        public Nullable<short> PointSpread { get; set; }
        public Nullable<short> FavoriteScore { get; set; }
        public Nullable<short> UnderdogScore { get; set; }
        public bool Archived { get; set; }
    
        public virtual Team Favorite { get; set; }
        public virtual Location Location { get; set; }
        public virtual Season Season { get; set; }
        public virtual Team Underdog { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserPick> UserPicks { get; set; }
    }
}
