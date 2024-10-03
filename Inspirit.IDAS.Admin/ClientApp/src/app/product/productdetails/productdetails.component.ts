import { Component, OnInit, OnDestroy } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { ActivatedRoute } from '@angular/router';
import { Customer, CustomerService, CustomerCrudResponse, ProductDetails, Service, UserPermission, UserService } from '../../services/services';
import { Product, ProductService, ProductCrudResponse, ProductPackageRate } from '../../services/services';
import { DatePipe } from '@angular/common';
import { EventEmitter } from 'events';
import { isNullOrUndefined } from 'util';
import { forEach } from '@angular/router/src/utils/collection';
import { NgbModal, NgbActiveModal, NgbModalOptions } from '@ng-bootstrap/ng-bootstrap';
import { PopupComponent } from '../../popup/popup.component';
import { PopupService } from '../../popup/popupService';


@Component({
  selector: 'app-productdetails',
  templateUrl: './productdetails.component.html'
})

export class ProductdetailComponent implements OnInit, OnDestroy {
  profileType = 1;
  currentObj: Product = new Product();
  services: Service[];
  public usageType = [];
  productpkg: ProductPackageRate = new ProductPackageRate();
  id: any;
  private sub: any;
  mode: string = "view";
  serviceName: string = "";
  readonly: boolean = false;
  disable: boolean = false;
  isdelete: boolean = false;
  errorMessage: string = '';
  addNew: number = 0;
  userid: any;
  userper: UserPermission = new UserPermission();
  success: boolean;
  loading: boolean = false;
  message: any;


  constructor(public router: Router, private route: ActivatedRoute,
    private productService: ProductService, private datePipe: DatePipe, public userService: UserService, private modalService: NgbModal, public popupservice: PopupService) {

    this.userid = localStorage.getItem('userid');
    this.userService.getPermission(this.userid, "Products").subscribe(result => {
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
    //any change inthe below list will have impact on subcsriptions and invoices
    this.usageType = ["Yearly", "Monthly", "Credits"];
    this.productService.getServices().subscribe((resp) => {
      this.services = resp.services;
    });
    if (typeof this.id == 'undefined' || typeof this.id == null) {
      this.mode = "add";
      this.readonly = false;
      this.productService.productsAdd().subscribe((resp) => {
        this.currentObj = resp;
      });
    }
    else {
      this.mode = "view";
      this.loading = true;
      this.productService.productsView(this.id).subscribe((result) => {
        this.currentObj = result.productlist[0];
        this.loading = false;
        this.readonly = true;
        this.disable = true;
      });
    }
  }
  ngOnDestroy() {
    this.sub.unsubscribe();
  }
  ProductsSave() {
    this.loading = true;
    var date = this.datePipe.transform(new Date(), 'yyyy-MM-dd');
    this.currentObj.activatedDate = new Date(date);
    this.currentObj.createdOn = new Date(date);
    this.currentObj.deactivatedDate = new Date(date);
    this.currentObj.lastUpdatedDate = new Date(date);
    if (isNullOrUndefined(this.currentObj.name) || this.currentObj.name == "") {
      this.message = "Product Name can not be empty.";
      this.loading = false;
      document.getElementById('errormsg').click();
      return;
    }
    else if (isNullOrUndefined(this.currentObj.serviceId) || this.currentObj.serviceId == "00000000-0000-0000-0000-000000000000") {
      this.message = "Select the Service";
      this.loading = false;
      document.getElementById('errormsg').click();
      return;
    }
    else if (isNullOrUndefined(this.currentObj.code) || this.currentObj.code == "") {
      this.message = "Product Code is required";
      this.loading = false;
      document.getElementById('errormsg').click();
      return;
    } else if (this.currentObj.code.length != 3) {
      this.message = "Product Code should be 3 digits";
      this.loading = false;
      document.getElementById('errormsg').click();
      return;
    }
    else if (isNullOrUndefined(this.currentObj.usageType) || this.currentObj.usageType == "") {
      this.message = "Select the Usage Type";
      this.loading = false;
      document.getElementById('errormsg').click();
      return;
    }
    else if (this.mode == "add") {
      this.LimitValidation();
      if (this.errorMessage != '' || this.currentObj.packageRates.length == 0) {
        if (this.currentObj.packageRates.length == 0) {
          this.message = 'Atleast one rate should be add';
          this.loading = false;
          document.getElementById('errormsg').click();
          return;
        }
      }
      else {
        this.productService.productsInsert(this.currentObj).subscribe((result) => {
          this.loading = false;
          if (!result.isSuccess) {
            if (result.message != null && result.message != "undefined")
              this.message = result.message;
            this.loading = false;
            document.getElementById('errormsg').click();
            return;
          }
          this.router.navigate(['productlist']);
        });
      }
    }
    else if (this.mode == "edit") {
      this.LimitValidation();
      if (this.errorMessage != '' || this.currentObj.packageRates.length == 0) {
        if (this.currentObj.packageRates.length == 0) {
          this.loading = false;
          this.errorMessage = 'Atleast one rate should be add';
        }
        return
      }
      else {

        this.productService.productsUpdate(this.currentObj).subscribe((result) => {
          this.loading = false;
          if (!result.isSuccess) {
            
            if (result.message != null && result.message != "undefined") {
              this.message = result.message;
              this.loading = false;
              document.getElementById('errormsg').click();
              return;
            }
          }
          else
            this.router.navigate(['productlist']);
        });
      }
    }
  }
  dash() {
    this.router.navigate(['dashboard']);
  }
  AddPackageRate() {
    this.productpkg = new ProductPackageRate();
    this.productpkg.id = "00000000-0000-0000-0000-00000000000" + this.addNew++;
    this.productpkg.maxLimit = 0;
    this.productpkg.minLimit = 0;
    this.productpkg.unitPrice = 0;
    this.currentObj.packageRates.push(this.productpkg);
  }
  LimitValidation() {
    var minLimit = 0;
    var maxLimit = 0;
    var count = this.currentObj.packageRates.length;
    this.errorMessage = '';
    for (var j = count - 1; j >= 0; j--) {
      var curmin = this.currentObj.packageRates[j].minLimit;
      var curmax = this.currentObj.packageRates[j].maxLimit;
      var prevmax = this.currentObj.packageRates[(j - 1) >= 0 ? (j - 1) : j].maxLimit;
      var unitprice = this.currentObj.packageRates[j].unitPrice;
      if (curmax <= 0) {
        this.errorMessage = 'Maximum limit should be greater than 0' + '<br />';
        this.loading = false;
        return;
      }
      if (unitprice <= 0) {
        this.errorMessage = 'Unit Price should be greater than 0' + '<br />';
        this.loading = false;
        return;
      }
      if ((prevmax >= curmin && j > 0) || (curmin >= curmax)) {
        if ((curmin >= curmax)) {
          this.errorMessage = 'Maximum limit should be greater than current minimum limit' + '<br />';
          this.loading = false;
          return;
        }
        else {
          this.errorMessage = 'Minimum limit should be greater than prev maximum limit' + '<br />';
          this.loading = false;
        }
      }
     
    }
  }
  changeService(id) {
    this.currentObj.serviceId = id;
  }
  changeUsageType(value) {
    this.currentObj.usageType = value;
  }
  edit() {
    this.mode = "edit";
    this.readonly = false;
    this.disable = false;
    this.loading = true;
    this.productService.productsView(this.id).subscribe((result) => {
      this.loading = false;
    });
  }
  delete() {
   
    let ngbModalOptions: NgbModalOptions = {
      backdrop: 'static',
      keyboard: false
    };

    const modalRef = this.modalService.open(PopupComponent, ngbModalOptions);
    modalRef.componentInstance.message = "Are you sure want to delete ?";
    modalRef.componentInstance.isconfirm = true;

    this.popupservice.buttonchange.subscribe((credits = this.success) => {
      this.success = credits;
      if (this.success == true) {
        this.productService.productsDelete(this.id).subscribe((result) => {
          this.router.navigate(['productlist']);
        });
      } else
        return;
    });

  }
  list() {
    this.router.navigate(['productlist']);
  }
  remove(id) {
    let ngbModalOptions: NgbModalOptions = {
      backdrop: 'static',
      keyboard: false
    };


    const modalRef = this.modalService.open(PopupComponent, ngbModalOptions);
    modalRef.componentInstance.message = "Are you sure want to delete ?";
    modalRef.componentInstance.isconfirm = true;

    this.popupservice.buttonchange.subscribe((credits = this.success) => {
      this.success = credits;
      if (this.success == true) {
        this.productService.productPackageRatesRemove(id).subscribe((result) => {

          if (result.length == 0) {
            var productPackageRate = this.currentObj.packageRates.find(_id => _id.id == id);
            var index = this.currentObj.packageRates.indexOf(productPackageRate);
            if (index > -1) {
              this.currentObj.packageRates.splice(index, 1);
            }
          }
          else {
            this.currentObj.packageRates = result;
          }

        });
      } else
        return;
    });

  }
}
