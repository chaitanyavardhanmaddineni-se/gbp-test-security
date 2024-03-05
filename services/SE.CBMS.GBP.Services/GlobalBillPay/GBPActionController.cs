using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using SchneiderElectric.CBMS.GBP.Orchestration.GlobalBillPay;
using SchneiderElectric.CBMS.GBP.Orchestration.Interfaces;
using SchneiderElectric.CBMS.InvoiceReg.Services;
using SE.CBMS.InvoiceReg.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SchneiderElectric.CBMS.GBP.ViewModels.GlobalBillPay;
using SchneiderElectric.CBMS.GBP.DAO.Interfaces.GlobalBillPay;

namespace SchneiderElectric.CBMS.GBP.Services.GlobalBillPay
{
    [Route("api/[controller]")]
    [ApiController]
    public class GBPActionController : ControllerBase
    {
        readonly IGBPActionOrchestration gbpActionOrchestration;
        private readonly ILogger logger;
        readonly IGBPActionDAO gbpAction;
        public GBPActionController(IGBPActionDAO gbpAction,IGBPActionOrchestration _gbpActionOrchestration, ILoggerFactory loggerFactory)
        {
            this.gbpAction= gbpAction;
            gbpActionOrchestration = _gbpActionOrchestration;
            logger = loggerFactory.CreateLogger("Controllers.GBPActionController");
        }

        [HttpPost(APIUri.PaymentQuestion)]
        public async Task<int> PaymentQuestion(InvoiceInfo invObj)
        {
            try
            {
                //invObj.ExceptionTypeName = Constants.PaymentQuestion;
                return await gbpActionOrchestration.doProcessRaisePaymentException(invObj);
            }
            catch(Exception ex)
            {
                logger.LogError("PaymentQuestion catch block:  error : " + ex.Message + Environment.NewLine + ex.InnerException + Environment.NewLine + ex.StackTrace);
                throw;
            }      
        }

        [HttpPost(APIUri.PaymentAccountSetupIssue)]
        public async Task<int> PaymentAccountSetupIssue(InvoiceInfo invObj)
        {
            try
            {
                //invObj.ExceptionTypeName = Constants.PaymentAccountSetupIssue;
                return await gbpActionOrchestration.doProcessRaisePaymentException(invObj);
            }
            catch (Exception ex)
            {
                logger.LogError("PaymentAccountSetupIssue catch block:  error : " + ex.Message + Environment.NewLine + ex.InnerException + Environment.NewLine + ex.StackTrace);
                throw;
            }
        }

        [HttpPost(APIUri.SendPaymantData)]
        public async Task<List<PaymentImageDetailFields>> SendPaymentData(PaymentFieldDetails paymentFieldObj)
        {
            try
            {
                return await gbpActionOrchestration.sendPaymentFieldData(paymentFieldObj);
            }
            catch (Exception ex)
            {
                logger.LogError("SendPaymentData catch block:  error : " + ex.Message + Environment.NewLine + ex.InnerException + Environment.NewLine + ex.StackTrace);
                throw;
            }
        }

        [HttpGet(APIUri.GetPaymentSpecialistQueueId)]
        public async Task<int> GetPaymentSpecialistQueueId(int ClientId)
        {
            try
            {
                return await gbpActionOrchestration.getPaymentSpecialistQueueIDByClientId(ClientId);
            }
            catch (Exception ex)
            {
                logger.LogError("GetPaymentSpecialistQueueId catch block:  error : " + ex.Message + Environment.NewLine + ex.InnerException + Environment.NewLine + ex.StackTrace);
                throw;
            }
        }
        [HttpGet(APIUri.GetConfiguredPaymentlocationQueueId)]
        public async Task<int> GetConfiguredPaymentlocationQueueId()
        {
            try
            {
                return await gbpActionOrchestration.GetConfiguredPaymentlocationQueueId();
            }
            catch (Exception ex)
            {
                logger.LogError("GetConfiguredPaymentlocationQueueId catch block:  error : " + ex.Message + Environment.NewLine + ex.InnerException + Environment.NewLine + ex.StackTrace);
                throw;
            }
        }

        [HttpGet(APIUri.LoadParentImageInvoiceDetails)]
        public async Task<List<PaymentImageDetailFields>> LoadParentImageInvoiceDetails([FromQuery] InvoiceDetailQueryParam queryParam)
        {
            try
            {
                return await gbpActionOrchestration.LoadParentImageInvoiceDetails(queryParam.invoiceId, queryParam.startIndex, queryParam.endIndex, queryParam.sortColumn, queryParam.sortOrder, queryParam.moduleName);
            }
            catch (Exception ex)
            {
                logger.LogError("LoadParentImageInvoiceDetails catch block:  error : " + ex.Message + Environment.NewLine + ex.InnerException + Environment.NewLine + ex.StackTrace);
                throw;
            }
        }
        [HttpGet(APIUri.TriggerLargeGroupBillInvoices)]
        public void TriggerLargeGroupBillInvoices(int invoiceId,int userId)
        {
            try
            {
                 gbpActionOrchestration.TriggerLargeGroupBillInvoices(invoiceId, userId);
            }
            catch (Exception ex)
            {
                logger.LogError("TriggerLargeGroupBillInvoices catch block:  error : " + ex.Message + Environment.NewLine + ex.InnerException + Environment.NewLine + ex.StackTrace);
                throw;
            }
        }

        [HttpGet(APIUri.SubmitSSiblingInvoicesPaymentData)]
        public async Task<string> SubmitSSiblingInvoicesPaymentData(int invoiceId,int userId)
        {
            try
            {
                return await gbpActionOrchestration.SendSiblingInvoicesData(invoiceId,userId);
            }
            catch (Exception ex)
            {
                logger.LogError("SubmitSSiblingInvoicesPaymentData catch block:  error : " + ex.Message + Environment.NewLine + ex.InnerException + Environment.NewLine + ex.StackTrace);
                throw;
            }
        }
        [HttpGet(APIUri.TriggerUbmGenericEtlPaymentProcess)]
        public bool TriggerUbmGenericEtlPaymentProcess(int invoiceId, int userId)
        {
            bool result = false;
            try
            {
                result = gbpActionOrchestration.TriggerUbmGenericEtlPaymentProcess(invoiceId, userId).Result;
            }
            catch (Exception ex)
            {
                logger.LogError("TriggerUbmGenericEtlPaymentProcess catch block:  error : " + ex.Message + Environment.NewLine + ex.InnerException + Environment.NewLine + ex.StackTrace);
                throw;
            }
            return result;
        }
        
        [HttpGet(APIUri.PaymentDetailsCopyForGroupInvoices)]
        public void PaymentDetailsCopyForGroupInvoices(int? invoiceId, int userId)
        {
            try
            {
                gbpActionOrchestration.PaymentDetailsCopyForGroupInvoices(invoiceId, userId);
            }
            catch (Exception ex)
            {
                logger.LogError("TriggerLargeGroupBillInvoices catch block:  error : " + ex.Message + Environment.NewLine + ex.InnerException + Environment.NewLine + ex.StackTrace);
                throw;
            }
        }
        

        [HttpPost(APIUri.SaveMultipleInvoices)]
        public async Task<string> SaveMultipleInvoices(List<User>groupbillinvoiceobj)
        {
            try
            {
                return await gbpActionOrchestration.SaveMultipleInvoices(groupbillinvoiceobj);
            }
            catch (Exception ex)
            {
                logger.LogError("SaveMultipleInvoices catch block:  error : " + ex.Message + Environment.NewLine + ex.InnerException + Environment.NewLine + ex.StackTrace);
                throw;
            }
        }
        [HttpPost(APIUri.SavePaymentFieldsData)]
        public async Task<string> SavePaymentFieldsData(PaymentFieldDetails paymentFieldObj)
        {
            try
            {
                return await gbpActionOrchestration.SavePaymentFieldsData(paymentFieldObj);
            }
            catch (Exception ex)
            {
                logger.LogError("SendPaymentData catch block:  error : " + ex.Message + Environment.NewLine + ex.InnerException + Environment.NewLine + ex.StackTrace);
                throw;
            }
        }

    }
}
