import { Directive, ElementRef, HostListener, Input } from "@angular/core";
import { BillpaydataService } from "./core/services/billpaydata.service";

@Directive(
  { selector: '[rmNumericdecimal]'})
  export class NumericdecimalDirective { 
    @Input("decimals") decimals: number = 0; 
    @Input("negative") negative: number = 0;
    @Input("fieldControl") control:string; 
    private specialKeys: Array<string> = ['Control','Backspace', 'Tab', 'End', 'Home', 'ArrowLeft', 'ArrowRight', 'Del', 'Delete'];

    private regex: RegExp = new RegExp(/^(-)?\d*\.?\d{0,2}$/g);
    private checkAllowNegative(value: string) { 
      if (this.decimals <= 0) {
        return String(value).match(new RegExp(/^-?\d+$/)); 
      } 
      else { 
        var regExpString = "^-?\\s*((\\d+(\\.\\d{0," + this.decimals + "})?)|((\\d*(\\.\\d{1," + this.decimals + "}))))\\s*$"; 
        return String(value).match(new RegExp(regExpString)); 
      } 
    } 
    private check(value: string) {
      if (this.decimals <= 0) { 
        return String(value).match(new RegExp(/^\d+$/)); 
      } 
      else { 
        var regExpString = "^-?\\s*((\\d+(\\.\\d{0,2})?)|((\\d*(\\.\\d{1,2}))))\\s*$"; 
        return String(value).match(new RegExp(regExpString)); 
      } 
    } 
    private run(oldValue) {
      setTimeout(()=>{ 
        if(this.control=="PaymentAmount"){
          if (this.dataService.getpaymentFormGroup().controls['PaymentAmount'].value != null && this.dataService.getpaymentFormGroup().controls['PaymentAmount'].value != '') {
            this.dataService.getpaymentFormGroup().controls['Re-EnterPaymentAmount'].setErrors(
              this.dataService.getpaymentFormGroup().controls['PaymentAmount'].value == this.dataService.getpaymentFormGroup().controls['Re-EnterPaymentAmount'].value
                ? null
                : { isMatching: false });
          }
        }
        },0);
      }
      constructor(private el: ElementRef ,private dataService:BillpaydataService ) { } 
      @HostListener("keydown", ["$event"]) 
      onKeyDown(event:any) {
        event = event || window.event;  
        var key = event.which || event.keyCode; 
        var ctrl = event.ctrlKey ? event.ctrlKey : ((key === 17)
            ? true : false);
        if (key == 86 && ctrl) {
           return        
        }
        else if (key == 67 && ctrl) {
           return
        }
         if (!(this.specialKeys.indexOf(event.key) !== -1)) {
          let current: string = this.el.nativeElement.value;
          const position = this.el.nativeElement.selectionStart;
          const next: string = [current.slice(0, position), event.key == 'Decimal' ? '.' : event.key, current.slice(position)].join('');
          if(event.key =="-" && current.includes("-")){
            event.preventDefault();
          }
          else if (next && !String(next).match(this.regex)) {
              event.preventDefault();
          }
           }
          this.run(this.el.nativeElement.value);       
        }  

      @HostListener("paste", ["$event"])
      onPaste(event: ClipboardEvent) {
        const clipboardData = event.clipboardData.getData('text');
        if (!Number(+clipboardData)) {
          event.preventDefault();
        }
        else if(!String(clipboardData).match(this.regex) || !String(clipboardData + this.el.nativeElement.value).match(this.regex) || clipboardData==this.el.nativeElement.value) {
          event.preventDefault();
        }    
        this.run(this.el.nativeElement.value);
      }       
  }
