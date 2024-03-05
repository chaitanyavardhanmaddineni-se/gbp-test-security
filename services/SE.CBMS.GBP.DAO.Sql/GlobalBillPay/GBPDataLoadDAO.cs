using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SchneiderElectric.CBMS.GBP.ViewModels.GlobalBillPay;
using SchneiderElectric.CBMS.InvoiceReg.DAO.Sql;
using SE.CBMS.GBP.DAO.Interfaces.GlobalBillPay;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace SchneiderElectric.CBMS.GBP.DAO.Sql.GlobalBillPay
{
    public class GBPDataLoadDAO : BaseData, IGBPDataLoadDAO
    {
        private readonly string CbmsConnectionString;
        private readonly ILogger logger;
        IConfiguration _config;
        SummitDBUtils dbUtil = new SummitDBUtils();
        readonly ILoggerFactory loggerFactory;
        public GBPDataLoadDAO(IConfiguration config, ILoggerFactory loggerFactory)
        {
            this.CbmsConnectionString = config.GetSection("AppSettings").GetSection("ConnectionString").Value;
            _config = config;
            this.loggerFactory = loggerFactory;
            logger = loggerFactory.CreateLogger("GBPDataLoadDAO");
        }

        public List<GBPChargeDetails> LoadChargeValues(int invoiceId, string sortColumn)
        {
            List<GBPChargeDetails> chargeValues = null;
            SqlParameter[] spParams;
            dbUtil.ConnectionString = this.CbmsConnectionString;
            try
            {
                spParams = new SqlParameter[2];
                spParams[0] = new SqlParameter("@cu_invoice_id", SqlDbType.Int);
                spParams[0].Value = invoiceId;
                spParams[1] = new SqlParameter("@Sort_Column", SqlDbType.Char);
                spParams[1].Value = sortColumn;

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
                                    ChargeValue = GetDecimalValue(MakeSafeDecimal(!String.IsNullOrEmpty(objAccount.Field<string>("chargevalue"))? objAccount.Field<string>("chargevalue") : objAccount.Field<string>("CHARGE_VALUE"))),
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

        public decimal GetDecimalValue(decimal originalValue)
        {
            decimal decimalValue = 0;
            string formattedValue = originalValue.ToString("0.#################");
            decimalValue = decimal.Parse(formattedValue);
            return decimalValue;
        }
        public UserInfo LoadUserDetailsbyXid(string xId)
        {
            string result = String.Empty;
            UserInfo userInfo = null;
            DataSet dsResult = new DataSet();
            SqlParameter[] spParams;
            try
            {
                dbUtil.ConnectionString = this.CbmsConnectionString;
                spParams = new SqlParameter[1];
                spParams[0] = new SqlParameter("@session_id", SqlDbType.Int);
                spParams[0].Value = Convert.ToInt32(Base64Decode(xId));

                dsResult = dbUtil.ExecuteSQLQuery("[Workflow].[Workflow_Queue_User_Info]", spParams);
                if (dsResult != null && dsResult.Tables.Count > 0)
                {
                    if (dsResult.Tables[0].Rows.Count > 0)
                    {
                        int userId = dsResult.Tables[0].Rows[0].Field<int>("USER_INFO_ID");
                        userInfo = GetUserInfoDetails(userId);
                    }
                }
            }
            catch
            {
                userInfo = null;
            }

            return userInfo;


        }
        public UserInfo GetUserInfoDetails(int userInfoId)
        {
            string userName = string.Empty;
            SqlParameter[] spParams;
            UserInfo userInfo = null;
            dbUtil.ConnectionString = this.CbmsConnectionString;
            try
            {
                spParams = new SqlParameter[1];
                spParams[0] = new SqlParameter("@UserInfoId", SqlDbType.Int);
                spParams[0].Value = userInfoId;

                DataSet dsResult = dbUtil.ExecuteSQLQuery("[Workflow].[Workflow_Queue_GerUserInfoByUID]", spParams);

                if (dsResult != null && dsResult.Tables.Count > 0)
                {
                    if (dsResult.Tables[0].Rows.Count > 0)
                    {
                        var query = from objServiceValue in dsResult.Tables[0].AsEnumerable()
                                    select new UserInfo()
                                    {
                                        UserInfoId = userInfoId,
                                        UserName = objServiceValue.Field<string>("USERNAME"),
                                        UserQueueId = objServiceValue.Field<int>("QUEUE_ID"),

                                    };
                        userInfo = query.FirstOrDefault();
                    }
                }

            }
            catch
            {
                throw;
            }
            return userInfo;
        }
        public List<AccountDetails> LoadInvoiceDetails(int invoiceId)
        {
            bool isexcecheck = false;
            bool isinviceExist = false;
            var excedtl = LoadExceptionDetails(invoiceId);
            var paymentinvdtl = GetInvDtlFrompaymentConfig(invoiceId);
            string paymentStatus = string.Empty;
            if (excedtl != null && excedtl.Count > 0)
            {
                isexcecheck = excedtl.Any(a => a.ExceptionType != Constants.PaymentIncomplete);
            }
            if (paymentinvdtl != null)// && paymentinvdtl.Count > 0)
            {
                isinviceExist = paymentinvdtl.CuInvoiceId == invoiceId; //(a => a.CuInvoiceId == invoiceId);
                paymentStatus = paymentinvdtl.PaymentStatus;
            }
            List<AccountDetails> accountDetails = null;
            SqlParameter[] spParams;
            dbUtil.ConnectionString = this.CbmsConnectionString;
            try
            {
                spParams = new SqlParameter[1];
                spParams[0] = new SqlParameter("@CU_Invoice_ID", SqlDbType.Int);
                spParams[0].Value = invoiceId;

                DataSet dsResult = dbUtil.ExecuteSQLQuery("dbo.Site_Account_Dtl_Sel_By_Cu_Invoice_Id", spParams);

                if (dsResult != null && dsResult.Tables.Count > 0)
                {
                    if (dsResult.Tables[0].Rows.Count > 0)
                    {
                        var query = from objAccount in dsResult.Tables[0].AsEnumerable()
                                    select new AccountDetails()
                                    {
                                        SiteName = objAccount.Field<string>("Site_name"),
                                        Address = objAccount.Field<string>("Address"),
                                        AccountType = objAccount.Field<string>("Account_Type"),
                                        VendorName = objAccount.Field<string>("Account_Vendor_Name"),
                                        AccountId = objAccount.Field<int?>("ACCOUNT_ID"),
                                        AccountNumber = objAccount.Field<string>("Account_Number"),
                                        Commodity = objAccount.Field<string>("Commodity_Name"),
                                        MeterNumber = objAccount.Field<string>("Meter_Number"),
                                        ContractNumber = objAccount.Field<string>("Contract_Number"),
                                        SupplierAccountStartDt = objAccount.Field<DateTime?>("Supplier_Account_Begin_Dt"),
                                        SupplierAccountEndDt = objAccount.Field<DateTime?>("Supplier_Account_End_Dt"),
                                        Rate = objAccount.Field<string>("Rate_Name"),
                                        Currency = objAccount.Field<string>("currency_Symbol"),
                                        GroupAccount = objAccount.Field<string>("Group_Billing_Number"),
                                        ClientName = objAccount.Field<string>("Client_Name"),
                                        Ubm = objAccount.Field<string>("UBM_NAME"),
                                        GroupKey = objAccount.Field<string>("Group_Key"),
                                        ClientId = objAccount.Field<int?>("Client_Id"),
                                        InvoiceEndDate = objAccount.Field<DateTime?>("End_Dt"),
                                        InvoiceStartDate = objAccount.Field<DateTime?>("Begin_Dt"),
                                        IsPaymentAccount = objAccount.Field<bool?>("Is_Payment_Account"),
                                        IsOpenExceptions = isexcecheck,
                                        IsInvoiceExist = isinviceExist,
                                        ImageId = objAccount.Field<int?>("CBMS_IMAGE_ID"),
                                        PaymentStatus = paymentStatus,
                                        IsPaymentGroup = objAccount.Field<bool?>("Is_Payment_Group"),
                                        PaymentStartDate = objAccount.Field<DateTime?>("Payment_Effective_Date"),
                                        Country = objAccount.Field<string>("Country_Name"),
                                        GroupPGI = objAccount.Field<int?>("Payment_Group_Indicator_Id"),
                                    };
                        accountDetails = query.ToList();
                        if (isexcecheck == false && isinviceExist == false)
                        {
                            var movObj = GetMoveSelectedInvoices(accountDetails[0].ImageId);
                            if (movObj != null || movObj.Count > 0)
                            {
                                accountDetails[0].IsInvoiceExist = movObj.Any(a => !String.IsNullOrEmpty(a.PaymentAttributeValue));
                            }
                        }
                    }
                }

            }

            catch
            {
                throw;
            }
            return accountDetails;
        }
        
        public List<PaymentProcessInstructions> LoadPaymentProcess(int invoiceId)
        {
            List<PaymentProcessInstructions> payments = null;
            List<AccountDetails> acc = LoadInvoiceDetails(invoiceId);
            //SqlParameter[] spParams;
            dbUtil.ConnectionString = this.CbmsConnectionString;
            try
            {
                DataSet ds = new DataSet();
                foreach(AccountDetails accdet in acc)
                {
                    
                    
                    if (accdet.AccountId != null)
                    {
                        SqlParameter spParam = new SqlParameter("@Account_Id", SqlDbType.Int);
                        spParam.Value = accdet.AccountId;
                        ds.Clear();
                        ds.Merge(dbUtil.ExecuteSQLQuery("dbo.Account_Invoice_Processing_Config_And_Instruction_Sel_By_Account", new SqlParameter[] { spParam }));
                    }
                 
                    

                    if (ds != null && ds.Tables.Count > 0)
                    {
                        var query = from objAccount in ds.Tables[0].AsEnumerable()
                                    select new PaymentProcessInstructions()
                                    {
                                        processInstructions = objAccount.Field<string>("Processing_Instruction"),
                                        lastUpdate = objAccount.Field<DateTime?>("Last_change_Ts"),
                                        updatedUser = objAccount.Field<string>("Updated_User"),
                                        instructionCategory = objAccount.Field<string>("Instruction_Category"),
                                        isActive = objAccount.Field<bool>("Is_Active"),
                                        accountNumber = accdet.AccountNumber
                                    };
                        if (payments == null)
                        {
                            payments = query.ToList();
                        }
                        else
                        {
                            payments.AddRange(query.ToList());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError("GetPaymentProcessInstruction catch block:  error : " + ex.Message);
            }
            return payments;
        }

        public List<MoveSelectedInvoiceDetails> GetMoveSelectedInvoices(int? CbmsImageId)
        {
            List<MoveSelectedInvoiceDetails> paymentAttributeValue = null;
            SqlParameter[] spParams;
            dbUtil.ConnectionString = this.CbmsConnectionString;
            try
            {
                spParams = new SqlParameter[1];
                spParams[0] = new SqlParameter("@Cbms_Image_Id", SqlDbType.Int);
                spParams[0].Value = CbmsImageId;

                DataSet dsResult = dbUtil.ExecuteSQLQuery("dbo.Cu_Invoice_Group_Dtl_Sel", spParams);
                if (dsResult != null && dsResult.Tables.Count > 0)
                {
                    var query = from objAccount in dsResult.Tables[0].AsEnumerable()
                                select new MoveSelectedInvoiceDetails()
                                {
                                    PaymentAttributeValue = objAccount.Field<string>("Payment_Attribute_Value"),
                                    CuInvoiceId = objAccount.Field<int>("Cu_Invoice_Id"),
                                    ParentCuInvoiceId = objAccount.Field<int?>("Parent_Cu_Invoice_Id")
                                };

                    paymentAttributeValue = query.ToList();
                }

            }
            catch (Exception ex)
            {
                logger.LogError("GetMoveSelectedInvoices catch block:  error : " + ex.Message);
            }
            return paymentAttributeValue;
        }
        public List<ExceptionDetails> LoadExceptionDetails(int invoiceId)
        {
            List<ExceptionDetails> exceptionDetails = null;
            SqlParameter[] spParams;
            dbUtil.ConnectionString = this.CbmsConnectionString;
            try
            {
                spParams = new SqlParameter[1];
                spParams[0] = new SqlParameter("@CU_Invoice_ID", SqlDbType.Int);
                spParams[0].Value = invoiceId;

                DataSet dsResult = dbUtil.ExecuteSQLQuery("dbo.cbmsCuException_GetForDenorm", spParams);

                if (dsResult != null && dsResult.Tables.Count > 0)
                {
                    if (dsResult.Tables[0].Rows.Count > 0)
                    {
                        var query = from objFields in dsResult.Tables[0].AsEnumerable()
                                    select new ExceptionDetails()
                                    {
                                        ImageId = objFields.Field<int?>("CBMS_IMAGE_ID"),
                                        DocId = objFields.Field<string>("CBMS_DOC_ID"),
                                        QueueId = objFields.Field<int?>("QUEUE_ID"),
                                        ExceptionType = objFields.Field<string>("exception_type"),
                                        ExceptionStatusType = objFields.Field<string>("exception_status_type"),
                                        UBMAccountNumber = objFields.Field<string>("UBM_Account_Number"),
                                        ServiceMonth = objFields.Field<string>("Service_Month"),
                                        ClientName = objFields.Field<string>("UBM_Client_Name"),
                                        SiteName = objFields.Field<string>("UBM_Site_Name"),
                                        StateName = objFields.Field<string>("UBM_State_Name"),
                                        City = objFields.Field<string>("UBM_City"),
                                        DateInqueue = objFields.Field<DateTime?>("date_in_queue"),
                                        AccountId = objFields.Field<int?>("account_id"),
                                        ExceptionGroupName = objFields.Field<string>("Exception_group_name"),

                                    };
                        exceptionDetails = query.ToList();
                    }
                }

            }

            catch
            {
                throw;
            }
            return exceptionDetails;
        }
        public PaymentInvoiceDetails GetInvDtlFrompaymentConfig(int invoiceId)
        {
            PaymentInvoiceDetails paymentinvoiceDetails = null;
            SqlParameter[] spParams;
            dbUtil.ConnectionString = this.CbmsConnectionString;
            try
            {
                spParams = new SqlParameter[1];
                spParams[0] = new SqlParameter("@Cu_Invoice_Id", SqlDbType.Int);
                spParams[0].Value = invoiceId;

                DataSet dsResult = dbUtil.ExecuteSQLQuery("dbo.Cu_Invoice_Payment_Sel", spParams);

                if (dsResult != null && dsResult.Tables.Count > 0)
                {
                    if (dsResult.Tables[0].Rows.Count > 0)
                    {
                        var query = from objFields in dsResult.Tables[0].AsEnumerable()
                                    select new PaymentInvoiceDetails()
                                    {
                                        CuInvoicePaymentConfigId = objFields.Field<int?>("Cu_Invoice_Payment_Id"),
                                        CuInvoiceId = objFields.Field<int?>("Cu_Invoice_Id"),
                                        PaymentAttributeValue = objFields.Field<string>("Payment_Attribute_Value"),
                                        PaymentSubmissionUserId = objFields.Field<int?>("Payment_Submission_User_Id"),
                                        PaymentSubmissionTs = objFields.Field<DateTime?>("Payment_Submission_Ts"),
                                        PaymentResponseStatusCd = objFields.Field<int?>("Payment_Response_Status_Cd"),
                                        PaymentResponseTs = objFields.Field<DateTime?>("Payment_Response_Ts"),
                                        CreatedUserId = objFields.Field<int?>("Created_User_Id"),
                                        CreatedTs = objFields.Field<DateTime?>("Created_Ts"),
                                        UpdatedUserId = objFields.Field<int?>("Updated_User_Id"),
                                        LastChangeTs = objFields.Field<DateTime?>("Last_Change_Ts"),
                                        PaymentStatus = objFields.Field<string>("Payment_Status"),
                                        RequestJson = objFields.Field<string>("Request_Json")
                                    };
                        paymentinvoiceDetails = query.FirstOrDefault();
                    }
                }

            }

            catch
            {
                throw;
            }
            return paymentinvoiceDetails;

        }
        public string UbmGenericEtlPaymentDetails(int invoiceId)
        {
            string PaymentData = string.Empty;
            SqlParameter[] spParams;
            dbUtil.ConnectionString = this.CbmsConnectionString;
            try
            {
                spParams = new SqlParameter[1];
                spParams[0] = new SqlParameter("@Cu_Invoice_Id", SqlDbType.Int);
                spParams[0].Value = invoiceId;

                DataSet dsResult = dbUtil.ExecuteSQLQuery("dbo.Cu_Invoice_Payment_Attribute_Tracking_Sel_By_Cu_Invoice_Id", spParams);
                if (dsResult != null && dsResult.Tables.Count > 0)
                {
                    if (dsResult.Tables[0].Rows.Count > 0)
                    {
                        if (!String.IsNullOrEmpty(dsResult.Tables[0].Rows[0]["Deposit"].ToString()))
                        {
                            dsResult.Tables[0].Rows[0]["GrossValue"] = Convert.ToDecimal(dsResult.Tables[0].Rows[0]["GrossValue"]) + Convert.ToDecimal(dsResult.Tables[0].Rows[0]["Deposit"]);
                        }
                        if (!String.IsNullOrEmpty(dsResult.Tables[0].Rows[0]["BalanceBroughtForward"].ToString()))
                        {
                            dsResult.Tables[0].Rows[0]["GrossValue"] = Convert.ToDecimal(dsResult.Tables[0].Rows[0]["GrossValue"]) + Convert.ToDecimal(dsResult.Tables[0].Rows[0]["BalanceBroughtForward"]);
                        }
                        if (dsResult.Tables[0].Rows[0]["ReceivedDate"].ToString() == "1/1/1900 12:00:00 AM")
                        {
                            dsResult.Tables[0].Rows[0]["ReceivedDate"] = DBNull.Value;
                        }
                        if (dsResult.Tables[0].Rows[0]["DueDate"].ToString() == "1/1/1900 12:00:00 AM")
                        {
                            dsResult.Tables[0].Rows[0]["DueDate"] = DBNull.Value;
                        }
                        if (dsResult.Tables[0].Rows[0]["IssueDate"].ToString() == "1/1/1900 12:00:00 AM")
                        {
                            dsResult.Tables[0].Rows[0]["IssueDate"] = DBNull.Value;
                        }
                        var arrayData = JsonConvert.SerializeObject(dsResult.Tables[0]);
                        JArray jsonArray = JArray.Parse(arrayData);
                        var objectData = JObject.Parse(jsonArray[0].ToString());
                        PaymentData = JsonConvert.SerializeObject(objectData);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError("UbmGenericEtlPaymentDetails catch block:  error : " + ex.Message);
            }
            return PaymentData;
        }
        public List<paymentFields> LoadPaymentFields(int invoiceId, bool IsPaymentGroup)
        {
            List<paymentFields> fieldDetails = null;
            JObject objPayment = new JObject();
            var paymentPrefillDetails = "";

            var paymentInvoiceDetails = GetInvDtlFrompaymentConfig(invoiceId);

            if (paymentInvoiceDetails != null)
            {
                paymentPrefillDetails = paymentInvoiceDetails.PaymentAttributeValue;

                JObject ubmObj = JObject.Parse(paymentPrefillDetails);
                if (ubmObj.ContainsKey("Re-EnterPaymentAmount") == false)
                {
                    objPayment = ubmObj as JObject;
                    objPayment.Add("Re-EnterPaymentAmount", ubmObj["PaymentAmount"]);
                } else
                {
                    objPayment = JsonConvert.DeserializeObject<JObject>(paymentInvoiceDetails.PaymentAttributeValue);
                }
                
            }
            else
            {
                //if (IsPaymentGroup == true)
                //{
                    paymentPrefillDetails = UbmGenericEtlPaymentDetails(invoiceId);
                    objPayment = JsonConvert.DeserializeObject<JObject>(paymentPrefillDetails);
                //}
            }

            SqlParameter[] spParams;
            dbUtil.ConnectionString = this.CbmsConnectionString;
            try
            {
                spParams = new SqlParameter[1];
                spParams[0] = new SqlParameter("@CU_Invoice_ID", SqlDbType.Int);
                spParams[0].Value = invoiceId;

                DataSet dsResult = dbUtil.ExecuteSQLQuery("dbo.Cbms_Custom_Field_Entity_Map_Sel_By_Cu_Invoice_Id", spParams);
                if (dsResult != null && dsResult.Tables.Count > 0)
                {
                    if (dsResult.Tables[0].Rows.Count > 0)
                    {
                        var query = from objFields in dsResult.Tables[0].AsEnumerable()
                                    select new paymentFields()
                                    {
                                        FieldId = objFields.Field<int>("Cbms_Custom_Field_Id"),
                                        FieldName = objFields?.Field<string>("Cbms_Custom_Field_Name").Replace(" ", ""),
                                        FieldType = objFields.Field<string>("Custom_Field_Type"),
                                        ClientId = objFields.Field<int>("Client_Id"),
                                        VendorId = objFields.Field<int>("Vendor_Id"),
                                        Data = GetQueryList(objFields.Field<string>("Data_Source_Query"), objFields.Field<bool>("Is_Send_Value"), invoiceId),
                                        MatchFieldName = objFields?.Field<string>("Match_Cbms_Custom_Field_Name")?.Replace(" ", ""),
                                        DisplayFieldName = objFields.Field<string>("Display_Field_Name"),
                                        CustomFieldFormat = objFields.Field<string>("Custom_Field_Format"),
                                        DisplayOrder = objFields.Field<int>("Display_Order"),
                                        IsActive = objFields.Field<bool>("Is_Active"),
                                        IsHidden = objFields.Field<bool>("Is_Hidden"),
                                        IsRequired = objFields.Field<bool>("Is_Required"),
                                        PrefillValue = !String.IsNullOrEmpty(paymentPrefillDetails) &&
                                                        (paymentInvoiceDetails == null || (paymentInvoiceDetails != null && paymentInvoiceDetails.PaymentStatus == "Hold")) &&
                                                        (objFields?.Field<string>("Cbms_Custom_Field_Name") == "Gross Value" ||
                                                        objFields?.Field<string>("Cbms_Custom_Field_Name") == "Net Value" ||
                                                        objFields?.Field<string>("Cbms_Custom_Field_Name") == "VAT Value")
                                                        ? objFields.Field<string>("Prefill_Value")
                                                        : GetJArrayValue(objPayment, objFields?.Field<string>("Cbms_Custom_Field_Name").Replace(" ", ""), objFields.Field<string>("Prefill_Value")),
                                        IsSendValue = objFields.Field<bool>("Is_Send_Value"),
                                        MaxLength = objFields.Field<int?>("Max_Of_Length"),
                                    };
                        fieldDetails = query.ToList();

                    }
                }

            }

            catch
            {
                throw;
            }
            return fieldDetails;
        }

        
        public string GetJArrayValue(JObject jArray, string key, string prevalue)
        {
            var value = "";
            if (jArray != null)
            {
                foreach (KeyValuePair<string, JToken> keyValuePair in jArray)
                {
                    if (key == keyValuePair.Key)
                    {
                        value = keyValuePair.Value.ToString();
                    }
                }
            }

            return value != "" ? value : prevalue;
        }

        public List<Dropdownvalues> GetQueryList(string DataSourceQuery, bool isSendValue, int invoiceId)
        {
            List<Dropdownvalues> valueDetails = null;
            if (DataSourceQuery != null)
            {
                SqlParameter[] spParams;
                dbUtil.ConnectionString = this.CbmsConnectionString;
                try
                {
                    spParams = new SqlParameter[1];
                    spParams[0] = new SqlParameter("@CU_Invoice_ID", SqlDbType.Int);
                    spParams[0].Value = invoiceId;
                    DataSet dsResult = dbUtil.ExecuteSQLQuery(DataSourceQuery, spParams);
                    if (dsResult != null && dsResult.Tables.Count > 0)
                    {
                        if (dsResult.Tables[0].Rows.Count > 0)
                        {
                            var query = from objFields in dsResult.Tables[0].AsEnumerable()
                                        select new Dropdownvalues()
                                        {
                                            id = objFields.Field<int>("Id"),
                                            value = objFields.Field<string>("Value"),
                                            dynamicValue = isSendValue ? objFields.Field<string>("Value") : objFields.Field<int>("Id").ToString()
                                        };
                            valueDetails = query.ToList();
                        }
                    }
                }
                catch
                {
                    throw;
                }

            }

            return valueDetails;
        }

        public Task<List<GroupBillInvoices>> GetGroupBillInvoices(int invoiceId, int? startIndex, int? endIndex)
        {
            List<GroupBillInvoices> invoiceList = null;
            try
            {
                dbUtil.ConnectionString = this.CbmsConnectionString;
                SqlParameter[] spParams = new SqlParameter[3];
                spParams[0] = new SqlParameter("@Cu_Invoice_Id", SqlDbType.Int);
                spParams[0].Value = invoiceId;
                spParams[1] = new SqlParameter("@Start_Index", SqlDbType.Int);
                spParams[1].Value = startIndex;
                spParams[2] = new SqlParameter("@End_Index", SqlDbType.Int);
                spParams[2].Value = endIndex;

                DataSet dsResult = dbUtil.ExecuteSQLQuery("dbo.Cu_Invoice_Payment_Invoice_Number_Sel", spParams);
                if (dsResult != null && dsResult.Tables.Count > 0)
                {
                    var query = from objAccount in dsResult.Tables[0].AsEnumerable()
                                select new GroupBillInvoices()
                                {
                                    AccountId = objAccount.Field<int>("Account_Id"),
                                    AccountNumber = objAccount.Field<string>("Account_Number"),
                                    InvoiceId = objAccount.Field<int?>("CU_INVOICE_ID"),
                                    InvoiceNumber = objAccount.Field<string>("Invoice_Number"),
                                    TotalCount = objAccount.Field<int>("Total_Count"),
                                    ClientId = objAccount.Field<int?>("Client_Id"),
                                    SiteId = objAccount.Field<int?>("Site_Id")


                                };
                    invoiceList = query.ToList();
                }
            }
            catch (Exception ex)
            {
                logger.LogError("GetGroupBillInvoice catch block:  error : " + ex.Message);
                throw;
            }
            return Task.Run(() => invoiceList);
        }

    }
}
