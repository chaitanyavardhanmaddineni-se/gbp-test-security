import { Component, Input, OnChanges, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { environment } from 'src/environments/environment';
import { SiblinginvoiceDialogComponent } from '../common/siblinginvoice-dialog/siblinginvoice-dialog.component';
import { BillpaydataService, ModalService } from '../core';
import { InvoiceObjModel } from '../core/models/datamodel';
import { GlobalBillPayService } from '../core/services/globalbillpay.service';

@Component({
  selector: 'app-account-section',
  templateUrl: './account-section.component.html',
  styleUrls: ['./account-section.component.scss']
})
export class AccountSectionComponent implements OnInit, OnChanges {

  @Input() accountgridData: any;
  @Input() isLinkDisabled: boolean;
  userObj: any;
  JsonData: any;
  public accountData: any;
  public groupAcnt: any;
  public clientName: any;
  public currency: any;
  public vendorName: any;
  public ubm: any;
  public groupKey: any;
  public imageIddtl: number;
  public invoiceId: number;
  public comment: string;
  public country: any;
  public Country: any;
  public invoiceObj: InvoiceObjModel = new InvoiceObjModel();
  result: any;
  constructor(private billPayservice: GlobalBillPayService,
    private activatedrouter: ActivatedRoute,
    private router: Router,
    private modalService: ModalService,
    private userDataService: BillpaydataService) {
    this.userObj = this.userDataService.getuserInfo();
    this.activatedrouter.params.subscribe(params => {
      if (typeof params['InvoiceId'] != 'undefined' && params['InvoiceId']) {
        this.invoiceId = params['InvoiceId'];
        this.userDataService.setInvoiceId(this.invoiceId);
      }
    });
  }


  ngOnInit() {
  }
  ngOnChanges() {
    this.accountData = this.accountgridData;
    if (this.accountData != null && this.accountData.length > 0) {
      this.groupAcnt = this.accountData[0].groupAccount;
      this.clientName = this.accountData[0].clientName;
      this.currency = this.accountData[0].currency;
      this.vendorName = this.accountData[0].vendorName;
      this.ubm = this.accountData[0].ubm;
      this.groupKey = this.accountData[0].groupKey;
      this.imageIddtl = this.accountData[0].imageId;
      this.country = this.accountData[0].country;
    }

  }

  openImage(intImageId) {
    const url = environment.FetchImageDetails + 'cbmsimgid=' + `${intImageId}&mode=View`;
    let windowdetails: any = window.open(url, 'imageWindow', 'location=yes,scrollbars=yes,status=yes');
    this.billPayservice.loadwindow = windowdetails;
  }

  openInvoice() {
    let inputs = {
      displayMsg: ''
    };
    document.body.style.overflow = 'hidden';
    this.modalService.init(SiblinginvoiceDialogComponent, inputs, {})
  }
}
