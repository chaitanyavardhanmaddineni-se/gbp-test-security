using System;
using System.Collections.Generic;
using System.Text;

namespace SchneiderElectric.CBMS.GBP.ViewModels.GlobalBillPay
{
    public class PaymentFieldDetails: InvoiceInfo
    {
       
        //public bool Recalc { get; set; }
        //public bool DataQuality { get; set; }
        //public bool Variance { get; set; }
        //public bool CurrentInvoiceNotReported { get; set; }
        public string invPaymentDetails { get; set; }
        public int CbmsImageId { get; set; }
        public SaveAndSubmitTests Action { get; set; }
        public bool isPaymentStartDate { get; set; }
        public DateTime issueDate { get; set; }

        public DateTime startDate { get; set; }
        public int? GroupPGI { get; set; }

    }

    public class SaveAndSubmitTests
    {
        public bool Recalc { get; set; }

        public bool DataQuality { get; set; }

        public bool Variance { get; set; }
        public bool CurrentInvoiceNotReported { get; set; }
        
    }

}
