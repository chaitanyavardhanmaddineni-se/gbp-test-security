import { Component, Input, OnInit, Output } from '@angular/core';
import { AbstractControl, FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { PageChangeEvent } from '@progress/kendo-angular-grid';
import { BillpaydataService, ModalService } from 'src/app/core';
import { MultipleInvoiceDetails } from 'src/app/core/models/datamodel';
import { GlobalBillPayService } from 'src/app/core/services/globalbillpay.service';
import { environment } from 'src/environments/environment';
import bindAdditionData from 'src/assets/Data/bindAdditionData.json';

@Component({
  selector: 'app-add-multipleinvoice-dialog',
  templateUrl: './add-multipleinvoice-dialog.component.html',
  styleUrls: ['./add-multipleinvoice-dialog.component.scss']
})
export class AddMultipleinvoiceDialogComponent implements OnInit {
  public FormMultipleInvoices: any;
  private userInfoObj: any;
  public display;
  public skip = 0;
  public take = 25;
  public pageSize = 25;
  public buttonCount = 5;
  public isPageable: Boolean = false;
  public type: 'numeric' | 'input' = 'numeric';
  public info = true;
  public previousNext = true;
  public miInvoiceId: number;
  public gridData: any;
  public result: any;
  public userId: number;
  public isSaveDisabled: boolean = true;
  private getDtlObj: any = {};
  public invoiceDetails: any;
  public ispaymentsent:boolean=true;
  public msg: string;
  public miInvoiceExist:boolean;
  private multipleInvoiceobj: MultipleInvoiceDetails = new MultipleInvoiceDetails();
  @Input() public inputs: any;
  public title: string;
  public buttons: string[] = [];
  constructor(private invoice: BillpaydataService,
    private billPayservice: GlobalBillPayService,
    private modalService: ModalService,
    public _fb: FormBuilder, private route: Router) {
    this.userInfoObj = this.invoice.getuserInfo();
  }

  ngOnInit() {
    this.miInvoiceId = this.invoice.getInvoiceId();
    this.miInvoiceExist=this.invoice.getInvoiceExist();
    
    this.buildForm();
    this.getDtlObj.invoiceId = this.miInvoiceId;
    this.getDtlObj.isInvoiceExist = this.miInvoiceExist;
    this.getDtlObj.StartIndex = 0
    this.getDtlObj.EndIndex = this.take;
    this.GetMultipleInvoices()
  }
  buildForm() {
    this.FormMultipleInvoices = this._fb.array([]);
  }

  invoiceNumberGroup() {
    return this._fb.group({
      invoiceNumber: ['', [Validators.required, this.noSpaceValidator]],
      accountNumber:[''],
      clientId:[''],
      siteId:[''],
      accountId:[''],
      invoiceId:['']
    })
  };

  noSpaceValidator(control:FormControl){
    const value = control.value;
    if (value && value.indexOf(' ') !== -1) {
      return { noSpaces: true };
    }
    return null;
  }

  pageChange({ skip, take }: PageChangeEvent): void {
    this.skip = skip + 1;
    this.take = take - 1;
    this.pageSize = take;
    this.getDtlObj.StartIndex = this.skip
    this.getDtlObj.EndIndex = this.skip + this.take;
    this.GetMultipleInvoices();
  }

  public close() {
    document.body.style.overflow='';
    this.modalService.destroy();
  }
  GetMultipleInvoices() {
    this.billPayservice.GetMultipleInvoices(this.getDtlObj).subscribe(res => {
      if (res) {
        let total = res.length !== 0 ? res[0]['totalCount'] : 0
        res.forEach((resp, i) => {
          let obj=this.invoiceNumberGroup();
          obj.patchValue(resp);
          res[i]=obj;
          this.FormMultipleInvoices.push(obj);
        })
        this.gridData = {
          data: res,
          total: total
        };
        this.isPageable = total > this.pageSize ? true : false;
      }
    })
  }
  SaveMultipleInvoices() {
    let multipleInvoicedetails = [];
    this.gridData.data.forEach((obj) => {
      let multipleInvoiceobj={
      UserInfoId : this.userInfoObj.userInfoId,
      InvoiceId : obj.value.invoiceId,
      AccountId : obj.value.accountId,
      InvoiceNumber : obj.value.invoiceNumber
      };
      multipleInvoicedetails.push(multipleInvoiceobj);
    });
    this.modalService.setInvoiceNumberobj(multipleInvoicedetails);
    this.modalService.destroy();
  }

  assignInvoiceNumber(value, dataItem) {
    dataItem.invoiceNumber = value;
  }

  openAccount(dataItem: any) {
    const link = document.createElement('a');
    link.target = '_blank';
    link.href = environment.ViewAccount +
      '&accountID=' + `${dataItem.value.accountId}` +
      '&clientID=' + `${dataItem.value.clientId}` +
      '&siteID=' + `${dataItem.value.siteId}`
    link.style.display = 'none';
    link.click();
  }

  isDisable() {

    if(this.getDtlObj.isInvoiceExist==true){
        this.ispaymentsent = true;
        var x = bindAdditionData.AdditionData.filter(f=>f.Key == "InvoiceSubmittedMessage");
        this.msg = x[0].KeyData;
        return !this.FormMultipleInvoices.valid || this.ispaymentsent;
      }
      else{
      return this.FormMultipleInvoices.valid && !this.ispaymentsent;
    }
   
  }
}
