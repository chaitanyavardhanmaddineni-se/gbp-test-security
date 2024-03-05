export class InvoiceObjModel {
    InvoiceId: number;
    UserId: number;
    ExceptionTypeName: string;
    QueueId: number;
    AccountId: number;
    ClientId: number;
    Comment: string;
}
export class PaymentProcessModel{
    processInstructions :string;
    lastUpdate : string;
    updatedUser : string;
    instructionCategory : string;
    isActive : boolean;
    accountNumber : string;
}
export class PaymentFieldDetails extends InvoiceObjModel{
        Recalc: boolean;
        DataQuality: boolean;
        Variance: boolean;
        CurrentInvoiceNotReported: boolean;
        InvPaymentDetails:string;
        CbmsImageId : number;
        isPaymentStartDate : boolean;
        startDate : Date;
        IssueDate : Date;
        groupPGI:number;

}

export class InvoiceDetailsModel {
    commodity: any;
    meterNumber: any;
    contractNumber: any;
    SupplierAccountStartDt: any;
    SupplierAccountEndDt: any;
    rate: any;
    currency: any;
    invStartDate: Date;
    invEndDate: Date;
    clientId: number
    isPaymentAccount: boolean;
    isOpenExceptions: boolean;
    invImageId:number;
    isPaymentGroup:boolean = false;
    groupAccount:any;
    PaymentStartDate : any; 
    groupPGI:number;
}
export class PaymentDetails {
    InvoiceNumber: number;
    InvoiceType: string;
    PaymentType: string;
    ApprovedPaymentVendor: string;
    ReceivedDate: Date;
    DueDate: Date;
    BalanceBroughtForward: number;
    NetValue: number;
    GrossValue: number;
    PaymentAmount: number;
    ReEnterPaymentAmount: number;
    VatPercentage: number;
    VatValue: number;
    Deposit: number;
    MarketIndicator: number;
    InvoiceStartDate: Date;
    InvoiceEndDate: Date;
}

export class MultipleInvoiceDetails {
    InvoiceNumber: string;
    InvoiceId: number;
    UserInfoId: number;
    AccountId: number;
}