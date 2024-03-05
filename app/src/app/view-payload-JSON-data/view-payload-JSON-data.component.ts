import { Component, OnInit } from '@angular/core';
import { BreadcrumbService } from '../core';
import { ActivatedRoute } from '@angular/router';
import { GlobalBillPayService } from '../core/services/globalbillpay.service';

@Component({
  selector: 'app-view-payload-JSON-data',
  templateUrl: './view-payload-JSON-data.component.html',
  styleUrls: ['./view-payload-JSON-data.component.scss']
})
export class ViewPayloadJSONDataComponent implements OnInit {
  formatJSON: any;
  breadCRumbs: any[] = [];
  invoiceId: any;

  constructor(private breadcrumbService: BreadcrumbService,
    private activatedRoute: ActivatedRoute,
    private billPayservice: GlobalBillPayService) {
    this.invoiceId = Number(this.activatedRoute.snapshot.paramMap.get('invoiceId'));
  }

  ngOnInit() {
    this.breadCRumbs = [
      { label: "View Payload JSON", data: "/ViewPayloadJSONData" },
    ]
    this.breadcrumbService.changeName(this.breadCRumbs);
    this.billPayservice.LoadPaymentInvDetails(this.invoiceId).subscribe(res => {
      const jsonData = res.requestJson;
      this.formatJSON = JSON.stringify(JSON.parse(jsonData), null, 2);
    });
  }

}
