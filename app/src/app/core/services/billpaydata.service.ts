import { Injectable } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { UserobjModel } from '../../core/models/model';

@Injectable({
  providedIn: 'root'
})
export class BillpaydataService {
  private userInfo: UserobjModel;
  private invoiceId: number;
  private isInvoiceExist: boolean;
  private paymentFeilds: FormGroup;
  constructor() { }

  public setuserInfo(value) {
    this.userInfo = value;
  }
  public getuserInfo() {
    return this.userInfo;
  }
  public setInvoiceId(value) {
    this.invoiceId = value;
  }
  public getInvoiceId() {
    return this.invoiceId;
  }
  public setInvoiceExist(value) {
    this.isInvoiceExist = value;
  }
  public getInvoiceExist() {
    return this.isInvoiceExist;
  }
  public setFormGroup(form) {
    this.paymentFeilds = form;
  }
  public getpaymentFormGroup() {
    return this.paymentFeilds;
  }
}
