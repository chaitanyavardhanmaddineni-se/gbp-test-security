using System;
using System.Collections.Generic;
using System.Text;

namespace SchneiderElectric.CBMS.GBP.ViewModels.GlobalBillPay
{
    public class GroupBillInvoices:User
    {
        
        public string AccountNumber { get; set; }       
        public int TotalCount { get; set; }
        public int? ClientId { get; set; }
        public int? SiteId { get; set; }
    }
    public class User
    {
        public int? InvoiceId { get; set; }
        public int? AccountId { get; set; }
        public string InvoiceNumber { get; set; }
        public int UserInfoId { get; set; }
    }
}
