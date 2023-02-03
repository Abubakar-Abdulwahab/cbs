namespace Parkway.CBS.Police.Core.HelperModels
{
    public class CommandVM
    {
        public string Name { get; set; }

        public string Code { get; set; }

        public string ParentCode { get; set; }

        public int CommandCategoryId { get; set; }

        public int LGAId { get; set; }

        public int StateId { get; set; }

        public int Id { get; set; }

        public string LGAName { get; set; }

        public string StateName { get; set; }

        public string Address { get; set; }

        public int CommandTypeId { get; set; }

        public bool SelectAllSections { get; set; }

        public bool SelectAllSubSections { get; set; }

        public int AccessType { get; set; }
    }
}