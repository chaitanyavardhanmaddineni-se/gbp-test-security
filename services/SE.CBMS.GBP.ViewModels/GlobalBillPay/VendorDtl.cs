using System;
using System.Collections.Generic;
using System.Text;

namespace SchneiderElectric.CBMS.GBP.ViewModels.GlobalBillPay
{
    public class VendorDtl
    {
        public string InvoiceNumber { get; set; }
        public DateTime IssueDate { get; set; }
        public string NetValue { get; set; }
        public string VATValue { get; set; }
        public string InvoiceType { get; set; }
        public DateTime ReceivedDate { get; set; }
        public string GrossValue { get; set; }
        public string Deposit { get; set; }
        public string PaymentType { get; set; }
        public DateTime DueDate { get; set; }
        public string PaymentAmount { get; set; }
        public string MarketIndicator { get; set; }
        public string ApprovedPaymentVendor { get; set; }
        public DateTime InvoiceStartDate { get; set; }
        public string ReEnterPaymentAmount { get; set; }
        public string BalanceBroughtForward { get; set; }
        public DateTime InvoiceEndDate { get; set; }
    }
}
