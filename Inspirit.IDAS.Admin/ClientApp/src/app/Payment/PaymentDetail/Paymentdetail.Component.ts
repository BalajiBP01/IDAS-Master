import { Component, OnInit, OnDestroy } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { ActivatedRoute } from '@angular/router';
import { debug, isNullOrUndefined } from 'util';
import { DatePipe } from '@angular/common';
import { Payment, PaymentService, CrudResponsePayment, SubscriptionResponse, InvoiceService, Invoice, UserService, UserPermission, LookupData } from '../../services/services';
import { AsideNavService } from '../../aside-nav/aside-nav.service';
import { createElementCssSelector } from '@angular/compiler';
import { NgbModal, NgbActiveModal, NgbModalOptions } from '@ng-bootstrap/ng-bootstrap';
import { PopupComponent } from '../../popup/popup.component';
import { PopupService } from '../../popup/popupService';
import { debounce } from 'rxjs/operators';


@Component({
  selector: 'app-paymentdetail',
  templateUrl: './paymentdetail.component.html'
})

export class PaymentdetailComponent {
  id: any;
  sub: any;
  readonly: boolean = false;
  currentobject: Payment = new Payment();
  errorMessageValue: any;
  lookupValues: LookupData[];
  errorMessage: any;
  refnum: string;
  comments: string;
  paymenttype: string = "";
  type: any;
  PaymentAmountReceive: any;
  mode: any;
  date: any;
  paymentReceivedDatestring: any;
  amount: any;
  response: SubscriptionResponse = new SubscriptionResponse();
  success: boolean;
  paymentnumber: any;
  loading: boolean = false;

  constructor(public router: Router, private route: ActivatedRoute, public invoiceService: InvoiceService, public paymentService: PaymentService, private datepipe: DatePipe, private modalService: NgbModal, public popupservice: PopupService) {
  }
  onKey(e: any) {
    var str = e.target.value;
    var charCode = (e.which) ? e.which : e.keyCode;
    if (charCode == 8) return true;

    var keynum;
    var keychar;
    var charcheck = /[0-9.]/;
    if (window.event) // IE
    {
      keynum = e.keyCode;
    }
    else {
      if (e.which) // Netscape/Firefox/Opera
      {
        keynum = e.which;
      }
      else return true;
    }

    keychar = String.fromCharCode(keynum);
    return charcheck.test(keychar);
    
  }
  ngOnInit(): void {
    this.paymentService.getPaymentLookupvalues().subscribe((resp) => {
      this.lookupValues = resp;
    });
    this.sub = this.route.params.subscribe(params => {
      let arrays: string[] = params['id'].split(',');
      this.id = arrays[0];
      this.mode = params['type'];
      if (arrays.length > 1)
        this.amount = arrays[1];
    });
    if (this.mode == "view") {
      this.readonly = true;
      this.loading = true;
      this.paymentService.view(this.id).subscribe((result) => {
        this.currentobject = result;
        this.loading = false;
        this.paymentnumber= this.currentobject.invoice.invoiceDisplayNumber.replace("INV", "PMT");
        this.date = this.datepipe.transform(this.currentobject.date, 'dd-MM-yyyy');
      });
    }
    else {
      this.paymentReceivedDatestring = this.datepipe.transform(new Date(), 'yyyy-MM-dd');
    }
  }
  list() {
    this.router.navigate(['paymentlist']);
  }
  save(paymentdate: any) {
    this.loading = true;
    this.errorMessage = "";
    this.currentobject.paymentReceivedDate = new Date(paymentdate);
    if ((isNullOrUndefined(this.currentobject.comments) || this.currentobject.comments == "") || (isNullOrUndefined(this.currentobject.paymentAmountReceive) || this.currentobject.paymentAmountReceive.toString() == "")
      || (isNullOrUndefined(this.currentobject.reference) || this.currentobject.reference == "")
      || (isNullOrUndefined(this.currentobject.paymentType))) {
      this.errorMessage = "All fields are required";
    }
    else if (+this.currentobject.paymentAmountReceive > +this.amount) {
      this.errorMessage = "Amount received connot be greater than Invoice value : " + this.amount;
    }
   
    
  
    if (this.errorMessage != "") {
      this.loading = false;
      document.getElementById('errormsg').click();
      return;
     
    }
     else {
console.log(this.id);
console.log(this.currentobject.paymentType);

        this.invoiceService.createPayment(this.id, this.currentobject.paymentType,
          this.currentobject.reference, this.currentobject.comments,
          this.currentobject.paymentAmountReceive,
          this.currentobject.paymentReceivedDate).subscribe((result) => {
            this.response = result;
            this.loading = false;
            if (this.response.isSuccess == false) {
              if (this.response.message != null && this.response.message != "undfined") {
                this.loading = false;
                this.errorMessage = this.response.message;
                document.getElementById('errormsg').click();
              }
            }
            else {
              document.getElementById('paymentdone').click();
              
            }
          });
      }
  }
  ngOnDestroy() {
    this.sub.unsubscribe();
  }
  setTwoNumberDecimal($event) {
    $event.target.value = parseFloat($event.target.value).toFixed(2);
  }
}




