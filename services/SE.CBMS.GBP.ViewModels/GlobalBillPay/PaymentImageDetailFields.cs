using System;
using System.Collections.Generic;
using System.Text;

namespace SchneiderElectric.CBMS.GBP.ViewModels.GlobalBillPay
{
    public class PaymentImageDetailFields
    {
        public int? ParentCbmsImageId { get; set; }

        public int? CbmsImageId { get; set; }

        public string CBMSDOCID { get; set; }

        public int? InvoiceId { get; set; }
        public string PaymentStatus { get; set; }
        public int? PaymentAttributeDtlId { get; set; }
        public int TotalCount { get; set; }
        public string AccountNumber { get; set; }
        public string AccountType { get; set; }
        public int? ClientId { get; set; }
        public int? ClientHierId { get; set; }
        public int? SiteGroupID { get; set; }
        public int? AccountId { get; set; }
        public string PaymentAttributeValue { get; set; }
        public int UserInfoId { get; set; }
        public int? SiteId { get; set; }
        public bool? IsPaymentAccount { get; set; }
        public bool? IsLargeGroupBill { get; set; }
        public bool IsMoveSelectedMonthInv { get; set; }
        public int? IsMoveSelectedMonthInvoices { get; set; }
        public int? IsMoveSelectedMonthPaymentAttributeDtlId { get; set; }
        public bool? IsReported { get; set; }
        public bool? IsProcessed { get; set; }
        public bool isPaymentStartDate { get; set; }
        public string VendorId { get; set; }
        public string InvoiceClosed { get; set; }
    }
}
