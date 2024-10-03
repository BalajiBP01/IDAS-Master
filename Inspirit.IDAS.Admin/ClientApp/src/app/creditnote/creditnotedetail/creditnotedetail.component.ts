import { Component, OnInit, OnDestroy } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { ActivatedRoute } from '@angular/router';
import { debug, isNullOrUndefined } from 'util';
import { DatePipe } from '@angular/common';
import { Creditnote, InvoiceService, Invoice, SubscriptionResponse, UserPermission, UserService } from '../../services/services';
import { createElementCssSelector } from '@angular/compiler';
import { AsideNavService } from '../../aside-nav/aside-nav.service';
import { NgbModal, NgbActiveModal, NgbModalOptions } from '@ng-bootstrap/ng-bootstrap';
import { PopupComponent } from '../../popup/popup.component';
import { PopupService } from '../../popup/popupService';



@Component({
  selector: 'app-creditnotedetail',
  templateUrl: './creditnotedetail.component.html',

})

export class CreditnotedetailComponent implements OnInit {

  userid: any;
  userper: UserPermission = new UserPermission();
  id: any;
  sub: any;
  currentobject: Creditnote = new Creditnote();
  errorMessageValue: any;
  errorMessage: any;
  notenum: any;
  notevalue: any;
  invoiceAmount: any = 0;
  paymentAmount: any = 0;
  comments: string;
  paymenttype: string = "";
  response: SubscriptionResponse = new SubscriptionResponse();

  constructor(public router: Router, private route: ActivatedRoute, public invoiceService: InvoiceService, public userService: UserService, public asideNavService: AsideNavService, private modalService: NgbModal, public popupservice: PopupService) {
    this.asideNavService.toggle(true);

    this.userid = localStorage.getItem('userid');
    this.userService.getPermission(this.userid, "Invoice").subscribe(result => {
      this.userper = result;
      if (this.userper == null || this.userper.viewAction == false) {
        let ngbModalOptions: NgbModalOptions = {
          backdrop: 'static',
          keyboard: false
        };

        const modalRef = this.modalService.open(PopupComponent, ngbModalOptions);
        modalRef.componentInstance.message = "You don't have permission to access.";
        modalRef.componentInstance.isconfirm = false;

        this.popupservice.buttonchange.subscribe((credits) => {
          this.router.navigate(['dashboard']);
        });
      }
    });
  }
  onKey(e: any) {
    var str = e.target.value;
    var charCode = (e.which) ? e.which : e.keyCode;
    if (charCode == 8) return true;

    var keynum;
    var keychar;
    var charcheck = /[0-9-.]/;
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
    this.sub = this.route.params.subscribe(params => {
      let arrays: string[] = params['id'].split(',');
      this.id = arrays[0];
      this.invoiceAmount = arrays[1];
      if (arrays.length > 2)
        this.paymentAmount = (arrays[2] == "null" || arrays[2] == "") ? 0 : arrays[2];
    });
  }
  list() {
    this.router.navigate(['invoicelist']);
  }
  setTwoNumberDecimal($event) {
    $event.target.value = parseFloat($event.target.value).toFixed(2);
  }
  save() {
    this.errorMessageValue = "";
    var finalInvoiceAmount =(+this.invoiceAmount - +this.paymentAmount);
    if ((isNullOrUndefined(this.currentobject.comments) || this.currentobject.comments == "")  )
      this.errorMessageValue = "Comments is required";

    if ((isNullOrUndefined(this.currentobject.creditNoteValue) || this.currentobject.creditNoteValue.toString() == ""))
      this.errorMessageValue = "Credit Note Value is required";

    else if (+this.currentobject.creditNoteValue > +finalInvoiceAmount)
      this.errorMessageValue = "Credit Note Value should not be greater than" + " " + finalInvoiceAmount;

    if (this.errorMessageValue != "") {
      let ngbModalOptions: NgbModalOptions = {
        backdrop: 'static',
        keyboard: false
      };
      const modalRef = this.modalService.open(PopupComponent, ngbModalOptions);
      modalRef.componentInstance.message = this.errorMessageValue;
      modalRef.componentInstance.isconfirm = false;       
    }
    else {
      this.currentobject.invoiceId = this.id;
      this.notevalue = this.currentobject.creditNoteValue;
      this.comments = this.currentobject.comments;
      this.invoiceService.createCreditNote(this.currentobject).subscribe((result) => {
        this.response = result;
        let ngbModalOptions: NgbModalOptions = {
          backdrop: 'static',
          keyboard: false
        };
        const modalRef = this.modalService.open(PopupComponent, ngbModalOptions);
        modalRef.componentInstance.message = this.response.message;
        modalRef.componentInstance.isconfirm = this.response.isSuccess;
        if (this.response.isSuccess)
          this.popupservice.buttonchange.subscribe((credits) => {
            this.router.navigate(['invoicelist']);
          });
      });
    }
    
  }
}




