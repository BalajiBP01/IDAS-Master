import { Component, OnInit, Renderer, ViewChild, ElementRef } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { DataTablesModule, DataTableDirective } from 'angular-datatables';
import { Subject } from 'rxjs';
import 'datatables.net';
import * as _moment from 'moment';
import { DatePipe } from '@angular/common';
import {
  DataTableRequest, ProFormaInvoiceDataTableRequest, ProFormaInvoice, ProFormaInvoiceService, Customer,
  UserService, UserPermission, EmailProperty, ProFormaReport, ProFormaInvoiceVm
} from '../../services/services';
import { SearchCustomer } from '../../searchcustomer/searchcustomer.component';
import { AsideNavService } from '../../aside-nav/aside-nav.service';
import { DataTable } from 'angular2-datatable';
import { EventEmitter } from 'events';
import { NgbModal, NgbActiveModal, NgbModalOptions } from '@ng-bootstrap/ng-bootstrap';
import { PopupComponent } from '../../popup/popup.component';
import { PopupService } from '../../popup/popupService';
import { isNullOrUndefined } from 'util';


@Component({
  selector: 'app-proformainvoicelist',
  templateUrl: './proformainvoicelist.component.html'
})

export class ProFormaInvoiceListComponent implements OnInit {
  @ViewChild('tableid') table: ElementRef;
  @ViewChild(DataTableDirective)
  dtElement: DataTableDirective;
  dtOptions: any = {};
  dtTrigger: Subject<any> = new Subject();
  emailProperty: EmailProperty = new EmailProperty();
  proformaReport: ProFormaReport = new ProFormaReport();
  _proFormaInvoiceVm: ProFormaInvoiceVm[];
  _proFormaInvoiceDataTableRequest: ProFormaInvoiceDataTableRequest = new ProFormaInvoiceDataTableRequest();
  _DataTableRequest: DataTableRequest = new DataTableRequest();
  fromDate: string = Date.toString();
  toDate: string = Date.toString();
  fromDatestring: string;
  toDatestring: string;
  customer: Customer = new Customer();   
  mode: string = "View";
  currentObj: ProFormaInvoice = new ProFormaInvoice();
  userid: any;
  userper: UserPermission = new UserPermission();
  message: any;

  constructor(public router: Router, private renderer: Renderer,
    private _proFormaInvoiceService: ProFormaInvoiceService, public datePipe: DatePipe, public userService: UserService, public asideNavService: AsideNavService, private modalService: NgbModal, public popupservice: PopupService) {

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
    this.fromDatestring = this.datePipe.transform(new Date(new Date().getFullYear(), new Date().getMonth(), 1), 'yyyy-MM-dd');
    this.toDatestring = this.datePipe.transform(new Date(), 'yyyy-MM-dd');

    this._proFormaInvoiceDataTableRequest.fromdate = this.fromDatestring;
    this._proFormaInvoiceDataTableRequest.todate = this.toDatestring;
    this._proFormaInvoiceService.proFormaInvoiceDataList(this._proFormaInvoiceDataTableRequest).subscribe((res) => {
      this._proFormaInvoiceVm = res.data;
    });

    let origin = window.location.href;
    this.customer.id = "00000000-0000-0000-0000-000000000000";
    if (origin.indexOf('proformainvoicelistcustomer') >= 0) {
      var customerid = localStorage.getItem('customerforproformainvoicelist');
      if (!isNullOrUndefined(customerid)) {
        this._proFormaInvoiceService.getCustomerById(customerid).subscribe((result) => {
          this.customer = result;
        });
      }
    }     
    var req = JSON.stringify(this._proFormaInvoiceDataTableRequest);
    this.dtOptions = {
      ajax: {
        url: "/api/ProFormaInvoice/ProFormaInvoiceDataList",
        type: 'POST',
        contentType: 'application/json; charset=UTF-8',
        error: function (xhr, error, code) { console.log(error); },

        data: function (data) {
          var req1 = JSON.parse(req);
          req1.fromdate = $('#fromdate').val();
          req1.todate = $('#todate').val();
          req1.customerId = $('#customer').val();
          req1.status = $('#status').val();
          req1.dtRequest = data;
          var req2 = JSON.stringify(req1);
          return req2;
        }
      },

      columns: [
        { data: 'proDisplayNumber', title: 'ProForma Invoice Number', name: 'proFormaInvoiceNumber' },
        { data: 'customerName', title: 'Customer Name', name: 'customerName' },
        { data: 'customerCode', title: 'Customer Code', name: 'customerCode' },
        {
          data: 'date', title: 'Date', name: 'date',
          "render": function (data, type, row) {
            return _moment(new Date(data).toString()).format('YYYY-MM-DD');
          }
        },
        {
          data: 'subTotal', title: 'Sub Total', name: 'total',
          "render": function (data, type, row) {
            var value = data.toFixed(2);
            return value;
          }
        },
        {
          data: 'vatTotal', title: 'Vat Total', name: 'total',
          "render": function (data, type, row) {
            var value = data.toFixed(2);
            return value;
          }
        },
        {
          data: 'total', title: 'Proforma Total', name: 'total',
          "render": function (data, type, row) {
            var value = data.toFixed(2);
            return value;
          }
        },
        {
          data: 'emailDate', title: 'Email Date', name: 'emailDate',
          "render": function (data, type, row) {
            return _moment(new Date(data).toString()).format('YYYY-MM-DD');
          }
        },
        { data: 'status', title: 'Status', name: 'status' },
        {
          data: 'id', title: 'Action', name: 'id',
          "render": function (data: any, type: any, full: any) {
            return '<button class="btn btn-link" style="padding:0px" view-id=' + data + '> View </button>'
          }
        }
      ],
      processing: true,
      serverSide: false,
      pagingType: 'full_numbers',
      pageLength: 10,
      scrollX: true,
      dom: 'lBfrtip',
      buttons: [
        {
          extend: 'collection',
          text: 'Export',
          buttons: [
            'copy',
            'excel',
            'csv',
            'pdf',
            'print'
          ]
        }
      ]
    };
  }
  dash() {
    this.router.navigate(['dashboard']);
  }
  ngAfterViewInit(): void {
    this.renderer.listenGlobal('document', 'click', (event) => {
      if (event.target.hasAttribute("view-id")) {
        this.router.navigate(['proformainvoicelist/proformainvoicedetail', event.target.getAttribute("view-id")]);
      }
    });
    this.dtTrigger.next();
  }
  rerender(): void {
    if (new Date($('#fromdate').val().toString()) > new Date($('#todate').val().toString())) {
      this.message = 'To date should be greater than from date';
      document.getElementById('errormsg').click();
      return;
    }
    this.dtElement.dtInstance.then((dtInstance1: DataTables.Api) => {
      dtInstance1.ajax.reload();
    });
  }
  ngOnDestroy() {
    this.dtTrigger.unsubscribe();
  }
  proformainvoicedetailform() {
    this.router.navigate(['proformainvoicelist/proformainvoicedetail']);
  }
  proformasendbulkemail() {
    this.router.navigate(['proformainvoicelist/proformainvoicebulkemail']);
  }
  searchcustomer() {
    const modalRef = this.modalService.open(SearchCustomer, { size: 'lg' });
    modalRef.componentInstance.componentName = "proformainvoicelist";
    this.router.navigate(['proformainvoicelist']);
  }
}
