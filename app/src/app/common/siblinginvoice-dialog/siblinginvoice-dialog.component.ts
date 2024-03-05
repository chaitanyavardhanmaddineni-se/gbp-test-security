import { Component, Input, OnInit } from '@angular/core';
import { ModalService } from 'src/app/core';

@Component({
  selector: 'app-siblinginvoice-dialog',
  templateUrl: './siblinginvoice-dialog.component.html',
  styleUrls: ['./siblinginvoice-dialog.component.scss']
})
export class SiblinginvoiceDialogComponent implements OnInit {

  public showSiblingInvoices = false;
  @Input() public inputs: any;
  public title: string;
  public buttons: string[] = [];
  constructor(private modalService: ModalService) { }

  ngOnInit() {
    //this.title = this.inputs.title;
    this.defineButtons();
    this.showSiblingInvoices=true;
  }
  private defineButtons() {
    if (this.inputs.buttons) {
      this.buttons = this.inputs.buttons;
    } else {
      this.buttons.push('Close');
    }
  }

  public close() {
    document.body.style.overflow='';
      this.modalService.destroy();
    }
}
