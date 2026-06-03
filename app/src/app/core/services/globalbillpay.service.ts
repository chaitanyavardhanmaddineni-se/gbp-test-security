import { Injectable } from '@angular/core';
import { ApiService } from './api.service';
import { ApiDictionary } from './api-dictionary';
import { map } from 'rxjs';
import { environment } from '../../../environments/environment';
import { LocalBillPayStorageService } from './local-billpay-storage.service';


@Injectable({
  providedIn: 'root'
})
export class GlobalBillPayService {
  constructor(
    private apiService: ApiService,
    private localBillPayStorageService: LocalBillPayStorageService
  ) { }
  public loadwindow: any;

  LoadChargeValues(invoiceId, sortColumn) {
    if (environment.useLocalBillPayStorage) {
      return this.localBillPayStorageService.loadChargeValues(Number(invoiceId), sortColumn);
    }

    return this.apiService.get(ApiDictionary.LoadChargeValues.url + '?invoiceId=' + invoiceId + '&sortColumn=' + sortColumn)
      .pipe(map(data => data));
  }

  LoadInvoiceDetails(Data) {
    if (environment.useLocalBillPayStorage) {
      return this.localBillPayStorageService.loadInvoiceDetails(Number(Data));
    }

    return this.apiService.get(ApiDictionary.LoadInvoiceDetails.url + '?invoiceId=' + Data)
      .pipe(map(data => data));
  }
  LoadPaymentProcess(Data){
    if (environment.useLocalBillPayStorage) {
      return this.localBillPayStorageService.loadPaymentProcess(Number(Data));
    }

    return this.apiService.get(ApiDictionary.LoadPaymentProcess.url+ '?invoiceId=' + Data)
    .pipe(map(data => data));
  }
  
  loadPaymentFields(Data,IsPaymentGroup){
    if (environment.useLocalBillPayStorage) {
      return this.localBillPayStorageService.loadPaymentFields(Number(Data));
    }

    return this.apiService.get(ApiDictionary.loadPaymentFields.url+ '?invoiceId=' + Data +'&IsPaymentGroup='+IsPaymentGroup)
    .pipe(map(data => data));
  }

  LoadPaymentInvDetails(invoiceId) {
    if (environment.useLocalBillPayStorage) {
      return this.localBillPayStorageService.loadPaymentInvoiceDetails(Number(invoiceId));
    }

    return this.apiService.get(ApiDictionary.LoadPaymentInvDetails.url + '?invoiceId=' + invoiceId)
      .pipe(map(data => data));
  }
  MarkInvoiceAsPaymentQuestion(invoiceData) {
    if (environment.useLocalBillPayStorage) {
      return this.localBillPayStorageService.markInvoiceAsPaymentQuestion(invoiceData);
    }

    return this.apiService.post(ApiDictionary.PaymentQuestion.url, invoiceData)
      .pipe(map(data => data));
  }
  MarkInvoiceAsPaymentAccountSetupIssue(invoiceData) {
    if (environment.useLocalBillPayStorage) {
      return this.localBillPayStorageService.markInvoiceAsPaymentAccountSetupIssue(invoiceData);
    }

    return this.apiService.post(ApiDictionary.PaymentAccountSetupIssue.url, invoiceData)
      .pipe(map(data => data));
  }
  SendPaymentData(invoiceData) {
    if (environment.useLocalBillPayStorage) {
      return this.localBillPayStorageService.sendPaymentData(invoiceData);
    }

    return this.apiService.post(ApiDictionary.SendPaymentData.url, invoiceData)
      .pipe(map(data => data));
  }
  SavePaymentFieldsData(invoiceData) {
    if (environment.useLocalBillPayStorage) {
      return this.localBillPayStorageService.savePaymentFieldsData(invoiceData);
    }

    return this.apiService.post(ApiDictionary.SavePaymentFieldsData.url, invoiceData)
      .pipe(map(data => data));
  }
  LoadParentImageInvoiceDetails(Data) {
    if (environment.useLocalBillPayStorage) {
      return this.localBillPayStorageService.loadParentImageInvoiceDetails(Data);
    }

    return this.apiService.get(ApiDictionary.LoadParentImageInvoiceDetails.url + '?invoiceId=' + Data.InvoiceId + '&startIndex=' + Data.StartIndex + '&endIndex=' + Data.EndIndex + '&sortColumn=' + Data.SortColumn + '&sortOrder=' + Data.SortOrder + '&moduleName=' + Data.ModuleName)
      .pipe(map(data => data));
  }
  GetMultipleInvoices(Data) {
    if (environment.useLocalBillPayStorage) {
      return this.localBillPayStorageService.getMultipleInvoices(Data);
    }

    return this.apiService.get(ApiDictionary.GetMultipleInvoices.url + '?invoiceId=' + Data.invoiceId + '&startIndex=' + Data.StartIndex + '&endIndex=' + Data.EndIndex)
      .pipe(map(data => data));
  }
  SaveMultipleInvoices(Data) {
    if (environment.useLocalBillPayStorage) {
      return this.localBillPayStorageService.saveMultipleInvoices(Data);
    }

    return this.apiService.post(ApiDictionary.SaveMultipleInvoices.url, Data)
      .pipe(map(data => data));
  }
}
