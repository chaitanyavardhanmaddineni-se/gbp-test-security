using System;
using System.Collections.Generic;
using System.Text;

namespace SchneiderElectric.CBMS.GBP.ViewModels.GlobalBillPay
{
    public class InvoiceDetailQueryParam
    {
        public int invoiceId { get; set; }
        public int? startIndex { get; set; }
        public int? endIndex { get; set; }
        public string sortColumn { get; set; }
        public string sortOrder { get; set; }
        public string moduleName { get; set; }
    }
}
