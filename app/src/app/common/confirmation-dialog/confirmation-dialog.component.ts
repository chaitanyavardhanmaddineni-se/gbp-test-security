import { Component, OnInit, Input } from '@angular/core';
import { ModalService } from '../../core/services';

@Component({
  selector: 'app-confirmation-dialog',
  templateUrl:'./confirmation-dialog.component.html',
  styleUrls: ['./confirmation-dialog.component.scss']
})
export class ConfirmationDialogComponent implements OnInit {
  @Input() public inputs: any;
  @Input() public error: any;
  @Input() public outputs: any;
  public title: string;
  public message: string;
  public buttons: string[] = [];
  public height: number;
  public width: number;
  public messageClass: string;
  public exceptionDetails: any;

  constructor(private modalService: ModalService) { }

  ngOnInit() {
    this.title = this.inputs.title ? this.inputs.title : 'Warning';
    this.message = this.inputs.displayMsg;
    this.exceptionDetails = this.inputs.exception === undefined ? '' : this.inputs.exception.message;
   // this.height = this.inputs.height ? this.inputs.height : 200;
    this.width = this.inputs.width ? this.inputs.width : 499;
    this.messageClass = this.inputs.messageClass ? this.inputs.messageClass : 'defaultClass';
    this.defineButtons();
  }

  private defineButtons() {
    if (this.inputs.buttons) {
      this.buttons = this.inputs.buttons;
    } else {
      this.buttons.push('Close');
    }
  }

  public close(answer: string) {
    if (answer === '') {
      answer = 'No';
    }
    this.modalService.destroy(answer);
  }

}
