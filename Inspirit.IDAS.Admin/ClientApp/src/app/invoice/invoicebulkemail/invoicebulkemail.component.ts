import { Component, OnInit, OnDestroy } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { ActivatedRoute } from '@angular/router';
import {
  InvoiceService, UserService,
  UserPermission, InvoiceBulkEmail, EmailProperty
} from '../../services/services';
import { createElementCssSelector } from '@angular/compiler';
import { Guid } from "guid-typescript";
import { isNullOrUndefined } from 'util';
import { DatePipe } from '@angular/common';
import { AsideNavService } from '../../aside-nav/aside-nav.service';
import * as jsPDF from 'jspdf';
import * as html2canvas from 'html2canvas';
import { EventEmitter } from 'events';

@Component({
  selector: 'invoicebulkemail',
  templateUrl: './invoicebulkemail.component.html'
})

export class InvoiceBulkEmailComponent implements OnInit {
  message: string = "";
  startMonth: string;
  startYear: string;
  emailProperty: EmailProperty = new EmailProperty();
  temp: InvoiceBulkEmail[];
  invoicecustomerlistvm: InvoiceBulkEmail[];
  invoicecustomerlist: InvoiceBulkEmail[];
  invoiceSelectedcustomerlist: InvoiceBulkEmail[];
  year: number;
  preyear: any;
  selectcompany: boolean;
  userid: any;
  userper: UserPermission;
  usermsg: any = "";
  nxtyear: any;
  custdisplay: boolean = false;
  loading: boolean = false;

  constructor(public router: Router,
    private invoiceservice: InvoiceService,
    public userService: UserService, public asideNavService: AsideNavService) {

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
    this.year = new Date().getFullYear();
    this.preyear = this.year - 1;
    this.nxtyear = this.year + 1;
  }
  submityear(stYear: any, stmonth: any) {
    this.message = "";
    if (isNullOrUndefined(this.startMonth)) {
      if (this.message != "")
        this.message += ",";
      this.message += "Please select month";
    }
    if (isNullOrUndefined(this.startYear)) {
      if (this.message != "")
        this.message += ",";
      this.message += "Please select year";
    }
    if (this.message != "") {
      document.getElementById('errormsg').click();
      return;
    }

    this.invoiceservice.getInvoiceBulkEmail(stYear, stmonth).subscribe((result) => {
      this.invoicecustomerlist = result;
      this.invoicecustomerlistvm = result;
      if (this.invoicecustomerlist == null || this.invoicecustomerlist.length == 0) {
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
  processSelected() {
    this.message = "";
    var list = this.invoiceSelectedcustomerlist;
    if (isNullOrUndefined(this.startMonth)) {
      if (this.message != "")
        this.message += ",";
      this.message += "Please select month";
    }
    if (isNullOrUndefined(this.startYear)) {
      if (this.message != "")
        this.message += ",";
      this.message += "Please select year";
    }
    if (list.length == 0) {
      if (this.message != "")
        this.message += ",";
      this.message += "Atleast should be select an one customer";
    }
    if (this.message != "") {
      document.getElementById('errormsg').click();
      return;
    }

    this.loading = true;
    var length = list.length;
    list.forEach((item, i) => {
      var Invoice = document.getElementById('TaxInvoice' + i);
      html2canvas(Invoice, {
        onclone: function (document) {
          document.querySelector("#TaxInvoice" + i).style.display = "block";
        }
      }).then(canvas => {
        this.emailProperty = new EmailProperty();
        this.emailProperty.id = item.invoice.id;
        this.emailProperty.base64 = canvas.toDataURL().split(',')[1];
        this.emailProperty.toMail = item.email;
        let filetype: string = item.isPaid ? "TaxInvoice" : "ProformaInvoice";
        this.emailProperty.fileName = filetype + item.invoice.invoiceDisplayNumber + ".pdf";
        this.emailProperty.istaxinv = false;
        this.emailProperty.ispaid = item.isPaid;
        this.invoiceservice.getPDFFile(this.emailProperty).subscribe((resp) => {
          console.log(resp);
          if ((i + 1) == length)
            this.loading = false;
        });
      });
    });
  }
  selectall(value: boolean) {
    if (value == true) {
      this.selectcompany = true;
      this.invoicecustomerlist.forEach((item) => {
        item.isSelected = true;
      });
    }
    else if (value == false) {
      this.selectcompany = false;
      this.invoicecustomerlist.forEach((item, index) => {
        item.isSelected = false;
      });
    }
    this.isSelectedList();
  }
  comselect(isselect: boolean) {
    if (isselect == false) {
      this.selectcompany = false;
      this.loading = false;
    }
    this.isSelectedList();
  }
  isSelectedList() {
    this.invoiceSelectedcustomerlist = this.invoicecustomerlist.filter(x => x.isSelected == true);
  }
  updateFilter(val: string) {
    if (this.temp == null)
      this.temp = this.invoicecustomerlistvm;
    this.invoicecustomerlist = this.temp;
    if (isNullOrUndefined(val)) {
      if (this.temp != null)
        this.invoicecustomerlist = this.temp;
    } else {
      this.invoicecustomerlist = this.invoicecustomerlistvm.filter(
        m => m.clientName.toLowerCase().includes(val.trim()) == true
          || m.refrenceNumber.toString().includes(val.trim()) == true
          || m.invoicevalue.toString().includes(val.trim()) == true);
    }
  }
  dash() {
    this.router.navigate(['dashboard']);
  }
  list() {
    this.router.navigate(['invoicelist']);
  }
}
