using Parkway.CBS.Core.Models;

public class EditMDAModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Code { get; set; }
    public string CompanyEmail { get; set; }
    public string CompanyAddress { get; set; }
    public string CompanyMobilePhoneNumber { get; set; }
    public string BusinessNature { get; set; }
    public BankDetails AdditionalBankAccount { get; set; }
    public bool MakeDefaultBankAccout { get; set; }
    public bool MakeLogoDefault { get; set; }
    public string UserEmail { get; set; }
}