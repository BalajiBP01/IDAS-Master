import { Component, OnInit, Renderer, ViewChild, ElementRef } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { DataTablesModule, DataTableDirective } from 'angular-datatables';
import { Subject } from 'rxjs';
import { DatePipe } from '@angular/common';
import * as $ from 'jquery';
import 'datatables.net';
import { SearchCustomer } from '../../searchcustomer/searchcustomer.component';
import { DataTableRequest, Customer, PaymentService, UserService, UserPermission } from '../../services/services';
import { AsideNavService } from '../../aside-nav/aside-nav.service';
import * as _moment from 'moment';
import { NgbModal, NgbActiveModal, NgbModalOptions } from '@ng-bootstrap/ng-bootstrap';
import { PopupComponent } from '../../popup/popup.component';
import { PopupService } from '../../popup/popupService';
import { EventEmitter } from 'events';
import { isNullOrUndefined } from 'util';



@Component({
  selector: 'app-paymentlist',
  templateUrl: './paymentlist.component.html'
})

export class PaymentlistComponent implements OnInit {
  @ViewChild('tableid') table: ElementRef;
  @ViewChild(DataTableDirective)
  dtElement: DataTableDirective;

  dtOptions: any = {};
  dtTrigger: Subject<any> = new Subject();

  errormsg: any;

  fromDate: string = Date.toString();
  toDate: string = Date.toString();
  fromDatestring: string;
  toDatestring: string;
  _DataTableRequest: DataTableRequest = new DataTableRequest();
  customer: Customer = new Customer();
  mode: string = "View";
  type: any;
  userid: any;
  userper: UserPermission = new UserPermission();

  constructor(public router: Router, private renderer: Renderer,
    public datePipe: DatePipe, private _paymentService: PaymentService, public userService: UserService, public asideNavService: AsideNavService, private modalService: NgbModal, public popupservice: PopupService) {
    this.asideNavService.toggle(true);

    this.userid = localStorage.getItem('userid');
    this.userService.getPermission(this.userid, "Payment").subscribe(result => {
      this.userper = result;
      if (this.userper == null || this.userper.viewAction == false) {
        document.getElementById('nopermission').click();
      }
    });
  }
  ngOnInit(): void {

    let origin = window.location.href;
    this.customer.id = "00000000-0000-0000-0000-000000000000";
    if (origin.indexOf('paymentlistcustomer') >= 0) {
      var customerid = localStorage.getItem('customerforpaymentlist');
      if (!isNullOrUndefined(customerid)) {
        this._paymentService.getCustomerById(customerid).subscribe((result) => {
          this.customer = result;
        });
      }
    }

    this.fromDatestring = this.datePipe.transform(new Date(new Date().getFullYear(), new Date().getMonth(), 1), 'yyyy-MM-dd');
    this.toDatestring = this.datePipe.transform(new Date(), 'yyyy-MM-dd');

    var req = JSON.stringify(this._DataTableRequest);
    this.dtOptions = {
      ajax: {
        url: "/api/Payment/PaymentList",
        type: 'POST',
        contentType: 'application/json; charset=UTF-8',
        error: function (xhr, error, code) { console.log(error); },

        data: function (data) {
          var req1 = JSON.parse(req);
          req1.fromdate = $('#fromdate').val();
          req1.todate = $('#todate').val();
          req1.customerId = $('#customer').val();
          req1.dtRequest = data;
          var req2 = JSON.stringify(req1);
          return req2;
        }
      },
      columns: [

        { data: 'number', title: 'Payment Number', name: 'number' },
        {
          data: 'date', title: 'Payment Entered Date', name: 'date',
          "render": function (data, type, row) {
            return _moment(new Date(data).toString()).format('YYYY-MM-DD');
          }
        },
        {
          data: 'paymentReceivedDate', title: 'Payment Received Date', name: 'paymentReceivedDate',
          "render": function (data, type, row) {
            return _moment(new Date(data).toString()).format('YYYY-MM-DD');
          }
        },
        { data: 'customerName', title: 'Customer Name', name: 'customerName' },
        { data: 'customerCode', title: 'Customer Code', name: 'customerCode' },
        { data: 'invoiceNumber', title: 'Invoice Number', name: 'invoiceNumber' },
        {
          data: 'invoiceDate', title: 'Invoice Date', name: 'invoiceDate',
          "render": function (data, type, row) {
            return _moment(new Date(data).toString()).format('YYYY-MM-DD');
          }
        },
        { data: 'reference', title: 'Reference', name: 'reference' },        
        {
          data: 'paymentReceivedAmount', title: 'Payment Received Amount', name: 'PaymentReceivedAmount',
          "render": function (data, type, row) {
            if (data > 0) {
              var value = data.toFixed(2);
              return value;
            }
            return "0.00";
          }
        },
        { data: 'comments', title: 'Comments', name: 'comments' },
        {
          data: 'id', title: 'Action', name: 'id',
          "render": function (data: any, type: any, full: any) {
            return '<button class="btn btn-link" style="padding:0px" view-pay-id=' + data + '> View </button>'
          }
        },
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
  ngAfterViewInit(): void {
    this.renderer.listenGlobal('document', 'click', (event) => {
      if (event.target.hasAttribute("view-pay-id")) {
        this.type = "view";
        this.router.navigate(['paymentlist/paymentdetail', event.target.getAttribute("view-pay-id"), { type: this.type }]);
      }
    });
    this.dtTrigger.next();
  }
  paymentdetailform() {
    this.router.navigate(['paymentlist/paymentdetail']);
  }
  rerender(): void {
    if (new Date($('#fromdate').val().toString()) > new Date($('#todate').val().toString())) {
     
      this.errormsg = "To date should be greater than from date";
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
    modalRef.componentInstance.componentName = "paymentlist";
    this.router.navigate(['paymentlist']);
  }
  dash() {
    this.router.navigate(['dashboard']);
  }
}
