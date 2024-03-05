using System;
using System.Collections.Generic;
using System.Text;

namespace SchneiderElectric.CBMS.InvoiceReg.DAO.Sql
{
    public class BaseData
    {
        public dynamic MakeSafeDBNull(object value)
        {
            if (string.IsNullOrEmpty(Convert.ToString(value)))
                return DBNull.Value;
            else
                return value;
        }
        public dynamic MakeSafeDBNull(int value)
        {
            if (value <= 0)
                return DBNull.Value;
            else
                return value;
        }
        public dynamic MakeSafeDBNull(decimal value)
        {
            if (value <= 0)
                return DBNull.Value;
            else
                return value;
        }

        public string MakeSafeString(object strIn)
        {
            if ((strIn == DBNull.Value))
                return string.Empty;
            else
                return Convert.ToString(strIn);
        }
        public bool MakeSafeBoolean(object strIn)
        {
            if ((strIn == DBNull.Value))
                return false;
            else
                return Convert.ToBoolean(strIn);
        }
        public int MakeSafeInt(object strIn)
        {
            if ((strIn == DBNull.Value))
                return 0;
            else
                return Convert.ToInt32(strIn);
        }
        public decimal MakeSafeDecimal(object strIn)
        {
            if ((strIn == DBNull.Value) || (strIn.ToString() == ""))
                return 0;
            else
                return Convert.ToDecimal(strIn);
        }
        
        public double? MakeSafeDouble(object strIn)
        {
            if ((strIn == DBNull.Value))
                return null;
            else
                return Convert.ToDouble(strIn);
        }
        public int? MakeSafeNullableInt(object strIn)
        {
            if ((strIn == DBNull.Value))
                return null;
            else
                return Convert.ToInt32(strIn);
        }
        public DateTime? MakeSafeDate(object strIn)
        {
            if ((strIn == System.DBNull.Value) || (strIn.ToString() == "NaN"))
                return null;
            else
                return Convert.ToDateTime(strIn);
        }
        public string Base64Decode(string text)
        {
            char[] Data;
            byte[] Result;
            Int32 I;
            System.Text.ASCIIEncoding ASCII = new System.Text.ASCIIEncoding();

            Result = ASCII.GetBytes(text);
            Data = ASCII.GetChars(Result);
            Result = System.Convert.FromBase64CharArray(Data, 0, Data.Length);

            text = "";
            for (I = 0; I <= Result.Length - 1; I++)
                text += (char)(Result[I]);
            return text;
        }
    }
}

