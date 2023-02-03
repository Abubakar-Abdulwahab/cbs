using Orchard.Users.Models;
using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Police.Core.Models
{
    /// <summary>
    /// this model will hold the history of a police officer
    /// the command they belong to and the rank
    /// <para>Officer A will appear in command B and has a rank C, and also 6 months later 
    /// might appear in command D and rank E. These scenarios will be individual rows on the table. 
    /// So if the command or rank of the officer changes, these changes should captured here</para>
    /// </summary>
    public class PoliceOfficerCommandRankLog : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual PoliceOfficer PoliceOfficer { get; set; }

        public virtual PoliceRanking Rank { get; set; }

        public virtual Command Command { get; set; }

        public virtual UserPartRecord AddedBy { get; set; }
    }
}