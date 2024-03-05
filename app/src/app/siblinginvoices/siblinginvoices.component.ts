import { Component, Input, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { ActivatedRoute, Route, Router } from '@angular/router';
import { PageChangeEvent } from '@progress/kendo-angular-grid';
import { SortDescriptor } from '@progress/kendo-data-query';
import { environment } from 'src/environments/environment';
import { BillpaydataService } from '../core/services/billpaydata.service';
import { GlobalBillPayService } from '../core/services/globalbillpay.service';
import { BillPayConstants } from '../core/constants/billpayconstants';

@Component({
  selector: 'app-siblinginvoices',
  templateUrl: './siblinginvoices.component.html',
  styleUrls: ['./siblinginvoices.component.scss']
})
export class SiblinginvoicesComponent implements OnInit {
  public FormSiblingInvoice: FormGroup;
  public sort: SortDescriptor[] = [];
  private userInfoObj: any;
  public display;
  public skip = 0;
  public take = 25;
  public pageSize = 25;
  public buttonCount = 5;
  public type: 'numeric' | 'input' = 'numeric';
  public info = true;
  public previousNext = true;
  public siInvoiceId: number;
  private getDtlObj: any = {};
  public gridData: any;
  public result: any;
  public links:any[]=[{name: BillPayConstants.LargeGroupBill},{name: BillPayConstants.MoveSelectedOtherMonth},{name: BillPayConstants.UBMGroup}]
  public activeindex : number;
  public columnTitle: string;
  constructor(private invoice: BillpaydataService,
    private billPayservice: GlobalBillPayService,
    public _fb: FormBuilder,
    private activatedrouter: ActivatedRoute) {
    this.userInfoObj = this.invoice.getuserInfo();
    this.activatedrouter.params.subscribe(params => {
      if (typeof params['InvoiceId'] != 'undefined' && params['InvoiceId']) {
        this.siInvoiceId = params['InvoiceId'];
        this.invoice.setInvoiceId(this.siInvoiceId);
      }
    });

  }

  ngOnInit() {
    if (this.siInvoiceId == undefined) {
      this.siInvoiceId = this.invoice.getInvoiceId();
    }
    this.buildForm();
    this.getDtlObj.InvoiceId = this.siInvoiceId;
    this.getDtlObj.StartIndex = 0
    this.getDtlObj.EndIndex = this.take;
    this.getDtlObj.SortColumn = 'invoiceId';
    this.getDtlObj.SortOrder = 'Asc';
    this.activeindex = 0;
    this.moduleChange(this.links[0])
    //this.LoadSiblingInvoices();
  }

  public moduleChange(leader){
    this.getDtlObj.ModuleName = leader.name;
    this.columnTitle = this.getDtlObj.ModuleName == BillPayConstants.UBMGroup ? BillPayConstants.GroupPGI : BillPayConstants.ParentImageId;  
    this.LoadSiblingInvoices();
  }

  buildForm() {
    this.FormSiblingInvoice = this._fb.group({
      ParentCbmsImageId: [''],
      CBMSDOCID: [''],
      CbmsImageId: [''],
      AccountNumber: [''],
      PaymentStatus: ['']
    })
  }

  pageChange({ skip, take }: PageChangeEvent): void {
    this.skip = skip + 1;
    this.take = take - 1;
    this.pageSize = take;
    this.getDtlObj.StartIndex = this.skip
    this.getDtlObj.EndIndex = this.skip + this.take;
    this.LoadSiblingInvoices();
  }

  private buildURL(templateURL: string): string {
    if (templateURL) {
      const replacements = templateURL.match(/\{.*?\}/g);
      if (replacements) {
        replacements.forEach(element => {
          let value = '';
          if (element === '{xid}') {
            value = this.getLocalID('xId');
          } else if (element === '{uid}') {
            value = this.getLocalID('uid');
          }
          templateURL = templateURL.replace(element, value);
        });
        return templateURL;
      }
      else {
        return templateURL;
      }
    }
  }
  private getLocalID(id: string): string {
    const localValue = this.userInfoObj[id];
    return localValue !== null ? localValue : '';
  }
  openViewCost(dataItem: any) {
    const link = document.createElement('a');
    let currentYear = new Date().getFullYear();
    link.target = '_blank';
    link.href = environment.ViewCostAndUsuageUrl +
      'selClientId=' + `${dataItem.clientId}` +
      '&selDivisionId=' + `${dataItem.siteGroupID}` +
      '&selSite_ClientHierId=' + `${dataItem.clientHierId}` +
      '&selAccountId=' + `${dataItem.accountId}` +
      '&selReportYear=' +  currentYear + this.buildURL("&xid={xid}&uid={uid}")
    link.style.display = 'none';
    link.click();
  }
  openAccount(dataItem: any) {
    const link = document.createElement('a');
    link.target = '_blank';
    if (dataItem.accountType.toUpperCase() === "UTILITY") {
      link.href = environment.ViewAccount +
        '&accountID=' + `${dataItem.accountId}` +
        '&clientID=' + `${dataItem.clientId}` +
        '&siteID=' + `${dataItem.siteId}`
    }
    else if (dataItem.accountType.toUpperCase() === "SUPPLIER") {
      link.href = environment.ViewAccountSupplier +
        '&accountId=' + `${dataItem.accountId}` +
        '&site=' + `${dataItem.siteId}` +
        '&contractId=0'
    }
    link.style.display = 'none';
    link.click();
  }

  openInvoice(dataItem: any) {
    const link = document.createElement('a');
    link.target = '_blank';
    link.href = environment.ViewInvoice +
      'CuInvoiceId=' + `${dataItem.invoiceId}` +
      this.buildURL("&xid={xid}&uid={uid}")
    link.style.display = 'none';
    link.click();
  }
  public sortChange(sort: SortDescriptor[]): void {
    this.sort = sort;
    this.FormSiblingInvoice.value.SortColumn = sort[0].field;
    this.FormSiblingInvoice.value.SortOrder = sort[0].dir;
    if (sort[0].dir == "asc") {
      this.getDtlObj.SortOrder = 'Asc';
    } else {
      this.getDtlObj.SortOrder = 'Desc';
    }
    this.LoadSiblingInvoices();
  }
  LoadSiblingInvoices() {
    this.billPayservice.LoadParentImageInvoiceDetails(this.getDtlObj).subscribe(res => {
      this.gridData = res == null ? null : this.gridData = {
        data: res,
        total: res.length !== 0 ? res[0]['totalCount'] : 0,
      };
    });
  }

}
