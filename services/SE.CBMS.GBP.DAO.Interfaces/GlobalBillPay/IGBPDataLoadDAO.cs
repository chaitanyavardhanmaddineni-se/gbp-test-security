using Newtonsoft.Json.Linq;
using SchneiderElectric.CBMS.GBP.ViewModels.GlobalBillPay;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SE.CBMS.GBP.DAO.Interfaces.GlobalBillPay
{
    public interface IGBPDataLoadDAO
    {
        List<GBPChargeDetails> LoadChargeValues(int invoiceId, string sortColumn);
        UserInfo LoadUserDetailsbyXid(string xId);
        UserInfo GetUserInfoDetails(int userInfoId);
        List<AccountDetails> LoadInvoiceDetails(int invoiceId);
        List<PaymentProcessInstructions> LoadPaymentProcess(int invoiceId);
        List<paymentFields> LoadPaymentFields(int invoiceId, bool IsPaymentGroup);
        List<ExceptionDetails> LoadExceptionDetails(int invoiceId);
        PaymentInvoiceDetails GetInvDtlFrompaymentConfig(int invoiceId);
        string UbmGenericEtlPaymentDetails(int invoiceId);
        Task<List<GroupBillInvoices>> GetGroupBillInvoices(int invoiceId, int? startIndex, int? endIndex);
    }
}
