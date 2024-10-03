import { Component, Inject, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router, ActivatedRoute } from '@angular/router';
import {
  SubscriptionService, ProductPackageRate, ProductsVm, Product,
  Customer, SubscriptionVm, SubscriptionResponse, UserService, UserPermission
} from '../services/services';
import { AsideNavService } from '../aside-nav/aside-nav.service';
import { DatePipe } from '@angular/common';
import { NgbModal, NgbActiveModal, NgbModalOptions }
  from '@ng-bootstrap/ng-bootstrap';
import { SearchCustomer } from '../searchcustomer/searchcustomer.component';
import { PopupComponent } from '../popup/popup.component';
import { PopupService } from '../popup/popupService';
import { isNullOrUndefined } from 'util';
import { EventEmitter } from 'events';



@Component({
  selector: 'app-subscriptiondetail',
  templateUrl: './subscriptiondetail.component.html',

})

export class SubscriptionDetailComponent implements OnInit {

  currentObject: SubscriptionVm = new SubscriptionVm();
  type: any;

  readonly: boolean = false;
  sub: any;
  id: any;
  prodname: string;
  customer: Customer = new Customer();
  rates: ProductPackageRate[] = [new ProductPackageRate()];
  public products: ProductsVm[];
  productList: Product[];
  public prod: ProductsVm = new ProductsVm();
  public response: SubscriptionResponse = new SubscriptionResponse();
  public errormsg: any = '';
  public mode: string = " ";
  userid: any;
  userper: UserPermission = new UserPermission();
  loading: boolean = false;

  constructor(public subscriptionService: SubscriptionService,
    public router: Router, public route: ActivatedRoute,
    public datepipe: DatePipe, public userService: UserService,
    public asideNavService: AsideNavService, private modalService: NgbModal,
    public popupservice: PopupService) {


    this.asideNavService.toggle(true);

    this.userid = localStorage.getItem('userid');
    this.userService.getPermission(this.userid, "Subscriptions").subscribe(result => {
      this.userper = result;
      if (this.userper == null || this.userper.viewAction == false) {
        document.getElementById('nopermission').click();
      }
    });


    this.subscriptionService.getData().subscribe((result) => {
      this.products = result;
    });
  }
  ngOnInit(): void {
    let origin = window.location.href;
    if (origin.indexOf('subscriptioncustomer') >= 0) {
      var customerid = localStorage.getItem('customerforsubscription');
      if (!isNullOrUndefined(customerid)) {
        this.subscriptionService.getCustomerById(customerid).subscribe((result) => {
          this.customer = result;
          this.prod.customerId = result.id;
          this.customerchange(result.id);
        });
      }
    }

    this.sub = this.route.params.subscribe(params => {
      this.id = params['id'];
      if (typeof this.id != 'undefined' && typeof this.id != null) {
        this.loading = true;
        this.subscriptionService.getSubscription(this.id).subscribe((result) => {
          this.mode = "view";
          this.loading = false;
          this.currentObject = result;
          this.prod.customerId = this.currentObject.customerId;
          this.prod.billingType = this.currentObject.billingType;
          this.prod.duration = this.currentObject.duration;
          this.prod.name = this.currentObject.productName;
          this.prod.quantity = this.currentObject.quantity;
          this.prod.startDate = this.datepipe.transform(this.currentObject.startDate, 'dd-MM-yyyy');

          this.readonly = true;
          this.subscriptionService.getCustomerById(this.prod.customerId).subscribe((result) => {
            this.customer = result;
          });
          this.customerchange(this.prod.customerId);
          this.prod.id = this.currentObject.productId;
        });
      }
      else {
        this.mode = "add";
        this.readonly = false;
      }
    });
  }
  edit() {
    this.mode = "edit";
    this.loading = true;
    this.subscriptionService.getSubscription(this.id).subscribe((result) => {
      this.currentObject = result;
      this.readonly = false;
      this.loading = false;
    });
  }
  dash() {
    this.router.navigate(['dashboard']);
  }
  customerchange(customerid: any) {
    this.prod.customerId = customerid;
    this.subscriptionService.getProducts(customerid).subscribe((res) => {
      this.productList = res;
      if (this.mode == "add")
        this.prod.id = "00000000-0000-0000-0000-000000000000";
    });
    this.rates = [new ProductPackageRate()];
  }
  submit() {
    this.loading = true;
    if (this.prod.id != null && this.prod.billingType != null && this.prod.quantity != null && this.prod.customerId != null && this.prod.startDate) {
      if (this.mode == "add") {
        if (this.prod.quantity <= 0) {
         
          this.errormsg = "Quantity should be greater than 0";
          this.loading = false;
          document.getElementById('errormsg').click();
          return;
        }
        else if (this.prod.duration <= 0) {
          this.errormsg = "Duration should be greater than 0";
          this.loading = false;
          document.getElementById('errormsg').click();
          return;
        }
        else {
          this.prod.rates = this.rates;
          if (this.prod.rates.filter(t => this.prod.quantity >= t.minLimit && this.prod.quantity <= t.maxLimit && t.isDeleted == 0) == null) {
           
           
            this.errormsg = "Enter the valid Quantity";
            this.loading = false;
            document.getElementById('errormsg').click();
            return;
          }
          else {
            this.subscriptionService.addSubscription(this.prod).subscribe((result) => {
              this.response = result;
              this.loading = false;
              if (this.response.isSuccess)
                this.router.navigate(['subscribtions']);
              else {
                if (this.response.message != null && this.response.message != "undefine") {
                  this.errormsg = this.response.message;
                  this.loading = false;
                  document.getElementById('errormsg').click();
                  return;
                }
              }
            });
          }
        }
      }
      else if (this.mode == "edit") {
        {
          if (this.prod.quantity <= 0) {
            this.errormsg = "Quantity should be greater than 0";
            this.loading = false;
            document.getElementById('errormsg').click();
            return;
          }
          else if (this.prod.duration <= 0) {
            this.errormsg = "Duration should be greater than 0";
            document.getElementById('errormsg').click();
            return;
          }
          else {
            this.prod.rates = this.rates;

            if (this.prod.rates.filter(t => this.prod.quantity >= t.minLimit && this.prod.quantity <= t.maxLimit && t.isDeleted == 0) == null) {
              this.errormsg = "Enter the valid Quantity";
              document.getElementById('errormsg').click();
              return;
            }
          }
        }
      }
    }
    else {
      this.errormsg = "All mandatory fields required";
      this.loading = false;
    }
  }
  onChange(id: any) {
    this.prodname = this.products.find(x => x.id == id).name;
    this.subscriptionService.getProductList(id).subscribe((result) => {
      this.rates = result;
    });
  }
  list() {
    this.router.navigate(['subscribtions']);
  }
  searchcustomer() {    
    const modalRef = this.modalService.open(SearchCustomer, { size: 'lg' });
    modalRef.componentInstance.componentName = "subscriptiondetail";
    this.router.navigate(['subscribtions/subscriptiondetail']);
  }
}
