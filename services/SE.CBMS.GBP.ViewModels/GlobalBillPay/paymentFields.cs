using System;
using System.Collections.Generic;
using System.Text;

namespace SchneiderElectric.CBMS.GBP.ViewModels.GlobalBillPay
{
    public class paymentFields
    {
        public int FieldId { get; set; }
        public string FieldName { get; set; }
        public string FieldType { get; set; }
        public int ClientId { get; set; }
        public int VendorId { get; set; }
        public List<Dropdownvalues> Data { get; set; }
        public string MatchFieldName { get; set; }
        public string CustomFieldFormat { get; set; }
        public string DisplayFieldName { get; set; }
        public string Custom_Field_Format { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
        public bool IsHidden { get; set; }
        public bool IsRequired { get; set; }
        public string PrefillValue { get; set; }
        public bool IsSendValue { get; set; }
        public int? MaxLength { get; set; }
    }
    public class Dropdownvalues
    {
        public int id { get; set; }
        public string value { get; set; }
        public string dynamicValue { get; set; }
    }
}
