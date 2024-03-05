import { Component, Input, OnChanges, OnInit } from '@angular/core';
import { PageChangeEvent } from '@progress/kendo-angular-grid';
import { SortDescriptor, orderBy } from '@progress/kendo-data-query';
import { environment } from 'src/environments/environment';
import { BillpaydataService } from '../core';

@Component({
  selector: 'app-charge-section',
  templateUrl: './charge-section.component.html',
  styleUrls: ['./charge-section.component.scss']
})
export class ChargeSectionComponent implements OnInit, OnChanges {
  @Input() chargeGridData: any;
  public chargeData: any;
  public accountNumber: string
  public isPageable: Boolean = false;
  public skip = 0;
  public pageSize = 25;
  public buttonCount = 5;
  public type: 'numeric' | 'input' = 'numeric';
  public info = true;
  public previousNext = true;
  public sort: SortDescriptor[] = [{ field: 'invoiceId', dir: 'asc' }];
  private userInfoObj: any;

  constructor(private invoice: BillpaydataService) { this.userInfoObj = this.invoice.getuserInfo(); }

  ngOnInit() {
  }
  ngOnChanges() {
    this.chargeData = this.chargeGridData;
    let total = this.chargeData.length;
    this.isPageable = total > this.pageSize ? true : false;
    this.loadItems();

  }
  pageChange({ skip, take }: PageChangeEvent): void {
    this.skip = skip;
    this.pageSize = take;
    this.loadItems();
  }

  sortChange(sort: SortDescriptor[]): void {
    this.sort = sort;
    this.loadItems();
  }

  public loadItems(): void {
    this.chargeData = {
      data: orderBy(this.chargeGridData, this.sort).slice(this.skip, this.skip + this.pageSize),
      total: this.chargeGridData.length,
    };
  }
  openInvoice(dataItem: any) {
    const link = document.createElement('a');
    link.target = '_blank';
    link.href = environment.EditInvoice + 
      '?CuInvoiceId=' + `${dataItem.invoiceId}` +
      this.buildURL("&xid={xid}&uid={uid}")
    link.style.display = 'none';
    link.click();
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

}
