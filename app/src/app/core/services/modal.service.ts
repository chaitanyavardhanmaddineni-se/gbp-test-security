import { Injectable } from '@angular/core';
import { DomService } from './dom.service';
import { Subject, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})

export class ModalService {

  private answer = new Subject<boolean>();
  private commentSubject = new Subject<string>();
  private modalElementId = 'modal-container';
  private overlayElementId = 'overlay';
  public actionPerformed = new Subject<any>();
  constructor(private domService: DomService) { }

  init(component: any, inputs: object, outputs: object) {

    const componentConfig = {
      inputs,
      outputs
    };

    this.domService.appendComponentTo(this.modalElementId, component, componentConfig);
    document.getElementById(this.modalElementId).className = 'show';
    document.getElementById(this.overlayElementId).className = 'show';
  }

  destroy(answer?: any) {
    if (answer) {
      this.SetAnswer(answer);
    }
    this.domService.removeComponent();
  }

  private SetAnswer(value: any): void {
    if (value) {
      return this.answer.next(value);
    }
  }

  public GetAnswer(): Observable<any> {
    return this.answer.asObservable();
  }

  public sendComment(comment: string) {
    this.commentSubject.next(comment);
  }

  public getComment(): Observable<string> {
    if (this.commentSubject.observers.length == 0)
      return this.commentSubject.asObservable();
    else {
      this.commentSubject.observers.length = 0;
      return this.commentSubject.asObservable();
    }
  }
  public setInvoiceNumberobj(obj: any) {
    this.actionPerformed.next(obj)
  }
  public getInvoiceNumbers(): Observable<any> {
    return this.actionPerformed.asObservable();
  }





}
