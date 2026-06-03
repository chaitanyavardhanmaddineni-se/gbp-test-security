import { Injectable } from '@angular/core';
import { of, Observable } from 'rxjs';

import localBillPayData from 'src/assets/Data/local-billpay-data.json';

interface LocalUserInfo {
  userInfoId: number;
  userQueueId: number;
  userName: string;
}

interface LocalPaymentField {
  fieldId: number;
  fieldName: string;
  fieldType: string;
  clientId: number;
  vendorId: number;
  data: LocalDropdownValue[] | null;
  matchFieldName: string | null;
  customFieldFormat: string | null;
  displayFieldName: string;
  displayOrder: number;
  isActive: boolean;
  isHidden: boolean;
  isRequired: boolean;
  prefillValue: string | null;
  isSendValue: boolean;
  maxLength: number | null;
}

interface LocalDropdownValue {
  id: number;
  value: string;
  dynamicValue: string;
}

interface LocalPaymentInvoiceDetails {
  cuInvoicePaymentConfigId: number | null;
  cuInvoiceId: number;
  paymentAttributeValue: string;
  paymentSubmissionUserId: number | null;
  paymentSubmissionTs: string | null;
  paymentResponseStatusCd: number | null;
  paymentResponseTs: string | null;
  createdUserId: number | null;
  createdTs: string;
  updatedUserId: number | null;
  lastChangeTs: string;
  paymentStatus: string;
  requestJson: string;
}

interface LocalBillPayInvoice {
  invoiceId: number;
  invoiceDetails: Record<string, unknown>[];
  charges: Record<string, unknown>[];
  paymentFields: LocalPaymentField[];
  paymentProcessInstructions: Record<string, unknown>[];
  groupBillInvoices: Record<string, unknown>[];
  siblingInvoices: Record<string, unknown>[];
  exceptions: Record<string, unknown>[];
  paymentInvoiceDetails?: LocalPaymentInvoiceDetails;
}

interface LocalBillPayStore {
  schemaVersion: number;
  user: LocalUserInfo;
  invoices: LocalBillPayInvoice[];
}

@Injectable({
  providedIn: 'root'
})
export class LocalBillPayStorageService {
  private readonly storageKey = 'globalBillPay.localData.v1';
  private readonly schemaVersion = 1;
  private fallbackStore: LocalBillPayStore | null = null;

  getUserInfo(): LocalUserInfo {
    return this.clone(this.getStore().user);
  }

  loadChargeValues(invoiceId: number, sortColumn: string): Observable<Record<string, unknown>[]> {
    const charges = this.getInvoice(invoiceId).charges;
    const sortedCharges = [...charges].sort((left, right) => this.compareBySortColumn(left, right, sortColumn));

    return of(this.clone(sortedCharges));
  }

  loadInvoiceDetails(invoiceId: number): Observable<Record<string, unknown>[]> {
    return of(this.clone(this.getInvoice(invoiceId).invoiceDetails));
  }

  loadPaymentProcess(invoiceId: number): Observable<Record<string, unknown>[]> {
    return of(this.clone(this.getInvoice(invoiceId).paymentProcessInstructions));
  }

  loadPaymentFields(invoiceId: number): Observable<LocalPaymentField[]> {
    const invoice = this.getInvoice(invoiceId);
    const savedPaymentDetails = this.getSavedPaymentValues(invoice);
    const paymentFields = invoice.paymentFields.map(field => ({
      ...field,
      prefillValue: savedPaymentDetails[field.fieldName] !== undefined
        ? this.toPrefillValue(savedPaymentDetails[field.fieldName])
        : field.prefillValue
    }));

    return of(this.clone(paymentFields));
  }

  loadPaymentInvoiceDetails(invoiceId: number): Observable<LocalPaymentInvoiceDetails> {
    return of(this.clone(this.ensurePaymentInvoiceDetails(this.getInvoice(invoiceId))));
  }

  markInvoiceAsPaymentQuestion(invoiceData: Record<string, unknown>): Observable<number> {
    return this.saveException(invoiceData);
  }

  markInvoiceAsPaymentAccountSetupIssue(invoiceData: Record<string, unknown>): Observable<number> {
    return this.saveException(invoiceData);
  }

  savePaymentFieldsData(invoiceData: Record<string, unknown>): Observable<string> {
    const invoiceId = this.getInvoiceId(invoiceData);
    this.savePaymentDetails(invoiceId, invoiceData, 'Hold');

    return of('Saved locally');
  }

  sendPaymentData(invoiceData: Record<string, unknown>): Observable<unknown[]> {
    const invoiceId = this.getInvoiceId(invoiceData);
    this.savePaymentDetails(invoiceId, invoiceData, 'Submitted');
    this.updateInvoicePaymentStatus(invoiceId, true, 'Submitted');

    return of([]);
  }

  loadParentImageInvoiceDetails(query: Record<string, unknown>): Observable<Record<string, unknown>[]> {
    const invoiceId = this.getInvoiceId(query);
    const siblingInvoices = this.getInvoice(invoiceId).siblingInvoices;

    return of(this.clone(this.paginate(siblingInvoices, query)));
  }

  getMultipleInvoices(query: Record<string, unknown>): Observable<Record<string, unknown>[]> {
    const invoiceId = this.getInvoiceId(query);
    const groupBillInvoices = this.getInvoice(invoiceId).groupBillInvoices;

    return of(this.clone(this.paginate(groupBillInvoices, query)));
  }

  saveMultipleInvoices(invoices: Record<string, unknown>[]): Observable<string> {
    const store = this.getStore();
    invoices.forEach(invoiceData => {
      const invoiceId = this.getInvoiceId(invoiceData);
      const localInvoice = store.invoices.find(invoice => invoice.invoiceId === invoiceId)
        ?? store.invoices[0];

      localInvoice.groupBillInvoices = localInvoice.groupBillInvoices.map(groupBillInvoice => {
        if (Number(groupBillInvoice['invoiceId']) !== invoiceId) {
          return groupBillInvoice;
        }

        return {
          ...groupBillInvoice,
          invoiceNumber: invoiceData['InvoiceNumber'] ?? invoiceData['invoiceNumber'] ?? '',
          userInfoId: invoiceData['UserInfoId'] ?? invoiceData['userInfoId'] ?? groupBillInvoice['userInfoId']
        };
      });
    });
    this.saveStore(store);

    return of('Saved locally');
  }

  private saveException(invoiceData: Record<string, unknown>): Observable<number> {
    const invoiceId = this.getInvoiceId(invoiceData);
    const store = this.getStore();
    const invoice = this.findInvoice(store, invoiceId);
    invoice.exceptions = [
      ...invoice.exceptions,
      {
        ...invoiceData,
        createdTs: new Date().toISOString()
      }
    ];
    this.updateInvoicePaymentStatus(invoiceId, false, 'Hold', store);
    this.saveStore(store);

    return of(invoiceId);
  }

  private savePaymentDetails(invoiceId: number, invoiceData: Record<string, unknown>, paymentStatus: string): void {
    const store = this.getStore();
    const invoice = this.findInvoice(store, invoiceId);
    const paymentDetails = this.getInvPaymentDetails(invoiceData);
    const currentPaymentDetails = this.ensurePaymentInvoiceDetails(invoice);
    const now = new Date().toISOString();

    invoice.paymentInvoiceDetails = {
      ...currentPaymentDetails,
      cuInvoiceId: invoiceId,
      paymentAttributeValue: paymentDetails,
      paymentSubmissionUserId: Number(invoiceData['UserId'] ?? invoiceData['userId'] ?? currentPaymentDetails.paymentSubmissionUserId ?? 0),
      paymentSubmissionTs: paymentStatus === 'Submitted' ? now : currentPaymentDetails.paymentSubmissionTs,
      paymentResponseStatusCd: paymentStatus === 'Submitted' ? 200 : currentPaymentDetails.paymentResponseStatusCd,
      paymentResponseTs: paymentStatus === 'Submitted' ? now : currentPaymentDetails.paymentResponseTs,
      updatedUserId: Number(invoiceData['UserId'] ?? invoiceData['userId'] ?? currentPaymentDetails.updatedUserId ?? 0),
      lastChangeTs: now,
      paymentStatus,
      requestJson: this.buildRequestJson(invoiceData, paymentDetails, paymentStatus)
    };
    this.saveStore(store);
  }

  private ensurePaymentInvoiceDetails(invoice: LocalBillPayInvoice): LocalPaymentInvoiceDetails {
    if (invoice.paymentInvoiceDetails) {
      return invoice.paymentInvoiceDetails;
    }

    const now = new Date().toISOString();

    return {
      cuInvoicePaymentConfigId: null,
      cuInvoiceId: invoice.invoiceId,
      paymentAttributeValue: '{}',
      paymentSubmissionUserId: null,
      paymentSubmissionTs: null,
      paymentResponseStatusCd: null,
      paymentResponseTs: null,
      createdUserId: null,
      createdTs: now,
      updatedUserId: null,
      lastChangeTs: now,
      paymentStatus: 'Draft',
      requestJson: JSON.stringify({
        invoiceId: invoice.invoiceId,
        paymentStatus: 'Draft',
        paymentDetails: {}
      })
    };
  }

  private buildRequestJson(
    invoiceData: Record<string, unknown>,
    paymentDetails: string,
    paymentStatus: string
  ): string {
    return JSON.stringify({
      invoiceId: this.getInvoiceId(invoiceData),
      paymentStatus,
      savedAt: new Date().toISOString(),
      paymentDetails: this.parseJsonObject(paymentDetails),
      source: 'local-storage'
    });
  }

  private getSavedPaymentValues(invoice: LocalBillPayInvoice): Record<string, unknown> {
    return this.parseJsonObject(invoice.paymentInvoiceDetails?.paymentAttributeValue ?? '{}');
  }

  private getInvPaymentDetails(invoiceData: Record<string, unknown>): string {
    return String(invoiceData['InvPaymentDetails'] ?? invoiceData['invPaymentDetails'] ?? '{}');
  }

  private updateInvoicePaymentStatus(
    invoiceId: number,
    isInvoiceExist: boolean,
    paymentStatus: string,
    currentStore?: LocalBillPayStore
  ): void {
    const store = currentStore ?? this.getStore();
    const invoice = this.findInvoice(store, invoiceId);

    invoice.invoiceDetails = invoice.invoiceDetails.map(invoiceDetail => ({
      ...invoiceDetail,
      isInvoiceExist,
      isOpenExceptions: invoice.exceptions.length > 0,
      paymentStatus
    }));

    if (!currentStore) {
      this.saveStore(store);
    }
  }

  private findInvoice(store: LocalBillPayStore, invoiceId: number): LocalBillPayInvoice {
    const invoice = store.invoices.find(localInvoice => localInvoice.invoiceId === invoiceId);

    if (invoice) {
      return invoice;
    }

    const seedInvoice = this.clone(store.invoices[0]);
    seedInvoice.invoiceId = invoiceId;
    seedInvoice.invoiceDetails = seedInvoice.invoiceDetails.map(invoiceDetail => ({
      ...invoiceDetail,
      invoiceId,
      isInvoiceExist: false,
      paymentStatus: 'Hold'
    }));
    seedInvoice.charges = seedInvoice.charges.map(charge => ({
      ...charge,
      invoiceId
    }));
    seedInvoice.groupBillInvoices = seedInvoice.groupBillInvoices.map(groupBillInvoice => ({
      ...groupBillInvoice,
      invoiceId
    }));
    seedInvoice.siblingInvoices = seedInvoice.siblingInvoices.map(siblingInvoice => ({
      ...siblingInvoice,
      invoiceId
    }));
    delete seedInvoice.paymentInvoiceDetails;
    store.invoices = [...store.invoices, seedInvoice];

    return seedInvoice;
  }

  private getInvoice(invoiceId: number): LocalBillPayInvoice {
    const store = this.getStore();
    const invoice = this.findInvoice(store, invoiceId);
    this.saveStore(store);

    return invoice;
  }

  private getStore(): LocalBillPayStore {
    const storedValue = this.getStoredValue();

    if (!storedValue) {
      return this.initializeStore();
    }

    try {
      const parsedStore = JSON.parse(storedValue) as LocalBillPayStore;

      if (parsedStore.schemaVersion !== this.schemaVersion) {
        return this.initializeStore();
      }

      return parsedStore;
    } catch {
      return this.initializeStore();
    }
  }

  private initializeStore(): LocalBillPayStore {
    const store = this.clone(localBillPayData) as LocalBillPayStore;
    this.saveStore(store);

    return store;
  }

  private getStoredValue(): string | null {
    try {
      return window.localStorage.getItem(this.storageKey);
    } catch {
      return this.fallbackStore ? JSON.stringify(this.fallbackStore) : null;
    }
  }

  private saveStore(store: LocalBillPayStore): void {
    try {
      window.localStorage.setItem(this.storageKey, JSON.stringify(store));
    } catch {
      this.fallbackStore = this.clone(store);
    }
  }

  private paginate(
    items: Record<string, unknown>[],
    query: Record<string, unknown>
  ): Record<string, unknown>[] {
    const startIndex = Number(query['StartIndex'] ?? query['startIndex'] ?? 0);
    const endIndex = Number(query['EndIndex'] ?? query['endIndex'] ?? items.length);
    const pagedItems = items.slice(startIndex, endIndex);
    const totalCount = items.length;

    return pagedItems.map(item => ({
      ...item,
      totalCount
    }));
  }

  private compareBySortColumn(
    left: Record<string, unknown>,
    right: Record<string, unknown>,
    sortColumn: string
  ): number {
    if (!sortColumn) {
      return 0;
    }

    const sortKey = this.toCamelCase(sortColumn);
    const leftValue = String(left[sortKey] ?? '');
    const rightValue = String(right[sortKey] ?? '');

    return leftValue.localeCompare(rightValue);
  }

  private toCamelCase(value: string): string {
    return value
      .toLowerCase()
      .replace(/_([a-z])/g, (_, letter: string) => letter.toUpperCase());
  }

  private toPrefillValue(value: unknown): string {
    if (value === null || value === undefined) {
      return '';
    }

    return String(value);
  }

  private getInvoiceId(data: Record<string, unknown>): number {
    return Number(data['InvoiceId'] ?? data['invoiceId'] ?? 0);
  }

  private parseJsonObject(value: string): Record<string, unknown> {
    try {
      return JSON.parse(value) as Record<string, unknown>;
    } catch {
      return {};
    }
  }

  private clone<T>(value: T): T {
    return JSON.parse(JSON.stringify(value)) as T;
  }
}
