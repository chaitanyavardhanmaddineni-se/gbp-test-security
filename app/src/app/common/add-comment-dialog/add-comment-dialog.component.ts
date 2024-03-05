import { Component, Input, OnInit } from '@angular/core';
import { ModalService } from 'src/app/core/services';

@Component({
  selector: 'app-add-comment-dialog',
  templateUrl: './add-comment-dialog.component.html',
  styleUrls: ['./add-comment-dialog.component.scss']
})
export class AddCommentDialogComponent implements OnInit {
  @Input() public inputs: any;
  public title: string;
  public buttons: string[] = [];
  public commentText: string;
  public errorMsg:string;

  constructor(private modalService: ModalService) { }

  ngOnInit() {
    this.title = this.inputs.title;
    this.commentText = this.inputs.displayMsg;
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
    if (answer === '' || answer === 'Cancel') {
      answer = 'No';
      this.modalService.destroy(answer);
    }
    if (answer === 'Ok') {
      if (this.commentText === null || this.commentText === undefined || this.commentText === "" || this.commentText.trim().length === 0) {
        this.errorMsg="*Please enter comments."
      }
      else {
        this.modalService.sendComment(this.commentText);
        this.modalService.destroy(answer);
      }
    }
  }
}
