using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SchneiderElectric.CBMS.GBP.DAO.Interfaces.GlobalBillPay;
using SchneiderElectric.CBMS.GBP.ViewModels.GlobalBillPay;
using SchneiderElectric.CBMS.InvoiceReg.DAO.Sql;
using SE.CBMS.GBP.Business;
using SE.CBMS.InvoiceReg.Business;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace SchneiderElectric.CBMS.GBP.DAO.Sql.GlobalBillPay
{
    public class GBPActionDAO : BaseData, IGBPActionDAO
    {
        private readonly string CbmsConnectionString;
        private readonly ILogger logger;
        IConfiguration _config;
        SummitDBUtils dbUtil = new SummitDBUtils();
        readonly IRecalServiceCallsPost recalServiceCalls;
        //readonly ICBMSImageRouting imageRouting;
        public InvoiceInfo invObj { get; set; }
        readonly ILoggerFactory loggerFactory;
        public GBPActionDAO(IConfiguration config, IRecalServiceCallsPost recalcService, ILoggerFactory loggerFactory)
        {
            this.CbmsConnectionString = config.GetSection("AppSettings").GetSection("ConnectionString").Value;
            _config = config;
            this.recalServiceCalls = recalcService;
            //this.imageRouting = imageRouting;
            this.loggerFactory = loggerFactory;
            logger = loggerFactory.CreateLogger("GBPActionDAO");
        }

        public Task<int> doProcessPaymentSpecialistRoute(InvoiceInfo invObj)
        {
            try
            {
                int QueueId;
                QueueId = getPaymentSpecialistQueueIDByClientId(invObj.ClientId).Result;
                var obj = new InvoiceCreationData
                { InvoiceId = invObj.InvoiceId, UserId = invObj.UserId, QueueId = QueueId, AccountId = invObj.AccountId != null ? invObj.AccountId.Value : 0, ExceptionTypeName = invObj.ExceptionTypeName, Comment = invObj.Comment };
                RaiseException(obj);
                return Task.Run(() => invObj.InvoiceId);
            }
            catch (Exception ex)
            {
                logger.LogError("doProcessPaymentSpecialistRoute catch block:  error : " + ex.Message + Environment.NewLine + ex.InnerException + Environment.NewLine + ex.StackTrace);
                throw;
            }
        }

        public Task<int> GetConfiguredPaymentlocationQueueId()
        {
            int QueueId;

            QueueId = GetBackUpQueuefromDataRoute(Constants.PaymentRoutingLocation);

            return Task.Run(() => QueueId);
        }
        public Task<int> getPaymentSpecialistQueueIDByClientId(int ClientId)
        {
            int QueueId;
            if (ClientId == -1 || ClientId == 0)
            {
                QueueId = GetBackUpQueuefromDataRoute(Constants.PaymentClientUnknown);
            }
            else
            {
                QueueId = GetPaymentSpecialistByClientId(ClientId, Constants.PaymentSpecialist);
                if (QueueId == 0)
                {
                    QueueId = GetBackUpQueuefromDataRoute(Constants.PaymentBlankMapping);
                }
            }
            return Task.Run(() => QueueId);
        }

        public int GetPaymentSpecialistByClientId(int ClientId, string codeValue)
        {
            int? QueueId = 0;
            try
            {
                SqlParameter[] spParams;
                int codeID = GetCodeIdByCodeSet(codeValue, Constants.ClientMappingType);
                dbUtil.ConnectionString = this.CbmsConnectionString;
                spParams = new SqlParameter[2];
                spParams[0] = new SqlParameter("@Client_Id", SqlDbType.Int);
                spParams[0].Value = ClientId;
                spParams[1] = new SqlParameter("@Client_Mapping_Type_Cd", SqlDbType.Int);
                spParams[1].Value = codeID;
                DataSet dsResult = dbUtil.ExecuteSQLQuery("dbo.Client_CDOA_Map_Sel_By_Client_Id", spParams);
                if (dsResult != null && dsResult.Tables[0].Rows.Count > 0)
                {
                    QueueId = dsResult.Tables[0].Rows[0].Field<int?>("CDOA_Queue_Id");
                }
            }
            catch (Exception ex)
            {
                logger.LogError("GetCodeIdByCodeSet catch block:  error : " + ex.Message + Environment.NewLine + ex.InnerException + Environment.NewLine + ex.StackTrace);
                throw;
            }
            return QueueId.HasValue ? QueueId.Value : 0;
        }
        private int GetCodeIdByCodeSet(string codeValue, string codesetName)
        {
            int result = 0;
            try
            {
                dbUtil.ConnectionString = this.CbmsConnectionString;
                SqlParameter[] spParams = new SqlParameter[2];
                spParams[0] = new SqlParameter("@CodeSet_Name", SqlDbType.VarChar);
                spParams[0].Value = codesetName;
                spParams[1] = new SqlParameter("@Code_Value", SqlDbType.VarChar);
                spParams[1].Value = codeValue;
                DataSet dsResult = dbUtil.ExecuteSQLQuery("dbo.CODE_SEL_BY_CodeSet_Name", spParams);
                if (dsResult != null && dsResult.Tables.Count > 0 && dsResult.Tables[0].Rows.Count > 0)
                {
                    result = dsResult.Tables[0].Rows[0].Field<int>("Code_Id");
                }
            }
            catch (Exception ex)
            {
                logger.LogError("GetCodeIdByCodeSet catch block:  error : " + ex.Message + Environment.NewLine + ex.InnerException + Environment.NewLine + ex.StackTrace);
                throw;
            }
            return result;
        }
        public int GetBackUpQueuefromDataRoute(string DestinationfolderType)
        {
            int QueueId = 0;
            try
            {
                List<DefaultDestinationPaths> defaultDestinations = GetDefaultDestinationFolderPath();
                DefaultDestinationPaths FolderPath = defaultDestinations.Find(e => e.DefaultDestinationType.ToLower().Contains(DestinationfolderType.ToLower()));
                if (FolderPath != null)
                {
                    QueueId = FolderPath.QueueId.Value;
                }
            }
            catch (Exception ex)
            {
                logger.LogError("GetBackUpQueuefromDataRoute catch block:  error : " + ex.Message + Environment.NewLine + ex.InnerException + Environment.NewLine + ex.StackTrace);
                throw;
            }
            return QueueId;
        }

        public async void RaiseException(InvoiceCreationData obj)
        {
            //int invoiceId = 0;
            try
            {
                string response = await recalServiceCalls.PostCall("Impl/GlobalBillPayService.svc/GlobalBillPayment/doProcessPaymentRouteException",
                 JsonConvert.SerializeObject(obj));
            }
            catch (Exception ex)
            {
                logger.LogError("RaiseException Method GetDestinationRootFolderConfig catch block:  error : " + ex.Message + JsonConvert.SerializeObject(obj));

            }
        }
        private int ParseInvoiceId(string response)
        {
            int intInvoiceId = 0;
            XmlDocument infodoc = new XmlDocument();
            infodoc.LoadXml(response);
            String str = infodoc.InnerText;
            if (str != "")
            {
                intInvoiceId = Convert.ToInt32(str);
            }
            return intInvoiceId;
        }
        private List<DefaultDestinationPaths> GetDefaultDestinationFolderPath()
        {
            List<DefaultDestinationPaths> defaultDestinationPaths = null;
            try
            {
                dbUtil.ConnectionString = this.CbmsConnectionString;

                DataSet dsResult = dbUtil.ExecuteSQLQuery("dbo.Cu_Invoice_Image_Regn_Default_Destination_Location_Sel");
                if (dsResult != null && dsResult.Tables.Count > 0)
                {
                    var query = from obj in dsResult.Tables[0].AsEnumerable()
                                select new DefaultDestinationPaths()
                                {
                                    DestinationFolderPath = obj.Field<string>("Destination_Location"),
                                    DefaultDestinationTypeId = obj.Field<int>("Cbms_Destination_Type_Cd"),
                                    QueueId = obj.Field<int?>("Cbms_Queue_Id"),
                                    DefaultDestinationType = obj.Field<string>("Cbms_Destination_Type"),
                                    QueueName = obj.Field<string>("Cbms_Queue")
                                };
                    defaultDestinationPaths = query.ToList();
                }
            }
            catch (Exception ex)
            {
                logger.LogError("GetDefaultDestinationFolderPath catch block:  error : " + ex.Message);

            }
            return defaultDestinationPaths;
        }
        public async Task<int> doProcessSaveAndSubmit(SaveAndSubmitData obj)
        {
            int invoiceId = 0;
            try
            {
                string response = await recalServiceCalls.PostCall("Impl/GlobalBillPayService.svc/GlobalBillPayment/doProcessPaymentSaveAndSubmit",
                JsonConvert.SerializeObject(obj));
                invoiceId = ParseInvoiceId(response);

            }
            catch (Exception ex)
            {
                // InsertDestinationFilePathInReg("", "Failed to create invoice due to " + ex.Message + " Object= " + JsonConvert.SerializeObject(obj));
                logger.LogError("doProcessSaveAndSubmit Method catch block:  error : " + ex.Message + JsonConvert.SerializeObject(obj));

            }
            return invoiceId;
        }

        //Added for GBP-409
        public Task<string> GetSaveandSubmitTrack(int invoiceId)
        {
            string result = "";
            try
            {
                dbUtil.ConnectionString = this.CbmsConnectionString;
                SqlParameter[] spParams = new SqlParameter[1];
                spParams[0] = new SqlParameter("@Cu_Invoice_Id", SqlDbType.Int);
                spParams[0].Value = invoiceId;

                DataSet dsResult = dbUtil.ExecuteSQLQuery("CU_Invoice_Save_And_Submit_Tracking_Sel_By_Invoice_Id", spParams);

                if (dsResult != null && dsResult.Tables[0].Rows.Count > 0)
                {
                    result = dsResult.Tables[0].Rows[0].Field<string>("Cu_Invoice_Action_Value");
                } else
                {
                    result = "{'Recalc':true,'DataQuality':true,'Variance':true,'CurrentInvoiceNotReported':false}";
                }

            }
            catch (Exception ex)
            {
                logger.LogError("GetSaveandSubmitTrack catch block:  error : " + ex.Message);
            }
            return Task.Run(() => result);
        }
        //End //Added for GBP-409

        public Task<List<PaymentImageDetailFields>> SavePaymentFieldDetails(PaymentFieldDetails paymentFieldDetails)
        {
            List<PaymentImageDetailFields> paymentAttributes = null;
            try
            {
                DataSet dsResult;
                string spName;
                bool HasUbmGroupMoveSelectedInvoices = false;
                bool HasLargeGroupMoveSelectedInvoices = false;
                //var x = JsonConvert.SerializeObject(paymentFieldDetails.invPaymentDetails);
                SqlParameter[] spParams;
                dbUtil.ConnectionString = this.CbmsConnectionString;
                spParams = new SqlParameter[1];
                spParams[0] = new SqlParameter("@Cu_Invoice_Id", SqlDbType.Int);
                spParams[0].Value = paymentFieldDetails.InvoiceId;
                dsResult = dbUtil.ExecuteSQLQuery("dbo.Cu_Invoice_Payment_Copy_Move_Selected_Month_Exists", spParams);
                if (dsResult != null && dsResult.Tables.Count > 0)
                {

                    HasLargeGroupMoveSelectedInvoices = (bool)dsResult.Tables[0].Rows[0]["Has_Copy_Move_Selected_Invoice"];
                    HasUbmGroupMoveSelectedInvoices = (bool)dsResult.Tables[0].Rows[0]["Has_Ubm_Generic_Move_Selected_Invoice"];
                }
                spParams = new SqlParameter[3];
                spParams[0] = new SqlParameter("@Cu_Invoice_Id", SqlDbType.Int);
                spParams[0].Value = paymentFieldDetails.InvoiceId;
                spParams[1] = new SqlParameter("@Payment_Attribute_Value", SqlDbType.NVarChar);
                spParams[1].Value = paymentFieldDetails.invPaymentDetails;
                spParams[2] = new SqlParameter("@User_Info_ID", SqlDbType.Int);
                spParams[2].Value = paymentFieldDetails.UserId;

                if (HasLargeGroupMoveSelectedInvoices)
                {
                    spName = "dbo.Cu_Invoice_Payment_For_Copy_Move_Selected_Ins";
                }
                else if(HasUbmGroupMoveSelectedInvoices)
                {
                    spName = "Cu_Invoice_Payment_For_Ubm_Group_Move_Selected_Ins";
                }
                else
                {
                    spName = "dbo.Cu_Invoice_Payment_Ins";
                }

                dsResult = dbUtil.ExecuteSQLQuery(spName, spParams);

                if (dsResult != null && dsResult.Tables.Count > 0)
                {
                    if (dsResult.Tables[0].Rows.Count > 0)
                    {

                        var query = from obj in dsResult.Tables[0].AsEnumerable()
                                    select new PaymentImageDetailFields()
                                    {
                                        InvoiceId = obj.Field<int>("Cu_Invoice_id"),
                                        //PaymentAttributeValue = obj.Field<string>("Payment_attribute_value"),
                                        PaymentAttributeDtlId = obj.Field<int>("Cu_Invoice_Payment_Dtl_Id"),
                                        //InvoicePaymentConfigId = obj.Field<int>("Cu_Invoice_Payment_Id"),
                                        UserInfoId = paymentFieldDetails.UserId,
                                        IsMoveSelectedMonthInv = obj.Field<int>("Is_Move_Selected_Month_Inv") == 0?false:true,
                                        CbmsImageId = obj.Field<int>("CBMS_IMAGE_ID")                                        
                                    };
                        paymentAttributes = query.ToList();
                    }
                }

            }
            catch (Exception ex)
            {
                logger.LogError("SavePaymentFieldDetails catch block:  error : " + ex.Message);
            }
            return Task.Run(() => paymentAttributes);
        }

        public Task<string> SavePaymentSnapshotDetails(List<PaymentImageDetailFields> paymentAtrributes)
        {
            string msg = string.Empty;
            try
            {
                foreach (var item in paymentAtrributes)
                {
                    SqlParameter[] spParams;
                    dbUtil.ConnectionString = this.CbmsConnectionString;
                    spParams = new SqlParameter[3];
                    spParams[0] = new SqlParameter("@Cu_Invoice_Id", SqlDbType.Int);
                    spParams[0].Value = item.InvoiceId;
                    spParams[1] = new SqlParameter("@Cu_Invoice_Payment_Dtl_Id", SqlDbType.Int);
                    spParams[1].Value = item.PaymentAttributeDtlId;
                    spParams[2] = new SqlParameter("@User_Info_ID", SqlDbType.Int);
                    spParams[2].Value = item.UserInfoId;
                    msg = dbUtil.ExecuteSQLNonQuery("dbo.Cu_Invoice_Payment_Snapshot_Ins", spParams);
                }

            }
            catch (Exception ex)
            {
                logger.LogError("SavePaymentSnapshotDetails catch block:  error : " + ex.Message);
            }
            return Task.Run(() => msg);
        }

        public void logEventDetails(int userId, int? invoiceId, string eventDescription)
        {
            try
            {
                SqlParameter[] spParams;
                dbUtil.ConnectionString = this.CbmsConnectionString;
                spParams = new SqlParameter[3];
                spParams[0] = new SqlParameter("@MyAccountId", SqlDbType.Int);
                spParams[0].Value = userId;
                spParams[1] = new SqlParameter("@cu_invoice_id", SqlDbType.Int);
                spParams[1].Value = invoiceId;
                spParams[2] = new SqlParameter("@event_description", SqlDbType.VarChar);
                spParams[2].Value = eventDescription;
                dbUtil.ExecuteSQLNonQuery("dbo.cbmsCuInvoiceEvent_Save", spParams);
            }
            catch (Exception ex)
            {
                logger.LogError("logEventDetails catch block:  error : " + ex.Message);
            }

        }
        public Task<List<PaymentImageDetailFields>> LoadParentImageInvoiceDetails(int invoiceId, int? startIndex, int? endIndex, [Optional] string type, string sortColumn, string sortOrder, string moduleName)
        {
            List<PaymentImageDetailFields> ImageDetails = null;
            SqlParameter[] spParams;
            dbUtil.ConnectionString = this.CbmsConnectionString;
            try
            {
                if (type == "Grid")
                {
                    spParams = new SqlParameter[5];
                    spParams[0] = new SqlParameter("@cu_invoice_id", SqlDbType.Int);
                    spParams[0].Value = invoiceId;
                    spParams[1] = new SqlParameter("@Start_Index", SqlDbType.Int);
                    spParams[1].Value = startIndex;
                    spParams[2] = new SqlParameter("@End_Index", SqlDbType.Int);
                    spParams[2].Value = endIndex;
                    spParams[3] = new SqlParameter("@Sort_Column", SqlDbType.VarChar);
                    spParams[3].Value = sortColumn;
                    spParams[4] = new SqlParameter("@Sort_Order", SqlDbType.VarChar);
                    spParams[4].Value = sortOrder;
                } else
                {
                    spParams = new SqlParameter[1];
                    spParams[0] = new SqlParameter("@cu_invoice_id", SqlDbType.Int);
                    spParams[0].Value = invoiceId;
                }
                DataSet dsResult = null;
                if(moduleName == Constants.MoveSelectedOtherMonth)
                {
                    dsResult = dbUtil.ExecuteSQLQuery("dbo.Move_Selected_Invoices_Dtls_By_Cu_Invoice_Id", spParams) ; 
                }
                else if(moduleName == Constants.UBMGroup)
                {
                    dsResult = dbUtil.ExecuteSQLQuery("dbo.Payment_Group_Ubm_Invoice_Dtls_By_Cu_Invoice_Id", spParams);
                }
                else
                {
                    if(moduleName == Constants.LargeGroupBill || string.IsNullOrEmpty(moduleName))
                    {
                        dsResult = dbUtil.ExecuteSQLQuery("dbo.Cu_Invoice_Image_Regn_Parent_Image_Dtl_Sel_By_Cu_Invoice_Id", spParams);
                    }
                }
                if (dsResult != null && dsResult.Tables.Count > 0)
                {
                    var query = from objAccount in dsResult.Tables[0].AsEnumerable()
                                select new PaymentImageDetailFields()
                                {
                                    ParentCbmsImageId = objAccount.Table.Columns.Contains("Parent_Cbms_Image_Id") ? objAccount.Field<int?>("Parent_Cbms_Image_Id") : objAccount.Field<int?>("Payment_Group_Indicator_Id") ,
                                    CbmsImageId = objAccount.Field<int?>("Cbms_Image_Id"),
                                    CBMSDOCID = objAccount.Field<string>("CBMS_DOC_ID"),
                                    InvoiceId = objAccount.Field<int?>("Cu_Invoice_Id"),
                                    PaymentStatus = GetpaymentStatus(objAccount.Field<string>("Payment_Status")),
                                    PaymentAttributeDtlId = objAccount.Field<int?>("Cu_Invoice_Payment_Dtl_Id"),
                                    TotalCount = objAccount.Field<int>("Total_Count"),
                                    AccountNumber = objAccount.Field<string>("Account_Number"),
                                    AccountType = objAccount?.Field<string>("Account_Type")?.Replace(" ", "").Trim(),
                                    ClientId = objAccount.Field<int?>("Client_Id"),
                                    ClientHierId = objAccount.Field<int?>("Client_Hier_Id"),
                                    SiteGroupID = objAccount.Field<int?>("Sitegroup_Id"),
                                    AccountId = objAccount.Field<int?>("ACCOUNT_ID"),
                                    SiteId = objAccount.Field<int?>("Site_Id"),
                                    IsPaymentAccount = objAccount.Field<bool?>("Is_Payment_Account"),
                                    IsLargeGroupBill = objAccount.Field<bool?>("Is_Large_Group_Bill"),
                                    IsMoveSelectedMonthInvoices = objAccount.Field<int?>("Move_Selected_Cu_Invoice_Id"),
                                    IsMoveSelectedMonthPaymentAttributeDtlId = objAccount.Field<int?>("Move_Selected_Cu_Invoice_Payment_Dtl_Id"),
                                    IsMoveSelectedMonthInv = objAccount.Field<int?>("Move_Selected_Cu_Invoice_Id") != null ? true:false,
                                    IsReported = objAccount.Field<bool?>("Is_Reported"),
                                    IsProcessed = objAccount.Field<bool?>("Is_Processed")
                                };
                    ImageDetails = query.ToList();
                }
            }
            catch (Exception ex)
            {
                logger.LogError("LoadParentImageInvoiceDetails catch block:  error : " + ex.Message);
            }
            return Task.Run(() => ImageDetails);
        }
        private string GetpaymentStatus(string status)
        {
            string strstatus = "Payment Not Submitted";
            if (status == "Sent")
            {
                strstatus = status;
            }

            if (status == "Hold")
            {
                strstatus = "Payment Submitted";
            }

            return strstatus;
        }
        private string UpdatePriorityFlag(int invoiceId, int userId)
        {
            string msg = string.Empty;
            SqlParameter[] spParams;
            dbUtil.ConnectionString = this.CbmsConnectionString;

            try
            {
                int codeID = GetCodeIdByCodeSet(Constants.ClientPriority, Constants.PaymentPriority);
                spParams = new SqlParameter[4];
                spParams[0] = new SqlParameter("@Cu_Invoice_Id", SqlDbType.Int);
                spParams[0].Value = invoiceId;
                spParams[1] = new SqlParameter("@Is_Priority", SqlDbType.Bit);
                spParams[1].Value = false;
                spParams[2] = new SqlParameter("@User_Info_Id", SqlDbType.Int);
                spParams[2].Value = userId;
                spParams[3] = new SqlParameter("@Source_Cd", SqlDbType.Int);
                spParams[3].Value = codeID;

                DataSet dsResult = dbUtil.ExecuteSQLQuery("[Workflow].[Cu_Invoice_Attribute_Ins]", spParams);
            }
            catch (Exception ex)
            {
                logger.LogError("PaymentDetailsCopyForGroupInvoices catch block:  error : " + ex.Message);
            }

            return msg;
        }
        private string UpdateInvoicePaymentType(int invoiceId)
        {
            string msg = string.Empty;
            SqlParameter[] spParams;
            dbUtil.ConnectionString = this.CbmsConnectionString;
            DataSet dsResult = null;

            spParams = new SqlParameter[2];
            spParams[0] = new SqlParameter("@CU_INVOICE_ID", SqlDbType.Int);
            spParams[0].Value = invoiceId;
            spParams[1] = new SqlParameter("@Payment_Invoice_Type", SqlDbType.VarChar);
            spParams[1].Value = DBNull.Value;

            dsResult =dbUtil.ExecuteSQLQuery("dbo.Upd_Payment_Type_Cd_By_Cu_Invoice_Id", spParams);

            return msg;
        }
        public void CommonUpdatePaymentflagAndType(int invoiceId, int userId)
        {
            UpdatePriorityFlag(invoiceId,userId);
            UpdateInvoicePaymentType(invoiceId);
        }
        public List<AccountDetails> GetPaymentStartDate(int invoiceId)
        {
            List<AccountDetails> paymentStartDate = null;
            SqlParameter[] spParams;
            dbUtil.ConnectionString = this.CbmsConnectionString;
            DataSet dsResult = null;
            try
            {
                spParams = new SqlParameter[1];
                spParams[0] = new SqlParameter("@CU_INVOICE_ID", SqlDbType.Int);
                spParams[0].Value = invoiceId;

                dsResult = dbUtil.ExecuteSQLQuery("dbo.Site_Account_Dtl_Sel_By_Cu_Invoice_Id", spParams);

                if (dsResult != null && dsResult.Tables.Count > 0)
                {
                    var query = from objAccount in dsResult.Tables[0].AsEnumerable()
                                select new AccountDetails()
                                {
                                    PaymentStartDate = objAccount.Field<DateTime?>("Payment_Effective_Date"),
                                    Ubm = objAccount.Field<string>("UBM_NAME"),
                                    GroupKey = objAccount.Field<string>("Group_Key"),
                                    ClientId = objAccount.Field<int?>("Client_Id"),
                                    IsPaymentAccount = objAccount.Field<bool?>("Is_Payment_Account"),
                                    ImageId = objAccount.Field<int?>("CBMS_IMAGE_ID"),
                                    IsPaymentGroup = objAccount.Field<bool?>("Is_Payment_Group"),
                                    AccountId = objAccount.Field<int?>("Account_Id"),
                                    PrimaryParentAccountId = objAccount.Field<int?>("Primary_Account_Id"),
                                    GroupPGI = objAccount.Field<int?>("Payment_Group_Indicator_Id")
                                };
                    paymentStartDate = query.ToList();
                }
            }
            catch (Exception ex)
            {
                logger.LogError("getPaymentStartDate catch block:  error : " + ex.Message);
            }

            return paymentStartDate;
        }
        public Task<PayloadDataToAzureSB> sendTvpDataGettingPaymentDetails(DataTable tvpdata,int pgiId)
        {
            PayloadDataToAzureSB payloadDataToAzureSB = null;
            SqlParameter[] spParams;
            int CbmsPriorityAccountId = 0;
            int InvoicePaymentId = pgiId;
            dbUtil.ConnectionString = this.CbmsConnectionString;
            try
            {
                spParams = new SqlParameter[1];
                spParams[0] = new SqlParameter("@Tvp_Cu_Invoice_Payment_Dtl_Ids", SqlDbType.Structured);
                spParams[0].Value = tvpdata;

                DataSet dsResult = dbUtil.ExecuteSQLQuery("dbo.Cu_Invoice_Payment_Dtl_Sel_By_Cu_Invoice_Payment_Dtl_Id", spParams);
                if (dsResult != null && dsResult.Tables.Count > 0)
                {
                    var CbmsAccountIdPriorityList = dsResult.Tables[0].AsEnumerable().Where(s => s.Field<bool?>("Is_Primary_Account") == true).Select(s => s.Field<Int32>("Cbms_Account_Id")).ToArray();
                    if (CbmsAccountIdPriorityList.Length>0)
                    {
                        CbmsPriorityAccountId = CbmsAccountIdPriorityList[0];
                    }
                    payloadDataToAzureSB = new PayloadDataToAzureSB();
                    payloadDataToAzureSB.paymentGroupIndicator = InvoicePaymentId;
                    foreach (DataRow obj in dsResult.Tables[0].AsEnumerable())
                    {
                        PaymentDetailsForD365 paymentDetailsForD365 = new PaymentDetailsForD365();
                        paymentDetailsForD365.cbmsInvoiceId = obj.Field<int>("Cu_Invoice_Id");
                        paymentDetailsForD365.invoiceNumber = obj.Field<string>("InvoiceNumber");
                        paymentDetailsForD365.invoiceType = obj.Field<string>("InvoiceType");
                        paymentDetailsForD365.paymentType = obj.Field<string>("PaymentType");
                        paymentDetailsForD365.d365IntegrationId = obj.Field<string>("External_Vendor_XId");
                        //paymentDetailsForD365.receivedDate = obj.Field<DateTime>("ReceivedDate").ToString("dd-MM-yyyy")
                        paymentDetailsForD365.dueDate = obj.Field<DateTime>("DueDate").ToString("dd-MM-yyyy");
                        paymentDetailsForD365.netValue = obj.Field<string>("NetValue");
                        //paymentDetailsForD365.grossValue = obj.Field<string>("GrossValue");
                        //paymentDetailsForD365.paymentAmount = obj.Field<string>("PaymentAmount");
                        //paymentDetailsForD365.vatValue = obj.Field<string>("VATValue");
                        paymentDetailsForD365.invoiceStartDate = obj.Field<DateTime>("InvoiceStartDate").ToString("dd-MM-yyyy");
                        paymentDetailsForD365.invoiceEndDate = obj.Field<DateTime>("InvoiceEndDate").ToString("dd-MM-yyyy");
                        paymentDetailsForD365.cbmsBucketName = obj.Field<string>("Cbms_Bucket_Name");
                        paymentDetailsForD365.lineItemName = obj.Field<string>("Line_Item_Name");
                        paymentDetailsForD365.lineItemAmount = GetDecimalValue(MakeSafeDecimal(obj.Field<string>("Line_Item_Amount"))).ToString();
                        paymentDetailsForD365.cbmsAccountId = CbmsPriorityAccountId != 0 ? CbmsPriorityAccountId : obj.Field<int?>("Cbms_Account_Id");
                        //paymentDetailsForD365.cbmsSiteId = objAccount.Field<int>("Cbms_Site_Id");
                        paymentDetailsForD365.commodity = obj.Field<string>("Commodity_Name");
                        paymentDetailsForD365.marketIndicator = obj.Field<string>("MarketIndicator");
                        paymentDetailsForD365.issueDate = obj.Field<DateTime>("IssueDate").ToString("dd-MM-yyyy");
                        paymentDetailsForD365.currency = obj.Field<string>("Currency");
                        paymentDetailsForD365.vendorType = obj.Field<string>("Approved_Vendor_Type");

                        if (paymentDetailsForD365.cbmsInvoiceId == InvoicePaymentId)
                        {
                            payloadDataToAzureSB.paymentGroupIndicator = InvoicePaymentId;
                            payloadDataToAzureSB.paymentAmount = obj.Field<string>("PaymentAmount");
                            payloadDataToAzureSB.grossValue = obj.Field<string>("GrossValue");
                        }
                        payloadDataToAzureSB.items.Add(paymentDetailsForD365);
                    };
                    payloadDataToAzureSB.itemsCount = payloadDataToAzureSB.items.Count; 
                }

            }
            catch (Exception ex)
            {
                logger.LogError("sendTvpDataGettingPaymentDetails catch block:  error : " + ex.Message);
            }
            return Task.Run(() => payloadDataToAzureSB);
        }

        public decimal GetDecimalValue(decimal originalValue)
        {
            decimal decimalValue = 0;
            string formattedValue = originalValue.ToString("0.#################");
            decimalValue = decimal.Parse(formattedValue);
            return decimalValue;
        }

        public Task<string> UbmGenericEtlPaymentDetails(int invoiceId)
        {
            string PaymentData = string.Empty;
            SqlParameter[] spParams;
            dbUtil.ConnectionString = this.CbmsConnectionString;
            bool decimalValue = true;
            int i = 0;
            List<int> chargeCount = new List<int>();
            StringBuilder moreDecimalComment = new StringBuilder();
            moreDecimalComment.Append(Constants.DecimalsStartingComment);
            int DecimalPlaceValue = Convert.ToInt32(_config.GetSection("AppSettings").GetSection("DecimalPlaceValue").Value);

            try
            {
                spParams = new SqlParameter[1];
                spParams[0] = new SqlParameter("@Cu_Invoice_Id", SqlDbType.Int);
                spParams[0].Value = invoiceId;

                DataSet dsResult = dbUtil.ExecuteSQLQuery("dbo.Cu_Invoice_Payment_Attribute_Tracking_Sel_By_Cu_Invoice_Id", spParams);
                if (dsResult != null && dsResult.Tables.Count > 0 && dsResult.Tables[0].Rows.Count>0)
                {
                    try
                    {
                        if (!String.IsNullOrEmpty(dsResult.Tables[0].Rows[0]["PaymentAmount"].ToString()))
                        {

                            string configcharges = _config.GetSection("AppSettings").GetSection("ChargesExcluded").Value;
                            int systemUser = Convert.ToInt32(_config.GetSection("AppSettings").GetSection("SystemUser").Value);
                            string[] excludeCharges = configcharges.Split(",");
                            List<GBPChargeDetails> charges = LoadChargeValues(invoiceId);
                            foreach (var charge in charges)
                            {
                                string formattedValue = charge.ChargeValue.ToString("0.#################");
                                decimal result = decimal.Parse(formattedValue);
                                int decimalPlaces = BitConverter.GetBytes(decimal.GetBits(result)[3])[2];
                                if (decimalPlaces > DecimalPlaceValue)
                                {
                                    moreDecimalComment.Append(" " + charge.ChargeName + ": " + result + " " + charge.CuDeterminantCode).Append(",");
                                    chargeCount.Add(i++);
                                }
                            }
                            if (!String.IsNullOrEmpty(dsResult.Tables[0].Rows[0]["Deposit"].ToString()))
                            {
                                string DepositformattedValue = Convert.ToDecimal(dsResult.Tables[0].Rows[0]["Deposit"]).ToString("0.#################");
                                decimal Depositresult = decimal.Parse(DepositformattedValue);
                                int decimalPlaces = BitConverter.GetBytes(decimal.GetBits(Depositresult)[3])[2];
                                if (decimalPlaces > DecimalPlaceValue)
                                {
                                    moreDecimalComment.Append(" Deposit : " + Depositresult).Append(",");
                                    chargeCount.Add(i++);
                                }
                            }
                            if (!String.IsNullOrEmpty(dsResult.Tables[0].Rows[0]["BalanceBroughtForward"].ToString()))
                            {
                                string BalanceBroughtForwardformattedValue = Convert.ToDecimal(dsResult.Tables[0].Rows[0]["BalanceBroughtForward"]).ToString("0.#################");
                                decimal BalanceBroughtForwardresult = decimal.Parse(BalanceBroughtForwardformattedValue);
                                int decimalPlaces = BitConverter.GetBytes(decimal.GetBits(BalanceBroughtForwardresult)[3])[2];
                                if (decimalPlaces > DecimalPlaceValue)
                                {
                                    moreDecimalComment.Append(" BalanceBroughtForward : " + BalanceBroughtForwardresult).Append(",");
                                    chargeCount.Add(i++);
                                }
                            }
                            if (!String.IsNullOrEmpty(dsResult.Tables[0].Rows[0]["PaymentAmount"].ToString()))
                            {
                                string paymentAmountformattedValue = Convert.ToDecimal(dsResult.Tables[0].Rows[0]["PaymentAmount"]).ToString("0.#################");
                                decimal paymentAmountresult = decimal.Parse(paymentAmountformattedValue);
                                int decimalPlaces = BitConverter.GetBytes(decimal.GetBits(paymentAmountresult)[3])[2];
                                if (decimalPlaces > DecimalPlaceValue)
                                {
                                    moreDecimalComment.Append(" PaymentAmount : " + paymentAmountresult);
                                    chargeCount.Add(i++);
                                }
                            }
                            if (chargeCount.Count > 0)
                            {
                                decimalValue = false;
                                cbmsCuInvoiceCommentSave(systemUser, invoiceId, moreDecimalComment.ToString());
                            }
                            if (decimalValue == true)
                            {
                                List<GBPChargeDetails> NetCharges = charges.FindAll(e => !excludeCharges.Contains(e.Bucket.ToLower()));
                                dsResult.Tables[0].Rows[0]["GrossValue"] = String.Format("{0:0.00}", charges.Sum(e => e.ChargeValue));
                                dsResult.Tables[0].Rows[0]["NetValue"] = String.Format("{0:0.00}", NetCharges.Sum(e => e.ChargeValue));
                                if (!String.IsNullOrEmpty(dsResult.Tables[0].Rows[0]["Deposit"].ToString()))
                                {
                                    dsResult.Tables[0].Rows[0]["GrossValue"] = Convert.ToDecimal(dsResult.Tables[0].Rows[0]["GrossValue"]) + Convert.ToDecimal(dsResult.Tables[0].Rows[0]["Deposit"]);
                                }
                                if (!String.IsNullOrEmpty(dsResult.Tables[0].Rows[0]["BalanceBroughtForward"].ToString()))
                                {
                                    dsResult.Tables[0].Rows[0]["GrossValue"] = Convert.ToDecimal(dsResult.Tables[0].Rows[0]["GrossValue"]) + Convert.ToDecimal(dsResult.Tables[0].Rows[0]["BalanceBroughtForward"]);
                                }
                                var arrayData = JsonConvert.SerializeObject(dsResult.Tables[0]);
                                JArray jsonArray = JArray.Parse(arrayData);
                                var objectData = JObject.Parse(jsonArray[0].ToString());
                                PaymentData = JsonConvert.SerializeObject(objectData);
                            }
                            else
                            {
                                PaymentData = String.Empty;
                            }
                        }
                    }
                    catch
                    {
                        logger.LogError("Charge aggrigation catch block:  error");
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError("UbmGenericEtlPaymentDetails catch block:  error : " + ex.Message);
            }
            return Task.Run(() => PaymentData);
        }

        public void cbmsCuInvoiceCommentSave(int invoiceId, int userId,string imageComment)
        {
            SqlParameter[] spParams;
            dbUtil.ConnectionString = this.CbmsConnectionString;
            try
            {
                spParams = new SqlParameter[3];
                spParams[0] = new SqlParameter("@MyAccountId", SqlDbType.Int);
                spParams[0].Value = invoiceId;
                spParams[1] = new SqlParameter("@cu_invoice_id", SqlDbType.Int);
                spParams[1].Value = userId;
                spParams[2] = new SqlParameter("@image_comment", SqlDbType.VarChar);
                spParams[2].Value = imageComment;

                DataSet dsResult = dbUtil.ExecuteSQLQuery("dbo.cbmsCuInvoiceComment_Save", spParams);
            }
            catch (Exception ex)
            {
                logger.LogError("cbmsCuInvoiceCommentSave catch block:  error : " + ex.Message);
            }
        }

        private Task<int> getCuInvoicePaymentReqId(int userInfoId,string payloadDataToD365)
        {
            int paymentRequestId = 0;
            try
            {
                dbUtil.ConnectionString = this.CbmsConnectionString;
                SqlParameter[] spParams = new SqlParameter[2];
                spParams[0] = new SqlParameter("@User_Info_Id", SqlDbType.Int);
                spParams[0].Value = userInfoId;
                spParams[1] = new SqlParameter("@Request_Json", SqlDbType.NVarChar);
                spParams[1].Value = payloadDataToD365;
                DataSet dsResult = dbUtil.ExecuteSQLQuery("dbo.Cu_Invoice_Payment_Request_Ins", spParams);
                if (dsResult != null && dsResult.Tables.Count > 0 && dsResult.Tables[0].Rows.Count > 0)
                {
                    paymentRequestId = dsResult.Tables[0].Rows[0].Field<int>("Cu_Invoice_Payment_Request_Id");
                }
            }
            catch (Exception ex)
            {
                logger.LogError("getCuInvoicePaymentReqId catch block:  error : " + ex.Message);
            }
            return Task.Run(() => paymentRequestId);
        }

        private Task<string> UpdatePaymentResponseStatus(List<PaymentImageDetailFields> paymentAttrbute, int paymentReqId)
        {
            string message = string.Empty;
            try
            {
                foreach (var item in paymentAttrbute)
                {
                    SqlParameter[] spParams;
                    dbUtil.ConnectionString = this.CbmsConnectionString;
                    spParams = new SqlParameter[6];
                    spParams[0] = new SqlParameter("@Cu_Invoice_Id", SqlDbType.Int);
                    spParams[0].Value = item.InvoiceId;
                    spParams[1] = new SqlParameter("@Cu_Invoice_Payment_Dtl_Id", SqlDbType.Int);
                    spParams[1].Value = item.PaymentAttributeDtlId;
                    spParams[2] = new SqlParameter("@Cu_Invoice_Payment_Request_Id", SqlDbType.Int);
                    spParams[2].Value = paymentReqId;
                    spParams[3] = new SqlParameter("@Payment_Status", SqlDbType.VarChar);
                    spParams[3].Value = "SENT";
                    spParams[4] = new SqlParameter("@Error_Desc", SqlDbType.VarChar);
                    spParams[4].Value = "";
                    spParams[5] = new SqlParameter("@User_Info_ID", SqlDbType.Int);
                    spParams[5].Value = item.UserInfoId;
                    message = dbUtil.ExecuteSQLNonQuery("dbo.Cu_Invoice_Payment_Status_Upd", spParams);
                }
            }
            catch (Exception ex)
            {
                logger.LogError("UpdatePaymentResponseStatus catch block:  error : " + ex.Message);
            }
            return Task.Run(() => message);
        }
        public async Task<bool> ProcessDataSendTod365(List<PaymentImageDetailFields> paymentAtrributes, int invoiceId, int userId,int CbmsImageId,int? GroupPGI=null)
        {

            bool datamsg = true;
            try
            {

                if (paymentAtrributes != null && paymentAtrributes.Count > 0)
                {
                    logger.LogInformation("paymentAtrributes with invoiceId : " + invoiceId);
                    // Saving Details in tvp
                    DataTable dtTvpQueueidData = new DataTable();
                    int idx = 1;
                    dtTvpQueueidData.Columns.Add("Row_Num");
                    dtTvpQueueidData.Columns.Add("Cu_Invoice_Id");
                    dtTvpQueueidData.Columns.Add("Cu_Invoice_Payment_Dtl_Id");
                    DataRow dr = null;
                    var atrrDetails = GetPaymentAttributesIncludeMoveSelected(paymentAtrributes);
                    foreach (var payment in atrrDetails)
                    {
                        logger.LogInformation("Start : Saving Payment Data in TVP with InvoiceId :" + payment.InvoiceId);
                        dr = dtTvpQueueidData.NewRow();
                        dr["Row_Num"] = idx++;
                        dr["Cu_Invoice_Id"] = payment.InvoiceId;
                        dr["Cu_Invoice_Payment_Dtl_Id"] = payment.PaymentAttributeDtlId;
                        dtTvpQueueidData.Rows.Add(dr);
                        logger.LogInformation("End : Saving Payment Data in TVP with InvoiceId :" + payment.InvoiceId);
                    }
                    string key = _config.GetSection("AppSettings").GetSection("Key").Value;
                    // Getting Details to send D365
                    logger.LogInformation("Start : Sending Details To D365 with InvoiceId Details :" + dtTvpQueueidData);
                    PayloadDataToAzureSB paymentDetailsForD365 = await sendTvpDataGettingPaymentDetails(dtTvpQueueidData, paymentAtrributes[0].InvoiceId.Value);
                    if (paymentDetailsForD365 != null)
                    {
                        //passing encrypted imageid to the URL
                        string encryptImg = EncryptionHelper.Encrypt(CbmsImageId.ToString(), key);
                        string imgUrl = _config.GetSection("AppSettings").GetSection("ExternalImageUrl").Value;

                        paymentDetailsForD365.invoiceFile = imgUrl.Replace("{encryptedId}", encryptImg);
                        //end passing encrypted imageid to the URL
                        logger.LogInformation("End : Sending Details To D365 with InvoiceId Details :" + dtTvpQueueidData);
                        string SendDataToServiceBus = Newtonsoft.Json.JsonConvert.SerializeObject(paymentDetailsForD365);
                        int paymentReqId = await getCuInvoicePaymentReqId(userId, SendDataToServiceBus);
                        logger.LogInformation("Start : Calling the AzureServiceBus");
                        var consolehit = new ServiceBusSenderModel(this._config.GetSection("AppSettings").GetSection("ServiceBusConnectionString").Value);
                        var message = consolehit.SendMessageAsync(SendDataToServiceBus, this._config.GetSection("AppSettings").GetSection("SBQueueName").Value).Result;
                        if (message != "Success")
                        {
                            logger.LogInformation("Start : Calling the Retry For AzureServiceBus with Message :" + message);
                            int RetryCount = Convert.ToInt32(_config.GetSection("AppSettings").GetSection("RetryCount").Value);
                            for (int i = 0; i < RetryCount; i++)
                            {
                                int timeout = Convert.ToInt32(_config.GetSection("AppSettings").GetSection("TimeoutForRetryInMilSec").Value);
                                Thread.Sleep(timeout);
                                message = consolehit.SendMessageAsync(SendDataToServiceBus, this._config.GetSection("AppSettings").GetSection("SBQueueName").Value).Result;
                                if (message == "Success")
                                {
                                    break;
                                }
                            }
                            logger.LogInformation("End : Calling the Retry For AzureServiceBus with Message :" + message);
                        }

                        logger.LogInformation("End : Calling the AzureServiceBus with Message :" + message);
                        await UpdatePaymentResponseStatus(paymentAtrributes, paymentReqId);
                        foreach (var paymentInv in paymentAtrributes)
                        {

                            if (paymentInv.IsMoveSelectedMonthInv == true)
                            {
                                // sel sp parent inv id  == gbp inv paytey true
                                List<MoveSelectedInvoiceDetails> moveSelectedInvoiceDetails = GetMoveSelectedInvoices(paymentInv.CbmsImageId);
                                if (moveSelectedInvoiceDetails != null)
                                {
                                    foreach (var movSelInv in moveSelectedInvoiceDetails)
                                    {
                                        HistoryProcessSection historyProcessSection = GetHistoryProcessDetails(movSelInv.CuInvoiceId);
                                        if (historyProcessSection != null)
                                        {
                                            historyProcessSection.IsPaymentSubmission = true;
                                            SaveHistoryProcessDetails(historyProcessSection);
                                            if (paymentInv.InvoiceId == movSelInv.CuInvoiceId)
                                            {
                                                logEventDetails(paymentInv.UserInfoId, paymentInv.InvoiceId, "Updated Payment Submission : " + historyProcessSection.IsPaymentSubmission);
                                                logEventDetails(paymentInv.UserInfoId, paymentInv.InvoiceId, "Passed Payment Validation - Sent to D365");
                                            }
                                            else
                                            {
                                                logEventDetails(paymentInv.UserInfoId, paymentInv.InvoiceId, "Updated Payment Submission : " + historyProcessSection.IsPaymentSubmission);
                                                logEventDetails(paymentInv.UserInfoId, movSelInv.CuInvoiceId, "Move Selected to Other Month - Payment Submission completed for copy invoice");
                                            }

                                        }
                                    }
                                }
                            }
                            else
                            {
                                logger.LogInformation("Generic ETL : " + paymentInv.InvoiceId);
                                HistoryProcessSection historyProcessSection = GetHistoryProcessDetails(paymentInv.InvoiceId);
                                logger.LogInformation("Generic ETL historyProcessSection : " + historyProcessSection);
                                if (historyProcessSection != null)
                                {
                                    logger.LogInformation("Generic ETL historyProcessSection started");
                                    historyProcessSection.IsPaymentSubmission = true;
                                    logger.LogInformation("Generic ETL historyProcessSection boolean data : " + historyProcessSection.IsPaymentSubmission);
                                    SaveHistoryProcessDetails(historyProcessSection);
                                    logEventDetails(paymentInv.UserInfoId, paymentInv.InvoiceId, "Updated Payment Submission : " + historyProcessSection.IsPaymentSubmission);
                                    logger.LogInformation("Generic ETL historyProcessSection end");
                                }
                                logEventDetails(paymentInv.UserInfoId, paymentInv.InvoiceId, "Passed Payment Validation - Sent to D365");
                            }
                        }
                    }

                }
                else
                {
                    datamsg = false;
                    logger.LogInformation("paymentAtrributes Failed with invoiceId : " + invoiceId + "Attributes Data :" + paymentAtrributes);
                    logEventDetails(userId, invoiceId, GroupPGI!=null?Constants.FailedUBMGroupMsg:Constants.FailedLargeGroupMsg);
                }

            }
            catch (Exception ex)
            {
                logger.LogError("ProcessDataSendTod365 catch block:  error : " + ex.Message);
                datamsg = false;
                throw;
            }
            return datamsg;
        }
        public void PaymentDetailsCopyForGroupInvoices(int? invoiceId, int userId)
        {
            SqlParameter[] spParams;
            dbUtil.ConnectionString = this.CbmsConnectionString;
            try
            {
                spParams = new SqlParameter[2];
                spParams[0] = new SqlParameter("@Sent_Payment_Cu_Invoice_Id", SqlDbType.Int);
                spParams[0].Value = invoiceId;
                spParams[1] = new SqlParameter("@User_Info_Id", SqlDbType.Int);
                spParams[1].Value = userId;

                DataSet dsResult = dbUtil.ExecuteSQLQuery("dbo.Cu_Invoice_Payment_Dtls_Copy_For_Group_Invoices", spParams);
            }
            catch (Exception ex)
            {
                logger.LogError("PaymentDetailsCopyForGroupInvoices catch block:  error : " + ex.Message);
            }
        }
        private HistoryProcessSection GetHistoryProcessDetails  (int? invoiceId)
        {
            HistoryProcessSection processData = null;
            try
            {
                SqlParameter[] spParams;
                dbUtil.ConnectionString = this.CbmsConnectionString;
                spParams = new SqlParameter[1];
                spParams[0] = new SqlParameter("@cu_invoice_id", SqlDbType.Int);
                spParams[0].Value = invoiceId;
                
                DataSet dsResult = dbUtil.ExecuteSQLQuery("dbo.cbmsCuInvoiceProcess_GetForCuInvoice", spParams);

                if (dsResult != null && dsResult.Tables.Count > 0)
                {
                    if (dsResult.Tables[0].Rows.Count > 0)
                    {
                        var query = from obj in dsResult.Tables[0].AsEnumerable()
                                    select new HistoryProcessSection()
                                    {
                                        CuInvoiceProcessId = obj.Field<int>("CU_INVOICE_PROCESS_ID"),
                                        CuInvoiceId = obj.Field<int>("CU_INVOICE_ID"),
                                        CuInvoiceCreated = obj.Field<bool>("CU_INVOICE_CREATED"),
                                        ServiceMapping = obj.Field<bool>("SERVICE_MAPPING"),
                                        ResolveToAccount = obj.Field<bool>("RESOLVE_TO_ACCOUNT"),
                                        CuDuplicateTest = obj.Field<bool>("CU_DUPLICATE_TEST"),
                                        RequiredData = obj.Field<bool>("REQUIRED_DATA"),
                                        DataMapped = obj.Field<bool>("DATA_MAPPED"),
                                        RaWatchList = obj.Field<bool>("RA_WATCH_LIST"),
                                        ResolveToMonth = obj.Field<int>("RESOLVE_TO_MONTH")==1?true:false,
                                        ContractRecalc = obj.Field<bool>("CONTRACT_RECALC"),
                                        InvoiceAggregate = obj.Field<bool>("INVOICE_AGGREGATE"),
                                        VarianceTest = obj.Field<bool>("VARIANCE_TEST"),
                                        IsDataQualityTest = obj.Field<bool>("Is_Data_Quality_Test"),
                                        Account_ID = obj.Field<int>("Account_ID"),
                                        IsPaymentSubmission = obj.Field<bool>("Is_Payment_Submission")
                                    };
                        processData = query.FirstOrDefault();
                    }
                }

            }
            catch (Exception ex)
            {
                logger.LogError("GetHistoryProcessDetails catch block:  error : " + ex.Message);
            }
            return processData;
        }
        private void SaveHistoryProcessDetails(HistoryProcessSection proObj)
        {
            try
            {
                logger.LogInformation("SaveHistoryProcessDetails Sp Execution started");
                SqlParameter[] spParams;
                dbUtil.ConnectionString = this.CbmsConnectionString;
                spParams = new SqlParameter[17];
                spParams[0] = new SqlParameter("@MyAccountId", SqlDbType.Int);
                spParams[0].Value = proObj.Account_ID;
                spParams[1] = new SqlParameter("@cu_invoice_process_id", SqlDbType.Int);
                spParams[1].Value = proObj.CuInvoiceProcessId;
                spParams[2] = new SqlParameter("@cu_invoice_id", SqlDbType.Int);
                spParams[2].Value = proObj.CuInvoiceId;
                spParams[3] = new SqlParameter("@cu_invoice_created", SqlDbType.Bit);
                spParams[3].Value = proObj.CuInvoiceCreated;
                spParams[4] = new SqlParameter("@service_mapping", SqlDbType.Bit);
                spParams[4].Value = proObj.ServiceMapping;
                spParams[5] = new SqlParameter("@resolve_to_account", SqlDbType.Bit);
                spParams[5].Value = proObj.ResolveToAccount;
                spParams[6] = new SqlParameter("@cu_duplicate_test", SqlDbType.Bit);
                spParams[6].Value = proObj.CuDuplicateTest;
                spParams[7] = new SqlParameter("@dm_watch_list", SqlDbType.Bit);
                spParams[7].Value = proObj.DmWatchList;
                spParams[8] = new SqlParameter("@required_data", SqlDbType.Bit);
                spParams[8].Value = proObj.RequiredData;
                spParams[9] = new SqlParameter("@data_mapped", SqlDbType.Bit);
                spParams[9].Value = proObj.DataMapped;
                spParams[10] = new SqlParameter("@ra_watch_list", SqlDbType.Bit);
                spParams[10].Value = proObj.RaWatchList;
                spParams[11] = new SqlParameter("@resolve_to_month", SqlDbType.Bit);
                spParams[11].Value = proObj.ResolveToMonth;
                spParams[12] = new SqlParameter("@contract_recalc", SqlDbType.Bit);
                spParams[12].Value = proObj.ContractRecalc;
                spParams[13] = new SqlParameter("@invoice_aggregate", SqlDbType.Bit);
                spParams[13].Value = proObj.InvoiceAggregate;
                spParams[14] = new SqlParameter("@variance_test", SqlDbType.Bit);
                spParams[14].Value = proObj.VarianceTest;
                spParams[15] = new SqlParameter("@Is_Data_Quality_Test", SqlDbType.Bit);
                spParams[15].Value = proObj.IsDataQualityTest;
                spParams[16] = new SqlParameter("@Is_Payment_Submitted", SqlDbType.Bit);
                spParams[16].Value = proObj.IsPaymentSubmission;

                DataSet dsResult = dbUtil.ExecuteSQLQuery("dbo.cbmsCuInvoiceProcess_Save", spParams);
                logger.LogInformation("SaveHistoryProcessDetails Sp Execution Ended");
            }
            catch (Exception ex)
            {
                logger.LogError("SaveHistoryProcessDetails catch block:  error : " + ex.Message);
            }
        }
        public List<MoveSelectedInvoiceDetails> GetMoveSelectedInvoices(int? CbmsImageId)
        {
            List<MoveSelectedInvoiceDetails> InvDetails = null;
            SqlParameter[] spParams;
            dbUtil.ConnectionString = this.CbmsConnectionString;
            try
            {
                spParams = new SqlParameter[1];
                spParams[0] = new SqlParameter("@Cbms_Image_Id", SqlDbType.Int);
                spParams[0].Value = CbmsImageId;

                DataSet dsResult = dbUtil.ExecuteSQLQuery("dbo.Cu_Invoice_Group_Dtl_Sel", spParams);
                if (dsResult != null && dsResult.Tables.Count > 0 && dsResult.Tables[0].Rows.Count > 0)
                {
                    var query = from objAccount in dsResult.Tables[0].AsEnumerable()
                                select new MoveSelectedInvoiceDetails()
                                {
                                    CbmsImageId = objAccount.Field<int>("Cbms_Image_Id"),
                                    CuInvoiceId = objAccount.Field<int>("Cu_Invoice_Id"),
                                    PaymentAttributeDtlId = objAccount.Field<int?>("Cu_Invoice_Payment_Dtl_Id"), 
                                    IsReported = objAccount.Field<bool>("IS_REPORTED"),
                                    IsProcessed = objAccount.Field<bool>("IS_PROCESSED"),
                                    IsClosed = (objAccount.Field<bool>("Is_Reported") == false && objAccount.Field<bool>("Is_Processed") == true) ? true :false
                                };

                    InvDetails = query.ToList();
                }

            }
            catch (Exception ex)
            {
                logger.LogError("GetMoveSelectedInvoices catch block:  error : " + ex.Message);
            }
            return InvDetails;
        }

        public Task<string> SaveMultipleInvoices(List<User> grpinvoices)
        {
            string msg = string.Empty;
            
            try
            {
                foreach (var item in grpinvoices)
                {
                    SqlParameter[] spParams;
                    dbUtil.ConnectionString = this.CbmsConnectionString;
                    spParams = new SqlParameter[4];
                    spParams[0] = new SqlParameter("@Cu_Invoice_Id", SqlDbType.Int);
                    spParams[0].Value = item.InvoiceId;
                    spParams[1] = new SqlParameter("@Account_Id", SqlDbType.Int);
                    spParams[1].Value = item.AccountId;
                    spParams[2] = new SqlParameter("@Invoice_Number", SqlDbType.NVarChar);
                    spParams[2].Value = item.InvoiceNumber != null ? item.InvoiceNumber : "";
                    spParams[3] = new SqlParameter("@user_Info_Id", SqlDbType.Int);
                    spParams[3].Value = item.UserInfoId;
                    msg = dbUtil.ExecuteSQLNonQuery("dbo.Cu_Invoice_Payment_Invoice_Number_Merge", spParams);

                }

            }
            catch (Exception ex)
            {
                logger.LogError("SaveMultipleInvoices catch block:  error : " + ex.Message);
            }
            return Task.Run(() => msg);
        }
        public Task<string> SavePaymentFieldsData(PaymentFieldDetails paymentFieldDetails)
        {
            string msg = string.Empty;
            try
            {
                DataSet dsResult;

                SqlParameter[] spParams;
                dbUtil.ConnectionString = this.CbmsConnectionString;
                spParams = new SqlParameter[3];
                spParams[0] = new SqlParameter("@Cu_Invoice_Id", SqlDbType.Int);
                spParams[0].Value = paymentFieldDetails.InvoiceId;
                spParams[1] = new SqlParameter("@Payment_Attribute_Value", SqlDbType.NVarChar);
                spParams[1].Value = paymentFieldDetails.invPaymentDetails;
                spParams[2] = new SqlParameter("@User_Info_ID", SqlDbType.Int);
                spParams[2].Value = paymentFieldDetails.UserId;
                dsResult = dbUtil.ExecuteSQLQuery("Cu_Invoice_Payment_Attribute_Tracking_Merge", spParams);
                msg = "Success";
            }
            catch (Exception ex)
            {
                logger.LogError("SavePaymentFieldsData catch block:  error : " + ex.Message);
            }
            return Task.Run(() => msg);
        }


        private List<PaymentImageDetailFields> GetPaymentAttributesIncludeMoveSelected(List<PaymentImageDetailFields> paymentAtrributes)
        {
            List<PaymentImageDetailFields> details = new List<PaymentImageDetailFields>(paymentAtrributes);
            try
            {
                List<PaymentImageDetailFields> moveselectedpaymentAtrributes = details.FindAll(e => e.IsMoveSelectedMonthInv);                
                if (moveselectedpaymentAtrributes.Count > 0)
                {
                    details.RemoveAll(e => e.IsMoveSelectedMonthInv);
                    foreach (var movinv in moveselectedpaymentAtrributes)
                    {
                        PaymentDetailsCopyForGroupInvoices(movinv.InvoiceId, movinv.UserInfoId);
                        List<MoveSelectedInvoiceDetails> moveSelectedInvoiceDetails = GetMoveSelectedInvoices(movinv.CbmsImageId);
                       
                        moveSelectedInvoiceDetails.ForEach(e =>
                        {
                            PaymentImageDetailFields attDtl = new PaymentImageDetailFields();
                            attDtl.InvoiceId = e.CuInvoiceId;
                            attDtl.PaymentAttributeDtlId = e.PaymentAttributeDtlId;
                            details.Add(attDtl);
                        });
                    }
                }
            }
            catch
            {
                logger.LogError("GetPaymentAttributesIncludeMoveSelected catch block:  error ");
            }
            return details;
        }
        private List<GBPChargeDetails> LoadChargeValues(int invoiceId)
        {
            List<GBPChargeDetails> chargeValues = null;
            SqlParameter[] spParams;
            dbUtil.ConnectionString = this.CbmsConnectionString;
            try
            {
                spParams = new SqlParameter[1];
                spParams[0] = new SqlParameter("@cu_invoice_id", SqlDbType.Int);
                spParams[0].Value = invoiceId;

                // DataSet dsResult = dbUtil.ExecuteSQLQuery("dbo.cbmsCuInvoiceCharge_GetForInvoice", spParams);
                DataSet dsResult = dbUtil.ExecuteSQLQuery("dbo.Cu_Invoice_Charge_Dtl_Sel_By_Invoice", spParams);
                if (dsResult != null && dsResult.Tables.Count > 0)
                {

                    var query = from objAccount in dsResult.Tables[0].AsEnumerable()
                                select new GBPChargeDetails()
                                {
                                    InvoiceId = objAccount.Field<int>("Cu_Invoice_Id"),
                                    AccountId = objAccount.Field<int?>("Account_Id"),
                                    AccountNumber = objAccount.Field<string>("Account_Number"),
                                    UbmService = objAccount.Field<string>("UBM_SERVICE_CODE"),
                                    Commodity = objAccount.Field<string>("commodity_type"),
                                    UbmBucket = objAccount.Field<string>("UBM_BUCKET_CODE"),
                                    Bucket = objAccount.Field<string>("bucket_type"),
                                    ChargeName = objAccount.Field<string>("CHARGE_NAME"),
                                    ChargeValue = MakeSafeDecimal(!String.IsNullOrEmpty(objAccount.Field<string>("chargevalue")) ? objAccount.Field<string>("chargevalue") : objAccount.Field<string>("CHARGE_VALUE")),
                                    CuDeterminantCode = objAccount.Field<string>("CU_DETERMINANT_CODE"),
                                };

                    chargeValues = query.ToList();

                }

            }

            catch
            {
                throw;
            }
            return chargeValues;
        }
        public Task<string> GetLargeGrpVendorDtl(int invoiceId)
        {
            string result = string.Empty;
            SqlParameter[] spParams;
            dbUtil.ConnectionString = this.CbmsConnectionString;
            try
            {
                dbUtil.ConnectionString = this.CbmsConnectionString;
                spParams = new SqlParameter[1];
                spParams[0] = new SqlParameter("@Cu_Invoice_Id", SqlDbType.Int);
                spParams[0].Value = invoiceId;

                DataSet dsResult = dbUtil.ExecuteSQLQuery("dbo.Check_Approved_Payment_Vendor_Exists_By_Cu_Invoice_Id", spParams);
                if (dsResult != null && dsResult.Tables.Count > 0)
                {
                    result = dsResult.Tables[0].AsEnumerable()
                     .Select(obj => obj.Field<string>("Approved_Payment_Vendor_Id")).FirstOrDefault();
                }
            }
            catch
            {
                throw;
            }
            return Task.Run(() => result);

        }

        //private PayloadDataToAzureSBFinal FormatJsonPayload(List<PaymentDetailsForD365> PaymentDetails,int pgiId)
        //{
        //    PayloadDataToAzureSBFinal data = new PayloadDataToAzureSBFinal();
        //    data.itemsCount = PaymentDetails.Count;
        //    data.paymentGroupIndicator = pgiId;
        //    data.invoices = new List<PaymentDetailsForD365Final>();
        //    List<int> invoiceIds = PaymentDetails.Select(e => e.cbmsInvoiceId).Distinct().ToList();
        //    try
        //    {
        //        foreach(int id in invoiceIds)
        //        {
        //            var invData = PaymentDetails.FindAll(e => e.cbmsInvoiceId == id);
        //            var invsingledata  = invData.FirstOrDefault();
        //            if (invsingledata != null)
        //            {
        //                if (invsingledata.cbmsInvoiceId == pgiId)
        //                {
        //                    data.paymentAmount = invsingledata.paymentAmount;
        //                }
        //                PaymentDetailsForD365Final invoiceDetailsForD365 = new PaymentDetailsForD365Final();
        //                invoiceDetailsForD365.invoiceType = invsingledata.invoiceType;
        //                invoiceDetailsForD365.paymentType = invsingledata.paymentType;
        //                invoiceDetailsForD365.d365IntegrationId = invsingledata.d365IntegrationId;
        //                invoiceDetailsForD365.vendorType = invsingledata.vendorType;
        //                invoiceDetailsForD365.issueDate = invsingledata.issueDate;
        //                invoiceDetailsForD365.dueDate = invsingledata.dueDate;
        //                invoiceDetailsForD365.marketIndicator = invsingledata.marketIndicator;
        //                invoiceDetailsForD365.currency = invsingledata.currency;
        //                invoiceDetailsForD365.cbmsInvoiceId = invsingledata.cbmsInvoiceId;
        //                invoiceDetailsForD365.invoiceStartDate = invsingledata.invoiceStartDate;
        //                invoiceDetailsForD365.invoiceEndDate = invsingledata.invoiceEndDate;
        //                invoiceDetailsForD365.netValue = invsingledata.netValue;
        //                //invoiceDetailsForD365.grossValue = invsingledata.grossValue;
        //                invoiceDetailsForD365.lines = new List<LineItemsData>();
        //                foreach (var lineData in invData)
        //                {
        //                    LineItemsData lineItemsData = new LineItemsData();
        //                    lineItemsData.lineItemName = lineData.lineItemName;
        //                    lineItemsData.invoiceNumber = lineData.invoiceNumber;
        //                    lineItemsData.cbmsBucketName = lineData.cbmsBucketName;
        //                    lineItemsData.cbmsAccountId = lineData.cbmsAccountId;
        //                    lineItemsData.commodity = lineData.commodity;
        //                    lineItemsData.lineItemAmount = lineData.lineItemAmount;
        //                    invoiceDetailsForD365.lines.Add(lineItemsData);
        //                }
        //                data.invoices.Add(invoiceDetailsForD365);
        //            }

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.LogError("FormatJsonPayload catch block:  error : " + ex.Message);
        //    }
        //    return data;
        //}
    }
}
