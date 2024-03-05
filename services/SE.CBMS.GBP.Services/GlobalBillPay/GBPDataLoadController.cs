using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SchneiderElectric.CBMS.GBP.Orchestration.GlobalBillPay;
using SchneiderElectric.CBMS.GBP.Orchestration.Interfaces;
using SchneiderElectric.CBMS.GBP.ViewModels.GlobalBillPay;
using SchneiderElectric.CBMS.InvoiceReg.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SchneiderElectric.CBMS.GBP.Services.GlobalBillPay
{
    [Route("api/[controller]")]
    [ApiController]
    public class GBPDataLoadController : ControllerBase
    {
        readonly IGBPDataLoadOrchestration gBPDataLoadOrchestration;
        private readonly ILogger logger;
        public GBPDataLoadController(IGBPDataLoadOrchestration gBPDataLoadOrchestration, ILoggerFactory loggerFactory)
        {
            this.gBPDataLoadOrchestration = gBPDataLoadOrchestration;
            logger = loggerFactory.CreateLogger("Controllers.GBPDataLoadController");
        }

        [HttpGet(APIUri.LoadChargeValues)]
        public async Task<List<GBPChargeDetails>> LoadChargeValues(int invoiceId, string sortColumn)
        {
            try
            {
                return await gBPDataLoadOrchestration.LoadChargeValues(invoiceId, sortColumn);
            }
            catch (Exception ex)
            {
                logger.LogError("LoadChargeValues catch block:  error : " + ex.Message + Environment.NewLine + ex.InnerException + Environment.NewLine + ex.StackTrace);
                throw;
            }
        }

        [HttpGet(APIUri.GetUserDetailsbyXid)]
        public async Task<UserInfo> LoadUserDetailsbyXid(string xid)
        {

            try
            {
                logger.LogInformation("LoadUserDetailsbyXid=" + "xid:" + xid);
                return await gBPDataLoadOrchestration.LoadUserDetailsbyXid(xid);
            }
            catch (Exception ex)
            {
                logger.LogError("LoadUserDetailsbyXid catch block:  error : " + ex.Message + Environment.NewLine + ex.InnerException + Environment.NewLine + ex.StackTrace);
                throw;
            }

        }
        [HttpGet(APIUri.GetUserInfoDetails)]
        public async Task<UserInfo> GetUserInfoDetails(int userInfoid)
        {

            try
            {
                logger.LogInformation("GetUserInfoDetails=" + "userInfoid:" + userInfoid);
                return await gBPDataLoadOrchestration.GetUserInfoDetails(userInfoid);
            }
            catch (Exception ex)
            {
                logger.LogError("GetUserInfoDetails catch block:  error : " + ex.Message + Environment.NewLine + ex.InnerException + Environment.NewLine + ex.StackTrace);
                throw;
            }

        }
        [HttpGet(APIUri.LoadInvoiceDetails)]
        public async Task<List<AccountDetails>> LoadInvoiceDetails(int invoiceId)
        {
            try
            {
                return await gBPDataLoadOrchestration.LoadInvoiceDetails(invoiceId);
            }
            catch (Exception ex)
            {
                logger.LogError("LoadInvoiceDetails catch block:  error : " + ex.Message + Environment.NewLine + ex.InnerException + Environment.NewLine + ex.StackTrace);
                throw;
            }
        }
        [HttpGet(APIUri.LoadPaymentProcess)]
        public async Task<List<PaymentProcessInstructions>> LoadPaymentProcess(int invoiceId)
        {
            try
            {
                return await gBPDataLoadOrchestration.LoadPaymentProcess(invoiceId);
            }
            catch (Exception ex)
            {
                logger.LogError("LoadPaymentProcess catch block:  error : " + ex.Message + Environment.NewLine + ex.InnerException + Environment.NewLine + ex.StackTrace);
                throw ex;
            }
        }
        [HttpGet(APIUri.LoadExceptionDetails)]
        public async Task<List<ExceptionDetails>> LoadExceptionDetails(int invoiceId)
        {
            try
            {
                return await gBPDataLoadOrchestration.LoadExceptionDetails(invoiceId);
            }
            catch (Exception ex)
            {
                logger.LogError("LoadExceptionDetails catch block:  error : " + ex.Message + Environment.NewLine + ex.InnerException + Environment.NewLine + ex.StackTrace);
                throw;
            }
        }  
        [HttpGet(APIUri.LoadPaymentInvDetails)]
        public async Task<PaymentInvoiceDetails> GetInvDtlFrompaymentConfig(int invoiceId)
        {
            try
            {
                return await gBPDataLoadOrchestration.GetInvDtlFrompaymentConfig(invoiceId);
            }
            catch (Exception ex)
            {
                logger.LogError("LoadExceptionDetails catch block:  error : " + ex.Message + Environment.NewLine + ex.InnerException + Environment.NewLine + ex.StackTrace);
                throw;
            }
        }  

        [HttpGet(APIUri.LoadPaymentFields)]
        public async Task<List<paymentFields>> LoadPaymentFields(int invoiceId,bool IsPaymentGroup)
        {
            try
            {
                return await gBPDataLoadOrchestration.LoadPaymentFields(invoiceId, IsPaymentGroup);
            }
            catch (Exception ex)
            {
                logger.LogError("LoadPaymentFields catch block:  error : " + ex.Message + Environment.NewLine + ex.InnerException + Environment.NewLine + ex.StackTrace);
                throw;
            }
        }
        [HttpGet(APIUri.GetGroupBillInvoices)]
        public async Task<List<GroupBillInvoices>> GetGroupBillInvoices([FromQuery] InvoiceDetailQueryParam queryParam)
        {
            try
            {
                return await gBPDataLoadOrchestration.GetGroupBillInvoices(queryParam.invoiceId, queryParam.startIndex, queryParam.endIndex);
            }
            catch (Exception ex)
            {
                logger.LogError("GetGroupBillInvoices catch block:  error : " + ex.Message);
                throw;
            }
        }

    }
}
