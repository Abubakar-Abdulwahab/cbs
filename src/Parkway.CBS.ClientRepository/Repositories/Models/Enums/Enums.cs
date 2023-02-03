using System.ComponentModel;

namespace Parkway.CBS.ClientRepository.Repositories.Models.Enums
{
    public enum PSBillingType
    {
        None = 0,
        [Description("One Off")]
        OneOff = 1,
        [Description("Daily")]
        Daily = 2,
        [Description("Weekly")]
        Weekly = 3,
        [Description("Monthly")]
        Monthly = 4,
    }


    public enum DeploymentStatus
    {
        None,
        Pending,
        Running,
        Completed,
        Terminated
    }

    public enum DeploymentAllowanceStatus
    {
        [Description("All")]
        None = 0,
        [Description("Pending Approval")]
        PendingApproval = 1,
        [Description("In Progress")]
        Waiting = 2,
        [Description("Declined")]
        Declined = 3,
        [Description("Failed")]
        Failed = 4,
        [Description("Paid")]
        Paid = 5,
    }

    public enum PSSAllowancePaymentStage
    {
        MobilizationFee = 1,
        MobilizationBalanceFee = 2
    }

}