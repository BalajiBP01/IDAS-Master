import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router } from '@angular/router';
import { ActivatedRoute } from '@angular/router';
import {
  InvoiceService, UserService, UserPermission,
  InvoiceGenerationVm, InvoiceCrudResponses,
  TaxInvoiceReport, Customer, Invoice, CustomerVModel,
  ProductVm, ProductPackageRateVm,
  InvoiceLineItem, EmailProperty
} from '../../services/services';
import * as jsPDF from 'jspdf';
import * as html2canvas from 'html2canvas';
import { createElementCssSelector } from '@angular/compiler';
import { Guid } from "guid-typescript";
import { isNullOrUndefined } from 'util';
import { DatePipe } from '@angular/common'
import { AsideNavService } from '../../aside-nav/aside-nav.service';
import { NgbModal, NgbModalOptions } from '@ng-bootstrap/ng-bootstrap';
import { PopupComponent } from '../../popup/popup.component';
import { PopupService } from '../../popup/popupService';



@Component({
  selector: 'app-invoicedetail',
  templateUrl: './invoicedetail.component.html'
})

export class InvoiceDetailComponent implements OnInit, OnDestroy {
  currentObj: Invoice = new Invoice();
  id: any;
  private sub: any;
  mode: string = "view";
  custom: any;
  message: string = "";
  customerlistvm: CustomerVModel[];
  temp: CustomerVModel[];
  productList: ProductVm[];
  customerlist: CustomerVModel[];
  reponse: InvoiceCrudResponses;
  productRates: ProductPackageRateVm[];
  selectedIndex: number;
  rate: ProductPackageRateVm;
  rates: ProductPackageRateVm[] = [];
  lineitem: InvoiceLineItem;
  lineitems: InvoiceLineItem[];
  readonly: boolean = false;
  years: number[] = [];
  year: number;
  emailPattern: any;
  emailProperty: EmailProperty = new EmailProperty();
  customer: Customer = new Customer();
  taxInvoiceReport: TaxInvoiceReport = new TaxInvoiceReport();
  request: InvoiceGenerationVm = new InvoiceGenerationVm();
  isSelected: boolean;
  amt: any;
  rateID: any;
  preyear: any;
  isSave: boolean;
  disPrice: number;
  discount: number;
  invDate: string;
  selectcompany: boolean;
  userid: any;
  userper: UserPermission;
  vatper: any;
  settingval: any;
  usermsg: any = "";
  nxtyear: any;
  custdisplay: boolean = false;
  success: boolean;
  tradingname: any;
  loading: boolean = false;

  constructor(public router: Router, private route: ActivatedRoute, private invoiceservice: InvoiceService, public datePipe: DatePipe, public userService: UserService, public asideNavService: AsideNavService,
    private modalService: NgbModal, public popupservice: PopupService) {

    this.asideNavService.toggle(true);

    this.userid = localStorage.getItem('userid');
    this.userService.getPermission(this.userid, "Invoice").subscribe(result => {
      this.userper = result;
      if (this.userper == null || this.userper.viewAction == false) {
        document.getElementById('nopermission').click();
      }
    });
  }
  ngOnInit(): void {

    this.invoiceservice.getValtAmount().subscribe(result => {
      this.vatper = result;
    });

    this.year = new Date().getFullYear();
    this.preyear = this.year - 1;
    this.nxtyear = this.year + 1;

    this.invoiceservice.getCustlist().subscribe((result) => {
      this.customerlistvm = result;
    });
    this.invoiceservice.getProducts().subscribe((result) => {
      this.productList = result;
    });
    this.currentObj = new Invoice();
    this.sub = this.route.params.subscribe(params => {
      this.id = params['id'];
    });
    if (typeof this.id == 'undefined' || typeof this.id == null) {
      this.mode = "add"
      this.readonly = false;
    }
    else {
      this.mode = "view";
      this.loading = true;
      this.invoiceservice.view(this.id).subscribe((result) => {
        this.currentObj = result.invoice;
        this.taxInvoiceReport = result;
        this.loading = false;
        this.emailProperty.toMail = this.taxInvoiceReport.email;
        this.tradingname = this.currentObj.customer.tradingName;
        this.currentObj.customer = null;
        this.invDate = this.datePipe.transform(this.currentObj.invoiceDate, 'MMMM-yyyy');
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
      });
    }
  }
  setTwoNumberDecimal($event) {
    $event.target.value = parseFloat($event.target.value).toFixed(2);
  }
  sendEmail(value: boolean) {
    this.loading = true;
    this.emailPattern = /^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,10})+$/;
    if (!this.emailProperty.toMail.match(this.emailPattern)
      || isNullOrUndefined(this.emailProperty.toMail)
      || this.emailProperty.toMail == "") {
      this.message = 'Invalid Email';
      this.loading = false;
      document.getElementById('errormsg').click();
      return;
    }
    this.loading = true;
    var Invoice = document.getElementById('TaxInvoice');
    html2canvas(Invoice, {
      onclone: function (document) {
        document.querySelector("#TaxInvoice").style.display = "block";
      }
    }).then(canvas => {
      this.emailProperty.id = this.id;
      let filetype: string = value ? "TaxInvoice" : "ProformaInvoice";
      this.emailProperty.fileName = filetype + this.currentObj.invoiceDisplayNumber + ".pdf";
      this.emailProperty.base64 = canvas.toDataURL().split(',')[1];
      this.emailProperty.istaxinv = value;
      this.emailProperty.ispaid = value;
      this.invoiceservice.getPDFFile(this.emailProperty).subscribe((r) => {
        this.loading = false;
        this.message = r;
        document.getElementById('errormsg').click();
        return;
      });
    });
  }
  downloadPdf() {
    this.loading = true;
    var ProformaInvoice = document.getElementById('TaxInvoice');
    html2canvas(ProformaInvoice, {
      onclone: function (document) {
        document.querySelector("#TaxInvoice").style.display = "block";
      }
    }).then(canvas => {
      var imgWidth = 208;
      var pageHeight = 295;
      var imgHeight = (canvas.height) * imgWidth / (canvas.width);
      var heightLeft = imgHeight;
      const contentDataURL = canvas.toDataURL('image/png');
      let pdf = new jsPDF('p', 'mm');
      var position = 0;
      pdf.addImage(contentDataURL, 'PNG', 0, position, imgWidth, imgHeight);
      heightLeft -= pageHeight;
      while (heightLeft >= 0) {
        position = heightLeft - imgHeight;
        pdf.addPage();
        pdf.addImage(contentDataURL, 'PNG', 0, position, imgWidth, imgHeight);
        heightLeft -= pageHeight;
      }
      pdf.save('TaxInvoice.pdf');
      this.loading = false;
    });
  }
  submityear(stYear: any, stmonth: any) {
    this.message = "";
    this.loading = true;
    if (isNullOrUndefined(this.request.startMonth)) {
      if (this.message != "")
        this.message += ",";
      this.message += "Please select month";
    }
    if (isNullOrUndefined(this.request.startYear)) {
      if (this.message != "")
        this.message += ",";
      this.message += "Please select year";
    }
    if (this.message != "") {
      this.loading = false;
      document.getElementById('errormsg').click();
      return;
    }
    this.invoiceservice.getValidCustomerlist(stYear, stmonth).subscribe(result => {
      this.customerlist = result;
      this.loading = false;
      if (this.customerlist == null || this.customerlist.length == 0) {
        this.loading = false;
        this.usermsg = "No Customers Found.";
      }
      else {
        this.custdisplay = true;
      }
    });
  }
  periodchange() {
    this.usermsg = "";
    this.custdisplay = false;
  }
  selectall(value: boolean) {
    if (value == true) {
      this.selectcompany = true;
      this.customerlist.forEach(obj => {
        obj.isSelected = true;
      });
    } else if (value == false) {
      this.selectcompany = false;
      this.customerlist.forEach(obj => {
        obj.isSelected = false;
      });
    }

  }
  comselect(isselect: boolean) {
    if (isselect == false) {
      this.selectcompany = false;
    }
  }
  updateFilter(val: string) {
    if (this.temp == null)
      this.temp = this.customerlistvm;
    this.customerlist = this.temp;
    if (val == "undefined" || val == null || val.trim() == "") {
      if (this.temp != null)
        this.customerlist = this.temp;
    } else {
      this.customerlist = this.customerlistvm.filter(m => m.customerName.toLowerCase().includes(val.trim()) == true || m.registrationNumber.toLowerCase().includes(val.trim()) == true);
    }
  }
  processUnselected() {
    this.loading = true;
    this.message = "";
    this.request.customers = this.customerlist.filter(x => x.isSelected == false);
    if (isNullOrUndefined(this.request.startMonth)) {
      if (this.message != "")
        this.message += ",";
      this.message += "Please select month";
    }
    if (isNullOrUndefined(this.request.startYear)) {
      if (this.message != "")
        this.message += ",";
      this.message += "Please select year";
    }
    if (this.request.customers.length == 0) {
      if (this.message != "")
        this.message += ",";
      this.message += "Atleast should be select an one customer";
    }
    if (this.message != "") {
      this.loading = false;
      document.getElementById('errormsg').click();
      return;
    }
    this.invoiceservice.monthlyInvoice(this.request).subscribe((result) => {
      let msg = result;
      this.loading = false;
      //alert(msg);
      //this.router.navigate(['invoicelist']);
      if (msg != null && msg != "undefined") {
        this.message = msg;
        this.loading = false;
        document.getElementById('list').click();
      }
    });
  }
  peocessSelected() {
    this.loading = true;
    this.message = "";
    this.request.customers = this.customerlist.filter(x => x.isSelected == true);
    if (isNullOrUndefined(this.request.startMonth)) {
      if (this.message != "")
        this.message += ",";
      this.message += "Please select month";
    }
    if (isNullOrUndefined(this.request.startYear)) {
      if (this.message != "")
        this.message += ",";
      this.message += "Please select year";
    }
    if (this.request.customers.length == 0) {
      if (this.message != "")
        this.message += ",";
      this.message += "Atleast should be select an one customer";
    }
    if (this.message != "") {
      this.loading = false;
      document.getElementById('errormsg').click();
      return;
    }
    this.invoiceservice.monthlyInvoice(this.request).subscribe((result) => {
      let msg = result;
      this.loading = false;
      if (msg != null && msg != "undefined") {
        this.message = msg;
        this.loading = false;
        document.getElementById('list').click();
      }
    });
  }
  processAll() {
    this.loading = true;
    this.message = "";
    if (isNullOrUndefined(this.request.startMonth)) {
      if (this.message != "")
        this.message += ",";
      this.message += "Please select month";
    }
    if (isNullOrUndefined(this.request.startYear)) {
      if (this.message != "")
        this.message += ",";
      this.message += "Please select year";
    }
    if (this.message != "") {
      document.getElementById('errormsg').click();
      return;
    }
    this.request.customers = this.customerlist;
    this.invoiceservice.monthlyInvoice(this.request).subscribe((result) => {
      this.loading = false;
      let msg = result;
      if (msg != null && msg != "undefined") {
        this.message = msg;
        this.loading = false;
        document.getElementById('list').click();
      }
    });
    return;
  }
  submit() {
    this.loading = true;
    this.currentObj.customer = null;
    this.currentObj.isSubmited = true;
    this.invoiceservice.update(this.currentObj).subscribe((result) => {
      this.reponse = result;
      this.loading = false;
      this.router.navigate(['invoicelist']);
    });
  }
  ngOnDestroy() {
    this.sub.unsubscribe();
  }
  onChange(id: any, i: number) {
    this.selectedIndex = i;
    this.invoiceservice.getProductRates(id).subscribe((result) => {
      this.productRates = result;
    });
  }
  quantityChange(uprice: any, i: number) {
    this.currentObj.invoiceLineItems[i].unitPrice = +uprice;
    this.currentObj.invoiceLineItems[i].netAmount = (this.currentObj.invoiceLineItems[i].unitPrice * this.currentObj.invoiceLineItems[i].quantity);
    this.selectedIndex = i;
    this.updateamount();
  }
  countChange(discountupdated: number) {
    if (discountupdated <= 100) {
      this.discount = +discountupdated;
      this.disPrice = this.currentObj.subTotal * (discountupdated / 100);
      this.settingval = this.vatper;
      this.updateamount();
    } else {
      this.message = "Discount should not be greater than 100%."
      document.getElementById('errormsg').click();
    }
  }
  add() {
    this.lineitem = new InvoiceLineItem();
    this.lineitem.id = Guid.create().toString();
    this.currentObj.invoiceLineItems.push(this.lineitem);
  }
  save() {
    this.loading = true;
    if (this.currentObj.discount <= 100) {
      this.isSave = true;
      if (this.mode == "edit") {
        {
          this.currentObj.customer = null;
          this.invoiceservice.update(this.currentObj).subscribe((result) => {
            this.reponse = result;
            this.loading = false;
            if (this.reponse.isSuccess) {
              this.message = "Updated Successfully";
              this.loading = false;
              document.getElementById('errormsg').click();
              return;
            }
            else {
              if (this.reponse.message != null && this.reponse.message != "undefined") {
                this.message = this.reponse.message;
                this.loading = false;
                this.router.navigate(['invoicelist']);
              }
            }
          });
        }
      }
      else {
        if (this.mode == "add") {
          this.currentObj.customer = null;
          this.invoiceservice.insert(this.currentObj).subscribe((result) => {
            this.reponse = result;
            if (this.reponse.isSuccess) {
              //alert(this.reponse.message);
              if (this.reponse.message != null && this.reponse.message != "undefined") {
                this.message = this.reponse.message;
                document.getElementById('list').click();
              }
            }
            else {
              if (this.reponse.message != null && this.reponse.message != "undefined") {
                this.message = this.reponse.message;
                document.getElementById('errormsg').click();
                return;
              }
            }
          });

        }
        else {
          if (this.currentObj.id != null) {
            this.currentObj.customer = null;
            this.invoiceservice.insert(this.currentObj).subscribe((result) => {
              this.reponse = result;
              if (this.reponse.isSuccess)
                this.router.navigate(['invoicelist']);
              else {
                //alert(this.reponse.message);
                if (this.reponse.message != null && this.reponse.message != "undefined") {
                  this.message = this.reponse.message;
                  document.getElementById('errormsg').click();
                  return;
                }
              }
            });
          }
        }
      }
    }
    else {
      this.message = "Discount should not be greater than 100%."
      document.getElementById('errormsg').click();
      return;
    }

  }
  updateamount() {
    this.settingval = this.vatper;
    this.currentObj.subTotal = 0;
    this.currentObj.invoiceLineItems.forEach(obj => {
      this.currentObj.subTotal += +(obj.netAmount).toFixed(2);
    });
    if (this.discount == 0 || this.discount == null) {
      this.discount = 0;
    }

    this.disPrice = this.currentObj.subTotal * (this.discount / 100);
    this.currentObj.vatTotal = +(((this.currentObj.subTotal - this.disPrice) * this.settingval / 100)).toFixed(2);
    this.currentObj.total = +((this.currentObj.subTotal - this.disPrice) + this.currentObj.vatTotal).toFixed(2);
  }
  edit() {
    this.mode = "edit";
    this.readonly = false;
    this.loading = true;
    this.invoiceservice.view(this.id).subscribe((result) => {
      this.currentObj = result.invoice;
      this.loading = false;
    });
  }
  delete() {
    this.invoiceservice.delete(this.id).subscribe((result) => {
      this.router.navigate(['invoicelist']);
    });
  }
  cancel() {
    this.loading = true;
    this.invoiceservice.cancel(this.id).subscribe((result) => {
      this.loading = false;
      this.router.navigate(['invoicelist']);
    });
  }
  list() {
    this.router.navigate(['invoicelist']);
  }
  dash() {
    this.router.navigate(['dashboard']);
  }
}

