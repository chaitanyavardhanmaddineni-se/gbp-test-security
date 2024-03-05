using System;
using System.Collections.Generic;
using System.Text;

namespace SchneiderElectric.CBMS.GBP.ViewModels.GlobalBillPay
{
    public class AccountDetails
    {
        public string SiteName { get; set; }
        public string Address { get; set; }
        public string AccountType { get; set; }
        public string VendorName { get; set; }
        public int? AccountId { get; set; }
        public string AccountNumber { get; set; }  
        public string Commodity { get; set; }
        public string MeterNumber { get; set; }
        public string ContractNumber { get; set; }
        public DateTime? SupplierAccountStartDt { get; set; }
        public DateTime? SupplierAccountEndDt { get; set; }
        public string Rate { get; set; }
        public string Currency { get; set; }
        public string GroupAccount { get; set; }
        public string ClientName { get; set; }
        public int?  ClientId { get; set; }
        public DateTime? InvoiceStartDate { get; set; }
        public DateTime? InvoiceEndDate { get; set; }
        public string Ubm { get; set; }
        public string GroupKey { get; set; }
        public bool? IsPaymentAccount { get; set; }
        public bool IsOpenExceptions { get; set; }
        public bool IsInvoiceExist { get; set; }
        public int? ImageId { get; set; }
        public string PaymentStatus { get; set; }
        public bool? IsPaymentGroup { get; set; }
        public DateTime? PaymentStartDate { get; set; }
        public string Country { get; set; }
        public int? PrimaryParentAccountId { get; set; }
        public int? GroupPGI { get; set; }
    }

}
