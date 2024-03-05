import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class BreadcrumbService {

  private pageName = new Subject<any>();
  currentName = this.pageName.asObservable();
  constructor() {

  }
  changeName(data: any) {
    this.pageName.next(data)
  }
}
