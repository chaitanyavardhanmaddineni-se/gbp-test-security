using System;
using System.Collections.Generic;
using System.Text;

namespace SchneiderElectric.CBMS.GBP.ViewModels.GlobalBillPay
{
    public class InvoiceTracking //Added for GBP-409
    {
        public int TrackId { get; set; }

        public int InvoiceId { get; set; }

        public string Source { get; set; }

        public ActionValue Action { get; set; }
    }

    public class ActionValue
    {
        public bool isRecalc { get; set; }

        public bool isDataQuality { get; set; }

        public bool isVariance { get; set; }
    }
}
