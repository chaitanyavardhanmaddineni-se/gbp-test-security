using System;
using System.Collections.Generic;
using System.Text;

namespace SchneiderElectric.CBMS.GBP.ViewModels.GlobalBillPay
{
    public static class Constants
    {
        #region API endpoints
        public const string SampleDataBreakDown = "SampleDataBreakDown";
        public const string SampleID = "SampleID";
        public const string RandomNumber = "RandomNumber";

        #endregion

        #region Exception Types
        public const string CloseInvoice = "CloseInvoice";
        public const string ImageIssue = "ImageIssue";
        public const string Copy = "Copy";
        public const string NotApplicable = "Not Applicable";
        public const string InvoiceRegistered = "Invoice Registered";
        public const string Processed = "Processed";
        public const string GroupBill = "GroupBill";
        public const string PaymentServices = "PaymentServices";
        public const string Split = "Split";
        public const string NeedsDataEntry = "NeedsDataEntry";
        public const string AddTemplate = "AddTemplate";
        public const string UnknownAccount = "UnknownAccount";
        public const string Closed = "Closed";
        public const string PaymentUnknownAccount = "PaymentUnknownAccount";
        public const string PaymentDNTNotManaged = "PaymentDNTNotManaged";
        public const string PaymentQuestion = "PaymentQuestion";
        public const string PaymentAccountSetupIssue="PaymentAccountSetupIssue";
        public const string PaymentIncomplete= "Payment - Incomplete";

        #endregion

        #region Permissions
        public const string Data_Breakdown_Permission = "ra.data.breakdown";
        #endregion

        public const string Grid = "Grid";
        public const string TriggerLargeGroupBill = "TriggerLargeGroupBill";
        public const string PaymentNotSubmitted = "Payment Not Submitted";
        public const string DecimalsStartingComment = "Too many decimal places for below items: ";
        public const string LargeGroupBill = "Large Group Bill";
        public const string MoveSelectedOtherMonth = "Move Selected to Other Month";
        public const string UBMGroup = "UBM Group";

        public const string PaymentIncompleteException = "PaymentIncomplete";
        public const string MoveSelectedPaymentIncompleteExceptionComment = "Invoice needs payment data entry. Move Selected to Other Month sibling invoice ID ";
        public const string MoveSelectedPaymentIncompleteExceptionLogEvent = "Invoice opened as Payment - Incomplete exception because Move Selected to Other Month sibling";
        public const string MoveSelectedInvoicesClosed = "All Move Selected Invoices Related Invoice";
        public const string FailedLargeGroupMsg = "Failed Large Group Bill Validation - Invoice on hold for other copies.";
        public const string FailedUBMGroupMsg = "Failed UBM Group Invoice Validation - Invoice on hold for other copies.";



        #region
        public const string INVOICE_IMAGE_SUBMITTED = "Invoice Image Submitted";
        public const string INVOICE_REGISTERED = "Invoice Registered";
        public const string DATA_QUALITY_REVIEW = "Data Quality Review";
        public const string INVOICE_PROCESSED = "Invoice Processed";
        public const string DATA_FEED = "Data Feed";
        public const string INTERNAL_UPLOAD = "Internal Upload";
        public const string PROCESSED = "Processed";
        public const string IN_PROCESS = "In Process";
        public const string NOT_APPLICABLE = "Not Applicable";
        public const string NoCurrentIssue = "No Current Issue";
        public const string NoCurrentException = "No Current Exception";
        public const string AwaitingRegistration = "Awaiting Registration";
        public const string PaymentRoutingLocation = "Payment Routing Location";
        public const string DNTArchive = "DNT Archive";
        public const string NotAnInvoice = "Not An Invoice";
        public const string ClientUnknown = "Client Unknown";
        public const string CDOAUnknown = "CDOA Unknown";
        public const string Telamon = "telamon";
        public const string InvRegnEntityName = "Inv Regn Entity Name";
        public const string DifferentLanguage = "Different Language";
        public const string IdentifyingAccount = "Identifying Account";
        public const string AccountAddOrReview = "Account Add or Review";

        public const string ExceptionJobErrorEmailBody = "Download File error";

        public const string PaymentClientUnknown = "Payment Client Unknown";
        public const string PaymentBlankMapping = "Payment Blank Mapping";

        public const string ClientMappingType = "Client Mapping Type";
        public const string CDOA = "CDOA";
        public const string PaymentSpecialist = "Payment Specialist";
        public const string EscalationRegnUser = "Escalation Regn User";
        public const string ClientPriority = "Client Priority";
        public const string PaymentPriority = "Payment Priority";

        #endregion
    }
}
