import { Component, OnInit, OnDestroy } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { ActivatedRoute } from '@angular/router';
import {
  ProFormaInvoiceService, ProFormaInvoiceCrudResponses,
  ProFormaInvoice, ProFormaReport, Customer, ProductVmodel,
  ProformaInvoiceLineItem, EmailProperty,
  UserService, UserPermission
} from '../../services/services';
import { SearchCustomer } from '../../searchcustomer/searchcustomer.component';
import { AsideNavService } from '../../aside-nav/aside-nav.service';
import { createElementCssSelector } from '@angular/compiler';
import { Guid } from "guid-typescript";
import { isNullOrUndefined, debug } from 'util';
import { DatePipe } from '@angular/common';
import * as jsPDF from 'jspdf';
import * as html2canvas from 'html2canvas';
import { EventEmitter } from 'events';
import { NgbModal, NgbModalOptions } from '@ng-bootstrap/ng-bootstrap';
import { PopupComponent } from '../../popup/popup.component';
import { PopupService } from '../../popup/popupService';
import { retry, debounce } from 'rxjs/operators';


@Component({
  selector: 'app-proformainvoicedetail',
  templateUrl: './proformainvoicedetail.component.html'
})

export class ProFormaInvoiceDetailComponent implements OnInit, OnDestroy {
  id: any;
  private sub: any;
  datestring: string;
  vatPercent: number = 0.0;
  public errorInfo: string;
  progress: number;
  mode: string = "view";
  emailPattern: any;
  emailProperty: EmailProperty = new EmailProperty();
  Emaillist: EmailProperty[] = [];
  customer: Customer = new Customer();
  customerlist: Customer[];
  productList: ProductVmodel[];
  reponse: ProFormaInvoiceCrudResponses;
  lineitem: ProformaInvoiceLineItem = new ProformaInvoiceLineItem();
  currentObj: ProFormaInvoice = new ProFormaInvoice();
  isreadonly: boolean = false;
  iscustomer: boolean = false;
  errorMessage: string = '';
  addNew: number = 0;
  disable: boolean = false;
  isSave: boolean = false;
  isSubmitted: boolean = false;
  isCancel: boolean = false;
  proformaReport: ProFormaReport = new ProFormaReport();
  userid: any;
  userper: UserPermission = new UserPermission();
  public success: boolean;
  loading: boolean = false;

  constructor(public router: Router, private route: ActivatedRoute,
    private proformainvoiceservice: ProFormaInvoiceService,
    private datePipe: DatePipe, private modalService: NgbModal,
    public userService: UserService, public asideNavService: AsideNavService,
    public popupservice: PopupService) {

    this.asideNavService.toggle(true);

    this.userid = localStorage.getItem('userid');
    this.userService.getPermission(this.userid, "Proforma Invoice").subscribe(result => {
      this.userper = result;
      if (this.userper == null || this.userper.viewAction == false) {
        document.getElementById('nopermission').click();
      }
    });

  }
  ngOnInit(): void {
    this.proformainvoiceservice.getCustomers().subscribe((result) => {
      this.customerlist = result;
    });

    this.proformainvoiceservice.getProducts().subscribe((result) => {
      this.productList = result;
    });

    this.proformainvoiceservice.applicationSetting().subscribe((result) => {
      this.vatPercent = result;
    });

    this.sub = this.route.params.subscribe(params => {
      this.id = params['id'];
    });

    if (typeof this.id == 'undefined' || typeof this.id == null) {
      this.mode = "add";
      this.isreadonly = false;
      this.disable = false;
      this.iscustomer = false;
      this.datestring = this.datePipe.transform(new Date(), 'yyyy-MM-dd');
      this.proformainvoiceservice.ratesAdd().subscribe((res) => {
        this.currentObj = res;
        this.currentObj.id = Guid.create().toString();
        this.isSave = false;
        this.isSubmitted = true;
        this.isCancel = true;
      });
    }
    else {
      this.mode = "view";
      this.loading = true;
      this.proformainvoiceservice.view(this.id).subscribe((result) => {
        this.currentObj = result.proFormaInvoice;
        this.loading = false;
        this.proformaReport = result;
        this.emailProperty.toMail = this.proformaReport.email;
        this.proformainvoiceservice.getCustomerById(this.currentObj.customerId).subscribe((result) => {
          this.customer = result;
          this.currentObj.customerId = result.id;
          this.saveattachment("view");
        });
        if (this.currentObj.status == "Cancelled") {
          this.isSave = true;
          this.isSubmitted = true;
          this.isCancel = true;
        }
        else {
          this.isSave = this.currentObj.isSubmitted;
          this.isSubmitted = this.currentObj.isSubmitted;
          this.isCancel = this.currentObj.isSubmitted;
        }
        this.datestring = this.datePipe.transform(this.currentObj.date, 'yyyy-MM-dd');
        this.isreadonly = true;
        this.iscustomer = true;
        this.disable = true;
      });
    }

    let origin = window.location.href;
    if (origin.indexOf('proformainvoicedetailcustomer') >= 0) {
      var customerid = localStorage.getItem('customerforproformainvoicedetail');
      if (!isNullOrUndefined(customerid)) {
        this.proformainvoiceservice.getCustomerById(customerid).subscribe((result) => {
          this.customer = result;
          this.currentObj.customerId = result.id;
        });
      }
    }
  }
  setTwoNumberDecimal($event) {
    $event.target.value = parseFloat($event.target.value).toFixed(2);
  }
  dash() {
    this.router.navigate(['dashboard']);
  }
  sendEmail() {
    this.emailPattern = /^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,10})+$/;
    if (!this.emailProperty.toMail.match(this.emailPattern)
      || isNullOrUndefined(this.emailProperty.toMail)
      || this.emailProperty.toMail == "") {
      this.errorMessage = "Invalid Email";
      document.getElementById('error').click();
    }
  }
  downloadPdf() {
    this.loading = true;
    var ProformaInvoice = document.getElementById('ProformaInvoice');
    html2canvas(ProformaInvoice, {
      onclone: function (document) {
        document.querySelector("#ProformaInvoice").style.display = "block";
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
      pdf.save('ProformaInvoicedetail.pdf');
      this.loading = false;
    });
  }
  ngOnDestroy() {
    this.sub.unsubscribe();
    this.currentObj = null;
  }
  save(value: string) {
    this.loading = true;
    this.errorMessage = '';
    this.currentObj.date = new Date(value);
    if (this.currentObj.customerId == Guid.createEmpty().toString()) {
      this.loading = false;
      this.errorMessage = 'Customer name should not be an empty';
    }
    if (this.currentObj.date.toString() == "Invalid Date") {
      if (this.errorMessage != '')
        this.loading = false;
        this.errorMessage = this.errorMessage + ',' + '<br />';
      this.errorMessage = this.errorMessage + this.currentObj.date.toString();
    }
    if (this.currentObj.proformaInvoiceLineItems.length == 0) {
      if (this.errorMessage != '')
        this.errorMessage = this.errorMessage + ',' + '<br />';
      this.loading = false;
      this.errorMessage = this.errorMessage + 'Atleast one product should be add';
    }
    this.Validation();
    if (this.errorMessage != '') return;
    this.currentObj.customer = null;
    if (this.mode == "add") {
      this.loading = true;
      this.proformainvoiceservice.insert(this.currentObj).subscribe((result) => {
        this.reponse = result;
        this.view(this.reponse.id);
        this.loading = false;
        if (!isNullOrUndefined(result.message)) {
          this.errorMessage = result.message;
          return;
        }
      });
    }
    else {
      if (!isNullOrUndefined(this.currentObj.id)) {
        this.currentObj.customer = null;
        this.loading = true;
        this.proformainvoiceservice.update(this.currentObj).subscribe((result) => {
          this.reponse = result;
          if (!isNullOrUndefined(result.message)) {
            this.loading = false;
            this.errorMessage = result.message;
            return;
          } else {
            this.view(this.reponse.id);
            this.loading = false;
          }
        });
      }
    }
  }
  AddProduct() {
    this.lineitem = new ProformaInvoiceLineItem();
    this.lineitem.id = "00000000-0000-0000-0000-00000000000" + this.addNew++;
    this.lineitem.quantity = 0;
    this.lineitem.unitPrice = 0;
    this.lineitem.amount = 0;
    this.currentObj.proformaInvoiceLineItems.push(this.lineitem);
  }
  Removeproduct(id) {
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
        this.proformainvoiceservice.proformaProductsRemove(id).subscribe((result) => {
          if (result.length == 0) {
            var product = this.currentObj.proformaInvoiceLineItems.find(_id => _id.id == id);
            var index = this.currentObj.proformaInvoiceLineItems.indexOf(product);
            if (index > -1) {
              this.currentObj.proformaInvoiceLineItems.splice(index, 1);
            }
          }
          else {
            this.currentObj.proformaInvoiceLineItems = result;
          }
          this.Onchange(0);
        });
      } else
        return;
    });
  }
  Onchange(i) {
    if (this.currentObj.proformaInvoiceLineItems.length == 0) {
      this.currentObj.subTotal = 0;
      this.currentObj.vatTotal = 0;
      this.currentObj.total = 0;
      return;
    }
    var unitPrice = this.currentObj.proformaInvoiceLineItems[i].unitPrice;
    var quantity = this.currentObj.proformaInvoiceLineItems[i].quantity;
    this.currentObj.proformaInvoiceLineItems[i].amount = Math.round((unitPrice * quantity) * 100) / 100;

    this.currentObj.subTotal = Math.round(this.currentObj.proformaInvoiceLineItems
      .reduce((sum, item) => sum + item.amount, 0) * 100) / 100;
    this.currentObj.vatTotal = Math.round((this.currentObj.subTotal * (this.vatPercent / 100)) * 100) / 100;
    this.currentObj.total = Math.round((this.currentObj.subTotal + this.currentObj.vatTotal) * 100) / 100;
  }
  Validation() {
    var length = this.currentObj.proformaInvoiceLineItems.length;
    for (var i = 0; i < length; i++) {
      var productid = this.currentObj.proformaInvoiceLineItems[i].productId;
      var unitPrice = this.currentObj.proformaInvoiceLineItems[i].unitPrice;
      var quantity = this.currentObj.proformaInvoiceLineItems[i].quantity;
      if (isNullOrUndefined(productid) || productid == "") {
        if (this.errorMessage != '')
          this.errorMessage = this.errorMessage + ',' + '<br />';
        this.loading = false;
        this.errorMessage = this.errorMessage + 'Product Id should not be an empty';
      }
      if (unitPrice <= 0) {
        if (this.errorMessage != '')
          this.errorMessage = this.errorMessage + ',' + '<br />';
        this.loading = false;
        this.errorMessage = this.errorMessage + 'Unit price should be greater than 0';
      }
      if (quantity <= 0) {
        if (this.errorMessage != '')
          this.errorMessage = this.errorMessage + ',' + '<br />';
        this.loading = false;
        this.errorMessage = this.errorMessage + 'Quantity should be greater than 0';
      }
      if (this.errorMessage != '')
        break;
    }
  }
  invoicegenerate() {
    this.errorInfo = "ProformaInvoice will be generated. Do you want to continue ?";
  }
  invoicecancel() {
    this.errorInfo = "Proformainvoice will be cancelled. Do you want to continue ?";
  }
  saveORcancelProformainvoice(value: string) {
    if (this.currentObj.proformaInvoiceLineItems.length == 0) {
      if (this.errorMessage != '')
        this.errorMessage = this.errorMessage + ',' + '<br />';
      this.errorMessage = this.errorMessage + 'Atleast one product should be add';
      this.errorMessage = this.errorMessage;
      document.getElementById('error').click();
      return;
    }
    else {
      this.currentObj.customer = null;
      if (value == "ProformaInvoice will be generated. Do you want to continue ?") {
        this.proformainvoiceservice.finalInvoiceSave(this.currentObj)
          .subscribe((res) => {
            this.router.navigate(['proformainvoicelist']);
          });
      }
      else if (value == "Proformainvoice will be cancelled. Do you want to continue ?") {
        this.proformainvoiceservice.cancel(this.currentObj)
          .subscribe((res) => {
            this.router.navigate(['proformainvoicelist']);
          });
      }
    }
  }
  edit() {
    this.mode = "edit";
    this.isreadonly = false;
    this.disable = false;
    this.loading = true;
    this.proformainvoiceservice.view(this.id).subscribe((result) => {
      this.loading = false;
      this.currentObj = result.proFormaInvoice;
    });
  }
  delete() {
    this.proformainvoiceservice.delete(this.id).subscribe((result) => {
      this.router.navigate(['proformainvoicelist']);
    });
  }
  list() {
    this.router.navigate(['proformainvoicelist']);
  }
  searchcustomer() {
    const modalRef = this.modalService.open(SearchCustomer, { size: 'lg' });
    modalRef.componentInstance.componentName = "proformainvoicedetail";
    this.router.navigate(['proformainvoicelist/proformainvoicedetail']);
  }
  view(viewId: any) {
    this.proformainvoiceservice.view(viewId).subscribe((result) => {
      this.currentObj = result.proFormaInvoice;
      this.loading = false;
      this.proformaReport = result;
      this.emailProperty.toMail = this.proformaReport.email;
      this.proformainvoiceservice.getCustomerById(this.currentObj.customerId).subscribe((result) => {
        this.customer = result;
        this.currentObj.customerId = result.id;
        this.saveattachment("edit");
      });
      if (this.currentObj.status == "Cancelled") {
        this.isSave = true;
        this.isSubmitted = true;
        this.isCancel = true;
      }
      else {
        this.isSave = this.currentObj.isSubmitted;
        this.isSubmitted = this.currentObj.isSubmitted;
        this.isCancel = this.currentObj.isSubmitted;
      }
      this.datestring = this.datePipe.transform(this.currentObj.date, 'yyyy-MM-dd');
      this.isreadonly = true;
      this.iscustomer = true;
      this.disable = true;
    });
  }
  saveattachment(status: any) {
    this.mode = "view";
    this.loading = true;
    var Invoice = document.getElementById('ProformaInvoice');
    html2canvas(Invoice, {
      onclone: function (document) {
        document.querySelector("#ProformaInvoice").style.display = "block";
      }
    }).then(canvas => {
      this.emailProperty.id = this.currentObj.id;
      this.emailProperty.invoiceNumber = this.currentObj.proformaDisplyNumber;
      this.emailProperty.fileName = "ProformaInvoice" + this.currentObj.proformaDisplyNumber + ".pdf";
      this.emailProperty.base64 = canvas.toDataURL().split(',')[1];
      this.proformainvoiceservice.savePdfFile(this.emailProperty).subscribe((r) => {
        this.loading = false;
      });
      if (status != "view") {
        this.router.navigate(['proformainvoicelist']);
      }
    });
  }
  sendEmailAttachment() {
this.loading = true;
    this.emailProperty.id = this.currentObj.id;
    this.emailProperty.invoiceNumber = this.currentObj.proformaDisplyNumber;
    this.emailProperty.fileName = "ProformaInvoice" + this.currentObj.proformaDisplyNumber + ".pdf";
    this.proformainvoiceservice.getPDFFile(this.emailProperty).subscribe((resp) => {
        this.loading = false;
      if (resp == "Mail Sent") {
        this.errorInfo = "Mails has been send";
        document.getElementById('error').click();
      } else {
        this.errorInfo = "Problem with Email.";
        document.getElementById('error').click();
      }
    });
  }

}





