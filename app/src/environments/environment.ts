// This file can be replaced during build by using the `fileReplacements` array.
// `ng build --prod` replaces `environment.ts` with `environment.prod.ts`.
// The list of file replacements can be found in `angular.json`.

export const environment = {
  production: false,
  CBMSServiceUrl: 'http://cbmsnet-tk1.ems.schneider-electric.com/CBMSService/',
  CBMSManageClient: 'http://cbms-tk1.ems.schneider-electric.com/cbms/cis/viewClientDetailsAction.do?pager.offset=0&clientID=',
  CBMSHomePage: 'http://cbms-tk1.ems.schneider-electric.com/cbms/dealticket/cemHomePageAction.do',
  appLoginURL: "http://cbms-tk1.ems.schneider-electric.com",
  apiBaseUrl: 'https://localhost:44315/api/',
  //apiBaseUrl: 'http://cbmsnet-tk1.ems.schneider-electric.com/GlobalBillPay/services/api/',
  commonApiServiceUrl: "http://cbmsnet-tk1.ems.schneider-electric.com/Commonapi/api/",
  //commonApiServiceUrl: 'http://localhost:52965/api/',
  ViewCostAndUsuageUrl: "http://cbmsnet-tk1.ems.schneider-electric.com/ip2/costusagesite/default.aspx?",
  ViewInvoice: "http://cbmsnet-tk1.ems.schneider-electric.com/ip2/cuinvoice/history1.aspx?",
  ViewAccount: 'http://cbms-tk1.ems.schneider-electric.com/cbms/cis/viewUtilityAccountAction.do?mode=edit',
  ViewAccountSupplier: 'http://cbms-tk1.ems.schneider-electric.com/cbms/contract/supplierAccountConfigurationAction.do?mode=edit',
  EditInvoice: 'http://cbmsnet-tk1.ems.schneider-electric.com/ip2/cuinvoice/editmanualnew.aspx',
  QueueUrl: 'http://cbmsnet-tk1.ems.schneider-electric.com/Queues/app/',
  FetchImageDetails: "http://cbmsnet-tk1.ems.schneider-electric.com/imgserver/InternalImage.aspx?",
  moduleName: "",
};

/*
 * For easier debugging in development mode, you can import the following file
 * to ignore zone related error stack frames such as `zone.run`, `zoneDelegate.invokeTask`.
 *
 * This import should be commented out in production mode because it will have a negative impact
 * on performance if an error is thrown.
 */
// import 'zone.js/dist/zone-error';  // Included with Angular CLI.
