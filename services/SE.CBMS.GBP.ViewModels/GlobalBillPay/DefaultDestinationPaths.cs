using System;
using System.Collections.Generic;
using System.Text;

namespace SchneiderElectric.CBMS.GBP.ViewModels.GlobalBillPay
{
    public class DefaultDestinationPaths
    {
        public string DestinationFolderPath { get; set; }
        public int? QueueId { get; set; }
        public string DefaultDestinationType { get; set; }
        public int DefaultDestinationTypeId { get; set; }
        public string QueueName { get; set; }
    }
}
