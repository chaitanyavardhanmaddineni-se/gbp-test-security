using System;
using System.Collections.Generic;
using System.Text;

namespace SchneiderElectric.CBMS.GBP.ViewModels.GlobalBillPay
{
    public class MoveSelectedInvoiceDetails
    {
        public int CbmsImageId { get; set; }
        public int CuInvoiceId { get; set; }
        public int? ParentCuInvoiceId { get; set; }
        public string PaymentAttributeValue { get; set; }
        public int? PaymentAttributeDtlId { get; set; }
        public bool IsReported { get; set; }
        public bool IsProcessed { get; set; }
        public bool IsClosed { get; set; }  


    }
}
