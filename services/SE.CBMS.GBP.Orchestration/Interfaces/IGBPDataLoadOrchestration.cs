using Newtonsoft.Json.Linq;
using SchneiderElectric.CBMS.GBP.ViewModels.GlobalBillPay;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SchneiderElectric.CBMS.GBP.Orchestration.Interfaces
{
    public interface IGBPDataLoadOrchestration
    {
        Task<List<GBPChargeDetails>> LoadChargeValues(int invoiceId, string sortColumn);
        Task<UserInfo> LoadUserDetailsbyXid(string xid);
        Task<UserInfo> GetUserInfoDetails(int userInfoId);
        Task<List<AccountDetails>> LoadInvoiceDetails(int invoiceId);
        Task<List<PaymentProcessInstructions>> LoadPaymentProcess (int invoiceId);
        Task<List<paymentFields>> LoadPaymentFields(int invoiceId,bool IsPaymentGroup);
        Task<List<ExceptionDetails>> LoadExceptionDetails(int invoiceId);
        Task <PaymentInvoiceDetails> GetInvDtlFrompaymentConfig(int invoiceId);
        Task<List<GroupBillInvoices>> GetGroupBillInvoices(int invoiceId, int? startIndex, int? endIndex);

    }
}
