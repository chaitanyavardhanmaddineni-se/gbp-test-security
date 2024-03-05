using System;
using System.Collections.Generic;
using System.Text;

namespace SchneiderElectric.CBMS.GBP.ViewModels.GlobalBillPay
{
    public class InvoiceCreationData
    {
        public int UserId { get; set; }
        public int QueueId { get; set; }
        public int AccountId { get; set; }
        public int InvoiceId { get; set; }
        public string ExceptionTypeName { get; set; }
        public string Comment { get; set; }
    }

    public class SaveAndSubmitData
    {
        public int InvoiceId { get; set; }
        public int UserId { get; set; }

        public bool Recalc { get; set; }
        public bool DataQuality { get; set; }
        public bool Variance { get; set; }
        public bool CurrentInvoiceNotReported { get; set; }
    }
}
