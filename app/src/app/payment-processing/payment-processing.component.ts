import { Component, Input, OnChanges, OnInit, SimpleChanges } from '@angular/core';
import { PaymentProcessModel } from '../core/models/datamodel';

@Component({
  selector: 'app-payment-processing',
  templateUrl: './payment-processing.component.html',
  styleUrls: ['./payment-processing.component.scss']
})

export class PaymentProcessingComponent implements OnInit, OnChanges {
  @Input() paymentprocessinggridData: any;
  public paymentProcessingData :any;
  public processInstructions :any;
  public accountNumber : any;
  public lastUpdate :any;
  public updatedUser : any;
  public instructionCategory : any;
  public processobj:PaymentProcessModel = new PaymentProcessModel();
  records : boolean = false;
  groupedPaymentData: unknown[];
  constructor() {     
    }

  ngOnInit() {
    
  }
  ngOnChanges() {
    this.paymentProcessingData = this.paymentprocessinggridData;
    if (this.paymentProcessingData != null && this.paymentProcessingData.length > 0) {
        const accountData = {};
        this.paymentProcessingData.forEach(element => {
            if (element.instructionCategory === 'Payment' && element.isActive === true) {
                const accountKey = element.accountNumber;
                if (!accountData[accountKey]) {
                    accountData[accountKey] = {
                        accountNumber: element.accountNumber,
                        record: {
                            processInstructions: element.processInstructions,
                            lastUpdate: new Date(element.lastUpdate),
                            updatedUser: element.updatedUser
                        }
                    };
                }
            }
        });
        this.groupedPaymentData = Object.values(accountData);
    }
  }
}
