using System;
using System.Collections.Generic;
using System.Text;

namespace SchneiderElectric.CBMS.GBP.ViewModels.GlobalBillPay
{
    public class GBPChargeDetails
    {
        public int InvoiceId { get; set; }
        public int? AccountId { get; set; }
        public string AccountNumber { get; set; }
        public string UbmService { get; set; }
        public string Commodity { get; set; }
        public string UbmBucket { get; set; }
        public string Bucket { get; set; }
        public string ChargeName { get; set; }
        public decimal ChargeValue { get; set; }
        public string CuDeterminantCode { get; set; }

    }
}
