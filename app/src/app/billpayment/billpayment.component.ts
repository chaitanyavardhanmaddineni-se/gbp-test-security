import { Component, OnInit } from '@angular/core';
import { BreadcrumbService } from '../core'
import { GlobalBillPayService } from '../core/services/globalbillpay.service'
import { ModalService } from '../core/services/modal.service'
import { ActivatedRoute, NavigationStart, Router } from '@angular/router';
import { FormGroup, FormBuilder, FormControl, Validators, ValidationErrors, AbstractControl, FormArray } from '@angular/forms';
import { DropDownFilterSettings } from '@progress/kendo-angular-dropdowns';
import { BillpaydataService } from '../core/services/billpaydata.service';
import { UserobjModel } from '../core/models/model';
import { environment } from 'src/environments/environment';
import { InvoiceObjModel, PaymentFieldDetails, InvoiceDetailsModel, PaymentProcessModel } from '../core/models/datamodel';
import { BillPayConstants } from '../core/constants/billpayconstants';
import { ConfirmationDialogComponent } from '../common/confirmation-dialog/confirmation-dialog.component';
import { AddCommentDialogComponent } from '../common/add-comment-dialog/add-comment-dialog.component';
import { ToastrService } from 'ngx-toastr';
import { AddMultipleinvoiceDialogComponent } from '../common/add-multipleinvoice-dialog/add-multipleinvoice-dialog.component';
import bindAdditionData from 'src/assets/Data/bindAdditionData.json';
import { IntlService } from "@progress/kendo-angular-intl";
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-billpayment',
  templateUrl: './billpayment.component.html',
  styleUrls: ['./billpayment.component.scss']
})
export class BillpaymentComponent implements OnInit {
  accountId: any;
  result: any;
  clientId: number
  isDisabled: boolean;
  ispmtbtnDisabled: boolean;
  isPmtAcntSetupBtn: boolean;
  processInstructions : any;
  lastUpdate : any;
  updatedUser : any;
  public displaySucess: boolean = false;
  public breadCRumbs = [];
  public invoiceId: number;
  public invoiceDetails: any;
  public paymentProcess: any=[];
  public chargeGridData: any;
  public gbpJson: any;
  public comment: string;
  public FormInvoiceSearch: FormGroup;
  public ismatched: boolean = true;
  public isGroup: boolean = false;
  public imageIddtl: number;
  public accountNumber : string;
  public value: Date = new Date(2000, 2, 10);
  private userObj: UserobjModel = new UserobjModel();
  private invoiceObj: InvoiceObjModel = new InvoiceObjModel();
  private paymentFieldObj: PaymentFieldDetails = new PaymentFieldDetails();
  private invDetailModelObj: InvoiceDetailsModel = new InvoiceDetailsModel();
  public oldValue: any;
  public newValue: any;
  public isPaymentstartdate:boolean = false;
  public invStartDatearr: any[];
  public startdatearr = [];  
  public processarr = [];
  public issueDate : any;
  public startDate:Date; 
  public filterSettings: DropDownFilterSettings = {
    caseSensitive: false,
    operator: 'contains'
  };

  constructor(public _fb: FormBuilder,
    private router: Router,
    private modalService: ModalService,
    private breadcrumbService: BreadcrumbService,
    private billPayservice: GlobalBillPayService,
    private userDataservice: BillpaydataService,
    public toastr: ToastrService,
    private activatedrouter: ActivatedRoute,
    private intl: IntlService,
    private datePipe: DatePipe) {
    this.userObj = this.userDataservice.getuserInfo();
    this.activatedrouter.params.subscribe(params => {
      if (typeof params['InvoiceId'] != 'undefined' && params['InvoiceId']) {
        this.invoiceId = params['InvoiceId'];
        this.userDataservice.setInvoiceId(this.invoiceId);
      }
    });
  }

  ngOnInit() {
    window.addEventListener("pageshow", function (event) {
      var historyTraversal = event.persisted ||
        (typeof window.performance != "undefined" &&
          window.performance.navigation.type === 2);
      if (historyTraversal) {
        window.location.reload();
      }
    });
    this.loadgridData();
    this.buildForm();
    this.GetInvoiceDetails();
  }
  GetPaymentProcessingInstructions() {
    this.billPayservice.LoadPaymentProcess(this.invoiceId).subscribe(res=>{
      this.paymentProcess = res; 
    });
  }
  
  loadPaymentFields() {
    this.billPayservice.loadPaymentFields(this.invoiceId, this.invDetailModelObj.isPaymentGroup).subscribe(res => {
      this.isGroup = this.invDetailModelObj.groupAccount === BillPayConstants.GroupBill ? false : true;
      if (this.isGroup) {
        this.checkInput();
      }
      this.gbpJson = res
      this.LoadSdForm();
      for (let item of res) {
        if (item.fieldType == BillPayConstants.NumberText || item.fieldType == BillPayConstants.PaymentAmount) {
          let decimalboolValue = this.decimalPlacesValidator(item.prefillValue)
          if (decimalboolValue == true) {
            this.getDialogMsg(BillPayConstants.MoreThanTwoDecimalMessage, "Message");
            this.modalService.GetAnswer().subscribe(res => {
              if (res != '' && res === 'Close') {
                //this.navigateToQueue();
              }
            })
            return
          }
        }
      }
    });
  }
  decimalPlacesValidator(value: string) {
    if (value !=undefined && value.includes('.') && value.split('.')[1].length > 2 ) {
      // return { 'decimalPlaces': true }; // Validation fails
      return true
    }
    return false; // Validation passes
  }
  buildForm() {
    this.FormInvoiceSearch = this._fb.group({
    })
  }
  LoadSdForm() {
    this.FormInvoiceSearch.reset();
    this.CreateFormGroup();
  }
  CreateFormGroup() {
    this.gbpJson.sort(function (x, y) {
      return x.displayOrder - y.displayOrder
    })
    this.BindToForm(this.gbpJson.filter(e => !e.isHidden));


  }
  BindToForm(data: any[]) {
    let group: any = {};
    data.forEach(e => {
      if (e.fieldType == BillPayConstants.DatePicker) {
        group[e.fieldName] = e.isRequired === true ? new FormControl(e.prefillValue != undefined ? new Date(e.prefillValue) : "" || '', [this.yearValidator(), Validators.required])
          : new FormControl(e.prefillValue != undefined ? new Date(e.prefillValue) : "",this.yearValidator());
      }
      else {
        group[e.fieldName] = e.isRequired === true ? new FormControl(e.prefillValue != undefined ? e.prefillValue : "", [Validators.required, e.fieldType == 'Free Text' ? this.noWhitespaceValidator : this.nullValidator])
          : new FormControl(e.prefillValue != undefined ? e.prefillValue : "", this.noWhitespaceValidator);
      }
      if (e[BillPayConstants.MatchFieldValue]) {
        group[e.fieldName][BillPayConstants.SetValidators]([this.matchValues(e.matchFieldName), Validators.required])
      }
    });
    this.FormInvoiceSearch = new FormGroup(group);
    this.userDataservice.setFormGroup(this.FormInvoiceSearch)
    if (this.ispmtbtnDisabled == false || this.ispmtbtnDisabled == undefined) {
      let GrossValue: any;
      if (this.FormInvoiceSearch.value[BillPayConstants.Deposit] != "") {
        GrossValue = parseFloat(this.FormInvoiceSearch.value[BillPayConstants.GrossValue]);
        this.FormInvoiceSearch.controls[BillPayConstants.GrossValue].setValue(this.addZeroes(GrossValue + parseFloat(this.FormInvoiceSearch.value[BillPayConstants.Deposit])));
      }
      if (this.FormInvoiceSearch.value[BillPayConstants.BalanceBroughtForward] != "") {
        GrossValue = parseFloat(this.FormInvoiceSearch.value[BillPayConstants.GrossValue]);
        this.FormInvoiceSearch.controls[BillPayConstants.GrossValue].setValue(this.addZeroes(GrossValue + parseFloat(this.FormInvoiceSearch.value[BillPayConstants.BalanceBroughtForward])));
      }
    }

  }
  public yearValidator() {

    return (control: AbstractControl): { [key: string]: any } | null => {
      const year = Number(control.value);
      if (!isNaN(year) && year >= 1900) {
        return null; // Valid year
      } else {
        return { 'invalidYear': { value: control.value } }; // Invalid year
      }
    }
      
  }

  noWhitespaceValidator(control: FormControl): { [key: string]: any } {
    const isSpace = /^\s+$/.test(control.value);
    if (control.value && isSpace) {
      return { 'whitespace': true }
    }
    return null;
  }
  nullValidator(control: FormControl): ValidationErrors | null {
    return null;
  }

  toggledropdown(control) {
    control.toggle(true);
  }

  GetInvoiceDetails() {
    this.billPayservice.LoadInvoiceDetails(this.invoiceId).subscribe(res => {
      this.invoiceDetails = res;
      if (this.invoiceDetails != null && this.invoiceDetails.length > 0) {
        var x = this.invoiceDetails.filter(f => f.isPaymentAccount == true) //any one account has payment=true
        this.accountId = this.invoiceDetails[0].accountId
        this.accountNumber = this.invoiceDetails[0].accountNumber
        this.invDetailModelObj.commodity = this.invoiceDetails[0].commodity
        this.invDetailModelObj.meterNumber = this.invoiceDetails[0].meterNumber
        this.invDetailModelObj.contractNumber = this.invoiceDetails[0].contractNumber
        this.invDetailModelObj.SupplierAccountStartDt = this.invoiceDetails[0].supplierAccountStartDt
        this.invDetailModelObj.SupplierAccountEndDt = this.invoiceDetails[0].supplierAccountEndDt
        this.invDetailModelObj.rate = this.invoiceDetails[0].rate
        this.invDetailModelObj.currency = this.invoiceDetails[0].currency
        this.invDetailModelObj.invStartDate = this.invoiceDetails[0].invoiceStartDate
        this.invDetailModelObj.invEndDate = this.invoiceDetails[0].invoiceEndDate
        this.invDetailModelObj.PaymentStartDate = this.invoiceDetails[0].paymentStartDate
        this.invDetailModelObj.clientId = this.invoiceDetails[0].clientId
        this.invDetailModelObj.isPaymentAccount = x.length > 0 ? true : this.invoiceDetails[0].isPaymentAccount
        this.invDetailModelObj.invImageId = this.invoiceDetails[0].imageId;
        this.imageIddtl = this.invDetailModelObj.invImageId;
        this.invDetailModelObj.isOpenExceptions = this.invoiceDetails[0].isOpenExceptions
        this.invDetailModelObj.isPaymentGroup = this.invoiceDetails[0].isPaymentGroup == undefined ? false : this.invoiceDetails[0].isPaymentGroup;
        this.invDetailModelObj.groupAccount = this.invoiceDetails[0].groupAccount; 
        this.invDetailModelObj.groupPGI = this.invoiceDetails[0].groupPGI;
        if (this.invoiceDetails.length > 0)
        {
          this.invoiceDetails.forEach(start => {
            this.startdatearr.push(start.paymentStartDate);
          });
          
        } 
        
        this.userDataservice.setInvoiceExist(this.invoiceDetails[0].isInvoiceExist);
        if ((this.accountId == null || this.accountId == undefined) || (this.invoiceDetails[0].isInvoiceExist == true && this.invDetailModelObj.isOpenExceptions == true) || (this.invoiceDetails[0].isInvoiceExist == true && this.invoiceDetails[0].paymentStatus != BillPayConstants.Hold)) {
          this.ispmtbtnDisabled = true;
          this.isPmtAcntSetupBtn = true;
          if ((this.invoiceDetails[0].isInvoiceExist == true)) {
            this.isDisabled = true;
          }
        }
        else if ((this.invDetailModelObj.isOpenExceptions == true && this.accountId != undefined) || this.invDetailModelObj.isPaymentAccount == false) {
          this.isDisabled = false;
          this.ispmtbtnDisabled = true;
          this.isPmtAcntSetupBtn = false;
        }
        if ((this.accountId == null || this.accountId == undefined) || (this.invoiceDetails[0].isInvoiceExist == true)) {
          this.ispmtbtnDisabled = true;
          this.isPmtAcntSetupBtn = true;
          if (this.invoiceDetails[0].isInvoiceExist == true) {
            this.isDisabled = true;
          }
        }
        if (this.invoiceDetails[0].paymentStatus == BillPayConstants.Hold) {
          this.ispmtbtnDisabled = false;
        }
        this.GetPaymentProcessingInstructions();
      }
      this.loadPaymentFields();
      
    });
  }

  PaymentQuestionClick() {
    this.invoiceObj.InvoiceId = this.invoiceId;
    this.invoiceObj.UserId = this.userObj.userInfoId;
    this.invoiceObj.AccountId = this.accountId;
    this.invoiceObj.ClientId = this.invDetailModelObj.clientId;
    this.invoiceObj.ExceptionTypeName = BillPayConstants.PaymentQuestionException;
    let inputs = {
      displayMsg: this.comment,
      title: 'Message',
      buttons: ['Ok', 'Cancel']
    };
    this.modalService.init(AddCommentDialogComponent, inputs, {})
    let disposable = this.modalService.getComment().subscribe(res => {
      disposable.unsubscribe();
      this.invoiceObj.Comment = res;
      this.billPayservice.MarkInvoiceAsPaymentQuestion(this.invoiceObj).subscribe(res => {
        this.result = res;
        this.SavePaymentFieldsData(BillPayConstants.Queue);
      });
    });
  }

  PaymentAccountSetupIssueClick() {
    this.invoiceObj.InvoiceId = this.invoiceId;
    this.invoiceObj.UserId = this.userObj.userInfoId;
    this.invoiceObj.AccountId = this.accountId;
    this.invoiceObj.ClientId = this.invDetailModelObj.clientId;
    this.invoiceObj.ExceptionTypeName = BillPayConstants.PaymentAccountSetupIssueException;
    let inputs = {
      displayMsg: this.comment,
      title: 'Message',
      buttons: ['Ok', 'Cancel']
    };
    this.modalService.init(AddCommentDialogComponent, inputs, {})
    let disposable = this.modalService.getComment().subscribe(res => {
      disposable.unsubscribe();
      this.invoiceObj.Comment = res;
      this.billPayservice.MarkInvoiceAsPaymentAccountSetupIssue(this.invoiceObj).subscribe(res => {
        this.result = res;
        this.SavePaymentFieldsData(BillPayConstants.Queue);
      });
    });
  }

  SavePaymentFieldsData(Type: string) {
    const formValue = this.FormInvoiceSearch.value;
    Object.keys(formValue).forEach(key => {
      const value = formValue[key];
      if (value instanceof Date) {
        // Format date with local timezone offset
        formValue[key] = this.intl.formatDate(value, "yyyy-MM-dd'T'HH:mm:ss.SSS'Z'");
      }
    });
    this.paymentFieldObj.InvPaymentDetails = JSON.stringify(formValue)
    this.paymentFieldObj.InvoiceId = this.invoiceId;
    this.paymentFieldObj.UserId = this.userObj.userInfoId;

    this.billPayservice.SavePaymentFieldsData(this.paymentFieldObj).subscribe(res => {
      if (Type == BillPayConstants.Queue) {
        this.navigateToQueue();
      } else if (Type == BillPayConstants.EditInvoice) {
        this.EditInvoiceNavigate();
      }
    });
  }

  SendPaymentData() {
    for (let item of this.chargeGridData) {
      let Chargesdecimalbool = this.decimalPlacesValidator(JSON.stringify(item.chargeValue))
      if (Chargesdecimalbool == true) {
        this.getDialogMsg(BillPayConstants.MoreThanTwoDecimalMessage, "Message");
        this.modalService.GetAnswer().subscribe(res => {
          if (res != '' && res === 'Close') {
            //this.navigateToQueue();
          }
        })
        return
      }
    }
    for (let item of this.gbpJson) {
      if (item.fieldType == BillPayConstants.NumberText || item.fieldType == BillPayConstants.PaymentAmount) {
        let decimalboolValue = this.decimalPlacesValidator(item.prefillValue)
        if (decimalboolValue == true) {
          this.getDialogMsg(BillPayConstants.MoreThanTwoDecimalMessage, "Message");
          this.modalService.GetAnswer().subscribe(res => {
            if (res != '' && res === 'Close') {
              //this.navigateToQueue();
            }
          })
          return
        }
      }
    }
    const formValue = this.FormInvoiceSearch.value;
    Object.keys(formValue).forEach(key => {
      const value = formValue[key];
      if (value instanceof Date) {
        // Format date with local timezone offset
        formValue[key] = this.intl.formatDate(value, "yyyy-MM-dd'T'HH:mm:ss.SSS'Z'");
      }
    });
    this.paymentFieldObj.InvPaymentDetails = JSON.stringify(formValue)
    this.paymentFieldObj.InvoiceId = this.invoiceId;
    this.paymentFieldObj.UserId = this.userObj.userInfoId;
    this.paymentFieldObj.CurrentInvoiceNotReported = false;
    this.paymentFieldObj.CbmsImageId = this.invDetailModelObj.invImageId;
    this.paymentFieldObj.groupPGI = this.invDetailModelObj.groupPGI;

    const valueDate = JSON.parse(this.paymentFieldObj.InvPaymentDetails);
    this.paymentFieldObj.IssueDate = new Date(valueDate.IssueDate);
    this.billPayservice.SendPaymentData(this.paymentFieldObj).subscribe(res => {
      this.result = res;
      if (res && res.length>0 && res[0].vendorId !=null) {
        this.getDialogMsg(BillPayConstants.ApprovedVendorValidationMsg, "Message");
          this.modalService.GetAnswer().subscribe(res => {
            if (res != '' && res === 'Close') {
              //this.navigateToQueue();
            }
          })
        return
      }
      if (res != null || (res != null && res[0].isPaymentStartDate == false)) {
        this.showSuccess(BillPayConstants.Success);
        setTimeout(function () {
          this.navigateToQueue();
        }.bind(this), 3000);
      }    
      else {
        this.getDialogMsg(this.paymentFieldObj.groupPGI!=null?BillPayConstants.UBMGroupValidationMsg:BillPayConstants.LargeGroupBillValidationMsg, "Message");
        this.modalService.GetAnswer().subscribe(res => {
          if (res != '' && res === 'Close') {
            this.navigateToQueue();
          }
        })
      }
    });
  }
  
  showSuccess(msg) {
    this.toastr.success('', msg, {
      timeOut: 3000,
    });
  }

  setBreadcrumb() {
    this.router.routeReuseStrategy.shouldReuseRoute = () => false;
    this.breadCRumbs = [
      { label: 'Invoice', data: '/' },
      { label: 'Bill Payment' },
    ];
    this.breadcrumbService.changeName(this.breadCRumbs);
  }


  public loadgridData() {
    var sortColumn = "Cu_Determinant_Code";
    this.billPayservice.LoadChargeValues(this.invoiceId, sortColumn).subscribe(res => {
      this.chargeGridData = res;
    })
  }

  EditInvoiceNavigate() {
    let xiduidurl = ""
    if (this.userObj.xId) {
      xiduidurl = 'xid=' + this.userObj.xId
    }
    else {
      xiduidurl = 'uid=' + this.userObj.uid
    }
    let url = environment.EditInvoice + '?CuInvoiceId=' + this.invoiceId + '&Workflow=1&fromPage=Global Bill Payment&' + xiduidurl
    window.location.assign(url)
  }

  navigateToEditInvoice() {
    this.SavePaymentFieldsData(BillPayConstants.EditInvoice);
  }

  public matchValues(
    matchTo: string // name of the control to match to
  ) {

    return (control: AbstractControl): ValidationErrors | null => {
      return !!control.parent &&
        !!control.parent.value &&
        control.value == control.parent.controls[matchTo].value
        ? null
        : { isMatching: false };

    };
  }

  valuechange(name) {
    if (this.FormInvoiceSearch.controls[name].invalid) {
      this.ismatched = false;
    } else {
      this.ismatched = true;

    }
  }
  eventFocus(item: any) {
    bindAdditionData.AdditionData.forEach(element => {
      if (element.Key == item) {
        this.oldValue = this.FormInvoiceSearch.controls[element.Key].value == null ? 0 : this.FormInvoiceSearch.controls[item].value;
      }
    });
  }

  handleChange(item) {
    if (item === 'PaymentAmount') {
      this.valuechange('Re-EnterPaymentAmount');
    }
    bindAdditionData.AdditionData.forEach(element => {
      if (element.Key === item) {
        this.newValue = this.FormInvoiceSearch.controls[item].value == null ? 0 : this.FormInvoiceSearch.controls[item].value;
        if (this.oldValue == this.newValue) {
          // no changes for Gross Value
        } else {
          if (this.FormInvoiceSearch.controls[item].value != null && this.FormInvoiceSearch.controls[item].value != '' && this.newValue > this.oldValue) {
            let aggregatedValue = this.oldValue != "" ? (parseFloat(this.FormInvoiceSearch.value[item]) - parseFloat(this.oldValue)) : parseFloat(this.FormInvoiceSearch.value[item]);
            this.FormInvoiceSearch.controls[element.KeyData].setValue(this.addZeroes(parseFloat(this.FormInvoiceSearch.value[element.KeyData]) + aggregatedValue));
          } else if (this.FormInvoiceSearch.controls[item].value != null && this.FormInvoiceSearch.controls[item].value != '' && this.newValue < this.oldValue) {
            let itemValue = this.oldValue - this.newValue;
            this.FormInvoiceSearch.controls[element.KeyData].setValue(this.addZeroes(parseFloat(this.FormInvoiceSearch.value[element.KeyData]) - itemValue));
          } else {
            this.FormInvoiceSearch.controls[element.KeyData].setValue(this.addZeroes(parseFloat(this.FormInvoiceSearch.value[element.KeyData]) - parseFloat(this.oldValue)))
          }
        }
      }
    });
  }
  addZeroes(num) {
    return num.toLocaleString("en", { useGrouping: false, minimumFractionDigits: 2 })
  }

  onChange(data) {
    if (this.FormInvoiceSearch.controls['PaymentAmount'].value != null && this.FormInvoiceSearch.controls['PaymentAmount'].value != '') {
      this.FormInvoiceSearch.controls['Re-EnterPaymentAmount'].setErrors(
        this.FormInvoiceSearch.controls['PaymentAmount'].value == this.FormInvoiceSearch.controls['Re-EnterPaymentAmount'].value
          ? null
          : { isMatching: false });
    }
    // Commented below code because not working for matching prepopulated field.
    /* if (data['matchFieldName'] || data.fieldName=="PaymentAmount") {
      if (this.FormInvoiceSearch.value[data['matchFieldName']] != null && this.FormInvoiceSearch.value[data['matchFieldName']] != '') {
        this.FormInvoiceSearch.controls[data.matchFieldName].setErrors(
          this.FormInvoiceSearch.value[data.matchFieldName] == this.FormInvoiceSearch.value[data.fieldName]
            ? null
            : { isMatching: false });
      }
      else if(this.FormInvoiceSearch.controls['PaymentAmount'].value!=null){
          this.FormInvoiceSearch.controls['Re-EnterPaymentAmount'].setErrors(
            this.FormInvoiceSearch.controls['PaymentAmount'].value == this.FormInvoiceSearch.controls['Re-EnterPaymentAmount'].value
              ? null
              : { isMatching: false });
      }
    } */
  }

  SendPaymentClick() {
    let isInvalidFields = false;
    let formInvoiceSearch: any = this.FormInvoiceSearch;
    var formControlArray: FormArray = formInvoiceSearch.controls as FormArray;
    Object.keys(formControlArray).forEach((inputFieldKey: string) => {
      if (inputFieldKey != 'Re-EnterPaymentAmount' && this.FormInvoiceSearch.controls[inputFieldKey].invalid) {
        isInvalidFields = true;
      }
    });

    if (this.ismatched == false && !isInvalidFields) {
      this.getDialogMsg(BillPayConstants.paymentIssue, "Message");
      return;
    }


    var y: any[] = this.gbpJson;
    var data: any[] = y.filter(k => k.controltype == 'label')
    data.forEach(e => {
      if (e.data != '') {
        this.FormInvoiceSearch.controls[e.controlname].setErrors(null);
      }
    });
    if (this.FormInvoiceSearch.valid) {
      this.SendPaymentData();
    }
    else {
      this.getDialogMsg(BillPayConstants.ValidateIssue, "Message");
      return;
    }

  }

  navigateToQueue() {
    let xiduidurl = ""
    if (this.userObj.xId) {
      xiduidurl = 'xid=' + this.userObj.xId
    }
    else {
      xiduidurl = 'uid=' + this.userObj.uid
    }
    let url = environment.QueueUrl + '?' + xiduidurl
    window.location.assign(url)
  }

  getDialogMsg(displayMsg: string, title?: string, buttons?: any[]) {
    const inputs = {
      displayMsg: displayMsg,
      title: title,
      buttons: buttons
    };
    this.modalService.init(ConfirmationDialogComponent, inputs, {});

  }

  openMultipleInvoices() {
    let inputs = {
      displayMsg: ''
    };
    document.body.style.overflow = 'hidden';
    this.modalService.init(AddMultipleinvoiceDialogComponent, inputs, {});

    this.modalService.getInvoiceNumbers().subscribe(obj => {
      let getInvoices = [];
      getInvoices = obj.filter(field => field.InvoiceNumber && field.InvoiceNumber.trim() !== '');
      if(getInvoices.length > 1){
        this.billPayservice.SaveMultipleInvoices(obj).subscribe(res => {
          this.FormInvoiceSearch.controls["InvoiceNumber"].setValue("Multiple")
        })
      }
      else{
        const singleInvoiceNumber = getInvoices.length > 0 ? getInvoices[0].InvoiceNumber : "";
        this.billPayservice.SaveMultipleInvoices(obj).subscribe(res => {
          this.FormInvoiceSearch.controls["InvoiceNumber"].setValue(singleInvoiceNumber)
        })
      }
    });
  }

  checkInput() {
    const obj = {
      "invoiceId": this.invoiceId,
      "StartIndex": 0,
      "EndIndex": 25
    };
    this.billPayservice.GetMultipleInvoices(obj).subscribe(res => {
      let getSingleInvoices = [];
      getSingleInvoices = res.filter(field => field.invoiceNumber && field.invoiceNumber.trim() !== '');
      if(getSingleInvoices.length > 1){
        res.forEach(field => {
          if (field.hasOwnProperty("invoiceNumber")) {
            this.FormInvoiceSearch.controls["InvoiceNumber"].setValue("Multiple");
          }
        });
      }
      else {
        if(this.FormInvoiceSearch.controls["InvoiceNumber"].value != null && this.FormInvoiceSearch.controls["InvoiceNumber"].value.trim() !== ''){
          this.FormInvoiceSearch.controls["InvoiceNumber"].value;
        }
        else{
          const singleInvoiceNumber = getSingleInvoices.length > 0 ? getSingleInvoices[0].invoiceNumber : "";
          this.FormInvoiceSearch.controls["InvoiceNumber"].setValue(singleInvoiceNumber);
        }
      }
    });

  }
}
