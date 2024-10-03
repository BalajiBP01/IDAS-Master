import { Component, Inject } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import {
  SecurityService, SearchCustomerRequest, SearchCustomerResponse
} from '../services/services';
import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { EventEmitter } from 'events';
import { isNullOrUndefined } from 'util';
import { docClick } from '@syncfusion/ej2-richtexteditor';

@Component({
  selector: 'searchcustomer',
  templateUrl: './searchcustomer.component.html'
})

export class SearchCustomer {
  public componentName: string = "";
  selectcompanyid: any;
  request: SearchCustomerRequest = new SearchCustomerRequest();
  response: SearchCustomerResponse[];
  message: any;

  constructor(public _myModel: NgbActiveModal,
    public router: Router, private route: ActivatedRoute,
    private securityService: SecurityService,
    private modalService: NgbModal) {

  }
  close() {
    this._myModel.close();
  }
  search() {
    if ((isNullOrUndefined(this.request.customerName) || this.request.customerName == "")
      && (isNullOrUndefined(this.request.customerCode) || this.request.customerCode == "")
      && (isNullOrUndefined(this.request.registrationNumber) || this.request.registrationNumber == "")) {
      this.message = "Enter anyone!!!!";
      document.getElementById('error').click();
      return;
    }
    this.securityService.getSearchCustomers(this.request).subscribe((res) => {
      this.response = res;
    });
  }
  onchange(value) {
    this.selectcompanyid = value;
  }
  submit() {
    if (isNullOrUndefined(this.selectcompanyid)) {
      this.message = "Atleast one customer should be select";
      document.getElementById('error').click();
      return;
    }
    this._myModel.close();
    if (this.componentName == "subscriptionlist") {
      localStorage.setItem('customerforsubscriptionlist', this.selectcompanyid);
      this.router.navigate(['subscribtions/subscriptionlistcustomer']);
    }
    else if (this.componentName == "subscriptiondetail") {
      localStorage.setItem('customerforsubscription', this.selectcompanyid);
      this.router.navigate(['subscribtions/subscriptioncustomer']);
    }
    else if (this.componentName == "invoicelist") {
      localStorage.setItem('customerforinvoice', this.selectcompanyid);
      this.router.navigate(['invoicelist/invoicecustomer']);
    }
    else if (this.componentName == "proformainvoicelist") {
      localStorage.setItem('customerforproformainvoicelist', this.selectcompanyid);
      this.router.navigate(['proformainvoicelist/proformainvoicelistcustomer']);
    }
    else if (this.componentName == "proformainvoicedetail") {
      localStorage.setItem('customerforproformainvoicedetail', this.selectcompanyid);
      this.router.navigate(['proformainvoicelist/proformainvoicedetailcustomer']);
    }
    else if (this.componentName == "paymentlist") {
      localStorage.setItem('customerforpaymentlist', this.selectcompanyid);
      this.router.navigate(['paymentlist/paymentlistcustomer']);
    }
    else if (this.componentName == "BatchtracinglistComponent") {
      localStorage.setItem('customerforbatch', this.selectcompanyid);
      this.router.navigate(['batchtrace/batchtracelistcustomer']);
    }
    else if (this.componentName == "LeadlistComponent") {
      localStorage.setItem('customerforlead', this.selectcompanyid);
      this.router.navigate(['leadlist/leadlistcustomer']);
    }
  }
  changecustmerinputs() {
    this.response = null;
  }
}
