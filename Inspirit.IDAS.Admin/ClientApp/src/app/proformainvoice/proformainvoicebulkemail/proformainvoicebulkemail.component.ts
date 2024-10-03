import { Component, OnInit, OnDestroy } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { ActivatedRoute } from '@angular/router';
import {
  ProFormaInvoiceService, UserService, ProFormaInvoice, ProFormaReport,
  UserPermission, ProFormaInvoiceBulkEmail, EmailProperty, ProductVmodel
} from '../../services/services';
import { createElementCssSelector } from '@angular/compiler';
import { Guid } from "guid-typescript";
import { isNullOrUndefined } from 'util';
import { DatePipe } from '@angular/common'
import { AsideNavService } from '../../aside-nav/aside-nav.service';
import * as jsPDF from 'jspdf';
import * as html2canvas from 'html2canvas';
import { EventEmitter } from 'events';

@Component({
  selector: 'proformainvoicebulkemail',
  templateUrl: './proformainvoicebulkemail.component.html'
})

export class ProformaInvoiceBulkEmailComponent implements OnInit {

  message: string = "";
  startMonth: string;
  startYear: string;
  emailProperty: EmailProperty = new EmailProperty();
  emailList: EmailProperty[] = [];
  currentObj: ProFormaInvoice = new ProFormaInvoice();
  proformaReport: ProFormaReport = new ProFormaReport();
  temp: ProFormaInvoiceBulkEmail[];
  productList: ProductVmodel[];
  proformacustomerlistvm: ProFormaInvoiceBulkEmail[];
  proformacustomerlist: ProFormaInvoiceBulkEmail[];
  proformaSelectedcustomerlist: ProFormaInvoiceBulkEmail[];
  year: number;
  preyear: any;
  selectcompany: boolean;
  userid: any;
  userper: UserPermission;
  usermsg: any = "";
  nxtyear: any;
  custdisplay: boolean = false;
  loading: boolean = false;
  islist: boolean = false;

  constructor(public router: Router,
    private proFormaInvoiceservice: ProFormaInvoiceService,
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

    this.proFormaInvoiceservice.getProducts().subscribe((result) => {
      this.productList = result;
    });
  }
  submityear(stYear: any, stmonth: any) {
    this.loading = true;
    this.message = "";
    if (isNullOrUndefined(this.startMonth)) {
      if (this.message != "")
        this.message += ",";
      this.loading = false;
      this.message += "Please select month";
    }
    if (isNullOrUndefined(this.startYear)) {
      if (this.message != "")
        this.message += ",";
      this.loading = false;
      this.message += "Please select year";
    }
    if (this.message != "") {
      //alert(this.message);
      this.loading = false;
      document.getElementById('error').click();
      return;
    }

    this.proFormaInvoiceservice.getProFormaInvoiceBulkEmail(stYear, stmonth).subscribe((result) => {
      this.loading = false;
      this.proformacustomerlist = result;
      this.proformacustomerlistvm = result.sort(t => t.proNumber);
      if (this.proformacustomerlist == null || this.proformacustomerlist.length == 0) {
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
    this.proformaSelectedcustomerlist = this.proformacustomerlist.filter(x => x.isSelected == true);
    var list = this.proformaSelectedcustomerlist;
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
      document.getElementById('error').click();
      return;
    }
    this.loading = true;
    var length = list.length;
    list.forEach((item, i) => {
      var ProformaInvoice = document.getElementById('ProformaInvoice' + i);
      this.emailProperty = new EmailProperty();
      this.emailProperty.id = item.proFormaInvoice.id;
      this.emailProperty.toMail = item.email;
      this.emailProperty.fileName = "ProformaInvoice" + item.proFormaInvoice.proformaDisplyNumber + ".pdf";
      this.proFormaInvoiceservice.getPDFFile(this.emailProperty).subscribe((resp) => {
        console.log(resp);
        if ((i + 1) == length) {
          this.loading = false;
          this.islist = true;
          this.message = "Mails has been send";
          document.getElementById('error').click();
        }
      });
    });
  }

  selectall(value: boolean) {

    if (value == true) {
      this.selectcompany = true;
      this.proformacustomerlist.forEach((item) => {
        item.isSelected = true;
      });
    }
    else if (value == false) {
      this.selectcompany = false;
      this.proformacustomerlist.forEach((item, index) => {
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
    this.proformaSelectedcustomerlist = this.proformacustomerlist.filter(x => x.isSelected == true);
  }
  updateFilter(val: string) {
    if (this.temp == null)
      this.temp = this.proformacustomerlistvm;
    this.proformacustomerlist = this.temp;
    if (isNullOrUndefined(val)) {
      if (this.temp != null)
        this.proformacustomerlist = this.temp;
    } else {
      this.proformacustomerlist = this.proformacustomerlistvm.filter(
        m => m.clientName.toLowerCase().includes(val.trim()) == true
          || m.refrenceNumber.toString().includes(val.trim()) == true
          || m.proformaInvoicevalue.toString().includes(val.trim()) == true);
    }
  }
  list() {
    this.router.navigate(['proformainvoicelist']);
  }
  dash() {
    this.router.navigate(['dashboard']);
  }
}
