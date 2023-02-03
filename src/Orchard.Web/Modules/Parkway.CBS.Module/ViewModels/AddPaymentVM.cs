using Parkway.CBS.Core.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Module.ViewModels
{
    public class AddPaymentVM
    {
        public string Recipient { get; set; }

        public string InvoiceNumber { get; set; }

        public string PhoneNumber { get; set; }

        public string TIN { get; set; }

        public string Email { get; set; }

        //[Required(ErrorMessage = "This value is required")]
        public PaymentProvider PaymentProvider { get; set; }

        [Required(ErrorMessage = "This value is required")]
        public PaymentChannel PaymentChannel { get; set; }


        [Required(ErrorMessage = "This value is required")]
        [StringLength(Int32.MaxValue, MinimumLength = 1, ErrorMessage = "Enter a valid amount.")]
        [DataType(DataType.Currency, ErrorMessage = "Enter a valid amount")]
        public string AmountPaid { get; set; }

        public decimal AmountDue { get; set; }
        
        [Required(ErrorMessage = "Reference is required.")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Reference must be between 5 to 100 characters long.")]
        public string Reference { get; set; }

        [Required(ErrorMessage = "Date value is required")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Enter a valid date. Format (dd/MM/YYYY).")]
        public string PaymentDate { get; set; }

        public DateTime DueDate { get; set; }

        public int Status { get; set; }

    }
}