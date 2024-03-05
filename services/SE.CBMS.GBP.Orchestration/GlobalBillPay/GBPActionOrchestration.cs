using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SchneiderElectric.CBMS.GBP.DAO.Interfaces.GlobalBillPay;
using SchneiderElectric.CBMS.GBP.Orchestration.Interfaces;
using SchneiderElectric.CBMS.GBP.ViewModels.GlobalBillPay;
using SE.CBMS.InvoiceReg.Business;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Globalization;
using SE.CBMS.GBP.DAO.Interfaces.GlobalBillPay;

namespace SchneiderElectric.CBMS.GBP.Orchestration.GlobalBillPay
{
    public class GBPActionOrchestration : IGBPActionOrchestration
    {
        readonly IGBPActionDAO gbpAction;
        readonly IGBPDataLoadDAO gBPDataLoadDAO;
        private readonly ILogger logger;

        public GBPActionOrchestration(IGBPActionDAO gbpAction, ILoggerFactory loggerFactory, IGBPDataLoadDAO gBPDataLoadDAO)
        {
            this.gbpAction = gbpAction;
            logger = loggerFactory.CreateLogger("Controllers.GBPActionOrchestration");
            this.gBPDataLoadDAO = gBPDataLoadDAO;
        }

        public async Task<int> doProcessRaisePaymentException(InvoiceInfo invObj)
        {
            try
            {
                return await gbpAction.doProcessPaymentSpecialistRoute(invObj);
            }
            catch (Exception ex)
            {
                logger.LogError("doProcessRaisePaymentException catch block:  error : " + ex.Message + Environment.NewLine + ex.InnerException + Environment.NewLine + ex.StackTrace);
                throw;
            }
        }

        public async Task<int> getPaymentSpecialistQueueIDByClientId(int ClientId)
        {
            return await gbpAction.getPaymentSpecialistQueueIDByClientId(ClientId);
        }

        public async Task<int> GetConfiguredPaymentlocationQueueId()
        {
            return await gbpAction.GetConfiguredPaymentlocationQueueId();
        }

        public async Task<List<PaymentImageDetailFields>> sendPaymentFieldData(PaymentFieldDetails paymentFieldObj)
        {
            try
            {
                string ExistVendor = await gbpAction.GetLargeGrpVendorDtl(paymentFieldObj.InvoiceId);
                VendorDtl invoiceData = null;
                invoiceData = JsonConvert.DeserializeObject<VendorDtl>(paymentFieldObj.invPaymentDetails);
                List<PaymentImageDetailFields> paymentAtrributes = null;
                if (ExistVendor != null && ExistVendor != invoiceData.ApprovedPaymentVendor)
                {
                    paymentAtrributes = new List<PaymentImageDetailFields> { new PaymentImageDetailFields { VendorId = invoiceData.ApprovedPaymentVendor } };
                }
                else
                {
                    PaymentEffectiveDate isPaymentStartDate = GetPaymentStartDateDetails(paymentFieldObj.InvoiceId, paymentFieldObj.issueDate);
                    if (isPaymentStartDate != null)
                    {
                        paymentFieldObj.isPaymentStartDate = isPaymentStartDate.isPaymentStartDate;
                    }

                    if (paymentFieldObj.isPaymentStartDate == true || isPaymentStartDate == null)
                    {
                        logger.LogInformation("Start : Inserting Payment Data fields with InvoiceId :" + paymentFieldObj.InvoiceId);
                        paymentAtrributes = await gbpAction.SavePaymentFieldDetails(paymentFieldObj);
                        logger.LogInformation("End : Inserting Payment Data fields with InvoiceId :" + paymentFieldObj.InvoiceId);
                        await gbpAction.ProcessDataSendTod365(paymentAtrributes, paymentFieldObj.InvoiceId, paymentFieldObj.UserId, paymentFieldObj.CbmsImageId, paymentFieldObj.GroupPGI);
                    }
                    else
                    {
                        List<MoveSelectedInvoiceDetails> moveSelectedInvoiceDetails = gbpAction.GetMoveSelectedInvoices(paymentFieldObj.CbmsImageId);
                        if (moveSelectedInvoiceDetails != null)
                        {
                            foreach (var movSelInv in moveSelectedInvoiceDetails)
                            {
                                gbpAction.CommonUpdatePaymentflagAndType(movSelInv.CuInvoiceId, paymentFieldObj.UserId);
                                gbpAction.logEventDetails(paymentFieldObj.UserId, movSelInv.CuInvoiceId, "Issue Date of " + paymentFieldObj.issueDate.ToString("MM/dd/yyyyy") + "  is before Payment Effective Date of " + isPaymentStartDate.PaymentStartDate?.ToString("MM/dd/yyyy") + ", invoice not sent to D365");
                            }
                        }
                        else
                        {
                            gbpAction.CommonUpdatePaymentflagAndType(paymentFieldObj.InvoiceId, paymentFieldObj.UserId);
                            gbpAction.logEventDetails(paymentFieldObj.UserId, paymentFieldObj.InvoiceId, "Issue Date of " + paymentFieldObj.issueDate.ToString("MM/dd/yyyyy") + "  is before Payment Effective Date of " + isPaymentStartDate.PaymentStartDate?.ToString("MM/dd/yyyy") + ", invoice not sent to D365");
                        }
                        paymentAtrributes = new List<PaymentImageDetailFields>();
                        PaymentImageDetailFields paymentImageDetailFields = new PaymentImageDetailFields();
                        paymentImageDetailFields.isPaymentStartDate = paymentFieldObj.isPaymentStartDate;
                        paymentAtrributes.Add(paymentImageDetailFields);
                    }

                    //Getting save and submit check box test details
                    logger.LogInformation("Start : Save and Submit Logic with InvoiceId :" + paymentFieldObj.InvoiceId);
                    var result = await gbpAction.GetSaveandSubmitTrack(paymentFieldObj.InvoiceId);
                    if (result != "")
                    {
                        paymentFieldObj.Action = JsonConvert.DeserializeObject<SaveAndSubmitTests>(result);
                        //paymentFieldObj.Recalc = objtest.Recalc;
                        //paymentFieldObj.DataQuality = objtest.DataQuality;
                        //paymentFieldObj.Variance = objtest.Variance;
                        //paymentFieldObj.CurrentInvoiceNotReported = objtest.CurrentInvoiceNotReported;
                    }
                    //Triggering save and submit details and Snapshot details
                    doProcessSaveAndSubmit(paymentFieldObj, paymentAtrributes);                  
                }
               
                return paymentAtrributes;
            }
            catch (Exception ex)
            {
                logger.LogError("sendPaymentFieldData catch block:  error : " + ex.Message + Environment.NewLine + ex.InnerException + Environment.NewLine + ex.StackTrace);
                throw;
            }
        }
        public PaymentEffectiveDate GetPaymentStartDateDetails(int invoiceId,DateTime issueDate)
        {
            PaymentEffectiveDate paymentEffectiveDate = null;
            List<AccountDetails> lststartDate = gbpAction.GetPaymentStartDate(invoiceId);
            var filteredParentAccountList = lststartDate.FindAll(l => l.PrimaryParentAccountId != null).ToList();
            if (filteredParentAccountList.Count > 0)
            {
                lststartDate = filteredParentAccountList;
            }
            foreach (AccountDetails account in lststartDate)
            {
                if (account.PaymentStartDate != null)
                {
                    paymentEffectiveDate = new PaymentEffectiveDate();
                    paymentEffectiveDate.PaymentStartDate = account.PaymentStartDate;
                }
                if (account.PaymentStartDate != null && issueDate >= account.PaymentStartDate)
                {
                    paymentEffectiveDate.isPaymentStartDate = true;
                    
                    break;
                }
                if(account.PaymentStartDate == null && account.IsPaymentAccount == true)
                {
                    paymentEffectiveDate = new PaymentEffectiveDate();
                    paymentEffectiveDate.isPaymentStartDate = true;
                    break;
                }
            }
            return paymentEffectiveDate;
        }

        public async Task<string> SaveMultipleInvoices(List<User> groupbillinvoiceobj)
        {
            try
            {

                return await gbpAction.SaveMultipleInvoices(groupbillinvoiceobj);

            }
            catch (Exception ex)
            {
                logger.LogError("saveMultipleInvoices catch block:  error : " + ex.Message + Environment.NewLine + ex.InnerException + Environment.NewLine + ex.StackTrace);
                throw;
            }
        }

        private async void doProcessSaveAndSubmit(PaymentFieldDetails paymentFieldObj, List<PaymentImageDetailFields> paymentAtrributes)
        {
            var obj = new SaveAndSubmitData
            { InvoiceId = paymentFieldObj.InvoiceId, UserId = paymentFieldObj.UserId, Recalc = paymentFieldObj.Action.Recalc, DataQuality = paymentFieldObj.Action.DataQuality, Variance = paymentFieldObj.Action.Variance, CurrentInvoiceNotReported = paymentFieldObj.Action.CurrentInvoiceNotReported };
            await gbpAction.doProcessSaveAndSubmit(obj);
            logger.LogInformation("End : Save and Submit Logic with InvoiceId :" + paymentFieldObj.InvoiceId);
            //if (paymentAtrributes != null)
            //{
            //    logger.LogInformation("Start : Saving Snap Shot Details with InvoiceId :" + paymentFieldObj.InvoiceId);
            //    await gbpAction.SavePaymentSnapshotDetails(paymentAtrributes);
            //    logger.LogInformation("End : Saving Snap Shot Details with InvoiceId :" + paymentFieldObj.InvoiceId);
            //}
            logger.LogInformation("End : sendPaymentFieldData with InvoiceId :" + paymentFieldObj.InvoiceId);
        }
        public async Task<List<PaymentImageDetailFields>> LoadParentImageInvoiceDetails(int invoiceId, int? startIndex, int? endIndex, string sortColumn, string sortOrder, string moduleName)
        {
            return await Task.Run(() => gbpAction.LoadParentImageInvoiceDetails(invoiceId, startIndex, endIndex, Constants.Grid, sortColumn, sortOrder, moduleName));
        }
        public async void TriggerLargeGroupBillInvoices(int invoiceId, int userId)
        {
            PaymentInvoiceDetails paymentInvoiceDetails = gBPDataLoadDAO.GetInvDtlFrompaymentConfig(invoiceId);
            List<AccountDetails> lststartDate = gbpAction.GetPaymentStartDate(invoiceId);
            if (paymentInvoiceDetails == null)
            {
                logger.LogInformation("Start : TriggerLargeGroupBillInvoices with InvoiceId :" + invoiceId);
               
                List<MoveSelectedInvoiceDetails> moveSelectedInvoiceDetails = gbpAction.GetMoveSelectedInvoices(lststartDate[0].ImageId);
                if (moveSelectedInvoiceDetails != null)
                {
                    int newInvoiceId = 0;
                    string comment = "";
                    string logEvent = "";
                    logger.LogInformation("Start : TriggerLargeGroupBillInvoices move selected scenario with InvoiceId :" + invoiceId);
                    var invoiceList = moveSelectedInvoiceDetails.FindAll(l =>l.CuInvoiceId != invoiceId &&  (l.IsReported != false || l.IsProcessed != true)).ToList();
                    invoiceList = invoiceList.OrderBy(i => i.CuInvoiceId).ToList();
                    if (invoiceList.Count > 0)
                    {
                        newInvoiceId = invoiceList[0].CuInvoiceId;
                        comment = Constants.MoveSelectedPaymentIncompleteExceptionComment;
                        logEvent = Constants.MoveSelectedPaymentIncompleteExceptionLogEvent;
                    } else
                    {
                        newInvoiceId = invoiceId;
                        comment = Constants.MoveSelectedInvoicesClosed;
                        logEvent = Constants.MoveSelectedInvoicesClosed;
                    }
                    int queueId = await gbpAction.GetConfiguredPaymentlocationQueueId();
                    List<AccountDetails> lstAccountId = gbpAction.GetPaymentStartDate(newInvoiceId);
                    int? AccountId = lstAccountId[0].AccountId == null ? 0 : lstAccountId[0].AccountId;
                    var obj = new InvoiceCreationData
                    { InvoiceId = newInvoiceId, UserId = userId, QueueId = queueId, AccountId = (int)AccountId, ExceptionTypeName = Constants.PaymentIncompleteException, Comment = comment + invoiceId + " was closed" };
                    gbpAction.RaiseException(obj);
                    gbpAction.logEventDetails(userId, newInvoiceId, logEvent + invoiceId + " was closed");
                    logger.LogInformation("End : TriggerLargeGroupBillInvoices move selected scenario opened an exception for InvoiceId :" + newInvoiceId);
                }
                else
                {

                    List<PaymentImageDetailFields> ParentImageInvoiceDetails = await gbpAction.LoadParentImageInvoiceDetails(invoiceId, null, null, Constants.TriggerLargeGroupBill, "", "", lststartDate[0].GroupPGI!=null? Constants.UBMGroup: Constants.LargeGroupBill);
                    logger.LogInformation("Out side condition PaymentListData count = " + ParentImageInvoiceDetails.Count);
                    if (ParentImageInvoiceDetails != null)
                    {
                        var PaymentListData = ParentImageInvoiceDetails.Any(l => (l.PaymentAttributeDtlId == null && l.IsMoveSelectedMonthPaymentAttributeDtlId == null) && l.IsReported == false && l.IsProcessed == false);
                        if (PaymentListData == false)
                        {
                            var paymentStatusList = ParentImageInvoiceDetails.FindAll(l => (((l.PaymentAttributeDtlId != null || l.IsMoveSelectedMonthPaymentAttributeDtlId != null) && l.IsReported == false && l.IsProcessed == false)) || ((l.PaymentAttributeDtlId != null || l.IsMoveSelectedMonthPaymentAttributeDtlId != null) && l.IsReported == false && l.IsProcessed == true) || ((l.PaymentAttributeDtlId != null || l.IsMoveSelectedMonthPaymentAttributeDtlId != null) && l.IsReported == true && l.IsProcessed == true)).ToList();
                            paymentStatusList = paymentStatusList.GroupBy(x => x.InvoiceId).Select(g => g.First()).ToList();
                            var sentPaymentList = paymentStatusList.FindAll(l => l.PaymentStatus == "Sent").ToList();
                            logger.LogInformation("Inside condition PaymentListData count = " + paymentStatusList.Count);
                            logger.LogInformation("Inside condition sentPaymentList count = " + sentPaymentList.Count);
                            foreach (var payment in paymentStatusList)
                            {
                                payment.UserInfoId = userId;
                                if (payment.IsMoveSelectedMonthInvoices != null)
                                {
                                    payment.InvoiceId = payment.IsMoveSelectedMonthInvoices;
                                }
                                if (payment.IsMoveSelectedMonthPaymentAttributeDtlId != null)
                                {
                                    payment.PaymentAttributeDtlId = payment.IsMoveSelectedMonthPaymentAttributeDtlId;
                                }
                            }
                            if (paymentStatusList.Count > 0 && sentPaymentList.Count == 0)
                            {
                                logger.LogInformation("Start : Sending Payment Details to D365 with invoice Id Details :" + paymentStatusList);
                                await gbpAction.ProcessDataSendTod365(paymentStatusList, invoiceId, userId, 0);
                                logger.LogInformation("End : Sending Payment Details to D365 with invoice Id Details :" + paymentStatusList);
                                logger.LogInformation("Start : TriggerLargeGroupBillInvoices with InvoiceId :" + invoiceId);
                            }
                        }
                    }
                }
                logger.LogInformation("End : TriggerLargeGroupBillInvoices with InvoiceId :" + invoiceId);
            } 

        }
        public async Task<string> SendSiblingInvoicesData(int invoiceId, int userId)
        {
            logger.LogInformation("Start : SendSiblingInvoicesData with InvoiceId :" + invoiceId);
            string successMessage = null;
            List<PaymentImageDetailFields> ParentImageInvoiceDetails = await gbpAction.LoadParentImageInvoiceDetails(invoiceId, null, null, "", "", "", "");

            if (ParentImageInvoiceDetails.Count > 0)
            {
                var PaymentListData = ParentImageInvoiceDetails.FindAll(l => l.PaymentStatus != Constants.PaymentNotSubmitted).ToList();
                logger.LogInformation("Start : Sending Payment Details to D365 with invoice Id Details :" + PaymentListData);
                var data = await gbpAction.ProcessDataSendTod365(PaymentListData, invoiceId, userId, 0);
                successMessage = "Success";
                logger.LogInformation("Start : Sending Payment Details to D365 with invoice Id Details :" + PaymentListData);
                logger.LogInformation("End : SendSiblingInvoicesData with InvoiceId :" + invoiceId);
            }
            return successMessage;
        }
        public async Task<bool> TriggerUbmGenericEtlPaymentProcess(int invoiceId, int userId)
        {
            bool Success = false;
            //DateTime? startDate = null;
            bool isPaymentstartdate = false;
            bool isPaymentAccount = false;
            List<AccountDetails> lststartDate = gbpAction.GetPaymentStartDate(invoiceId);
            int? groupPGI = lststartDate[0].GroupPGI;
            int cbmsImageId = lststartDate[0].ImageId.Value;
            isPaymentAccount = lststartDate.Any(e => e.IsPaymentAccount == true);
            if (isPaymentAccount == true)
            {
                var PaymentData = await gbpAction.UbmGenericEtlPaymentDetails(invoiceId);
                if (PaymentData != string.Empty)
                {

                    PaymentFieldDetails paymentFieldObj = new PaymentFieldDetails();
                    paymentFieldObj.InvoiceId = invoiceId;
                    paymentFieldObj.UserId = userId;
                    paymentFieldObj.invPaymentDetails = PaymentData;

                    JToken obj = JToken.Parse(paymentFieldObj.invPaymentDetails);
                    DateTime dtissueDate = obj["IssueDate"].ToObject<DateTime>();
                    string strissueDate = dtissueDate.ToString("dd/MM/yyyy");
                    DateTime issueDate = DateTime.ParseExact(strissueDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                    PaymentEffectiveDate isPaymentStartDate = GetPaymentStartDateDetails(invoiceId, issueDate);
                    if (isPaymentStartDate != null)
                    {
                        isPaymentstartdate = isPaymentStartDate.isPaymentStartDate;
                    }

                    if (isPaymentstartdate == true || isPaymentStartDate == null)
                    {
                        logger.LogInformation("Start : Inserting Payment Data fields with InvoiceId :" + paymentFieldObj.InvoiceId);
                        List<PaymentImageDetailFields> paymentAtrributes = await gbpAction.SavePaymentFieldDetails(paymentFieldObj);
                        logger.LogInformation("End : Inserting Payment Data fields with InvoiceId :" + paymentFieldObj.InvoiceId);

                        Success = await gbpAction.ProcessDataSendTod365(paymentAtrributes, paymentFieldObj.InvoiceId, paymentFieldObj.UserId, cbmsImageId, groupPGI);
                    }
                    else
                    {
                        gbpAction.CommonUpdatePaymentflagAndType(paymentFieldObj.InvoiceId, paymentFieldObj.UserId);
                        gbpAction.logEventDetails(paymentFieldObj.UserId, paymentFieldObj.InvoiceId, "Issue Date of " + issueDate.ToString("MM/dd/yyyy") + " is before Payment Effective Date of " + isPaymentStartDate.PaymentStartDate?.ToString("MM/dd/yyyyy") + ", invoice not sent to D365");
                    }
                }

            }
            else
            {
                gbpAction.CommonUpdatePaymentflagAndType(invoiceId, userId);
                Success = true;
            }
            return Success;

        }
        public void PaymentDetailsCopyForGroupInvoices(int? invoiceId, int userId)
        {
            gbpAction.PaymentDetailsCopyForGroupInvoices(invoiceId, userId);
        }
        public async Task<string> SavePaymentFieldsData(PaymentFieldDetails paymentFieldDetails)
        {
            return await gbpAction.SavePaymentFieldsData(paymentFieldDetails);
        }
    }
}
