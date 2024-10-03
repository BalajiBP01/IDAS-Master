import { Component, OnInit, OnDestroy } from '@angular/core';
import { HttpClient, HttpRequest, HttpEventType, HttpResponse } from '@angular/common/http';
import { Router } from '@angular/router';
import { ActivatedRoute } from '@angular/router';
import { debug, isNullOrUndefined } from 'util';
import {
  CustomerProdcutVm, Product,
  CustomerProductProperty, CustomerService, CustomerCrudResponse , UserService, UserPermission
} from '../../services/services';
import { AsideNavService } from '../../aside-nav/aside-nav.service';
import { createElementCssSelector } from '@angular/compiler';

@Component({
  selector: 'app-customerproduct',
  templateUrl: './customerproduct.component.html'
})

export class customerProductComponent implements OnInit, OnDestroy {
  id: any;
  errorMessageValue: any = "";
  sub: any;
  mode: string;
  productList: Product[];
  companyname: string = "";
  reponse: CustomerCrudResponse;
  readonly: boolean = false;
  showStatusUpdate: boolean = false;
  statusOption: string;
  addNew: number = 0;
  hasFile: any;
  currentObj: CustomerProdcutVm = new CustomerProdcutVm();
  currentObjProperty: CustomerProductProperty = new CustomerProductProperty();
  userid: any;
  userper: UserPermission = new UserPermission();


  constructor(public router: Router, private route: ActivatedRoute,
      private customerService: CustomerService, private http: HttpClient, public userService: UserService, public asideNavService: AsideNavService) {
      this.asideNavService.toggle(true);

      this.userid = localStorage.getItem('userid');
      this.userService.getPermission(this.userid, "Customers").subscribe(result => {
          this.userper = result;
          if (this.userper == null || this.userper.viewAction == false) {
            document.getElementById('nopermission').click();
          }
      });

  }
  ngOnInit(): void {
    this.sub = this.route.params.subscribe(params => {
      this.id = params['id'];
    });
    this.customerService.getProducts().subscribe((result) => {
      this.productList = result;
    });
    this.customerService.viewCustomerProdcut(this.id).subscribe(result => {
      this.currentObj = result;
      if (isNullOrUndefined(this.currentObj.customerId)
        || this.currentObj.customerId == "00000000-0000-0000-0000-000000000000") {
        this.mode = "add";
        this.currentObj.customerId = this.id;
      }
      else {
        this.mode = "view";
      }
    });
    this.customerService.getName(this.id).subscribe(result => {
      this.companyname = result;
    });

  }
  AddProduct() {
    this.currentObjProperty = new CustomerProductProperty();
    this.currentObjProperty.id = "00000000-0000-0000-0000-00000000000" + this.addNew++;
    this.currentObj.customerProductProperties.push(this.currentObjProperty);
  }
  save() {
    this.validation();
    if (this.currentObj.customerProductProperties.length == 0) {
      this.errorMessageValue = "Atleast one product should be add";
      document.getElementById('error').click();
      return;
    }
    if (this.errorMessageValue != "") {
      this.errorMessageValue = "Atleast one product should be add";
      document.getElementById('error').click();
      return;
    }
    if (this.mode == "add") {
      this.customerService.insertCustomerProduct(this.currentObj).subscribe((res) => {
        if (res.message != "") {
          this.errorMessageValue = res.message;
          document.getElementById('error').click();
          return;
        }
        this.router.navigate(['customerlist']);
      });
    }
    else if (this.mode == "view") {
      this.customerService.updateCustomerProduct(this.currentObj).subscribe((res) => {
        if (res.message != "") {
          this.errorMessageValue = res.message;
          document.getElementById('error').click();
          return;
        }
        this.router.navigate(['customerlist']);
      });
    }
  }
  list() {
    this.router.navigate(['customerlist']);
  }
  validation() {
    this.errorMessageValue = "";
    var length = this.currentObj.customerProductProperties.length;   
    for (var i = 0; i < length; i++) {
      var productid = this.currentObj.customerProductProperties[i].productId;
      if (isNullOrUndefined(productid) || productid == "00000000-0000-0000-0000-000000000000") {
        this.errorMessageValue = "Product Id should not be an empty";
        break;
      }
    }
  }
  ngOnDestroy() {
    this.sub.unsubscribe();
  }
  dash() {
    this.router.navigate(['dashboard']);
  }
}





