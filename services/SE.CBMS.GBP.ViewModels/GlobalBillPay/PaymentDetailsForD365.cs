using System;
using System.Collections.Generic;
using System.Text;

namespace SchneiderElectric.CBMS.GBP.ViewModels.GlobalBillPay
{
    public class PayloadDataToAzureSBFinal
    {
        public int itemsCount { get; set; }
        public int paymentGroupIndicator { get; set; }
        public string paymentAmount { get; set; }
        public string grossValue { get; set; }
        public List<PaymentDetailsForD365Final> invoices { get; set; }
    }
    public class PaymentDetailsForD365Final
    {
        public string invoiceType { get; set; }
        public string paymentType { get; set; }
        public string d365IntegrationId { get; set; }
        public string dueDate { get; set; }
        public string netValue { get; set; }
        public string grossValue { get; set; }
        public string marketIndicator { get; set; }
        public string invoiceStartDate { get; set; }
        public string invoiceEndDate { get; set; }
        public int cbmsInvoiceId { get; set; }
        public string vendorType { get; set; }
        public string issueDate { get; set; }
        public string currency { get; set; }
        public List<LineItemsData> lines  { get; set; }
    }
    public class LineItemsData
    {
        public string commodity { get; set; }
        public string cbmsBucketName { get; set; }
        public string lineItemName { get; set; }
        public string lineItemAmount { get; set; }
        public int cbmsAccountId { get; set; }
        public string invoiceNumber { get; set; }
    }
}
