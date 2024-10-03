//import { Component, OnInit, OnDestroy } from '@angular/core';
//import { HttpClient } from '@angular/common/http';
//import { Router } from '@angular/router';
//import { ActivatedRoute } from '@angular/router';
////import { Customer, CustomerService, CustomerCrudResponse } from '../../services/services';
//import { ProductPackageRate, ProductRateService, ProductRateCrudResponse, ProductRateResponse } from '../../services/services';


//@Component({
//  selector: 'app-productratesdetails',
//  templateUrl: './productratesdetails.component.html',

//})

//export class ProductRatedetailComponent implements OnInit, OnDestroy {

//  currentObj: ProductPackageRate = new ProductPackageRate();
//  //productPackageRates: ProductDetails = new ProductDetails();
//  id: any;
//  public table_result: any;
//  public lists: boolean;
//  private sub: any;
//  mode: string = "view";
//  reponse: ProductRateCrudResponse;
//  readonly: boolean = false;
//  showStatusUpdate: boolean = false;
//  statusOption: string;
//  constructor(public router: Router, private route: ActivatedRoute, private productRateService: ProductRateService) {
//  }
//  ngOnInit(): void {
//    this.sub = this.route.params.subscribe(params => {
//      this.id = params['id'];
//    });
//    if (typeof this.id == 'undefined' || typeof this.id == null) {
//      this.mode = "add";
//      this.currentObj = new ProductPackageRate();
//      this.readonly = false;
//    }
//    else {
//      debugger;
//      this.mode = "view";
//      this.productRateService.view(this.id).subscribe((result) => {
//        this.currentObj = result;
//        //console.log(this.productRateService);
//        //this.readonly = true;
//      });
//    }
//  }
//  ngOnDestroy() {
//    this.sub.unsubscribe();
//  }
//  save() {
//    debugger;
//    if (this.mode == "add") {
//      this.productRateService.productRateInsert(this.currentObj).subscribe((result) => {
//        this.reponse = result;
//      });

//    }
//    else if (this.mode == "edit") {
//      {
//        this.productRateService.productRateUpdate(this.currentObj).subscribe((result) => {
//          this.reponse = result;
//        });
//        this.router.navigate(['productrateslist']);
//      }

//    }
//  }
//  //edit() {
//  //  this.mode = "edit";
//  //  this.readonly = false;
//  //  this.productRateService.productRateUpdate(this.id).subscribe((result) => {
//  //   this.currentObj = result;
//  //  });
//  //}
//  delete() {
//    this.productRateService.productRateDelete(this.id).subscribe((result) => {
//      this.router.navigate(['productrateslist']);
//    });
//  }
//  list() {

//    this.router.navigate(['productrateslist']);
//  }
//  //changeStatus() {
//  //  if (this.currentObj.status == "Pending") {
//  //    this.currentObj.status = "Active"
//  //  }
//  //  else
//  //    if (this.currentObj.status == "Active") {
//  //      this.currentObj.status = "Inactive"
//  //    }
//  //    else
//  //      if (this.currentObj.status == "Inactive") {
//  //        this.currentObj.status = "Active"
//  //      }

//  //  this.customerService.updateStatus(this.currentObj).subscribe((result) => {
//  //    this.reponse = result;
//  //  });
//  //  this.router.navigate(['customerlist']);

//  //}


//}





