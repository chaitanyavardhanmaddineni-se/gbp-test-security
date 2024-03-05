import { BrowserModule, Title } from '@angular/platform-browser';
import { NgModule, ModuleWithProviders } from '@angular/core';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { GridModule } from '@progress/kendo-angular-grid';
import { InputsModule } from '@progress/kendo-angular-inputs';
import { DropDownsModule } from '@progress/kendo-angular-dropdowns';
import { DialogsModule } from '@progress/kendo-angular-dialog';
import { DatePickerModule, DateInputsModule } from '@progress/kendo-angular-dateinputs';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { environment } from 'src/environments/environment';
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { PopupModule } from '@progress/kendo-angular-popup';
import { DropDownListModule } from '@progress/kendo-angular-dropdowns';
import { UrlSerializer } from "@angular/router";
import { cleanUrlSerializer } from './cleanUrlSerializer';
import { DatePipe } from '@angular/common';
import { CBMSBaseAppModule } from "se-cbms-base";
import { IEnvironmentsInfo } from "se-cbms-base/lib/Models/environmentsModel.info";
// import { Ng4LoadingSpinnerModule } from "ng4-loading-spinner";
import { BreadcrumbComponent } from './common/breadcrumb/breadcrumb.component'
import { CoreModule, UserResolver } from './core';
import { ToastrModule } from 'ngx-toastr'
import { CommonModule } from '@angular/common';
import { ConfirmationDialogComponent } from './common/confirmation-dialog/confirmation-dialog.component';
import { BillpaymentComponent } from './billpayment/billpayment.component';
import { ChargeSectionComponent } from './charge-section/charge-section.component';
import { AccountSectionComponent } from './account-section/account-section.component';
import { AddCommentDialogComponent } from './common/add-comment-dialog/add-comment-dialog.component';
import { SiblinginvoicesComponent } from './siblinginvoices/siblinginvoices.component';
import { SiblinginvoiceDialogComponent } from './common/siblinginvoice-dialog/siblinginvoice-dialog.component';
import { NumericdecimalDirective } from './check-numeric-decimal-directory.directive';
import { AddMultipleinvoiceDialogComponent } from './common/add-multipleinvoice-dialog/add-multipleinvoice-dialog.component';
import { ViewPayloadJSONDataComponent } from './view-payload-JSON-data/view-payload-JSON-data.component';
import { PaymentProcessingComponent } from './payment-processing/payment-processing.component';
import { NgxSpinnerModule } from "ngx-spinner";

@NgModule({
  declarations: [
    AppComponent,
    BreadcrumbComponent,
    ConfirmationDialogComponent,
    BillpaymentComponent,
    ChargeSectionComponent,
    AccountSectionComponent,
    AddCommentDialogComponent,
    SiblinginvoicesComponent,
    SiblinginvoiceDialogComponent,
    NumericdecimalDirective,
    AddMultipleinvoiceDialogComponent,
    ViewPayloadJSONDataComponent,
    PaymentProcessingComponent
  ],
  imports: [
    CoreModule,
    BrowserModule,
    AppRoutingModule,
    FormsModule,
    ReactiveFormsModule,
    BrowserModule,
    AppRoutingModule,
    DialogsModule,
    InputsModule,
    DropDownsModule,
    GridModule,
    FormsModule,
    ReactiveFormsModule,
    PopupModule,
    DropDownListModule,
    DatePickerModule, DateInputsModule, BrowserAnimationsModule,
    CBMSBaseAppModule.forRoot(<IEnvironmentsInfo>environment),
    CBMSBaseAppModule,
    // Ng4LoadingSpinnerModule,
    NgxSpinnerModule,
    ToastrModule.forRoot(),
    CommonModule
  ],
  entryComponents: [
    ConfirmationDialogComponent,
    AddCommentDialogComponent,
    SiblinginvoiceDialogComponent,
    AddMultipleinvoiceDialogComponent
  ],
  exports: [

  ],
  providers: [DatePipe, UserResolver, { provide: UrlSerializer, useClass: cleanUrlSerializer }, Title], //,{provide: UrlSerializer,useClass: cleanUrlSerializer}
  bootstrap: [AppComponent]
})
export class AppModule { }
