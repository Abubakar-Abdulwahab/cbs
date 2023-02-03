namespace Parkway.CBS.Police.Core.DTO
{
    public class PSServiceVM
    {
        public int ServiceType { get; set; }

        public bool IsActive { get; set; }

        public string ServicePrefix { get; set; }

        public string ServiceName { get; set; }

        public string ServiceNote { get; set; }

        public bool HasDifferentialWorkFlow { get; set; }

        public int ServiceId { get; set; }

        public int CategoryId { get; set; }
    }
}