import { Component, OnInit, Renderer, ViewChild, ElementRef } from '@angular/core';
import { Router } from '@angular/router';
import { DataTablesModule } from 'angular-datatables';
import { DataTableDirective } from 'angular-datatables';
import * as $ from 'jquery';
//import * as XLSX from 'xlsx';
import * as _moment from 'moment';
import { UserService, UserPermission } from '../../services/services';
import { Subject } from 'rxjs';
import { DatePipe } from '@angular/common';
import {
  InvoiceDataTableRequest, InvoiceService,
  CustomerVModel, Customer
} from '../../services/services';
import { SearchCustomer } from '../../searchcustomer/searchcustomer.component';
import { NgbModal, NgbActiveModal, NgbModalOptions } from '@ng-bootstrap/ng-bootstrap';
import { PopupComponent } from '../../popup/popup.component';
import { PopupService } from '../../popup/popupService';
import { EventEmitter } from 'events';




import { AsideNavService } from '../../aside-nav/aside-nav.service';
import { isNullOrUndefined } from 'util';


@Component({
  selector: 'app-invoicelist',
  templateUrl: './invoicelist.component.html'
})

export class InvoiceListComponent implements OnInit {
  @ViewChild('tableid') table: ElementRef;
  @ViewChild(DataTableDirective)
  dtElement: DataTableDirective;

  dtOptions: any = {};
  dtTrigger: Subject<any> = new Subject();
  fromDatestring: string;
  toDatestring: string;
  customer: Customer = new Customer();

  _DataTableRequest: InvoiceDataTableRequest = new InvoiceDataTableRequest();

  fromDate: string = Date.toString();
  toDate: string = Date.toString();
  userid: any;
  userper: UserPermission = new UserPermission();
  message: any;

  mode: string = "View";
  type: any;
  constructor(public router: Router, private renderer: Renderer, public invoiceService: InvoiceService, public datePipe: DatePipe, public userService: UserService, public asideNavService: AsideNavService
    , private modalService: NgbModal, public popupservice: PopupService) {

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
    let origin = window.location.href;
    this.customer.id = "00000000-0000-0000-0000-000000000000";
    if (origin.indexOf('invoicecustomer') >= 0) {
      var customerid = localStorage.getItem('customerforinvoice');
      if (!isNullOrUndefined(customerid)) {
        this.invoiceService.getCustomerById(customerid).subscribe((result) => {
          this.customer = result;
        });
      }
    }       
    this.fromDatestring = this.datePipe.transform(new Date(new Date().getFullYear(), new Date().getMonth(), 1), 'yyyy-MM-dd');
    this.toDatestring = this.datePipe.transform(new Date(), 'yyyy-MM-dd');
    this.change();
  }
  change() {
    var req = JSON.stringify(this._DataTableRequest);
    this.dtOptions = {
      ajax: {
        url: "/api/Invoice/InvoiceDataList",
        type: 'POST',
        contentType: 'application/json; charset=UTF-8',
        error: function (xhr, error, code) { console.log(error); },
        data: function (data) {
          var req1 = JSON.parse(req);
          req1.fromdate = $('#fromdate').val();
          req1.todate = $('#todate').val();
          req1.customerId = $('#customer').val();
          req1.ispaid = $('#pay').val();
          req1.dtRequest = data;
          var req2 = JSON.stringify(req1);
          console.log(req2);
          return req2;
        }
      },
      columns: [
        {
          data: 'isPayed', title: 'Status', name: 'isPayed', orderable: true,
          "render": function (data: any, type: any, full: any) {
            if (data == true)
              return "Tax Invoice";
            else
              return "Proforma Invoice";
          }
        },
        {
          data: 'isProformaInvoice', title: 'Type', name: 'isProformaInvoice',
          "render": function (data: any, type: any, full: any) {
            if (data == true)
              return "One Time";
            else
              return "Recurring";

          }
        },
        {
          data: 'id', title: 'Invoice Number', name: 'id',
          "render": function (data: any, type: any, full: any) {
            return '<button class="btn btn-link" style="padding:0px" view-id=' + data + '>' + full.invoiceDisplayNumber + '</button>'
          }
        },
        {
          data: 'date', title: 'Date of created', name: 'date',

          "render": function (data, type, row) {
            return _moment(new Date(data).toString()).format('YYYY-MM-DD');
          }
        },
        {
          data: 'invoiceDate', title: 'Invoice Date', name: 'invoiceDate',

          "render": function (data, type, row) {
            return _moment(new Date(data).toString()).format('YYYY-MM-DD');
          }
        },
        { data: 'tradingName', title: 'Customers', name: 'tradingName' },
        { data: 'customerCode', title: 'Customer Code', name: 'customerCode' },
        {
          data: 'subTotal', title: 'Total Amount', name: 'subTotal',
          "render": function (data, type, row) {
            var value = data.toFixed(2);
            return value;
          }
        },
        {
          data: 'vatTotal', title: 'Vat Amount', name: 'vatTotal',
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
        {
          data: 'total', title: 'Invoice Value', name: 'total',
          "render": function (data, type, row) {
            var value = data.toFixed(2);
            return value;
          }
        },
        {
          data: 'paymentValue', title: 'Payment Value', name: 'PaymentValue',
          "render": function (data, type, row) {
            if (data > 0) {
              var value = data.toFixed(2);
              return value;
            }
            return "0.00";
          }
        },
        {
          data: 'paymentValue', title: 'Payment Number', name: 'paymentValue',
          "render": function (data, type, row) {
            if (row.isPayed == true) {
              var payment = row.invoiceDisplayNumber.replace("INV", "PMT");
            }
            else {
              payment = null;
            }
            return payment;
          }
        },
        {
          data: 'creditNoteNumber', title: 'Credit Note Number', name: 'creditNoteNumber',
          "render": function (data, type, row) {
            if (row.isCreditNoteRaised == true) {
              var criditnote = row.invoiceDisplayNumber.replace("INV", "CRN");
            }
            else {
              criditnote = null;
            }
            return criditnote;
          }
        },
        {
          data: 'creditNoteTotal', title: 'Credit Note Value', name: 'creditNoteTotal',
          "render": function (data, type, row) {
            if (data > 0) {
              var value = data.toFixed(2);
              return value;
            }
            return "0.00";
          }
        },
        { data: 'isCancelled', title: 'Is Cancelled', name: 'isCancelled' },
        {
          data: 'isPayed', title: 'Payment', name: 'isPayed', orderable: true,
          "render": function (data: any, type: any, full: any) {
            if (data == true) {
              return "Paid";
            }
            else if (data == false && full.isCreditNoteRaised == true) {
              return "Not Received";
            } else if (data == false && full.isCreditNoteRaised == false) {
              return '<button class="btn btn-link" style="padding:0px" inv-payment-id=' + full.id + ',' + full.total + '> Pay </button>'
            }
          }
        },
        {
          data: 'id', title: 'Credit Note', name: 'id', orderable: false,
          "render": function (data: any, type: any, full: any) {
            if ((full.paymentValue == full.total) && full.isPayed)
              return "Not Allowed";
            if (full.isCreditNoteRaised == false)
              return '<button class="btn btn-link" style="padding:0px" inv-credit-id=' + data + ',' + full.total + ',' + full.paymentValue + '> Raise </button>';
            else
              return "Raised";
          }
        }
      ],
      processing: true,
      serverSide: false,
      pagingType: 'full_numbers',
      pageLength: 10,
      scrollX: true,
      columnDefs: [{ orderable: false, targets: [3, 4] }],
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
  ngAfterViewInit(): void {
    this.renderer.listenGlobal('document', 'click', (event) => {
      if (event.target.hasAttribute("view-id")) {
        this.router.navigate(['invoicelist/invoicedetail', event.target.getAttribute("view-id")]);
      }
      else if (event.target.hasAttribute("inv-payment-id")) {
        this.type = "add";
        this.router.navigate(['paymentlist/paymentdetail', { id: event.target.getAttribute("inv-payment-id"), type: this.type }]);
      }
      else if (event.target.hasAttribute("inv-credit-id")) {
        this.router.navigate(['creditnotelist/creditnotedetail', event.target.getAttribute("inv-credit-id")]);
      }
    });
    this.dtTrigger.next();
  }
  dash() {
    this.router.navigate(['dashboard']);
  }
  invoicedetailform() {
    this.router.navigate(['invoicelist/invoicedetail']);
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
  searchcustomer() {
    const modalRef = this.modalService.open(SearchCustomer, { size: 'lg' });
    modalRef.componentInstance.componentName = "invoicelist";
    this.router.navigate(['invoicelist']);
  }
  invoicesendbulkemail() {
    this.router.navigate(['invoicelist/invoicebulkemail']);
  }
}
