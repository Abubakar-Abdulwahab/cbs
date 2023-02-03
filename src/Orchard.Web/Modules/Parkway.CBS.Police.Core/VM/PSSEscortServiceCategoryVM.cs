namespace Parkway.CBS.Police.Core.VM
{
    public class PSSEscortServiceCategoryVM
    {
        public long Id { get; set; }

        public long ParentId { get; set; }

        public string Name { get; set; }

        public string ParentName { get; set; }

        public int MinimumRequiredOfficers { get; set; }

        public bool ShowExtraFields { get; set; }
    }
}