using System;
using System.Collections.Generic;
using System.Text;

namespace SchneiderElectric.CBMS.GBP.ViewModels.GlobalBillPay
{
    public class UserInfo
    {
        public int UserInfoId { get; set; }
        public int UserQueueId { get; set; }
        public string UserName { get; set; }
    }
    public class InvoiceInfo 
    {
        public int InvoiceId { get; set; }
        public int UserId { get; set; }
        public int QueueId { get; set; }
        public int? AccountId { get; set; }
        public string Comment { get; set; }
        public string ExceptionTypeName { get; set; }
        public int ClientId { get; set; }
    }
}
