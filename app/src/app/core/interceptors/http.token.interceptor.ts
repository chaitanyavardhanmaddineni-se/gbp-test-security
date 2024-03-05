
import { tap, catchError } from 'rxjs/operators';
import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
  HttpResponse,
  HttpErrorResponse
} from '@angular/common/http';
// import { Ng4LoadingSpinnerService } from 'ng4-loading-spinner';
import { NgxSpinnerService } from 'ngx-spinner';
import { Observable, throwError } from 'rxjs';
import { ModalService } from '../services/modal.service';
import { ConfirmationDialogComponent } from 'src/app/common/confirmation-dialog/confirmation-dialog.component';


@Injectable()
export class HttpTokenInterceptor implements HttpInterceptor {
  private loaderExcludedUrls = [

  ]
  couter = 0;
  constructor(private spinner: NgxSpinnerService, private modalService: ModalService) { }
  private isLoaderExcludedForUrl(url: string): boolean {

    let isExcluded = false;
    try {
      for (var item of this.loaderExcludedUrls) {
        if (url.includes(item.url)) {
          isExcluded = true;
          break;
        }
      }
    }
    catch {
      isExcluded = true
    }
    return isExcluded;

  }
  private totalRequests = 0;
  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    request = request.clone({
      setHeaders: {
        'Access-Control-Allow-Origin': '*',
        'Access-Control-Allow-Methods': 'POST, GET, OPTIONS, PUT',
        'Accept': 'application/json',
        'Content-Type': 'application/json'
      }
    });

    this.totalRequests++;
    this.spinner.show();

    if (this.isLoaderExcludedForUrl(request.url)) {
      this.spinner.hide();
      this.totalRequests--
    }
    return next.handle(request).pipe(
      tap(res => {
        if (res instanceof HttpResponse) {
          this.decreaseRequests();
        }
      }),
      catchError((error: HttpErrorResponse) => {
        let errorMessage = 'Application encounter error, please try again';
        if (error.error instanceof ErrorEvent) {
          errorMessage = `Error: ${error.error.message}`;
        }

        this.decreaseRequests();
        let inputs = {};
        inputs = {
          displayMsg: errorMessage,
          exception: error

        };
        this.modalService.init(ConfirmationDialogComponent, inputs, {});
        return throwError('');
      })

    );
  }
  private decreaseRequests() {
    this.totalRequests--;
    if (this.totalRequests === 0 || this.totalRequests < 0) {
      this.spinner.hide();
    }
  }
  private ChangeDates(body) {
    if (body === null || body === undefined) {
      return body;
    }

    if (typeof body !== 'object') {
      return body;
    }

    for (const key of Object.keys(body)) {
      const value = body[key];
      if (value instanceof Date) {
        body[key] = new Date(Date.UTC(value.getFullYear(), value.getMonth(), value.getDate(), value.getHours(), value.getMinutes()
          , value.getSeconds()));
      } else if (typeof value === 'object') {
        this.ChangeDates(value);
      }
    }
  }
}


