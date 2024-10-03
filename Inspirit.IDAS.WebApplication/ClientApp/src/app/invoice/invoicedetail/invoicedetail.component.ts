import { Component, OnInit, OnDestroy } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { ActivatedRoute } from '@angular/router';
import { InvoiceService, InvoiceCrudResponses, Invoice, Customer, ProductVm, InvoiceLineItem, ProductPackageRateVm } from '../../services/services';
import { createElementCssSelector } from '@angular/compiler';
import { headernavService } from '../../header-nav/header-nav.service';




@Component({
  selector: 'app-invoicedetail',
  templateUrl: './invoicedetail.component.html'
})

export class InvoiceDetailComponent {
  dtOptions: DataTables.Settings = {};
  currentObj: Invoice = new Invoice();
  id: any;
  private sub: any;
  mode: string = "view";
  custom: any;
  customerlist: Customer[];
  productList: ProductVm[];
  reponse: InvoiceCrudResponses;
  selectedIndex: number;
  rate: ProductPackageRateVm;
  lineitem: InvoiceLineItem;
  lineitems: InvoiceLineItem[];
  readonly: boolean = false;
  name: any;
  isuseExists: any;
  loading: boolean = false;
  rateID: any;
  rates: ProductPackageRateVm[] = [];
  tradingname: any;
  companyname: any;
  constructor(public router: Router, private route: ActivatedRoute, private invoiceservice: InvoiceService, public headernavService: headernavService) {
  }

  ngOnInit(): void {

    this.loading = true;

    this.isuseExists = localStorage.getItem('userid');
    if (this.isuseExists != null && this.isuseExists != "undefined") {


      this.headernavService.toggle(true);
      this.name = localStorage.getItem('name');
      if (this.name != null && this.name != 'undefined') {
        this.headernavService.updateUserName(this.name);
      }

     
      this.invoiceservice.getProducts().subscribe((result) => {
        this.productList = result;
      });

      this.currentObj = new Invoice();
      this.sub = this.route.queryParams.subscribe(params => {
        this.id = params['id'];
      });

      if (typeof this.id == 'undefined' || typeof this.id == null) {
        this.mode = "add";
        this.readonly = false;
      }
      else {
        this.mode = "view";
        this.invoiceservice.view(this.id).subscribe((result) => {
          this.currentObj = result;
          this.invoiceservice.getCustomers().subscribe((result) => {
            this.customerlist = result;
            this.companyname = this.customerlist.find(t => t.id == this.currentObj.customerId).tradingName;
          });
          this.currentObj.invoiceLineItems.forEach(obj => {
            this.rateID = obj.productPackageRateID;
            if (this.rateID != null && this.rateID != "undefined") {
              this.invoiceservice.productRate(this.rateID).subscribe((result) => {
                this.rate = result;
                this.rates.push(this.rate);
              });
            }
          });
          this.readonly = true;
          this.loading = false;
        });
      }
    } else {
      this.router.navigate(['/login']);
    }
  }


  ngOnDestroy() {
    this.sub.unsubscribe();
  }
 
  quantityChange(id: any, i: number) {

    this.currentObj.invoiceLineItems[i].netAmount = (this.currentObj.invoiceLineItems[i].unitPrice * this.currentObj.invoiceLineItems[i].quantity);
    this.selectedIndex = i;
    
  }
 setTwoNumberDecimal($event) {
    $event.target.value = parseFloat($event.target.value).toFixed(2);
  }
  add() {
  }

  save() {

    if (this.mode == "edit") {
      {
        this.invoiceservice.insert(this.currentObj).subscribe((result) => {
          this.reponse = result;
        });
        this.router.navigate(['invoicelist']);
      }
    }
    else {
      if (this.mode == "add") {
        this.invoiceservice.insert(this.currentObj).subscribe((result) => {
          this.reponse = result;
        });
        this.router.navigate(['invoicelist']);
      }

      else {
        if (this.currentObj.id != null) {
          this.invoiceservice.insert(this.currentObj).subscribe((result) => {
            this.reponse = result;
          });
          this.router.navigate(['invoicelist']);
        }

      }
    }





  }

  edit() {
    this.mode = "edit";
    this.readonly = false;
    this.invoiceservice.view(this.id).subscribe((result) => {
      this.currentObj = result;
    });
  }



  list() {
    this.router.navigate(['invoicelist']);
  }

}

