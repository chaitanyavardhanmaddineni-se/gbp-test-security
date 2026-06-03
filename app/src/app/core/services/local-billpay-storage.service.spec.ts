import { TestBed } from '@angular/core/testing';

import { LocalBillPayStorageService } from './local-billpay-storage.service';

describe('LocalBillPayStorageService', () => {
  let service: LocalBillPayStorageService;

  beforeEach(() => {
    window.localStorage.removeItem('globalBillPay.localData.v1');

    TestBed.configureTestingModule({});
    service = TestBed.inject(LocalBillPayStorageService);
  });

  afterEach(() => {
    window.localStorage.removeItem('globalBillPay.localData.v1');
  });

  it('loads invoice details from the predefined local data store', (done: DoneFn) => {
    service.loadInvoiceDetails(1001).subscribe(invoiceDetails => {
      expect(invoiceDetails.length).toBe(1);
      expect(invoiceDetails[0]['clientName']).toBe('Local Trial Client');
      expect(invoiceDetails[0]['paymentStatus']).toBe('Hold');
      done();
    });
  });

  it('uses locally saved payment details as payment field prefill values', (done: DoneFn) => {
    const paymentDetails = {
      InvoiceNumber: 'LOCAL-SAVED-1',
      PaymentAmount: '200.00',
      'Re-EnterPaymentAmount': '200.00'
    };

    service.savePaymentFieldsData({
      InvoiceId: 1001,
      UserId: 1,
      InvPaymentDetails: JSON.stringify(paymentDetails)
    }).subscribe(() => {
      service.loadPaymentFields(1001).subscribe(paymentFields => {
        const invoiceNumberField = paymentFields.find(field => field.fieldName === 'InvoiceNumber');
        const paymentAmountField = paymentFields.find(field => field.fieldName === 'PaymentAmount');

        expect(invoiceNumberField?.prefillValue).toBe('LOCAL-SAVED-1');
        expect(paymentAmountField?.prefillValue).toBe('200.00');
        done();
      });
    });
  });

  it('stores submitted payment data and returns it as view payload JSON', (done: DoneFn) => {
    const paymentDetails = {
      InvoiceNumber: 'LOCAL-SUBMITTED-1',
      PaymentAmount: '300.00'
    };

    service.sendPaymentData({
      InvoiceId: 1001,
      UserId: 1,
      InvPaymentDetails: JSON.stringify(paymentDetails)
    }).subscribe(() => {
      service.loadPaymentInvoiceDetails(1001).subscribe(paymentInvoiceDetails => {
        const requestJson = JSON.parse(paymentInvoiceDetails.requestJson);

        expect(paymentInvoiceDetails.paymentStatus).toBe('Submitted');
        expect(requestJson.paymentStatus).toBe('Submitted');
        expect(requestJson.paymentDetails.InvoiceNumber).toBe('LOCAL-SUBMITTED-1');
        done();
      });
    });
  });
});
