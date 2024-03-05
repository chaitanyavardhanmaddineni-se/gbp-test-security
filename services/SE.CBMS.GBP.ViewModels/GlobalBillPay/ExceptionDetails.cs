using System;
using System.Collections.Generic;
using System.Text;

namespace SchneiderElectric.CBMS.GBP.ViewModels.GlobalBillPay
{
    public class ExceptionDetails
    {
        public int? ImageId { get; set; }
        public string DocId { get; set; }
        public int? QueueId { get; set; }
        public string ExceptionType { get; set; }
        public string ExceptionStatusType { get; set; }
        public string UBMAccountNumber { get; set; }
        public string ServiceMonth { get; set; }
        public string ClientName { get; set; }
        public string SiteName { get; set; }
        public string StateName { get; set; }
        public string City { get; set; }
        public DateTime? DateInqueue { get; set; }
        public int? AccountId { get; set; }
        public string ExceptionGroupName { get; set; }
    }
}
