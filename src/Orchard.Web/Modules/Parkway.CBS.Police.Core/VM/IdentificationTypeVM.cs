namespace Parkway.CBS.Police.Core.VM
{
    public class IdentificationTypeVM
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public bool HasIntegration { get; set; }

        public string ImplementationClass { get; set; }

        public string ImplementingClassName { get; set; }

        public string Hint { get; set; }

        public bool HasBiometricSupport { get; set; }
    }
}