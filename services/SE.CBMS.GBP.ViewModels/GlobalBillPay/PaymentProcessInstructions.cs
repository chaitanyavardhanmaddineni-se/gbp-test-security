using System;
using System.Collections.Generic;
using System.Text;

namespace SchneiderElectric.CBMS.GBP.ViewModels.GlobalBillPay
{
    public class PaymentProcessInstructions
    {
        public string processInstructions { get; set; }
        public DateTime? lastUpdate { get; set; }
        public string updatedUser { get; set; }
        public string instructionCategory { get; set; }
        public bool isActive { get; set; }
        public string accountNumber { get; set; }
    }
}
