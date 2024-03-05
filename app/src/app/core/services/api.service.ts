import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpHeaders, HttpClient, HttpParams } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';

import { catchError, map, tap } from 'rxjs';
import { Router } from '@angular/router';
import { Location } from "@angular/common";

@Injectable({
  providedIn: 'root'
})
export class ApiService {

  constructor(
    private http: HttpClient, private location: Location, private route: Router
  ) {
  }
  private serviceUrl: string = environment.apiBaseUrl //environment.apiBaseUrl;
  private formatErrors(error: any) {
    return throwError(error.error);
  }

  get(path: string, params: HttpParams = new HttpParams()): Observable<any> {
    return this.http.get(this.serviceUrl + path, { params })
      .pipe(map(data => data));
  }
  async getData(path: string, params: HttpParams = new HttpParams()) {
    let promise = new Promise((resolve, reject) => {

      this.http.get(this.serviceUrl + path, { params })
        .toPromise()
        .then(
          res => { // Success

            resolve('');
          }
        );
    });
    return await promise;
  }

  put(path: string, body: Object = {}): Observable<any> {
    return this.http.put(
      this.serviceUrl + path,
      JSON.stringify(body)
    ).pipe(catchError(this.formatErrors));
  }
  post(path: string, body: Object = {}): Observable<any> {
    return this.http.post(
      this.serviceUrl + path,
      body).pipe(catchError(this.formatErrors));
  }
  delete(path): Observable<any> {
    return this.http.delete(
      this.serviceUrl + path
    ).pipe(catchError(this.formatErrors));
  }
  genaratePdf(url, postdata) {
    return this.http.post(this.serviceUrl + url, postdata, { responseType: 'blob', observe: 'response' }).toPromise();
  }
  // This method is called in case of a PUT request where Accrual service is directly invoked
  PostReturnText(url: string, postdata: string) {
    return this.http.post(this.serviceUrl + url, postdata, {
      headers: new HttpHeaders().set('Content-Type', 'application/json'),
      responseType: 'text'
    }).toPromise();
  }
  PutReturnText(url: string, postdata: string) {
    return this.http.put(this.serviceUrl + url, postdata, {
      headers: new HttpHeaders().set('Content-Type', 'application/json'),
      responseType: 'text'
    }).toPromise();
  }

  async getasync(path: string, params: HttpParams = new HttpParams()) {
    return await this.http.get(this.serviceUrl + path, { params })
      .pipe(map(data => (data))).toPromise();
  }
  async postasync(path: string, body: Object = {}) {
    let loading = false;
    return await this.http
      .post(this.serviceUrl + path,
        body)
      .pipe(
        map(response => response),
        tap(() => loading = false)).toPromise();
  }

  public postApproval(path, data) {
    return this.http.post<Blob>(this.serviceUrl + path, data, { responseType: 'blob' as 'json' })
      .pipe(catchError(this.formatErrors));
  }
  public getApproval(path) {
    return this.http.get<Blob>(path, { responseType: 'blob' as 'json' })
      .pipe(catchError(this.formatErrors));
  }

}
