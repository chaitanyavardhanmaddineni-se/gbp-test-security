import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { UserResolver } from './core';
import { BillpaymentComponent } from './billpayment/billpayment.component';
import { SiblinginvoicesComponent } from './siblinginvoices/siblinginvoices.component';
import { ViewPayloadJSONDataComponent } from './view-payload-JSON-data/view-payload-JSON-data.component';
const routes: Routes = [
  { path: '', redirectTo: "BillPay", pathMatch: "full" },
  { path: "BillPay", component: BillpaymentComponent, data: { title: 'Invoice Bill Pay' }, resolve: { items: UserResolver } },
  { path: "BillPay/:InvoiceId", component: BillpaymentComponent, data: { title: 'Invoice Bill Pay' }, resolve: { items: UserResolver } },
  { path: "SiblingInvoice/:InvoiceId", component: SiblinginvoicesComponent, data: { title: 'Sibling Invoice' }, resolve: { items: UserResolver } },
  { path: "ViewPayloadJSONData/:invoiceId", component: ViewPayloadJSONDataComponent, data: { title: 'View Payload JSON' } },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
