import { Injectable } from '@angular/core';
import { ApiService } from './api.service';
import { ApiDictionary } from './api-dictionary';
import { map } from 'rxjs';


@Injectable({
  providedIn: 'root'
})
export class GlobalBillPayService {
  constructor(private apiService: ApiService) { }
  public loadwindow: any;

  LoadChargeValues(invoiceId, sortColumn) {
    return this.apiService.get(ApiDictionary.LoadChargeValues.url + '?invoiceId=' + invoiceId + '&sortColumn=' + sortColumn)
      .pipe(map(data => data));
  }

  LoadInvoiceDetails(Data) {
    return this.apiService.get(ApiDictionary.LoadInvoiceDetails.url + '?invoiceId=' + Data)
      .pipe(map(data => data));
  }
  LoadPaymentProcess(Data){
    return this.apiService.get(ApiDictionary.LoadPaymentProcess.url+ '?invoiceId=' + Data)
    .pipe(map(data => data));
  }
  
  loadPaymentFields(Data,IsPaymentGroup){
    return this.apiService.get(ApiDictionary.loadPaymentFields.url+ '?invoiceId=' + Data +'&IsPaymentGroup='+IsPaymentGroup)
    .pipe(map(data => data));
  }

  LoadPaymentInvDetails(invoiceId) {
    return this.apiService.get(ApiDictionary.LoadPaymentInvDetails.url + '?invoiceId=' + invoiceId)
      .pipe(map(data => data));
  }
  MarkInvoiceAsPaymentQuestion(invoiceData) {
    return this.apiService.post(ApiDictionary.PaymentQuestion.url, invoiceData)
      .pipe(map(data => data));
  }
  MarkInvoiceAsPaymentAccountSetupIssue(invoiceData) {
    return this.apiService.post(ApiDictionary.PaymentAccountSetupIssue.url, invoiceData)
      .pipe(map(data => data));
  }
  SendPaymentData(invoiceData) {
    return this.apiService.post(ApiDictionary.SendPaymentData.url, invoiceData)
      .pipe(map(data => data));
  }
  SavePaymentFieldsData(invoiceData) {
    return this.apiService.post(ApiDictionary.SavePaymentFieldsData.url, invoiceData)
      .pipe(map(data => data));
  }
  LoadParentImageInvoiceDetails(Data) {
    return this.apiService.get(ApiDictionary.LoadParentImageInvoiceDetails.url + '?invoiceId=' + Data.InvoiceId + '&startIndex=' + Data.StartIndex + '&endIndex=' + Data.EndIndex + '&sortColumn=' + Data.SortColumn + '&sortOrder=' + Data.SortOrder + '&moduleName=' + Data.ModuleName)
      .pipe(map(data => data));
  }
  GetMultipleInvoices(Data) {
    return this.apiService.get(ApiDictionary.GetMultipleInvoices.url + '?invoiceId=' + Data.invoiceId + '&startIndex=' + Data.StartIndex + '&endIndex=' + Data.EndIndex)
      .pipe(map(data => data));
  }
  SaveMultipleInvoices(Data) {
    return this.apiService.post(ApiDictionary.SaveMultipleInvoices.url, Data)
      .pipe(map(data => data));
  }
}
