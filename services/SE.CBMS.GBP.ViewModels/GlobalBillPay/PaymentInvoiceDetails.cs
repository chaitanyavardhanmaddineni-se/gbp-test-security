using System;
using System.Collections.Generic;
using System.Text;

namespace SchneiderElectric.CBMS.GBP.ViewModels.GlobalBillPay
{
    public class PaymentInvoiceDetails
    {
        public int? CuInvoicePaymentConfigId { get; set; }
        public int? CuInvoiceId { get; set; }
        public string PaymentAttributeValue { get; set; }
        public int? PaymentSubmissionUserId { get; set; }
        public DateTime? PaymentSubmissionTs { get; set; }
        public int? PaymentResponseStatusCd { get; set; }
        public DateTime? PaymentResponseTs { get; set; }
        public int? CreatedUserId { get; set; }
        public DateTime? CreatedTs { get; set; }
        public int? UpdatedUserId { get; set; }
        public DateTime? LastChangeTs { get; set; }
        public string PaymentStatus { get; set; }
        public string RequestJson { get; set; }


    }
}