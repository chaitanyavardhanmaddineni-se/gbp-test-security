using SchneiderElectric.CBMS.GBP.ViewModels.GlobalBillPay;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SchneiderElectric.CBMS.GBP.Orchestration.Interfaces
{
    public interface IGBPActionOrchestration
    {

        Task<int> doProcessRaisePaymentException(InvoiceInfo invObj);
        Task<int> getPaymentSpecialistQueueIDByClientId(int ClientId);
        Task<int> GetConfiguredPaymentlocationQueueId();
        Task<List<PaymentImageDetailFields>> sendPaymentFieldData(PaymentFieldDetails paymentFieldDetails);
        void TriggerLargeGroupBillInvoices(int invoiceId, int UserId);
        Task<string> SendSiblingInvoicesData(int invoiceId, int UserId);
        Task<bool> TriggerUbmGenericEtlPaymentProcess(int invoiceId, int userId);
        Task<List<PaymentImageDetailFields>> LoadParentImageInvoiceDetails(int invoiceId,int? startIndex, int? endIndex, string sortColumn, string sortOrder, string moduleName);
        void PaymentDetailsCopyForGroupInvoices(int? invoiceId, int userId);
        Task<string> SaveMultipleInvoices(List<User> groupbillinvoiceobj);
        Task<string> SavePaymentFieldsData(PaymentFieldDetails paymentFieldDetails);
    }
}
