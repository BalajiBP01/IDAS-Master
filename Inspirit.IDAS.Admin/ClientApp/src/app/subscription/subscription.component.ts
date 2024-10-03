import { Component, OnInit, Renderer, ViewChild, ElementRef } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router, ActivatedRoute } from '@angular/router';
import * as $ from 'jquery';
import { Subject } from 'rxjs';
//import * as XLSX from 'xlsx';
import { DatePipe } from '@angular/common';
import { DataTablesModule, DataTableDirective } from 'angular-datatables';
import 'datatables.net';
import {
  Customer, SubscriptionService,
  DataTableRequest, SubscriptionResponse, UserService, UserPermission
} from '../services/services';
import { SearchCustomer } from '../searchcustomer/searchcustomer.component';
import { AsideNavService } from '../aside-nav/aside-nav.service';
import * as _moment from 'moment';
import { NgbModal, NgbActiveModal, NgbModalOptions } from '@ng-bootstrap/ng-bootstrap';
import { PopupComponent } from '../popup/popup.component';
import { PopupService } from '../popup/popupService';
import { EventEmitter } from 'events';
import { isNullOrUndefined } from 'util';

@Component({
  selector: 'app-subscription',
  templateUrl: './subscription.component.html'
})
export class SubscriptionComponent implements OnInit {
  @ViewChild('tableid') table: ElementRef;
  @ViewChild(DataTableDirective)
  dtElement: DataTableDirective;

  dtOptions: any = {};
  dtTrigger: Subject<any> = new Subject();

  fromDate: string = Date.toString();
  toDate: string = Date.toString();
  fromDatestring: string;
  toDatestring: string;
  customer: Customer = new Customer();
  public sub: any;
  public id: any;
  public customerid: any;
  public subid: any;
  public dataTablereq: DataTableRequest = new DataTableRequest();
  public response: SubscriptionResponse = new SubscriptionResponse();
  userid: any;
  userper: UserPermission = new UserPermission();

  message: any;

  constructor(public router: Router, public route: ActivatedRoute,
    public renderer: Renderer, public datePipe: DatePipe,
    public subscriptionservice: SubscriptionService, public userService: UserService, public asideNavService: AsideNavService, private modalService: NgbModal, public popupservice: PopupService) {


    this.asideNavService.toggle(true);

    this.userid = localStorage.getItem('userid');
    this.userService.getPermission(this.userid, "Subscriptions").subscribe(result => {
      this.userper = result;
      if (this.userper == null || this.userper.viewAction == false) {
        document.getElementById('nopermission').click();
      }
    });

  }
  ngOnInit(): void {
    let origin = window.location.href;
    this.customer.id = "00000000-0000-0000-0000-000000000000";
    if (origin.indexOf('subscriptionlistcustomer') >= 0) {
      var customerid = localStorage.getItem('customerforsubscriptionlist');
      if (!isNullOrUndefined(customerid)) {
        this.subscriptionservice.getCustomerById(customerid).subscribe((result) => {
          this.customer = result;
        });
      }
    }
   
    this.fromDatestring = this.datePipe.transform(new Date(new Date().getFullYear(), new Date().getMonth(), 1), 'yyyy-MM-dd');
    this.toDatestring = this.datePipe.transform(new Date(), 'yyyy-MM-dd');

    var req = JSON.stringify(this.dataTablereq);
    this.dtOptions = {
      ajax: {
        url: '/api/Subscription/GetSubscriptionList',
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
        { data: 'customerName', title: 'Customer Name', name: 'customerName' },
        {
          data: 'subscriptionId', title: 'Subscription Number', name: 'subscriptionid',
          "render": function (data: any, type: any, row: any) {
            return '<button class="btn btn-link" style="padding:0px"  subscription-view-id=' + data + '> ' + row.subDisplayNumber + ' </button>'
          }
        },
        { data: 'productName', title: 'Product Name', name: 'productName' },
        {
          data: 'startDate', title: 'Start Date', name: 'startDate',
          "render": function (data, type, row) {
            return _moment(new Date(data).toString()).format('YYYY-MM-DD');
          }
        },
        {
          data: 'endDate', title: 'End Date', name: 'endDate',
          "render": function (data, type, row) {
            return _moment(new Date(data).toString()).format('YYYY-MM-DD');
          }
        },
        { data: 'quantity', title: 'Quantity', name: 'quantity' },
        { data: 'usageType', title: 'Usage Type', name: 'usageType' }
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
  rerender(): void {
    if (new Date($('#fromdate').val().toString()) > new Date($('#todate').val().toString())) {
      this.message = "To date should be greater than from date";
      document.getElementById('errormsg').click();
      return;
    }
    this.dtElement.dtInstance.then((dtInstance1: DataTables.Api) => {
      dtInstance1.ajax.reload();
    });
  }
  subscriptiondetailform() {
    this.router.navigate(['subscribtions/subscriptiondetail']);
  }
  dash() {
    this.router.navigate(['dashboard']);
  }
  ngAfterViewInit(): void {
    this.renderer.listenGlobal('document', 'click', (event) => {
      if (event.target.hasAttribute("subscription-view-id")) {
        this.router.navigate(['subscribtions/subscriptiondetail', event.target.getAttribute("subscription-view-id")]);
      }
    });
    this.dtTrigger.next();
  }
  searchcustomer() {
    const modalRef = this.modalService.open(SearchCustomer, { size: 'lg' });
    modalRef.componentInstance.componentName = "subscriptionlist";
    this.router.navigate(['subscribtions']);
  }
}
