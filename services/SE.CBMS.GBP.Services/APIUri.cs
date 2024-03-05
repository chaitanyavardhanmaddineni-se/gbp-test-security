using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchneiderElectric.CBMS.InvoiceReg.Services
{
    public class APIUri
    {
        #region GBPDataLoadController

        public const string LoadChargeValues = "LoadChargeValues";
        public const string GetUserDetailsbyXid = "GetUserDetailsbyXid";
        public const string GetUserInfoDetails = "GetUserInfoDetails";
        public const string LoadInvoiceDetails = "LoadInvoiceDetails";
        public const string LoadPaymentFields = "LoadPaymentFields";
        public const string LoadExceptionDetails = "LoadExceptionDetails";
        public const string LoadPaymentInvDetails = "LoadPaymentInvDetails";
        public const string LoadPaymentProcess = "LoadPaymentProcess";
        #endregion

        #region GBPActionController
        public const string PaymentQuestion = "PaymentQuestion";
        public const string PaymentAccountSetupIssue = "PaymentAccountSetupIssue";
        public const string SendPaymantData = "SendPaymantData";
        public const string GetPaymentSpecialistQueueId = "GetPaymentSpecialistQueueId";
        public const string GetConfiguredPaymentlocationQueueId = "GetConfiguredPaymentlocationQueueId";
        public const string LoadParentImageInvoiceDetails = "LoadParentImageInvoiceDetails";
        public const string TriggerLargeGroupBillInvoices = "TriggerLargeGroupBillInvoices";
        public const string SubmitSSiblingInvoicesPaymentData = "SubmitSSiblingInvoicesPaymentData";
        public const string TriggerUbmGenericEtlPaymentProcess = "TriggerUbmGenericEtlPaymentProcess";
        public const string PaymentDetailsCopyForGroupInvoices = "PaymentDetailsCopyForGroupInvoices";
        public const string GetGroupBillInvoices = "GetGroupBillInvoices";
        public const string SaveMultipleInvoices = "SaveMultipleInvoices";
        public const string SavePaymentFieldsData = "SavePaymentFieldsData";
        #endregion

    }
}
