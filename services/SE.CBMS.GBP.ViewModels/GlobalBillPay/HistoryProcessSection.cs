using System;
using System.Collections.Generic;
using System.Text;

namespace SchneiderElectric.CBMS.GBP.ViewModels.GlobalBillPay
{
    public class HistoryProcessSection
    {
        public int CuInvoiceProcessId { get; set; }
        public int CuInvoiceId { get; set; }
        public bool CuInvoiceCreated { get; set; }
        public bool ServiceMapping { get; set; }
        public bool ResolveToAccount { get; set; }
        public bool CuDuplicateTest { get; set; }
        public bool DmWatchList { get; set; }
        public bool RequiredData { get; set; }
        public bool DataMapped { get; set; }
        public bool RaWatchList { get; set; }
        public bool ResolveToMonth { get; set; }
        public bool ContractRecalc { get; set; }
        public bool InvoiceAggregate { get; set; }
        public bool VarianceTest { get; set; }
        public bool IsDataQualityTest { get; set; }
        public int Account_ID { get; set; }
        public bool IsPaymentSubmission { get; set; }
    }
}
