using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SchneiderElectric.CBMS.InvoiceReg.DAO.Sql
{
    public class SummitDBUtils
    {
        #region "Variable declaration"
        private SqlConnection m_sqlCon;
        private int m_sqlCommandTimeout = 0;
        private string m_strConnectionString = string.Empty;
        private bool _returnProviderSpecificTypes = false;

        private int _sqlCommandTimeout = 0;
        public int CommandTimeout
        {
            get
            {
                return _sqlCommandTimeout;
            }
            set
            {
                _sqlCommandTimeout = value;
            }
        }

        private string _sqlConnectionString = string.Empty;
        public string ConnectionString
        {
            get
            {
                return _sqlConnectionString;
            }
            set
            {
                _sqlConnectionString = value;
            }
        }
        public Boolean ReturnProviderSpecificTypes
        {
            get
            {
                return _returnProviderSpecificTypes;
            }
            set
            {
                _returnProviderSpecificTypes = value;
            }
        }

        public const string strSuccess = "success";
        #endregion

        #region "Transactional method"
        private SqlTransaction m_sqlTransaction;
        public void BeginTransaction(string argStrDBConnName, IConfigurationRoot config)
        {
            try
            {

                // IConfigurationRoot config;
                m_strConnectionString = config.GetSection("AppSettings").GetSection(argStrDBConnName).Value;
                m_sqlCon = new SqlConnection(m_strConnectionString);
                m_sqlCon.Open();

                m_sqlTransaction = m_sqlCon.BeginTransaction();
            }
            catch
            {
                throw;
            }

            finally
            {
            }
        }
        public void EndTransaction(bool rollback = false)
        {
            try
            {

                if (m_sqlTransaction != null)
                {
                    if (rollback)
                        m_sqlTransaction.Rollback();
                    else
                        m_sqlTransaction.Commit();
                }
            }
            catch
            {
                throw;
            }

            finally
            {
                try
                {
                    m_sqlTransaction = null;
                    if ((m_sqlCon.State == System.Data.ConnectionState.Open))
                        m_sqlCon.Close();
                    m_sqlCon.Dispose();
                    m_strConnectionString = string.Empty;
                }
                catch
                {
                    throw;
                }
            }
        }
        public string ExecuteSQLNonQuery(string argStrCmdText, string argStrDBConnName, IConfigurationRoot config, SqlParameter[] argSpParameters = null)
        {
            string strStat = string.Empty;
            SqlCommand ScCommandToExecute;
            SqlConnection m_sqlConn = new SqlConnection();
            try
            {

                // Selecting the appropriate DB connection string from the configuration and opening the connection
                m_strConnectionString = config.GetSection("AppSettings").GetSection(argStrDBConnName).Value;
                m_sqlConn = new SqlConnection(m_strConnectionString);
                m_sqlConn.Open();

                // Creating the SQL Command object. If there are parameters, then the command object with parameters are 
                // created
                if ((!(argSpParameters == null)))
                    ScCommandToExecute = CreateSqlCommand(ref argStrCmdText, ref argSpParameters, ref m_sqlConn);
                else
                    ScCommandToExecute = new SqlCommand(argStrCmdText, m_sqlConn);

                ScCommandToExecute.CommandType = CommandType.StoredProcedure;

                // Setting the command time out, if given in the configuration
                try
                {
                    m_sqlCommandTimeout = Int32.Parse(config.GetSection("AppSettings").GetSection("sqlCommandTimeout").Value);
                }
                catch
                {
                    m_sqlCommandTimeout = 0;
                    throw;
                }

                // If (m_sqlCommandTimeout > 0) Then
                ScCommandToExecute.CommandTimeout = m_sqlCommandTimeout;
                // End If

                //SummitDBTrace _objTrace = new SummitDBTrace(argSpParameters);
                //_objTrace.CommandText = argStrCmdText;

                //_objTrace.WriteBeforeExecuteSQL();

                ScCommandToExecute.ExecuteNonQuery();
                strStat = strSuccess;

                //_objTrace.WriteAfterExecuteSQL();
            }
            catch
            {
                throw;
            }

            finally
            {
                try
                {
                    m_sqlConn.Close();
                    m_sqlConn.Dispose();
                    m_strConnectionString = string.Empty;
                }
                catch
                {
                    throw;
                }
            }
            return strStat;
        }

        public DataSet ExecuteSQLQuery(string argStrCmdText, string argStrDBConnName, SqlParameter[] argSpParameters = null, string dbConnection = "DVServiceBaseURL")
        {
            DataSet ds = new DataSet();
            SqlCommand ScCommandToExecute;
            SqlConnection m_sqlConn = new SqlConnection();
            try
            {
                // Selecting the appropriate DB connection string from the configuration and opening the connection
                m_strConnectionString = dbConnection;
                m_sqlConn = new SqlConnection(m_strConnectionString);
                m_sqlConn.Open();

                // Creating the SQL Command object. If there are parameters, then the command object with parameters are 
                // created
                if ((!(argSpParameters == null)))
                    ScCommandToExecute = CreateSqlCommand(ref argStrCmdText, ref argSpParameters, ref m_sqlConn);
                else
                    ScCommandToExecute = new SqlCommand(argStrCmdText, m_sqlConn);

                ScCommandToExecute.CommandType = CommandType.StoredProcedure;

                // Setting the command time out, if given in the configuration
                try
                {
                    m_sqlCommandTimeout = 20;// Int32.Parse(config.GetSection("AppSettings").GetSection("sqlCommandTimeout").Value);
                }
                catch
                {
                    m_sqlCommandTimeout = 0;
                    throw;
                }

                // If (m_sqlCommandTimeout > 0) Then
                ScCommandToExecute.CommandTimeout = m_sqlCommandTimeout;
                // End If

                SqlDataAdapter da = new SqlDataAdapter(ScCommandToExecute);
                da.SelectCommand = ScCommandToExecute;
                da.ReturnProviderSpecificTypes = ReturnProviderSpecificTypes;
                //  SummitDBTrace _objTrace = new SummitDBTrace(argSpParameters);
                // _objTrace.CommandText = argStrCmdText;

                //  _objTrace.WriteBeforeExecuteSQL();
                da.Fill(ds);
                // _objTrace.WriteAfterExecuteSQL();
            }

            catch
            {
                throw;
            }
            finally
            {
                try
                {
                    m_sqlConn.Close();
                    m_sqlConn.Dispose();
                    m_strConnectionString = string.Empty;
                }
                catch
                {
                    throw;
                }
            }

            return ds;
        }

        public string ExecuteSQLNonQuery(string argStrCmdText, SqlParameter[] argSpParameters = null)
        {
            string strStat = string.Empty;
            SqlCommand ScCommandToExecute;
            SqlConnection m_sqlConn = new SqlConnection();
            try
            {

                // Setting connection string and opening the connection
                m_strConnectionString = ConnectionString;
                m_sqlConn = new SqlConnection(m_strConnectionString);
                m_sqlConn.Open();

                // Creating the SQL Command object. If there are parameters, then the command object with parameters are 
                // created
                if ((!(argSpParameters == null)))
                    ScCommandToExecute = CreateSqlCommand(ref argStrCmdText, ref argSpParameters, ref m_sqlConn);
                else
                    ScCommandToExecute = new SqlCommand(argStrCmdText, m_sqlConn);

                ScCommandToExecute.CommandType = System.Data.CommandType.StoredProcedure;

                // Setting the command time out, if set
                // If (CommandTimeout > 0) Then
                ScCommandToExecute.CommandTimeout = CommandTimeout;
                // End If

                //SummitDBTrace _objTrace = new SummitDBTrace(argSpParameters);
                //_objTrace.CommandText = argStrCmdText;

                //_objTrace.WriteBeforeExecuteSQL();

                ScCommandToExecute.ExecuteNonQuery();
                strStat = strSuccess;

                //_objTrace.WriteAfterExecuteSQL();
            }
            catch
            {
                throw;
            }

            finally
            {
                try
                {
                    m_sqlConn.Close();
                    m_sqlConn.Dispose();
                    m_strConnectionString = string.Empty;
                }
                catch
                {
                    throw;
                }
            }
            return strStat;
        }

        public System.Data.DataSet ExecuteSQLQuery(string argStrCmdText, SqlParameter[] argSpParameters = null)
        {
            DataSet ds = new DataSet();
            SqlCommand ScCommandToExecute;
            SqlConnection m_sqlConn = new SqlConnection();
            try
            {
                // Setting connection string and opening the connection
                m_strConnectionString = ConnectionString;
                m_sqlConn = new SqlConnection(m_strConnectionString);
                m_sqlConn.Open();

                // Creating the SQL Command object. If there are parameters, then the command object with parameters are 
                // created
                if ((!(argSpParameters == null)))
                    ScCommandToExecute = CreateSqlCommand(ref argStrCmdText, ref argSpParameters, ref m_sqlConn);
                else
                    ScCommandToExecute = new SqlCommand(argStrCmdText, m_sqlConn);

                ScCommandToExecute.CommandType = CommandType.StoredProcedure;

                // Setting the command time out, if set
                // If (CommandTimeout > 0) Then
                ScCommandToExecute.CommandTimeout = CommandTimeout;
                // End If

                SqlDataAdapter da = new SqlDataAdapter(ScCommandToExecute);
                da.SelectCommand = ScCommandToExecute;
                da.ReturnProviderSpecificTypes = ReturnProviderSpecificTypes;

                //SummitDBTrace _objTrace = new SummitDBTrace(argSpParameters);
                //_objTrace.CommandText = argStrCmdText;

                //_objTrace.WriteBeforeExecuteSQL();
                da.Fill(ds);
                //  _objTrace.WriteAfterExecuteSQL();
            }


            catch
            {
                throw;
            }
            finally
            {
                try
                {
                    m_sqlConn.Close();
                    m_sqlConn.Dispose();
                    m_strConnectionString = string.Empty;
                }
                catch
                {
                    throw;
                }
            }

            return ds;
        }
        public string DecodeValue(string value)
        {
            if (value == null || value == "")
            {
                return value;
            }
            else
            {
                System.Web.HttpUtility.HtmlDecode(value).ToString();
                value = System.Web.HttpUtility.HtmlDecode(value).ToString();
                return value;
            }

        }
        private SqlCommand CreateSqlCommand(ref string argStrCmdText, ref SqlParameter[] argSpParams, ref SqlConnection argSqlCon)
        {
            SqlCommand ScWithArgs = new SqlCommand(argStrCmdText, argSqlCon);
            int intCount = 0;
            while ((intCount < argSpParams.Length))
            {
                ScWithArgs.Parameters.Add(argSpParams[intCount]);
                intCount = intCount + 1;
            }

            return ScWithArgs;
        }

        #endregion
    }
}
