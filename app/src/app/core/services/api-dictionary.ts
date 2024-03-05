export const ApiDictionary = {
    GetUserDetailsByID: {
        url: 'GBPDataLoad/GetUserInfoDetails'
    },
    GetUserDetailsByXid: {
        url: 'GBPDataLoad/GetUserDetailsByXid'
    },
    LoadChargeValues: {
        url: 'GBPDataLoad/LoadChargeValues'
    },
    LoadInvoiceDetails: {
        url: 'GBPDataLoad/LoadInvoiceDetails'
    },
    LoadPaymentProcess:{
        url:'GBPDataLoad/LoadPaymentProcess'
    },
    loadPaymentFields:{
        url:'GBPDataLoad/loadPaymentFields'
    },
    LoadPaymentInvDetails: {
        url: 'GBPDataLoad/LoadPaymentInvDetails'
    },
    PaymentQuestion: {
        url: 'GBPAction/PaymentQuestion'
    },
    PaymentAccountSetupIssue: {
        url: "GBPAction/PaymentAccountSetupIssue"
    },
    SendPaymentData: {
        url: "GBPAction/SendPaymantData"
    },
    SavePaymentFieldsData: {
        url: "GBPAction/SavePaymentFieldsData"
    },
    LoadParentImageInvoiceDetails: {
        url: "GBPAction/LoadParentImageInvoiceDetails"
    },
    GetMultipleInvoices: {
        url: "GBPDataLoad/GetGroupBillInvoices"
    },
    SaveMultipleInvoices: {
        url: "GBPAction/SaveMultipleInvoices"
    }
}