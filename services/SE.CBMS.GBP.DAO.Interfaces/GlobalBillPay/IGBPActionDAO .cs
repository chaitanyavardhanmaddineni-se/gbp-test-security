using SchneiderElectric.CBMS.GBP.ViewModels.GlobalBillPay;
using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SchneiderElectric.CBMS.GBP.DAO.Interfaces.GlobalBillPay
{
    public interface IGBPActionDAO 
    {
        InvoiceInfo invObj { get; set; }
        int GetPaymentSpecialistByClientId(int ClientId, string codeValue);
        int GetBackUpQueuefromDataRoute(string DestinationfolderType);
        void RaiseException(InvoiceCreationData obj);
        Task<int> doProcessSaveAndSubmit(SaveAndSubmitData obj);
        void CommonUpdatePaymentflagAndType(int invoiceId, int userId);
         Task<string> GetSaveandSubmitTrack(int invoiceId); //Added for GBP-409
        Task<List<PaymentImageDetailFields>> SavePaymentFieldDetails(PaymentFieldDetails paymentFieldDetails);
        Task<string> SavePaymentSnapshotDetails(List<PaymentImageDetailFields> paymentAtrributes);
        void logEventDetails(int userId,int? invoiceId,string eventDescription);
        Task<int> doProcessPaymentSpecialistRoute(InvoiceInfo invObj);
        Task<int> getPaymentSpecialistQueueIDByClientId(int ClientId);
        Task<int> GetConfiguredPaymentlocationQueueId();
        Task<List<PaymentImageDetailFields>> LoadParentImageInvoiceDetails(int invoiceId, int? startIndex, int? endIndex, [Optional] string type, string sortColumn, string sortOrder, string moduleName);
        Task<string> UbmGenericEtlPaymentDetails(int invoiceId);
        //Task<int> getCuInvoicePaymentReqId(int userInfoId, string payloadDataToD365);
        //Task<string> UpdatePaymentResponseStatus(List<PaymentImageDetailFields> paymentAtrributes, int PaymentReqId);
        Task<bool> ProcessDataSendTod365(List<PaymentImageDetailFields> paymentAtrributes, int invoiceId, int userId, int CbmsImageId, int? GroupPGI=null);
        void PaymentDetailsCopyForGroupInvoices(int? invoiceId,int userId);
        List<AccountDetails> GetPaymentStartDate(int invoiceId);
        Task<string> SaveMultipleInvoices(List<User> groupbillinvoiceobj);
        Task<string> SavePaymentFieldsData(PaymentFieldDetails paymentFieldDetails);
        List<MoveSelectedInvoiceDetails> GetMoveSelectedInvoices(int? CbmsImageId);
        Task<string> GetLargeGrpVendorDtl(int invoiceId);
    }
}
