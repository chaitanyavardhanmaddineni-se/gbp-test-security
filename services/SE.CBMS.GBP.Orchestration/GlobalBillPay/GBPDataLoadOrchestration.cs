using SchneiderElectric.CBMS.GBP.Orchestration.Interfaces;
using SchneiderElectric.CBMS.GBP.ViewModels.GlobalBillPay;
using SE.CBMS.GBP.DAO.Interfaces.GlobalBillPay;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SchneiderElectric.CBMS.GBP.Orchestration.GlobalBillPay
{

    public class GBPDataLoadOrchestration: IGBPDataLoadOrchestration
    {
        readonly IGBPDataLoadDAO gBPDataLoadDAO;
        public GBPDataLoadOrchestration(IGBPDataLoadDAO gBPDataLoadDAO)
        {
            this.gBPDataLoadDAO = gBPDataLoadDAO;
        }
        public async Task<List<GBPChargeDetails>> LoadChargeValues(int invoiceId, string sortColumn)
        {
            return await Task.Run(() => gBPDataLoadDAO.LoadChargeValues(invoiceId, sortColumn));
        }
        public async Task<UserInfo> LoadUserDetailsbyXid(string xid)
        {
            return await Task.Run(() => gBPDataLoadDAO.LoadUserDetailsbyXid(xid));
        }
        public async Task<UserInfo> GetUserInfoDetails(int userInfoId)
        {
            return await Task.Run(() => gBPDataLoadDAO.GetUserInfoDetails(userInfoId));
        }
        public async Task<List<AccountDetails>> LoadInvoiceDetails(int invoiceId)
        {
            return await Task.Run(() => gBPDataLoadDAO.LoadInvoiceDetails(invoiceId));
        }
        public async Task<List<PaymentProcessInstructions>> LoadPaymentProcess(int invoiceId)
        {
            return await Task.Run(() => gBPDataLoadDAO.LoadPaymentProcess(invoiceId));
        }
        public async Task<List<ExceptionDetails>> LoadExceptionDetails(int invoiceId)
        {
            return await Task.Run(() => gBPDataLoadDAO.LoadExceptionDetails(invoiceId));
        }
        public async Task<List<paymentFields>> LoadPaymentFields(int invoiceId,bool IsPaymentGroup)
        {
            return await Task.Run(() => gBPDataLoadDAO.LoadPaymentFields(invoiceId, IsPaymentGroup));
        }
        public async Task <PaymentInvoiceDetails> GetInvDtlFrompaymentConfig(int invoiceId)
        {
            return await Task.Run(() => gBPDataLoadDAO.GetInvDtlFrompaymentConfig(invoiceId));
        }
        public async Task<List<GroupBillInvoices>> GetGroupBillInvoices(int invoiceId, int? startIndex, int? endIndex)
        {
            return await gBPDataLoadDAO.GetGroupBillInvoices(invoiceId, startIndex, endIndex);
        }
    }
}
